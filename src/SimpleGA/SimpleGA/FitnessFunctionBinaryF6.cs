using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using SimpleGA.Interfaces;

namespace SimpleGA
{
    /// <summary>
    /// Shaffer's F6 function. Good candidate for testing GA. Includes many peaks and valleys which makes convergence of hill-climbing techniques challenging as
    /// the algorithms tend to get stuck in local optima. The F6 function is designed to have its peak at the origin with a value of one and a valley at the origin
    /// with a value of zero. Knowing this we can use it to make sure the GA is minimizing towards global optimum (0 or 1) and not local optima.
    /// </summary>
    public class FitnessFunctionBinaryF6 : IFitnessFunction
    {
        public double Evaluate(Individual individual)
        {
            var decodedSequence = DecodeDnaBinarySequence(individual);
            var x = decodedSequence[0];
            var y = decodedSequence[1];

            //evaluate binary f6 function for x and y values
            var num1 = Math.Pow(Math.Sin(Math.Sqrt(x * x + y * y)), 2);
            var num2 =  Math.Pow(1 + 0.001 * (x * x + y * y), 2);
            var result = 0.5 + (num1  - 0.5) / num2;

            var fitness = 1 - result;

            return fitness;
        }

        public double[] DecodeDnaBinarySequence(Individual individual)
        {
            //get x and y values from binary gene sequence
            //first half of dna sequence for x, second half for y
            //convert the binary sequences into Int64
            double[] result = new double[2];
            var xSubstring = individual.DnaBinarySequence.Substring(0, individual.GeneCount / 2);
            var ySubstring = individual.DnaBinarySequence.Substring(individual.GeneCount / 2, individual.GeneCount / 2);
            var tempX = (int) Convert.ToInt64(xSubstring, 2);
            var tempY = (int) Convert.ToInt64(ySubstring, 2);

            //maintain numerical range between -100 and +100 as per Schaffer's F6 binary function.
            var domainLength = 200 / (Math.Pow(2, individual.GeneCount / 2) - 1);

            //remap values to function range
            var x = (tempX * domainLength) - 100;
            var y = (tempY * domainLength) - 100;
            result[0] = x;
            result[1] = y;
            return result;
        }
    }
}
