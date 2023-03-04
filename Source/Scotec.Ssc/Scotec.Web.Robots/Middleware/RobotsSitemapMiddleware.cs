using System.Globalization;
using System.Text.RegularExpressions;
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

    public async Task InvokeAsync(HttpContext context, ISitemapProvider sitemapProvider)
    {
        var path = context.Request.Path.ToUriComponent();
        if (path.EndsWith("/sitemap.xml"))
        {
            var language = ExtractLanguage(path);
            var sitemap = sitemapProvider.Build(language);

            context.Response.ContentType = "application/xml";
            await context.Response.Body.WriteAsync(sitemap.Content, context.RequestAborted);

            return;
        }

        await _next(context);
    }

    private string? ExtractLanguage(string path)
    {
        var segments = path.Split("/");
        return segments.Reverse()
                       .FirstOrDefault(segment => CultureInfo.GetCultures(CultureTypes.AllCultures)
                                                             .Any(culture => string.Compare(segment, culture.Name, StringComparison.InvariantCultureIgnoreCase) == 0));
    }
}