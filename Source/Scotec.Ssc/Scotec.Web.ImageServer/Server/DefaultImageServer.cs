using Scotec.Web.ImageServer.Caching;
using Scotec.Web.ImageServer.Processor;
using Scotec.Web.ImageServer.Provider;

namespace Scotec.Web.ImageServer.Server;

internal class DefaultImageServer : IImageServer
{
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
        var request = new ImageRequest
        {
            Format = GetImageFormat(path),
            Width = width,
            Height = height,
            Path = path
        };

        return await GetImageAsync(request);
    }

    public async Task<ImageResponse> GetImageAsync(ImageRequest imageRequest)
    {
        if (imageRequest.Format == ImageFormat.None)
            throw new ImageServerException($"Image format not supported. Path: {imageRequest.Path}");

        try
        {
            if (!_imageCache.TryGetImage(imageRequest, out var imageResponse))
            {
                var imageProvider = _imageProviderFactory.CreateImageProvider(imageRequest);

                imageResponse = await _imageProcessor.ProcessImageAsync(imageRequest, imageProvider);
                imageResponse = _imageCache.AddImage(imageResponse);
            }

            imageResponse!.LastAccess = DateTime.UtcNow;

            return imageResponse!;
        }
        catch (Exception e) when (e is not ImageServerException)
        {
            throw new ImageServerException($"Could not load image. Path:{imageRequest.Path}", e);
        }
    }

    public bool IsImage(string path)
    {
        return GetImageFormat(path) != ImageFormat.None;
    }

    public bool CanProcessImage(string path)
    {
        return IsImage(path) && _imageProviderFactory.HasImageProvider(path);
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