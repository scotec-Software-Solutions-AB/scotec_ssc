using System.Text;
using ImageMagick;

namespace Scotec.Web.ImageServer.Caching;

// TODO: Add max size and remove oldest images if max. size is reached.
public class InMemoryImageCache : IImageCache
{
    private static readonly ManualResetEventSlim Lock = new(true, 1);
    private readonly Dictionary<string, ImageResponseWrapper> _images = new();
    private readonly long _lowerThreshold = 1024 * 1024 * 8;
    private readonly SortedList<ulong, ImageResponseWrapper> _sortedImages = new();
    private readonly long _upperThreshold = 1024 * 1024 * 10;
    private long _cacheSize;
    private ulong _nextTimestamp;

    public InMemoryImageCache(IConfiguration configuration)
    {
        
    }

    public ImageResponse AddImage(ImageResponse imageResponse)
    {
        if (imageResponse.Format == ImageFormat.None)
        {
            throw new ImageServerException($"Unsupported image format. Path:{imageResponse.Path}");
        }

        var key = BuildKey(imageResponse);

        Lock.Wait();
        try
        {
            // Multiple threads may try to add the same image at the same time.
            // However, only the first writes the image into the cache.
            // All other will be ignored here.
            if (_images.ContainsKey(key))
            {
                return imageResponse;
            }

            var wrapper = new ImageResponseWrapper(_nextTimestamp, imageResponse);
            _images.Add(key, wrapper);
            _sortedImages.Add(_nextTimestamp, wrapper);
            _cacheSize += imageResponse.Image.Length;
            ++_nextTimestamp;

            if (_cacheSize > _upperThreshold)
            {
                RemoveOldest();
            }

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
        var key = BuildKey(imageRequest);
        Lock.Wait();
        try
        {
            if (!_images.TryGetValue(key, out var imageResponseWrapper))
            {
                return false;
            }

            if (_nextTimestamp > imageResponseWrapper.Timestamp + 1)
            {
                _sortedImages.Remove(imageResponseWrapper.Timestamp);
                imageResponseWrapper.Timestamp = _nextTimestamp;
                _sortedImages.Add(_nextTimestamp, imageResponseWrapper);
                ++_nextTimestamp;
            }

            imageResponse = imageResponseWrapper.ImageResponse;

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
            _sortedImages.Clear();
            _nextTimestamp = 0;
        }
        finally
        {
            Lock.Set();
        }
    }

    public void RemoveImage(ImageResponse imageResponse)
    {
        var key = BuildKey(imageResponse);

        Lock.Wait();
        try
        {
            if (_images.TryGetValue(key, out var imageResponseWrapper))
            {
                _images.Remove(key);
                _sortedImages.Remove(imageResponseWrapper.Timestamp);
                _cacheSize -= imageResponseWrapper.ImageResponse.Image.Length;
            }
        }
        finally
        {
            Lock.Set();
        }
    }

    private void RemoveOldest()
    {
        while (_cacheSize >= _lowerThreshold)
        {
            var wrapper = _sortedImages.First().Value;
            _sortedImages.RemoveAt(0);
            _images.Remove(wrapper.ImageResponse.Path);
            _cacheSize -= wrapper.ImageResponse.Image.Length;
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

    private class ImageResponseWrapper
    {
        public ImageResponseWrapper(ulong timestamp, ImageResponse imageResponse)
        {
            Timestamp = timestamp;
            ImageResponse = imageResponse;
        }

        public ulong Timestamp { get; set; }
        public ImageResponse ImageResponse { get; }
    }
}
