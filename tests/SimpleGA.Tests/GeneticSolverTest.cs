using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace SimpleGA.Tests
{
    public class GeneticSolverTest
    {
        private readonly ITestOutputHelper _testOutput;

        public GeneticSolverTest(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void It_Returns_A_Population_Better_Than_The_First()
        {
            //Arrange
            var configuration = new GeneticSolverConfiguration();
            configuration.PopulationSize = 1000;
            configuration.Generations = 100;
            configuration.IndividualGeneCount = 44;
            configuration.MutationProbability = 0.065;
            configuration.CrossoverProbability = 0.85;
            configuration.ElitismPercentage = 0.02;
            configuration.FitnessFunction = new SchafferBinaryF6();
            var solver = new GeneticSolver(configuration);

            _testOutput.WriteLine("Generation 0:");
            GeneticSolver.EvaluateFitness(solver.Population, configuration.FitnessFunction);
            _testOutput.WriteLine(GeneticSolver.PopulationMetrics(solver.Population));

            //Act
            solver.Run();

            //Assert
            _testOutput.WriteLine($"Generation {configuration.Generations - 1}:");
            _testOutput.WriteLine(GeneticSolver.PopulationMetrics(solver.Population));
        }
    }
}
