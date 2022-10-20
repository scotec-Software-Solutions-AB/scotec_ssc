using System;
using System.Collections.Generic;
using System.Linq;

namespace Scotec.Extensions.Linq
{
	public static partial class LinqExtensions
	{
		public static TAttribute? GetCustomAttribute<TAttribute>(this Type type, bool inherit) where TAttribute : Attribute
		{
			return (TAttribute?)type.GetCustomAttributes(typeof(TAttribute), inherit).FirstOrDefault();
		}

		public static bool HasCustomAttribute<TAttribute>(this Type type, bool inherit) where TAttribute : Attribute
		{
			return type.GetCustomAttributes(typeof(TAttribute), inherit).Any();
		}

		public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this Type type, bool inherit) where TAttribute : Attribute
		{
			return type.GetCustomAttributes(typeof(TAttribute), inherit).OfType<TAttribute>();
		}
	}
}
