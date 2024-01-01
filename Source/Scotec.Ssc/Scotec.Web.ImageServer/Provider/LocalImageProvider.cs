namespace Scotec.Web.ImageServer.Provider;

public class LocalImageProvider : IImageProvider
{
    private readonly IWebHostEnvironment _environment;

    public LocalImageProvider(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public Task<Stream> GetImageAsync(string path)
    {
        try
        {
            var rootPath = _environment.WebRootPath;
            var filePath = Path.Combine(rootPath, path.Replace('/', '\\').Trim('\\'));

            return Task.FromResult<Stream>(File.OpenRead(filePath));
        }
        catch (Exception e) when (e is not ImageServerException)
        {
            throw new ImageServerException($"Could not load image. Path:{path}", e);
        }
    }
}