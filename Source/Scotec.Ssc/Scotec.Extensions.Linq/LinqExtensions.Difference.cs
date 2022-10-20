#region

using System.Collections.Generic;
using System.Linq;

#endregion


namespace Scotec.Extensions.Linq
{
    public static partial class LinqExtensions
    {
        /// <summary>
        /// Returns all entries whose key is contained in only one of the collections.
        /// </summary>
        public static IEnumerable<T> Difference<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            return first.Except(second).Concat(second.Except(first));
        }

        /// <summary>
        /// Returns all entries whose key is contained in only one of the collections.
        /// </summary>
        public static IEnumerable<T> Difference<T>(this IEnumerable<T> first, IEnumerable<T> second,
                                                   IEqualityComparer<T> comparer)
        {
            return first.Except(second, comparer).Concat(second.Except(first, comparer));
        }
    }
}
