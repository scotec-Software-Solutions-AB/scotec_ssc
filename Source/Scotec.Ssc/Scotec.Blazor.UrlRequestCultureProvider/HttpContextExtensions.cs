using System.Globalization;

namespace Scotec.Blazor.UrlRequestCultureProvider;

public static class HttpContextExtensions
{
    public static string GetCultureFromRequest(this HttpContext httpContext)
    {
        return GetCultureFromPath(httpContext.Request.Path.Value);
    }

    public static CultureInfo? GetCultureFromReferer(this HttpContext httpContext)
    {
        var referer = httpContext.Request.Headers["Referer"].ToString();
        if (string.IsNullOrEmpty(referer))
        {
            return null;
        }
        var uri = new Uri(referer);

        return new CultureInfo(GetCultureFromPath(uri.LocalPath));
    }

    private static string GetCultureFromPath(string? path)
    {
        if (!string.IsNullOrEmpty(path))
        {
            var segments = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            // Only 2-letter codes are currently supported.
            if (segments is [{ Length: 2 }, ..])
            {
                var currentCulture = segments[0];
                return currentCulture;
            }
        }

        return string.Empty;
    }
}
