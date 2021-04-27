using System.Collections.Generic;

namespace GTFS_Explorer.BackEnd.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> Limit<T>(this IEnumerable<T> input, int limit)
        {
            int processed = 0;

            foreach (T item in input)
            {
                yield return item;
                if (++processed >= limit) yield break;
            }
        }

        public static IEnumerable<T> Limit<T>(this IEnumerable<T> input, long limit)
        {
            long processed = 0;

            foreach (T item in input)
            {
                yield return item;
                if (++processed >= limit) yield break;
            }
        }
    }
}