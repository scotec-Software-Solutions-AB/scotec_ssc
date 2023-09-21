namespace Scotec.Web.ImageServer.Provider;

public class LocalImageProvider : IImageProvider
{
    private readonly IWebHostEnvironment _environment;

    public LocalImageProvider(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public Stream GetImage(string path)
    {
        var rootPath = _environment.WebRootPath;
        var filePath = Path.Combine(rootPath, path.Replace('/', '\\').Trim('\\'));

        return File.OpenRead(filePath);
    }
}