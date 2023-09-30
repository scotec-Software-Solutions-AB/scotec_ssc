namespace Scotec.Web.ImageServer.Caching;

public interface IImageCache
{
    public ImageResponse AddImage(ImageResponse imageResponse);

    public bool TryGetImage(ImageRequest imageRequest, out ImageResponse? imageResponse);

    void Clear();

    void RemoveImage(ImageResponse imageResponse);
}