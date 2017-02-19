using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Sort
{
    internal static class SortCpu
    {
        // Library!
        internal static T[] Compute1<T>(T[] array) where T : IComparable<T>
        {
            Array.Sort(array);
            return array;
        }

        // Parallel Linq!
        // ReSharper disable once ParameterTypeCanBeEnumerable.Global
        internal static T[] Compute2<T>(T[] array) where T : IComparable<T>
        {
            // ToArray -> Copies a bunch of Memory! :|
            return array.AsParallel().OrderBy(x => x).ToArray();
        }

        // Custom!
        internal static T[] Compute3<T>(T[] array) where T : IComparable<T>
        {
            QuicksortParallel(array, 0, array.Length - 1);
            return array;
        }

        private static void QuicksortParallel<T>(T[] array, int left, int right) where T : IComparable<T>
        {
            if (right > left)
            {
                int pivot = Partition(array, left, right);

                if (right - left < 1024)
                {
                    QuicksortSequential(array, left, pivot - 1);
                    QuicksortSequential(array, pivot + 1, right);
                }
                else
                {
                    Parallel.Invoke(
                        () => QuicksortParallel(array, left, pivot - 1),
                        () => QuicksortParallel(array, pivot + 1, right));
                }
            }
        }

        private static void QuicksortSequential<T>(T[] array, int left, int right) where T : IComparable<T>
        {
            if (right > left)
            {
                int pivot = Partition(array, left, right);
                QuicksortSequential(array, left, pivot - 1);
                QuicksortSequential(array, pivot + 1, right);
            }
        }

        // Link: https://www.youtube.com/watch?v=BdWK41LNFqE
        private static int Partition<T>(T[] array, int low, int high) where T : IComparable<T>
        {
            var position = (high + low) / 2;
            var pivot = array[position];

            Exchange(array, low, position);

            var i = low;

            for (var k = low + 1; k <= high; k++)
            {
                if (array[k].CompareTo(pivot) < 0)
                {
                    Exchange(array, k, ++i);
                }
            }

            Exchange(array, low, i);

            return i;

            // Slower version
            // ---------------------------------

            //var i = -1;

            //for (var j = 0; j <= high; j++)
            //{
            //    if (array[j].CompareTo(array[high]) <= 0)
            //    {
            //        Exchange(array, ++i, j);
            //    }
            //}

            //return i;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Exchange<T>(T[] array, int i, int j)
        {
            var temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }
}