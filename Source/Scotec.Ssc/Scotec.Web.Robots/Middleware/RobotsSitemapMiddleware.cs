using System.Globalization;
using Microsoft.AspNetCore.Http;
using Scotec.Web.Robots.Sitemap;

namespace Scotec.Web.Robots.Middleware;

public class RobotsSitemapMiddleware
{
    private readonly RequestDelegate _next;

    public RobotsSitemapMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ISitemapProvider sitemapProvider, ISitemapOptions options)
    {
        var path = context.Request.Path.ToUriComponent();
        if (path.EndsWith("/sitemap.xml"))
        {
            var culture = ExtractCulture(path);
            if (string.IsNullOrEmpty(culture) && options.SupportedCultures.Length > 0)
            {
                await CreateSitemapIndex(context, sitemapProvider, options.SupportedCultures);
            }
            else
            {
                await CreateSitemap(context, sitemapProvider, culture);
            }

            return;
        }

        await _next(context);
    }

    private async Task CreateSitemapIndex(HttpContext context, ISitemapProvider sitemapProvider, CultureInfo[] supportedCultures)
    {
        var sitemap = sitemapProvider.Build(supportedCultures);

        context.Response.ContentType = "application/xml";
        await context.Response.Body.WriteAsync(sitemap.Content, context.RequestAborted);
    }

    private static async Task CreateSitemap(HttpContext context, ISitemapProvider sitemapProvider, string culture)
    {
        var sitemap = sitemapProvider.Build(culture);

        context.Response.ContentType = "application/xml";
        await context.Response.Body.WriteAsync(sitemap.Content, context.RequestAborted);
    }

    private static string ExtractCulture(string path)
    {
        var segments = path.Split("/");
        return segments.Reverse()
                       .FirstOrDefault(segment => CultureInfo.GetCultures(CultureTypes.AllCultures)
                                                             .Any(culture =>
                                                                 string.Compare(segment, culture.Name, StringComparison.InvariantCultureIgnoreCase) == 0),
                           string.Empty);
    }
}
