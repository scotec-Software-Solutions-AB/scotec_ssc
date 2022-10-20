using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Scotec.Web.Robots.Sitemap;

namespace Scotec.Web.Robots.Middleware
{
    public static class RobotsSitemapExtensions
    {
        public static IApplicationBuilder UseSitemap(this IApplicationBuilder builder)
        {
            
            return builder.MapWhen(httpContext => httpContext.Request.Path.StartsWithSegments("/sitemap.xml"), builder =>
                builder.UseMiddleware<RobotsSitemapMiddleware>());
        }

        public static IServiceCollection AddSitemap(this IServiceCollection services)
        {
            return AddSitemap(services, (options) => { });
        }

        public static IServiceCollection AddSitemap(this IServiceCollection services, Action<SitemapOptions> options)
        {
            services.AddScoped<ISitemapProvider, SitemapProvider>();
            services.AddScoped<ISitemapOptions, SitemapOptions>();
            services.AddSingleton(options);
            
            return services.AddScoped<ISitemapEntryProvider, SitemapEntryProvider>();
        }

    }
}
