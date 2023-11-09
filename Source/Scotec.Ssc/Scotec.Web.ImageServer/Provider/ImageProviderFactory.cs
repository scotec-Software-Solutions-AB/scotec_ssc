using Microsoft.Extensions.DependencyInjection;

namespace Scotec.Web.ImageServer.Provider;

public class ImageProviderFactory : IImageProviderFactory
{
    private readonly IDictionary<string, Type> _descriptors;
    private readonly IServiceProvider _serviceProvider;

    public ImageProviderFactory(IServiceProvider serviceProvider, IEnumerable<ImageProviderDescriptor> descriptors)
    {
        _serviceProvider = serviceProvider;
        _descriptors = descriptors.ToDictionary(descriptor => descriptor.Path, item => item.ImageProviderType);
    }

    public IImageProvider CreateImageProvider(ImageRequest request)
    {
        var key = GetImageProviderKey(request.Path);
        return _serviceProvider.GetRequiredKeyedService<IImageProvider>(key);
        //if (key != null && _descriptors.TryGetValue(key, out var type))
        //    return (IImageProvider)_serviceProvider.GetService(type)!;

        //throw new ImageServerException($"Could not load image provider for file '{request.Path}'");
    }

    public bool HasImageProvider(string path)
    {
        var key = GetImageProviderKey(path);

        return !string.IsNullOrEmpty(key) && _descriptors.ContainsKey(key);
    }

    private static string? GetImageProviderKey(string path)
    {
        var key = path.Split('/', 2, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        return key;
    }
}