namespace Scotec.Web.ImageServer;

internal class ImageServer : IImageServer
{
    private readonly IImageCache _imageCache;
    private readonly IImageProcessor _imageProcessor;
    private readonly IImageProviderFactory _imageProviderFactory;

    public ImageServer(IImageProviderFactory imageProviderFactory, IImageProcessor imageProcessor,
        IImageCache imageCache)
    {
        _imageProviderFactory = imageProviderFactory;
        _imageProcessor = imageProcessor;
        _imageCache = imageCache;
    }

    public async Task<ImageResponse?> GetImageInfoAsync(string path)
    {
        return await GetImageInfoAsync(path, null, null);
    }

    public async Task<ImageResponse?> GetImageInfoAsync(string path, int? width, int? height)
    {
        var format = GetImageFormat(path);
        if (format == ImageFormat.None)
        {
            return null;
        }

        var request = new ImageRequest
        {
            Width = width,
            Height = height,
            Path = path,
            Format = format
        };

        return await GetImageInfoAsync(request);
    }

    private static readonly ManualResetEventSlim Lock = new(true, 1);

    public async Task<ImageResponse?> GetImageInfoAsync(ImageRequest imageRequest)
    {
        try
        {
            Lock.Wait();
            if (!_imageCache.TryGetImage(imageRequest, out var imageResponse))
            {
                var imageProvider = _imageProviderFactory.CreateImageProvider(imageRequest);
                if (imageProvider == null)
                {
                    // Link does not point to an image or link is invalid.
                    return null;
                }

                imageResponse = await _imageProcessor.ProcessImageAsync(imageRequest, imageProvider);
                imageResponse = _imageCache.AddImage(imageResponse.Value);
            }

            return imageResponse;
        }
        finally
        {
            Lock.Set();
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