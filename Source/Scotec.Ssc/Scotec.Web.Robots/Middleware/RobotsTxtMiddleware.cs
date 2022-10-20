using Scotec.Web.Robots.RobotsTxt;
using Microsoft.AspNetCore.Http;

namespace Scotec.Web.Robots.Middleware
{
    public class RobotsTxtMiddleware
    {
        private readonly RequestDelegate _next;
        private IRobotsTxtProvider _robotsTxtProvider;

        public RobotsTxtMiddleware(IRobotsTxtProvider robotsTxtProvider, RequestDelegate next)
        {
            _robotsTxtProvider = robotsTxtProvider;
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments($"/robots.txt"))
            {
                var result = await _robotsTxtProvider.GetResultAsync(context.RequestAborted);

                context.Response.ContentType = "text/plain";
                context.Response.Headers.Add("Cache-Control", $"max-age={result.MaxAge.TotalSeconds}");

                await context.Response.Body.WriteAsync(result.Content, context.RequestAborted);

                return;
            }
                
            await _next(context);
        }
    }
}
