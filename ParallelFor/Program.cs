// Første eksempel viser en simpel ParallelFor-løkke.
// De to sidste demoeksempler indeholder et par eksempler på en algoritme til beregning af PI, hvor mange af problemerne ved parallel-beregninger viser sig.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelFor
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            double result = 0;
            watch.Start();

            ParallelFor();
            //result = ParallelCalculatePi(1000000000);
            //result = ParallelCalculatePiThreadSafe(1000000000);

            watch.Stop();
            double elapsedMS = watch.ElapsedMilliseconds;
            Console.WriteLine($"Time taken: {elapsedMS} milliseconds - Result: {result}");
        }

        #region DEMO AF ParallelFor()
        static void ParallelFor()
        {
            Parallel.For(0, 20, i =>
            {
                Console.WriteLine($"TaskId: {Task.CurrentId} - Counter: {i}");
                //Thread.SpinWait(100000000);   // Include this line to simulate more work
            });
        }
        #endregion

        #region FØRSTE FORSØG PÅ BEREGNING AF PI
        private static double ParallelCalculatePi(int iterations)
        {
            double pi = 1;
            double multiplier = -1;
            Parallel.For(0, (iterations - 3) / 2, loopIndex =>
            {
                int i = 3 + loopIndex * 2;
                pi += 1.0 / (double)i * multiplier;
                multiplier *= -1;
            });

            return pi * 4.0;
        }
        #endregion

        #region ANDET FORSØG PÅ BEREGNING AF PI
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
    }
}
