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
            throw new ImageServerException($"Unsupported image format. Path:{imageResponse.Path}");
        }

        try
        {
            var memoryStream = new MemoryStream();

            imageResponse.Image.CopyTo(memoryStream);

            memoryStream.Position = 0;
            imageResponse.Image = memoryStream;
            imageResponse.Id = Guid.NewGuid();

            _images.Add(BuildKey(imageResponse), imageResponse);

            return imageResponse;
        }
        catch (Exception e)
        {
            throw new ImageServerException($"Could not add image to cache. Path:{imageResponse.Path}", e);
        }
    }

    public bool TryGetImage(ImageRequest imageRequest, out ImageResponse imageResponse)
    {
        try
        {
            if (!_images.TryGetValue(BuildKey(imageRequest), out imageResponse))
            {
                return false;
            }

            imageResponse.Image = Clone(imageResponse.Image);
            return true;
        }
        catch (Exception e) when(e is not ImageServerException)
        {
            throw new ImageServerException("Error while trying to get image from cache.", e);
        }

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