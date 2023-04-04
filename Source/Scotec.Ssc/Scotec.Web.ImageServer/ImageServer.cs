namespace Scotec.Web.ImageServer;

internal class ImageServer : IImageServer
{
    private readonly IWebHostEnvironment _environment;

    public ImageServer(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public ImageMetadata GetImage(string path)
    {
        var imageType = GetImageType(path);
        if (imageType == ImageType.None)
        {
            return new ImageMetadata{ ImageType = ImageType.None};
        }
        
        var rootPath = _environment.WebRootPath;
        var filePath = Path.Combine(rootPath, path.Replace('/', '\\').Trim('\\'));

        var imagaData = new ImageMetadata
        {
            ImageType = imageType,
            Path = path,
            ImageStream = File.OpenRead(filePath)
        };

        return imagaData;
    }


    private static ImageType GetImageType(string path)
    {
        var extension = Path.GetExtension(path).ToLower();

        return extension switch
        {
            ".bmp" => ImageType.Bmp,
            ".jpg" => ImageType.Jpeg,
            ".jepg" => ImageType.Jpeg,
            ".gif" => ImageType.Gif,
            ".ico" => ImageType.Ico,
            ".png" => ImageType.Png,
            ".webp" => ImageType.Webp,
            _ => ImageType.None
        };
    }
}
