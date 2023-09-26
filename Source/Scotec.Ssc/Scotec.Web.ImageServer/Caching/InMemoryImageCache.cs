using System.Diagnostics;
using System.Text;

namespace Scotec.Web.ImageServer.Caching;

// TODO: Add max size and remove oldest images if max. size is reached.
public class InMemoryImageCache : IImageCache
{
    private readonly Dictionary<string, ImageResponse> _images = new();

    public ImageResponse AddImage(ImageResponse imageResponse)
    {
        if (imageResponse.Format == ImageFormat.None)
        {
            return imageResponse;
        }
        var memoryStream = new MemoryStream();

        Debug.Assert(imageResponse.Image != null, "imageResponse.Image != null");
        imageResponse.Image.CopyTo(memoryStream);

        memoryStream.Position = 0;
        imageResponse.Image = memoryStream;
        imageResponse.Id = Guid.NewGuid();

        _images.Add(BuildKey(imageResponse), imageResponse);

        return imageResponse;
    }

    public bool TryGetImage(ImageRequest imageRequest, out ImageResponse imageResponse)
    {
        if (!_images.TryGetValue(BuildKey(imageRequest), out imageResponse)
            || imageResponse.Image == null
            || imageResponse.Format == ImageFormat.None)
        {
            return false;
        }

        imageResponse.Image = Clone(imageResponse.Image);
        return true;

    }

    private static Stream Clone(Stream stream)
    {
        stream.Position = 0;
        var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        memoryStream.Position = 0;

        return memoryStream;
    }

    private void ClearCache()
    {
        _images.Clear();
    }

    private static string BuildKey(ImageResponse response)
    {
        var builder = new StringBuilder();
        builder.Append(response.Path);
        builder.Append(response.Width);
        builder.Append(response.Height);

        return builder.ToString();
    }

    private static string BuildKey(ImageRequest request)
    {
        var builder = new StringBuilder();
        builder.Append(request.Path);
        builder.Append(request.Width);
        builder.Append(request.Height);

        return builder.ToString();
    }
}