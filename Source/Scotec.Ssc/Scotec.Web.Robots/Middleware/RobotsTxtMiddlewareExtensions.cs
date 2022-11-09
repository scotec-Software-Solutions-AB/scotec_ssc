using Microsoft.AspNetCore.Builder;

namespace Scotec.Web.Robots.Middleware;

public static class RobotsTxtMiddlewareExtensions
{
    public static IApplicationBuilder UseRobotsTxt(this IApplicationBuilder builder)
    {
        return builder.MapWhen(httpContext => httpContext.Request.Path.StartsWithSegments("/robots.txt"), builder =>
            builder.UseMiddleware<RobotsTxtMiddleware>());
    }
}