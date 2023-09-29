namespace Scotec.Web.ImageServer.Server;

public interface IImageServer
{
    Task<ImageResponse> GetImageAsync(string path);
    Task<ImageResponse> GetImageAsync(string path, int? width, int? height);
    Task<ImageResponse> GetImageAsync(ImageRequest request);

    bool IsImage(string path);

    bool CanProcessImage(string path);
}