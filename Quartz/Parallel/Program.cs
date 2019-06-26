﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
namespace ParallelForEachExample
{
    class Program
    {
        static void Main()
        {
            string[] colors = {
                                  "1. Red",
                                  "2. Green",
                                  "3. Blue",
                                  "4. Yellow",
                                  "5. White",
                                  "6. Black",
                                  "7. Violet",
                                  "8. Brown",
                                  "9. Orange",
                                  "10. Pink"
                              };
            Console.WriteLine("Traditional foreach loop\n");
            //start the stopwatch for "for" loop
            var sw = Stopwatch.StartNew();
            
            Console.WriteLine("Using Parallel.ForEach");
            //start the stopwatch for "Parallel.ForEach"
            sw = Stopwatch.StartNew();
            Parallel.ForEach(colors, color =>
            {
                Console.WriteLine("{0}, Thread Id= {1}", color, Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(10);
            }
            );
            Console.WriteLine("Parallel.ForEach() execution time = {0} seconds", sw.Elapsed.TotalSeconds);
            Console.Read();
        }
    }
}