namespace Scotec.Web.ImageServer.Provider;

public interface IImageProvider
{
    Task<Stream> GetImageAsync(string path);
}