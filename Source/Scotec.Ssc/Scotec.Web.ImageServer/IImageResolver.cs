namespace Scotec.Web.ImageServer
{
    public interface IImageResolver
    {
        Stream GetImage(string url);
        Stream GetImage(string url, int height, int width);
    }
}
