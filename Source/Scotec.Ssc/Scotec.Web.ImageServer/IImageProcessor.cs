
namespace Scotec.Web.ImageServer
{
    public interface IImageProcessor
    {
        Task<ImageResponse> ProcessImageAsync(ImageRequest request, IImageProvider imageProvider);
    }
}
