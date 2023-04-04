namespace Scotec.Web.ImageServer
{
    public interface IImageServer
    {
        ImageMetadata GetImage(string path);
    }
}
