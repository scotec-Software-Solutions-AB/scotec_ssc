namespace Scotec.Web.ImageServer
{
    public interface IImageCache
    {
        public ImageResponse AddImage(ImageResponse imageResponse);

        public bool TryGetImage(ImageRequest imageRequest, out ImageResponse? imageResponse);
    }
}
