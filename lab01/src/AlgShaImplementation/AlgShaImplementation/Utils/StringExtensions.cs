using System;
using System.Collections;
using System.Linq;

namespace AlgShaImplementation.Utils
{
    public static class StringExtensions
    {
        public static BitArray ToBitArray(this string str)
        {
            var arr = new BitArray(str.Select(c => (Byte)c).ToArray());

            bool t;

            for (int i = 0; i < str.Length; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    t = arr[8 * i + j];
                    arr[8 * i + j] = arr[8 * (i + 1) - j - 1];
                    arr[8 * (i + 1) - j - 1] = t;
                }
            }

            return arr;
        }
    }
}

