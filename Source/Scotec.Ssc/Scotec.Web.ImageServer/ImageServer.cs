namespace Scotec.Web.ImageServer;

internal class ImageServer : IImageServer
{
    private readonly IImageProcessor _imageProcessor;
    private readonly IImageProvider _imageProvider;

    public ImageServer(IImageProvider imageProvider, IImageProcessor imageProcessor)
    {
        _imageProvider = imageProvider;
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

        return await _imageProcessor.ProcessImageAsync(request, _imageProvider);
    }

    public async Task<ImageResponse?> GetImageInfoAsync(ImageRequest request)
    {
        return await _imageProcessor.ProcessImageAsync(request, _imageProvider);
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
