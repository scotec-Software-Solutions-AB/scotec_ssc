using Microsoft.Extensions.Options;

namespace Scotec.Web.ImageServer;

public static class ImageServerMifddlewareExtensions
{
    public static IApplicationBuilder UseImageServer(this IApplicationBuilder builder)
    {
        var options = builder.ApplicationServices.GetService<IOptions<ImageServerOptions>>()!.Value;

        return builder.UseMiddleware<ImageServerMiddleware>(Options.Create(options));
    }

    public static IServiceCollection AddImageServer(this IServiceCollection services)
    {
        services.AddScoped<IImageServer, ImageServer>();

        return services;
    }
}
