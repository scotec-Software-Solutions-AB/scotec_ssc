using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Scotec.Web.ImageServer.Caching;
using Scotec.Web.ImageServer.Processor;
using Scotec.Web.ImageServer.Provider;
using Scotec.Web.ImageServer.Server;

namespace Scotec.Web.ImageServer;

public static class ImageServerMiddlewareExtensions
{
    public static IApplicationBuilder UseImageServer(this IApplicationBuilder builder)
    {
        var options = builder.ApplicationServices.GetService<IOptions<ImageServerOptions>>()!.Value;

        return builder.UseMiddleware<ImageServerMiddleware>(Options.Create(options));
    }

    public static IServiceCollection AddImageServer(this IServiceCollection services)
    {
        services.AddScoped<IImageServer, DefaultImageServer>()
                .AddScoped<IImageProcessor, MagickImageProcessor>()
                .AddSingleton<IImageCache, InMemoryImageCache>();

        return services;
    }

    public static IServiceCollection AddLocalImageProvider(this IServiceCollection services, string folderName)
    {
        services.AddImageProvider<IImageProvider, LocalImageProvider>(folderName);

        return services;
    }


    public static IServiceCollection AddAzureBlobStorageImageProvider(this IServiceCollection services, string containerName)
    {
        services.AddImageProvider<IImageProvider, AzureBlobStorageImageProvider>(containerName);

        return services;
    }

    public static IServiceCollection AddImageProvider<TService, TImplementation>(this IServiceCollection services, string key)
        where TService : class, IImageProvider
        where TImplementation : class, TService
    {
        services.TryAddScoped<IImageProviderFactory, ImageProviderFactory>();
        services.TryAddScoped<TImplementation>();
        services.AddSingleton(new ImageProviderDescriptor(key, typeof(TImplementation)));
        return services;
    }
}
