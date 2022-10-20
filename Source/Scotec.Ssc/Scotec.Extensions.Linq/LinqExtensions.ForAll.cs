
namespace Scotec.Extensions.Linq
{
    public static partial class LinqExtensions
    {
        /// <summary>
        ///   Interates the <paramref name = "source" /> enumeration and calls the <paramref name = "action" /> for each item.
        /// </summary>
        public static void ForAll<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

        /// <summary>
        ///   Interates the <paramref name = "source" /> enumeration and calls the <paramref name = "action" /> for each item.
        /// </summary>
        public static void ForAll<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            foreach (var entry in source.Select((item, index) => new { item, index }))
                action(entry.item, entry.index);
        }
    }
}
