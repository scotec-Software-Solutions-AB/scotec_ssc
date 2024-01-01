using System.Collections.Concurrent;
using System.Globalization;

namespace Scotec.Blazor.UrlRequestCultureProvider;

internal static class CultureByConnectionTokens
{
    private static readonly ConcurrentDictionary<string, CultureInfo> Tokens = new();

    public static void AddToken(string token, CultureInfo culture)
    {
        Tokens[token] = culture;
    }

    public static CultureInfo? GetCulture(string token)
    {
        return Tokens.GetValueOrDefault(token);
    }

    public static void RemoveToken(string token)
    {
        Tokens.TryRemove(token, out var _);
    }
}