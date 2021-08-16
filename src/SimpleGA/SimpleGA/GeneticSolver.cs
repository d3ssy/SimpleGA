using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using SimpleGA.Interfaces;

namespace SimpleGA
{
    public class GeneticSolver
    {
        private Population _population;
        private readonly GeneticSolverConfiguration _configuration;
        private static IFitnessFunction _fitnessFunction;

        public GeneticSolver(GeneticSolverConfiguration configuration)
        {
            _configuration = configuration;
            _fitnessFunction = configuration.FitnessFunction;
            _population = new Population(configuration.PopulationSize, configuration.IndividualGeneCount);
        }

        /// <summary>
        /// The population that evolves as the solver runs.
        /// </summary>
        public Population Population => _population;

        /// <summary>
        /// Main method. Runs for specified number of generations in configuration.
        /// At each iteration a new generation is created through selection, crossover and mutation.
        /// </summary>
        public void Run()
        {
            Population currentPopulation = _population;
            //repeat until max generations is reached, or some other stop criteria
            int k = 0;
            while (k <= _configuration.Generations)
            {
                //Evaluate population fitness using fitness function
                EvaluateFitness(currentPopulation, _fitnessFunction);
#if DEBUG
                //Output top 5 individuals, every 5 generations along with fitness metrics for the population.
                if (k % 1 == 0)
                {
                    Debug.WriteLine($"Generation {k}:");
                    Debug.WriteLine(PopulationMetrics(currentPopulation));
                }
#endif
                //Fitness Evaluation
                if (k == _configuration.Generations)
                {
                    break;
                }

                //New population
                var newPopulation = new Population();
                
                //Apply elitism
                if (_configuration.ElitismPercentage > 0)
                {
                    TagElites(currentPopulation, _configuration.ElitismPercentage);
                    newPopulation.Individuals.AddRange(currentPopulation.Individuals.Where(ind => ind.IsElite).ToList());
                }

                //Select parents for crossover (mating)
                for (int i = 0; i < (currentPopulation.Size - currentPopulation.Individuals.Count(ind => ind.IsElite)) / 2; i++)
                {
                    var (parentA, parentB) = BinaryTournamentSelection(currentPopulation);
                    
                    //Crossover
                    var (childA, childB) = CrossoverSinglePoint(parentA, parentB);
                    newPopulation.Individuals.Add(childA);
                    newPopulation.Individuals.Add(childB);
                }

                //Mutation
                if (_configuration.MutationProbability > 0)
                {
                    foreach (var individual in currentPopulation.Individuals)
                    {
                        BinaryMutation(individual, _configuration.MutationProbability);
                    }
                }

                //Update current population to new population
                currentPopulation = newPopulation;
                k++;
            }

            //Update final population to latest population
            _population = currentPopulation;
        }
        
        /// <summary>
        /// Evaluate fitness of each individual in population based on fitness function.
        /// </summary>
        /// <param name="population">Population to evaluate.</param>
        /// <param name="fitnessFunction">Fitness function.</param>
        public static void EvaluateFitness(Population population, IFitnessFunction fitnessFunction)
        {
            foreach (var individual in population.Individuals)
            {
                individual.Fitness = fitnessFunction.Evaluate(individual); //evaluate fitness of population
                individual.IsElite = false; //reset elitism for the entire generation
            }
        }

        /// <summary>
        /// Tags a certain percentage of fittest individuals as elites.
        /// </summary>
        /// <param name="population">Population.</param>
        /// <param name="percent">Percent of fittest individuals to tag as elit.</param>
        public static void TagElites(Population population, double percent)
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
        /// Selects two parents for crossover via binary tournament selection. 
        /// </summary>
        /// <param name="population">Population from which to select the parents</param>
        /// <returns>Two individuals from the population.</returns>
        public static (Individual parentA, Individual parentB) BinaryTournamentSelection(Population population)
        {
            var parentA = TournamentSelection(population, 2);
            var parentB = TournamentSelection(population, 2);
            return (parentA, parentB);
        }

        /// <summary>
        /// Produces two children from two parents by swapping a sub-sequence of the parents' dna binary sequences.
        /// The size of the sub-sequence is determined randomly.
        /// </summary>
        /// <param name="parentA">Individual representing first parent.</param>
        /// <param name="parentB">Individual representing second parent.</param>
        /// <returns>Two new child individuals.</returns>
        public static (Individual childA, Individual childB) CrossoverSinglePoint(Individual parentA, Individual parentB)
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

        /// <summary>
        /// Applies random mutation to individual's binary dna sequence by swapping 1's to 0's and 0's to 1's based on some probability.
        /// </summary>
        /// <param name="individual">Individual to mutate.</param>
        /// <param name="mutationProbability">Probability of mutation, values from 0 to 1.</param>
        public static void BinaryMutation(Individual individual, double mutationProbability)
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

        private static Individual TournamentSelection(Population population, int tournamentSize)
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

        /// <summary>
        /// Output top 5 individuals and their genes along with overall fitness values for the population.
        /// </summary>
        /// <param name="population"></param>
        public static string PopulationMetrics(Population population)
        {
            var stringBuilder = new StringBuilder();

            for (var rank = 0; rank < population.TopFive.Count; rank++)
            {
                var individual = population.TopFive[rank];
                stringBuilder.Append($"Individual {rank + 1}:");
                foreach (var value in _fitnessFunction.DecodeDnaBinarySequence(individual))
                {
                    stringBuilder.Append($"[{value}]");
                }

                stringBuilder.AppendLine();
            }

            stringBuilder.AppendLine($"Min Fitness: {population.FitnessMin}");
            stringBuilder.AppendLine($"Max Fitness: {population.FitnessMax}");
            stringBuilder.AppendLine($"Average Fitness: {population.FitnessAverage}");
            stringBuilder.AppendLine($"Total Fitness: {population.Fitness}");

            return stringBuilder.ToString();
        }
    }
}
