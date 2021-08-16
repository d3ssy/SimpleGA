using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleGA
{
    /// <summary>
    /// A Population represents a collection of Individuals.
    /// </summary>
    public class Population
    {
        public Population()
        {

        }
        public Population(int size, int individualGeneCount)
        {
            if (size % 2 != 0) throw new ArgumentException("Population size must be even.");
            Initialise(size, individualGeneCount);
        }

        public int Size => Individuals.Count;
        public double Fitness => Individuals.Sum(i => i.Fitness);
        public double FitnessAverage => Fitness / Individuals.Count;
        public double FitnessMax => Individuals.Max(i => i.Fitness);
        public double FitnessMin => Individuals.Min(i => i.Fitness);
        public List<Individual> TopFive => Individuals.Take(5).ToList();
        public List<Individual> TopTen => Individuals.Take(5).ToList();
        public List<Individual> Individuals { get; set; } = new List<Individual>();

        private void Initialise(int size, int individualGeneCount)
        {
            for (int i = 0; i < size; i++)
            {
                var individual = new Individual(individualGeneCount);
                Individuals.Add(individual);
            }
        }
    }
}
