namespace Scotec.Web.ImageServer.Provider;

public class ImageProviderDescriptor
{
    public ImageProviderDescriptor(string path, Type imageProviderType)
    {
        Path = path;
        ImageProviderType = imageProviderType;
    }

    public string Path { get; init; }

    public Type ImageProviderType { get; init; }
}