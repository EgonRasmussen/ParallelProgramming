// Normal LINQ udgave
// 1. Tilføj AsParallel() til numbers-collectionen
// 2. tilføj ToList() efter Enumerable.Range(0, 100000000)
//      for at opnå bedre Partitioning

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PLINQ
{
    class Program
    {
        static void Main(string[] args)
        {

            IEnumerable<int> numbers = Enumerable.Range(0, 200000000); //.ToList();

            TimeIt(() =>
            {
                var evenNumbers = from number in numbers//.AsParallel()
                                  where number % 2 == 0
                                  select number;

                Console.WriteLine(evenNumbers.Count());
            });
        }

        private static void TimeIt(Action func)
        {
            Stopwatch timer = Stopwatch.StartNew();
            func();
            Console.WriteLine("{0}() and took {1}", func.Method.Name, timer.Elapsed);

            var cts = new System.Threading.CancellationTokenSource();
        }
    }
}
