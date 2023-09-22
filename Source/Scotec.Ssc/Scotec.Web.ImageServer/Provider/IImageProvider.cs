namespace Scotec.Web.ImageServer.Provider;

public interface IImageProvider
{
    Stream GetImage(string path);
}