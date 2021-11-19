using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParallelForEach
{
    class Program
    {
        static void Main(string[] args)
        {
            NormalForEach();
            //SmallStepsForEach();
        }

        #region *** Normal ForEach ***
        static void NormalForEach()
        {
            IEnumerable<int> items = Enumerable.Range(0, 20);

            Parallel.ForEach(items, i =>
            {
                Console.WriteLine($"TaskId: {Task.CurrentId} - Counter: {i}");
            });
        }
        #endregion

        #region *** ForEach with double ***
        static void SmallStepsForEach()
        {
            Parallel.ForEach(FromTo(1, 2, 0.1), Console.WriteLine);
        }

        private static IEnumerable<double> FromTo(double start, double end, double step)
        {
            for (double current = start; current < end; current += step)
            {
                yield return current;
            }
        }
        #endregion
    }
}
