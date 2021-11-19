using System;
using System.Threading.Tasks;

namespace ParallelForDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Parallel.For(0, 20, i =>
            {
                Console.WriteLine($"TaskId: {Task.CurrentId} - Counter: {i}");
            });
        }
    }
}
