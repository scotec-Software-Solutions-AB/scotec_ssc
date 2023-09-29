using System.Text;

namespace Scotec.Web.ImageServer.Caching;

// TODO: Add max size and remove oldest images if max. size is reached.
public class InMemoryImageCache : IImageCache
{
    private static readonly ManualResetEventSlim Lock = new(true, 1);
    private readonly Dictionary<string, ImageResponse> _images = new();
    private long _cacheSize;

    public ImageResponse AddImage(ImageResponse imageResponse)
    {
        if (imageResponse.Format == ImageFormat.None)
            throw new ImageServerException($"Unsupported image format. Path:{imageResponse.Path}");

        Lock.Wait();
        try
        {
            var key = BuildKey(imageResponse);

            // Multiple threads may try to add the same image at the same time.
            // However, only the first writes the image into the cache.
            // All other will be forgotten here.
            if (_images.TryGetValue(key, out var cachedImageResponse)) return cachedImageResponse;

            _images.Add(key, imageResponse);
            _cacheSize += imageResponse.Image.Length;

            return imageResponse;
        }
        catch (Exception e)
        {
            throw new ImageServerException($"Could not add image to cache. Path:{imageResponse.Path}", e);
        }
        finally
        {
            Lock.Set();
        }
    }

    public bool TryGetImage(ImageRequest imageRequest, out ImageResponse? imageResponse)
    {
        imageResponse = null;

        Lock.Wait();
        try
        {
            if (!_images.TryGetValue(BuildKey(imageRequest), out imageResponse)) return false;

            // Refresh timestamp with each request.
            imageResponse.Timestamp = DateTime.UtcNow;

            return true;
        }
        catch (Exception e) when (e is not ImageServerException)
        {
            throw new ImageServerException("Error while trying to get image from cache.", e);
        }
        finally
        {
            Lock.Set();
        }
    }

    public void Clear()
    {
        Lock.Wait();
        try
        {
            _images.Clear();
        }
        finally
        {
            Lock.Set();
        }
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