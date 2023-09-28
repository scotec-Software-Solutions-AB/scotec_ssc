namespace Scotec.Web.ImageServer.Provider;

public interface IImageProviderFactory
{
    IImageProvider CreateImageProvider(ImageRequest request);

    bool HasImageProvider(string path);
}