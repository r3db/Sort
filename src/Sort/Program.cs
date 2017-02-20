using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Sort
{
    internal static class Program
    {
        private static void Main()
        {
            //const int length = 13000000;
            //const int length = 10000000;
            const int length = 100000;

            var data1 = Enumerable.Range(0, length).Reverse().Select(x => (x % 2 == 0 ? -1 : 1) * x).ToArray();
            var data2 = Enumerable.Range(0, length).Reverse().Select(x => (x % 2 == 0 ? -1 : 1) * x).ToArray();
            var data3 = Enumerable.Range(0, length).Reverse().Select(x => (x % 2 == 0 ? -1 : 1) * x).ToArray();
            var data4 = Enumerable.Range(0, length).Reverse().Select(x => (x % 2 == 0 ? -1 : 1) * x).ToArray();

            Measure(() => SortCpu.Compute1(data1), false, length, "CPU: Library!");
            Measure(() => SortCpu.Compute2(data2), false, length, "CPU: Parallel Linq!");
            Measure(() => SortCpu.Compute3(data3), false, length, "CPU: Custom!");

            Measure(() => SortGpu.Compute1(data4), true,  length, "GPU: Library!");

            Console.ReadLine();
        }

        private static void Measure(Func<IEnumerable<int>> func, bool isGpu, int length, string description)
        {
            const string format = "{0,9}";
            const int resultsToDisplay = 12;

            Func<Stopwatch, string> formatElapsedTime = w => w.Elapsed.TotalSeconds >= 1
                ? string.Format(CultureInfo.InvariantCulture, format + "  (s)", w.Elapsed.TotalSeconds)
                : w.Elapsed.TotalMilliseconds >= 1
                    ? string.Format(CultureInfo.InvariantCulture, format + " (ms)", w.Elapsed.TotalMilliseconds)
                    : string.Format(CultureInfo.InvariantCulture, format + " (μs)", w.Elapsed.TotalMilliseconds * 1000);

            Action<bool> consoleColor = error =>
            {
                if (error)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.Black;
                    return;
                }

                Console.ForegroundColor = isGpu
                    ? ConsoleColor.White
                    : ConsoleColor.Cyan;
            };

            Func<Stopwatch, string> bandwidth = w => string.Format(CultureInfo.InvariantCulture, "{0,8:F4} GB/s", length * sizeof(int) / (w.Elapsed.TotalMilliseconds * 1000000));

            var sw1 = Stopwatch.StartNew();
            var result1 = func();
            sw1.Stop();

            Console.WriteLine(new string('-', 88));
            Console.WriteLine(description);
            consoleColor(!IsValid(result1, 100));
            Console.WriteLine("{0,9} - {1} - {2} [Cold]", string.Join(", ", result1.Take(resultsToDisplay)), formatElapsedTime(sw1), bandwidth(sw1));
            Console.ResetColor();

            var sw2 = Stopwatch.StartNew();
            var result2 = func();
            sw2.Stop();
            consoleColor(!IsValid(result2, 100));
            Console.WriteLine("{0,9} - {1} - {2} [Warm]", string.Join(", ", result2.Take(resultsToDisplay)), formatElapsedTime(sw2), bandwidth(sw2));
            Console.ResetColor();
        }

        private static bool IsValid(IEnumerable<int> source, int length)
        {
            var segment = source.Take(length).ToList();

            for (var i = 0; i < segment.Count - 1; i++)
            {
                var a = segment[i + 0];
                var b = segment[i + 1];

                if (a > b)
                {
                    return false;
                }
            }

            return true;
        }
    }
}