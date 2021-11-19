// 1. Sæt MaxDegreeOfParallelism = 1 og se at kun en thread benyttes = sekventiel afvikling = tid er 15 sekunder
// 2. Her sættes MaxDegreeOfParallelism = 3 og alle tre operationer kører parallelt og vi venter kun 5 sekunder
// 3. Lad MaxDegreeOfParallelism = 1 igen. Kobl cts.CancelAfter(2000) ind og oplev Cancel efter 5 sekunder (SumX kan først afbrydes efter 5 sekunder)
// 4. Sæt MaxDegreeOfParallelism = 10, fjern CancelAfter() og kobl () => { throw new Exception("Boom!"); }, ind for at lave en Exception

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelInvoke
{
    class Program
    {
        static void Main(string[] args)
        {
            int sumX = 0;
            int sumY = 0;
            int sumZ = 0;

            CancellationTokenSource cts = new CancellationTokenSource();
            //cts.CancelAfter(2000);
            Stopwatch sw = new Stopwatch();

            try
            {
                sw.Start();
                // Executes SumX, SumY and SumZ in parallel
                Parallel.Invoke(new ParallelOptions() { CancellationToken = cts.Token, MaxDegreeOfParallelism = 1 },
                   // () => { throw new Exception("Boom!"); },
                   () => sumX = SumX(cts.Token),
                   () => sumY = SumY(cts.Token),
                   () => sumZ = SumZ(cts.Token)
                 );

                int total = sumX + sumY + sumZ;

                sw.Stop();
                Console.WriteLine("Sum = " + total);
                Console.WriteLine(sw.Elapsed.TotalSeconds + " s");
            }
            catch (OperationCanceledException operationCanceled)
            {
                Console.WriteLine(sw.Elapsed.TotalSeconds + " s");
                Console.WriteLine(operationCanceled.Message);
            }
            catch (AggregateException errors)
            {
                foreach (Exception error in errors.Flatten().InnerExceptions)
                {
                    Console.WriteLine(error.Message);
                }
            }
        }

        static int SumX(CancellationToken ct)
        {
            Console.WriteLine($"SumX on threadID {Thread.CurrentThread.ManagedThreadId}");
            if (ct.IsCancellationRequested)
            {
                return 0;
            }
            Thread.Sleep(5000);
            return 3;
        }

        static int SumY(CancellationToken ct)
        {
            Console.WriteLine($"SumY on threadID {Thread.CurrentThread.ManagedThreadId}");
            if (ct.IsCancellationRequested)
            {
                return 0;
            }
            Thread.Sleep(5000);
            return 3;
        }

        static int SumZ(CancellationToken ct)
        {
            Console.WriteLine($"SumZ on threadID {Thread.CurrentThread.ManagedThreadId}");
            if (ct.IsCancellationRequested)
            {
                return 0;
            }
            Thread.Sleep(5000);
            return 3;
        }
    }
}
