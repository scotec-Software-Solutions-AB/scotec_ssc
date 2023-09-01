using System.Linq.Expressions;

namespace Scotec.Extensions.Utilities;

public static class StringExtensions
{
#if NETSTANDARD2_1_OR_GREATER
    public static string Format(this string template, params Expression<Func<string, object>>[] args)
    {
        return template.Format(StringComparison.Ordinal, args);
    }
#endif

    public static string Format(this string template
#if NETSTANDARD2_1_OR_GREATER
                                , StringComparison comparison
#endif
                                , params Expression<Func<string, object>>[] args)
    {
        if (args == null)
        {
            throw new ArgumentNullException(nameof(args));
        }

        if (string.IsNullOrEmpty(template))
        {
            return string.Empty;
        }

        var parameters = args.Select(e => (Key: $"{{{e.Parameters[0].Name}}}", Value: e.Compile()(e.Parameters[0].Name!)));

        return parameters.Aggregate(template, (a, b) => a.Replace(b.Key, b.Value?.ToString() ?? string.Empty
#if NETSTANDARD2_1_OR_GREATER
            , comparison
#endif
        ));
    }
}
