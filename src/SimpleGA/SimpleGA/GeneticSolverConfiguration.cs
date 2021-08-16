using System;
using System.Collections.Generic;
using System.Text;
using SimpleGA.Interfaces;

namespace SimpleGA
{
    /// <summary>
    /// Defines configuration parameters of the GeneticSolver.
    /// </summary>
    public class GeneticSolverConfiguration
    {
        public int IndividualGeneCount { get; set; } = 6;
        public int PopulationSize { get; set; } = 50;
        public int Generations { get; set; } = 100;
        public double MutationProbability { get; set; } = 0.02;
        public double CrossoverProbability { get; set; } = 0.1;
        public double ElitismPercentage { get; set; } = 0.05;
        public IFitnessFunction FitnessFunction { get; set; }
    }
}
