using System;
namespace AlgShaImplementation.Utils
{
	public static class ArrayExtensions
	{
		public static T[] FillWith<T>(this T[] array, T value, int startIdx, int endIdx)
		{
            for (int i = startIdx; i < endIdx; i++)
            {
                array[i] = value;
            }

            return array;
        }
	}
}

