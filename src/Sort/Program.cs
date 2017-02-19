using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sort
{
    internal static class Program
    {
        private static void Main()
        {
            const int count = 3000000;

            var random = new Random();

            var data1 = Enumerable.Range(0, count).Reverse().Select(x => (random.NextDouble() > 0.5 ? -1 : 1) * x).ToArray();
            var data2 = Enumerable.Range(0, count).Reverse().Select(x => (random.NextDouble() > 0.5 ? -1 : 1) * x).ToArray();
            var data3 = Enumerable.Range(0, count).Reverse().Select(x => (random.NextDouble() > 0.5 ? -1 : 1) * x).ToArray();

            Measure(() => Array.Sort(data1),      "Quick Sort => A");
            Measure(() => RadixSort.Sort1(data2), "Radix Sort => B");
            Measure(() => RadixSort.Sort2(data3), "Radix Sort => C");

            Console.WriteLine();
            Console.WriteLine(string.Join(", ", data1.Take(8)));
            Console.WriteLine(string.Join(", ", data2.Take(8)));
            Console.WriteLine(string.Join(", ", data3.Take(8)));

            Console.ReadLine();
        }

        private static void Measure(Action action, string message)
        {
            var sw = Stopwatch.StartNew();
            action();
            Console.WriteLine("{0} {1} ms", message, sw.ElapsedMilliseconds);
        }
    }

    internal static class RadixSort
    {
        private const int Radix = 10;

        public static void Sort1(int[] data)
        {
            var buckets = Enumerable.Range(0, Radix).Select(x => new List<int>()).ToArray();
            var maxDigits = data.Max(x => Math.Floor(Math.Log10(x) + 1));
            var radix = 1;

            for (int i = 0; i < maxDigits; i++)
            {
                radix *= Radix;
                var w = radix / Radix;

                foreach (var item in data)
                {
                    var bucket = Math.Abs(item % radix / w);
                    buckets[bucket].Add(item);
                }

                var index = 0;

                foreach (var bucket in buckets)
                {
                    foreach (var item in bucket)
                    {
                        data[index++] = item;
                    }
                }

                foreach (var bucket in buckets)
                {
                    bucket.Clear();
                }
            }
        }

        public static void Sort2(int[] data)
        {
            int[] tmp = new int[data.Length];
            var maxDigits = -1;

            for (int shift = 31; shift > -1; --shift)
            {
                var j = 0;

                for (var i = 0; i < data.Length; ++i)
                {
                    if (shift == 31)
                    {
                        maxDigits = (int)Math.Max(maxDigits, Math.Floor(Math.Log10(data[i]) + 1));
                    }

                    bool move = data[i] << shift >= 0;

                    if (shift == 0 ? !move : move)
                    {
                        data[i - j] = data[i];
                    }
                    else
                    {
                        tmp[j++] = data[i];
                    }
                }

                Array.Copy(tmp, 0, data, data.Length - j, j);
            }
        }
    }
}