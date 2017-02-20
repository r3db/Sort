using System;
using System.Runtime.CompilerServices;
using Alea;
using Alea.Parallel;

namespace Sort
{
    // Todo: Use IComparable<T>
    // Todo: Use Blocks?!
    internal static class SortGpu
    {
        internal static int[] Compute1(int[] array) /*where T : IComparable<T>*/
        {
            var steps = array.Length % 2 == 0
                ? array.Length / 2
                : array.Length / 2 + 1;

            var gpu = Gpu.Default;

            var inputLength = array.Length;
            var inputMemory = gpu.Allocate(array);

            gpu.For(0, array.Length, i =>
            {
                for (var k = 0; k < steps; k++)
                {
                    if (i < inputLength - 1)
                    {
                        var c = inputMemory[i + 0];
                        var n = inputMemory[i + 1];

                        if (i % 2 == 0 && c > n)
                        {
                            Exchange(inputMemory, i, i + 1);
                        }

                        if (i % 2 != 0 && c > n)
                        {
                            Exchange(inputMemory, i, i + 1);
                        }

                        DeviceFunction.SyncThreads();
                    }
                }
            });

            return Gpu.CopyToHost(inputMemory);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // ReSharper disable once SuggestBaseTypeForParameter
        private static void Exchange<T>(T[] array, int c, int n)
        {
            var temp = array[c];
            array[c] = array[n];
            array[n] = temp;
        }
    }
}