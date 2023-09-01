namespace Scotec.Web.ImageServer
{
    public interface IImageProvider
    {
        Stream GetImage(string path);
    }
}
