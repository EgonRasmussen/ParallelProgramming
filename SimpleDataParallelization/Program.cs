using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace SimpleDataParallelization
{
    class Program
    {
        public static List<StockQuote> Stocks = new List<StockQuote>
        {
            new StockQuote { ID=1, Company="Microsoft", Price=5.34M},
            new StockQuote { ID=2, Company="IBM", Price=1.9M},
            new StockQuote { ID=3, Company="Yahoo", Price=2.34M},
            new StockQuote { ID=4, Company="Google", Price=1.54M},
            new StockQuote { ID=5, Company="Altavista", Price=4.74M},
            new StockQuote { ID=6, Company="Ask", Price=3.21M}
        };

        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();

            sw = Stopwatch.StartNew();
            RunInSerial();
            double serialSeconds = sw.Elapsed.TotalSeconds;

            sw = Stopwatch.StartNew();
            RunInParallel();
            double parallelSeconds = sw.Elapsed.TotalSeconds;

            Console.WriteLine("\n*** Serial time: " + serialSeconds);
            Console.WriteLine("*** Parallel time: " + parallelSeconds);
        }

        private static void RunInSerial()
        {
            for (int i = 0; i < Stocks.Count; i++)
            {
                Console.WriteLine("Serial processing stock: " + Stocks[i].Company);
                StockService.CallService(Stocks[i]);
                Console.WriteLine();
            }
        }

        private static void RunInParallel()
        {
            Parallel.For(0, Stocks.Count, i =>
            {
                Console.WriteLine("Parallel processing stock: " + Stocks[i].Company);
                StockService.CallService(Stocks[i]);
            });
        }
    }

    class StockService
    {
        public static decimal CallService(StockQuote quote)
        {
            Console.WriteLine("Executing long task for " + quote.Company);
            var rand = new Random(DateTime.Now.Millisecond);
            Thread.Sleep(1000);
            return Convert.ToDecimal(rand.NextDouble());
        }
    }

    class StockQuote
    {
        public int ID { get; set; }
        public string Company { get; set; }
        public decimal Price { get; set; }
    }
}
