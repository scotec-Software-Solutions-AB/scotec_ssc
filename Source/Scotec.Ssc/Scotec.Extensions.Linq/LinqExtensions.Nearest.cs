#region

using System;
using System.Collections.Generic;

#endregion


namespace Scotec.Extensions.Linq
{
    /// <summary>
    ///     Linq Extensions
    /// </summary>
    public static partial class LinqExtensions
    {
        public static TSource NearestMax<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.NearestMax(selector, Comparer<TKey>.Default, out _);
        }

        public static TSource NearestMax<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, out int index)
        {
            return source.NearestMax(selector, Comparer<TKey>.Default, out index);
        }

        public static TSource NearestMax<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer, out int index)
        {
            TSource result;
            var currentIndex = 0;
            index = 0;

            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext()) throw new InvalidOperationException("Sequence is empty");

                var maxValue = sourceIterator.Current;
                var maxValueKey = selector(maxValue);

                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);

                    currentIndex++;

                    if (comparer.Compare(candidateProjected, maxValueKey) > 0)
                    {
                        maxValue = candidate;
                        maxValueKey = candidateProjected;

                        index = currentIndex;
                    }
                }

                result = maxValue;
            }

            return result;
        }

        public static TSource NearestMin<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.NearestMin(selector, Comparer<TKey>.Default, out _);
        }

        public static TSource NearestMin<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, out int index)
        {
            return source.NearestMin(selector, Comparer<TKey>.Default, out index);
        }

        public static TSource NearestMin<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer, out int index)
        {
            TSource result;
            var currentIndex = 0;
            index = 0;

            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext()) throw new InvalidOperationException("Sequence is empty");

                var minValue = sourceIterator.Current;
                var minValueKey = selector(minValue);

                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);

                    currentIndex++;

                    if (comparer.Compare(candidateProjected, minValueKey) < 0)
                    {
                        minValue = candidate;
                        minValueKey = candidateProjected;

                        index = currentIndex;
                    }
                }

                result = minValue;
            }

            return result;
        }
    }
}
