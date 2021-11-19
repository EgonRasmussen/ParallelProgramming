using System;
using System.Collections.Generic;
using System.Linq;

namespace PLINQaggregate
{
    class Program
    {
        private static void Main(string[] args)
        {
            //http://en.wikipedia.org/wiki/Standard_deviation

            List<int> values = GenerateNumbers(100000000).ToList();

            double average = values.AsParallel().Average();

            double std = values
                    .AsParallel()
                    .Aggregate(
                        0.0,
                        // produces local total
                        (subTotal, nextNumber) => subTotal += Math.Pow(nextNumber - average, 2),
                        // sum of all local totals
                        (total, threadTotal) => total += threadTotal,
                        // final projection of the combined results
                        grandTotal => Math.Sqrt(grandTotal / (double)(values.Count - 1))
                       );


            Console.WriteLine(std);
        }

        private static IEnumerable<int> GenerateNumbers(int quantity)
        {
            Random rnd = new Random();
            for (int i = 0; i < quantity; i++)
            {
                yield return rnd.Next(1, 100);
            }
        }
    }
}
