using Scotec.Web.ImageServer.Provider;

namespace Scotec.Web.ImageServer.Processor;

public interface IImageProcessor
{
    Task<ImageResponse> ProcessImageAsync(ImageRequest request, IImageProvider imageProvider);
}