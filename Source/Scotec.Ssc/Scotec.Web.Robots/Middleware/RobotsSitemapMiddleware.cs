using Microsoft.AspNetCore.Http;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Scotec.Web.Robots.Sitemap;

namespace Scotec.Web.Robots.Middleware
{
    public class RobotsSitemapMiddleware
    {
        private readonly RequestDelegate _next;

        public RobotsSitemapMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context, ISitemapProvider sitemapProvider)
        {

            if (context.Request.Path.StartsWithSegments($"/sitemap.xml"))
            {
                var sitemap = sitemapProvider.Build();

                context.Response.ContentType = "application/xml";
                await context.Response.Body.WriteAsync(sitemap.Content, context.RequestAborted);

                return;
            }
                
            await _next(context);
        }
    }
}
