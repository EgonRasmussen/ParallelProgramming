// LoopStateBreaking(): Når RandomNumber = 1 standser den aktuelle Task blot med at starte nye tasks – 
//      dem der er i gang vil gøre deres arebejde færdigt!
// LoopStateBreak(): Når RandomNumber = 1 vil igangværende tasks ikke prøve at køre flere iterationer, 
//      kun igangværende iterationer kører færdigt.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelLoopState
{
    class Program
    {
        static void Main(string[] args)
        {
            LoopStateBreak();
            //LoopStateStop();
            //LoopStateObserve();
        }

        static void LoopStateBreak()
        {
            Random rnd = new Random();
            Parallel.For(0, 100, (i, loopState) =>
            {
                if (rnd.Next(1, 50) == 1)
                {
                    Console.WriteLine("Task Id: {0} : Breaking on i = {1} and no more new Tasks are instantiated!", Task.CurrentId, i);
                    loopState.Break();
                    return;

                }
                Console.WriteLine("Task Id: {0} : {1}", Task.CurrentId, i);
            });
        }

        static void LoopStateStop()
        {
            Random rnd = new Random();
            Parallel.For(0, 100, (i, loopState) =>
            {
                if (rnd.Next(1, 50) == 1)
                {
                    Console.WriteLine("Task Id: {0} : Stopping on i= {1} and only ongoing iterations are fullfilled", Task.CurrentId, i);
                    loopState.Stop();
                    return;

                }

                Console.WriteLine("{0} : {1}", Task.CurrentId, i);
            });
        }

        static void LoopStateObserve()
        {
            Random rnd = new Random();

            ParallelLoopResult loopResult = Parallel.For(0, 100, (i, loopState) =>
            {
                if (rnd.Next(1, 50) == 1)
                {
                    Console.WriteLine("Task Id: {0} : Stopping on {1}", Task.CurrentId, i);
                    loopState.Stop();
                    return;
                }

                Thread.Sleep(10);

                if (loopState.IsStopped)
                {
                    Console.WriteLine("Task Id: {0}:STOPPED", Task.CurrentId);
                    return;
                }

                Console.WriteLine("Task Id: {0} : {1}", Task.CurrentId, i);
            });


            Console.WriteLine("Loop ran to completion {0}", loopResult.IsCompleted);
        }
    }
}
