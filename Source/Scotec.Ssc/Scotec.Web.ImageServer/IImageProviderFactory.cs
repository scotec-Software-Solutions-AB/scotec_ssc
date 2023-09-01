namespace Scotec.Web.ImageServer
{
    public interface IImageProviderFactory
    {
        IImageProvider? CreateImageProvider(ImageRequest request);
    }
}
