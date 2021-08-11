using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace SimpleGA
{
    public class GeneticSolver
    {
        private Population _population;
        private GeneticSolverConfiguration _configuration;
        
        public GeneticSolver(GeneticSolverConfiguration configuration)
        {
            _configuration = configuration;
            _population = new Population(configuration.PopulationSize, configuration.IndividualGeneCount);
        }

        public GeneticSolver(Population initialPopulation, GeneticSolverConfiguration configuration):this(configuration)
        {
            _population = initialPopulation;
        }

        public Population Population => _population;

        public void Run(IFitnessFunction fitnessFunction)
        {
            Population tempGeneration = _population;
            //repeat until max generations is reached, or some other stop criteria
            int k = 0;
            while (k < _configuration.Generations)
            {
                var generation = new Population();
                //evaluate initial population fitness using fitness function
                foreach (var individual in tempGeneration.Individuals)
                {
                    individual.Fitness = fitnessFunction.Evaluate(individual); //evaluate fitness of population
                    individual.IsElite = false; //reset elitism for the entire generation
                }
                
#if DEBUG
                //Output top 5 individuals, every 5 generations
                if (k % 5 == 0)
                {
                    Debug.WriteLine($"///////////Generation {k} Top 5///////////");
                    foreach (var individual in tempGeneration.TopFive)
                    {
                        Debug.WriteLine(FitnessFunctionBinaryF6.DecodeDnaBinarySequence(individual).ToString());
                    }

                    Debug.WriteLine($"Min Fitness: {tempGeneration.FitnessMin}");
                    Debug.WriteLine($"Max Fitness: {tempGeneration.FitnessMax}");
                    Debug.WriteLine($"Average Fitness: {tempGeneration.FitnessAverage}");
                    Debug.WriteLine($"Total Fitness: {tempGeneration.Fitness}");
                    Debug.WriteLine("//////////////////////////////////////");
                }
#endif

                //tag elite individuals so they can form part of new generation without modification (e.g. mutation)
                if (_configuration.ElitismPercentage > 0)
                {
                    TagElites(tempGeneration, _configuration.ElitismPercentage);
                    generation.Individuals.AddRange(tempGeneration.Individuals.Where(ind => ind.IsElite).ToList());
                }

                //select parents and make some babies
                for (int i = 0; i < (tempGeneration.Size - tempGeneration.Individuals.Count(ind => ind.IsElite)) / 2; i++)
                {
                    var (parentA, parentB) = BinaryTournamentSelection(tempGeneration);
                    var (childA, childB) = CrossoverSinglePoint(parentA, parentB);
                    generation.Individuals.Add(childA);
                    generation.Individuals.Add(childB);
                }

                //mutate individuals in new generation to ensure diversity
                foreach (var individual in tempGeneration.Individuals)
                {
                    Mutate(individual, _configuration.MutationProbability);
                }

                tempGeneration = generation;
                k++;
            }

            _population = tempGeneration;
        }

        /// <summary>
        /// Returns the top x percent of the fittest individuals from the population.
        /// </summary>
        /// <param name="population">Population.</param>
        /// <param name="percent">Percent of fittest individuals.</param>
        /// <returns>Top x percent of individuals based on fitness.</returns>
        private void TagElites(Population population, double percent)
        {
            if (percent > 1 || percent < 0) throw new ArgumentException("Elitism percent must be between 0.0 and 1.0");
            var selectedCount = (int) Math.Round(population.Size * percent);
            if (selectedCount % 2 != 0) selectedCount += 1;
            var elites = population.Individuals.OrderByDescending(i => i.Fitness).Take(selectedCount).ToList();
            foreach (var individual in elites)
            {
                individual.IsElite = true;
            }
        }

        /// <summary>
        /// Selects two parents for mating using tournament selection.
        /// </summary>
        /// <param name="population"></param>
        /// <returns></returns>
        private Individual TournamentSelection(Population population, int tournamentSize)
        {
            Individual best = null;
            for (int i = 0; i < tournamentSize; i++)
            {
                var randomIndex = new Random().Next(0, population.Size);
                var individual = population.Individuals[randomIndex];
                if (best == null || individual.Fitness > best.Fitness)
                {
                    best = individual;
                }
            }

            return best;
        }

        public (Individual parentA, Individual parentB) BinaryTournamentSelection(Population population)
        {
            var parentA = TournamentSelection(population, 2);
            var parentB = TournamentSelection(population, 2);
            return (parentA, parentB);
        }

        public (Individual childA, Individual childB) CrossoverSinglePoint(Individual parentA, Individual parentB)
        {
            var childA = new Individual();
            var childB = new Individual();
            var geneCount = parentA.GeneCount;
            var swapCount = new Random().Next(0, geneCount);
            
            for (int i = 0; i < geneCount; i++)
            {
                if (i <= swapCount)
                {
                    childA.Dna.Add(parentB.Dna[i]);
                    childB.Dna.Add(parentA.Dna[i]);
                    continue;
                }

                childA.Dna.Add(parentA.Dna[i]);
                childB.Dna.Add(parentB.Dna[i]);
            }

            return (childA, childB);
        }

        public void Mutate(Individual individual, double mutationProbability)
        {
            if (individual.IsElite) return; //do not mutate individual if elite
            
            individual.Fitness = 0; //reset fitness
            for (int i = 0; i < individual.GeneCount; i++)
            {
                var randomDouble = new Random().NextDouble();
                if (randomDouble < mutationProbability)
                {
                    individual.Dna[i] = individual.Dna[i].Value == 0 ? new Gene(1) : new Gene(0);
                }

                individual.Dna[i] = individual.Dna[i];
            }
        }
    }
}
