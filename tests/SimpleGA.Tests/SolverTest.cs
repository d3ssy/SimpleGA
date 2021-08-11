using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace SimpleGA.Tests
{
    public class SolverTest
    {
        private readonly ITestOutputHelper _testOutput;

        public SolverTest(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void It_Returns_Best_Solution()
        {
            //Arrange
            var configuration = new GeneticSolverConfiguration();
            configuration.PopulationSize = 1000;
            configuration.Generations = 100;
            configuration.IndividualGeneCount = 44;
            configuration.MutationProbability = 0.065;
            configuration.CrossoverProbability = 0.85;
            configuration.ElitismPercentage = 0.02;
            var solver = new GeneticSolver(configuration);

            _testOutput.WriteLine("-------------------------INITIAL POPULATION-------------------------");
            foreach (var individual in solver.Population.TopFive)
            {
                _testOutput.WriteLine(FitnessFunctionBinaryF6.DecodeDnaBinarySequence(individual).ToString());
            }

            //Act
            solver.Run(new FitnessFunctionBinaryF6());

            //Assert
            _testOutput.WriteLine("-------------------------FINAL POPULATION-------------------------");
            //_testOutput.WriteLine($"Average Fitness: {solver.Population.FitnessAverage}");
            //_testOutput.WriteLine($"Max Fitness: {solver.Population.FitnessMax}");
            foreach (var individual in solver.Population.TopFive)       
            {
                _testOutput.WriteLine(FitnessFunctionBinaryF6.DecodeDnaBinarySequence(individual).ToString());
            }
        }
    }
}
