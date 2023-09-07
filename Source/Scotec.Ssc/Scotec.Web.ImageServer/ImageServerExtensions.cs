using Microsoft.Extensions.DependencyInjection.Extensions;
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
        services.AddScoped<IImageServer, ImageServer>()
                .AddScoped<IImageProviderFactory, ImageProviderFactory>()
                .AddScoped<IImageProcessor, MagickImageProcessor>()
                .AddImageProvider<LocalImageProvider>("images")
                .AddSingleton<IImageCache, InMemoryImageCache>();
         
        return services;
    }

    public static IServiceCollection AddImageProvider<TImplementation>(this IServiceCollection services, string key) 
        where TImplementation : class, IImageProvider
    {
        services.TryAddScoped<TImplementation>();
        services.AddSingleton(new ImageProviderDescriptor(key, typeof(TImplementation)));
        return services;
    }
}
