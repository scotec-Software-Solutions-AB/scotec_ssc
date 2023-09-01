namespace Scotec.Web.ImageServer
{
    public interface IImageServer
    {
        Task<ImageResponse?> GetImageInfoAsync(string path);
        Task<ImageResponse?> GetImageInfoAsync(string path, int? width, int? height);
        Task<ImageResponse?> GetImageInfoAsync(ImageRequest request);
    }
}
