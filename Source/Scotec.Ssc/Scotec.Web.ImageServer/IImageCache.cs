namespace Scotec.Web.ImageServer
{
    public interface IImageCache
    {
        public void AddImage(ImageInfo imageInfo);

        public bool TryGetImage(ImageInfo imageInfo);
    }
}
