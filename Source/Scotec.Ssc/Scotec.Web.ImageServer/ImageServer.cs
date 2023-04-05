namespace Scotec.Web.ImageServer;

internal class ImageServer : IImageServer
{
    private readonly IImageProcessor _imageProcessor;
    private readonly IImageProviderFactory _imageProviderFactory;

    public ImageServer(IImageProviderFactory imageProviderFactory, IImageProcessor imageProcessor)
    {
        _imageProviderFactory = imageProviderFactory;
        _imageProcessor = imageProcessor;
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

    public async Task<ImageResponse?> GetImageInfoAsync(ImageRequest request)
    {
        var imageProvider = _imageProviderFactory.CreateImageProvider(request);

        return imageProvider != null 
            ? await _imageProcessor.ProcessImageAsync(request, imageProvider)
            : null;
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
