#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#endregion


namespace Scotec.Extensions.Linq
{
    public static partial class LinqExtensions
    {
        public static IEnumerable<T> Repeat<T>(this int count, Func<int, T> func)
        {
            CheckCount(count);

            for (var i = 0; i < count; i++)
                yield return func(i);
        }

        public static IEnumerable<T> Repeat<T>(this int count, Func<T> func)
        {
            CheckCount(count);

            for (var i = 0; i < count; i++)
                yield return func();
        }

        public static IEnumerable<T> Repeat<T>(this uint count, Func<uint, T> func)
        {
            for (var i = 0u; i < count; i++)
                yield return func(i);
        }

        public static IEnumerable<T> Repeat<T>(this uint count, Func<T> func)
        {
            for (var i = 0u; i < count; i++)
                yield return func();
        }

        public static IEnumerable<T> Repeat<T>(this long count, Func<long, T> func)
        {
            CheckCount(count);

            for (var i = 0L; i < count; i++)
                yield return func(i);
        }

        public static IEnumerable<T> Repeat<T>(this long count, Func<T> func)
        {
            CheckCount(count);

            for (var i = 0L; i < count; i++)
                yield return func();
        }

        public static IEnumerable<T> Repeat<T>(this ulong count, Func<ulong, T> func)
        {
            for (var i = 0ul; i < count; i++)
                yield return func(i);
        }

        public static IEnumerable<T> Repeat<T>(this ulong count, Func<T> func)
        {
            for (var i = 0ul; i < count; i++)
                yield return func();
        }

        public static IEnumerable<T> Repeat<T>(this double value, double stepSize, Func<double, T> func, int precision = 8)
        {
            CheckCount(value);

            var end = (int) Math.Round(value / stepSize, precision);

            for (var i = 0; i < end; i++)
                yield return func(Math.Round(i * stepSize, precision));
        }

        public static IEnumerable<T> Repeat<T>(this double value, double stepSize, Func<T> func, int precision = 8)
        {
            CheckCount(value);

            var end = (int) Math.Round(value / stepSize, precision);

            for (var i = 0; i < end; i++)
                yield return func();
        }

        private static void CheckCount(double value)
        {
            if (value < 0.0)
                throw new ArgumentException("Count must not be less than 0.");
        }

        private static void CheckCount(int value)
        {
            if (value < 0)
                throw new ArgumentException("Count must not be less than 0.");
        }
    }
}
