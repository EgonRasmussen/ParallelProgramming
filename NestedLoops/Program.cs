using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;

namespace ParallelFor
{
    class Program
    {
        static void Main(string[] args)
        {
            Execute(ParallelCalculatePiThreadSafe, "Parallel SingleLoop   ");
            Execute(NonParallelNestedLoopsCalculatePi, "NonParallel NestedLoop");
            Execute(ParallelNestedLoopsCalculatePi, "Parallel NestedLoop   ");
            Execute(OptimizedParallelCalculatePiWithPartitioner, "With Partitioner      ");
        }

        static void Execute(Func<int, double> pgm, string version)
        {
            Stopwatch watch = new Stopwatch();
            double result = 0;
            watch.Start();

            result = pgm(1000000000);

            watch.Stop();
            double elapsedMS = watch.ElapsedMilliseconds;
            Console.WriteLine($"{version}: Time taken: {elapsedMS} milliseconds - Result: {result}");
        }

        #region *** Parallel Single loop ***
        private static double ParallelCalculatePiThreadSafe(int iterations)
        {
            double pi = 1;
            object combineLock = new object();

            Parallel.For(0, (iterations - 3) / 2,
                InitialiseLocalPi,
                (int loopIndex, ParallelLoopState loopState, double localPi) =>
                {
                    double multiplier = loopIndex % 2 == 0 ? -1 : 1;
                    int i = 3 + loopIndex * 2;
                    localPi += 1.0 / (double)i * multiplier;

                    return localPi;
                },
            (double localPi) =>
            {
                lock (combineLock)
                {
                    pi += localPi;
                }
            });
            return pi * 4.0;
        }
        private static double InitialiseLocalPi()
        { return 0.0; }
        #endregion

        #region *** NonParallel NestedLoop ***
        public static double NonParallelNestedLoopsCalculatePi(int iterations)
        {
            IEnumerable<Tuple<int, int>> ranges = Range(3, iterations, 10000);
            double pi = 1;

            foreach (var range in ranges)
            {
                double multiplier = range.Item1 % 2 == 0 ? 1 : -1;

                for (int i = range.Item1; i < range.Item2; i += 2)
                {
                    pi += 1.0 / (double)i * multiplier;
                    multiplier *= -1;
                }
            }
            pi *= 4.0;
            return pi;
        }

        private static IEnumerable<Tuple<int, int>> Range(int start, int end, int size)
        {
            for (int i = start; i < end; i += size)
            {
                yield return Tuple.Create(i, Math.Min(i + size, end));
            }
        }
        #endregion

        #region *** Parallel NestedLoop ***
        public static double ParallelNestedLoopsCalculatePi(int iterations)
        {
            IEnumerable<Tuple<int, int>> ranges = Range(3, iterations, 10000);
            double pi = 1;
            object combineLock = new object();

            Parallel.ForEach(ranges.ToList(),
                () => 0.0,
                (range, loopState, localPi) =>
                {
                    double multiplier = range.Item1 % 2 == 0 ? 1 : -1;

                    for (int i = range.Item1; i < range.Item2; i += 2)
                    {
                        localPi += 1.0 / (double)i * multiplier;
                        multiplier *= -1;
                    }

                    return localPi;
                },
                localPi =>
                {
                    lock (combineLock)
                    {
                        pi += localPi;
                    }
                });
            pi *= 4.0;

            return pi;
        }
        #endregion

        #region *** Parallel NestedLoop With Partitioner ***
        private static double OptimizedParallelCalculatePiWithPartitioner(int iterations)
        {
            //IEnumerable<Tuple<int, int>> ranges = Range(3, iterations, 10000);
            var ranges = System.Collections.Concurrent.Partitioner.Create(3, iterations, 10000);

            double pi = 1;
            object combineLock = new object();
            Parallel.ForEach(ranges, () => 0.0,
                (range, loopState, localPi) =>
                {
                    double multiplier = range.Item1 % 2 == 0 ? 1 : -1;

                    for (int i = range.Item1; i < range.Item2; i += 2)
                    {
                        localPi += 1.0 / (double)i * multiplier;
                        multiplier *= -1;
                    }

                    return localPi;
                },
                localPi =>
                {
                    lock (combineLock)
                    {
                        pi += localPi;
                    }
                });
            pi *= 4.0;

            return pi;
        }
        #endregion
    }
}
