using Scotec.Web.ImageServer.Caching;
using Scotec.Web.ImageServer.Processor;
using Scotec.Web.ImageServer.Provider;
using System.IO;

namespace Scotec.Web.ImageServer.Server;

internal class DefaultImageServer : IImageServer
{
    private static readonly ManualResetEventSlim _lock = new(true, 1);
    private readonly IImageCache _imageCache;
    private readonly IImageProcessor _imageProcessor;
    private readonly IImageProviderFactory _imageProviderFactory;

    public DefaultImageServer(IImageProviderFactory imageProviderFactory, IImageProcessor imageProcessor,
        IImageCache imageCache)
    {
        _imageProviderFactory = imageProviderFactory;
        _imageProcessor = imageProcessor;
        _imageCache = imageCache;
    }

    public async Task<ImageResponse> GetImageAsync(string path)
    {
        return await GetImageAsync(path, null, null);
    }

    public async Task<ImageResponse> GetImageAsync(string path, int? width, int? height)
    {
        var format = GetImageFormat(path);
        if (format == ImageFormat.None)
        {
            return new ImageResponse
            {
                Path = path,
                Format = ImageFormat.None
            };
        }

        var request = new ImageRequest
        {
            Width = width,
            Height = height,
            Path = path,
            Format = format
        };

        return await GetImageAsync(request);
    }

    public async Task<ImageResponse> GetImageAsync(ImageRequest imageRequest)
    {
        try
        {
            _lock.Wait();
            if (!_imageCache.TryGetImage(imageRequest, out var imageResponse))
            {
                var imageProvider = _imageProviderFactory.CreateImageProvider(imageRequest);
                if (imageProvider == null)
                {
                    // Link does not point to an image or link is invalid.
                    return new ImageResponse
                    {
                        Path = imageRequest.Path,
                        Format = ImageFormat.None
                    };
                }

                imageResponse = await _imageProcessor.ProcessImageAsync(imageRequest, imageProvider);
                if (imageResponse.Format != ImageFormat.None)
                {
                    imageResponse = _imageCache.AddImage(imageResponse);
                }
            }

            return imageResponse;
        }
        finally
        {
            _lock.Set();
        }
    }

    private static ImageFormat? GetImageFormat(string path)
    {
        var extension = Path.GetExtension(path).ToLower();

        return extension switch
        {
            ".bmp" => ImageFormat.Bmp,
            ".jpg" => ImageFormat.Jpeg,
            ".jepg" => ImageFormat.Jpeg,
            ".gif" => ImageFormat.Gif,
            ".ico" => ImageFormat.Ico,
            ".png" => ImageFormat.Png,
            ".webp" => ImageFormat.Webp,
            _ => ImageFormat.None
        };
    }
}