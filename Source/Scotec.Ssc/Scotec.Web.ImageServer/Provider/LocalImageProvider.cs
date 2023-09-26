namespace Scotec.Web.ImageServer.Provider;

public class LocalImageProvider : IImageProvider
{
    private readonly IWebHostEnvironment _environment;

    public LocalImageProvider(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public Task<Stream?> GetImageAsync(string path)
    {
        try
        {
            var rootPath = _environment.WebRootPath;
            var filePath = Path.Combine(rootPath, path.Replace('/', '\\').Trim('\\'));

            if (File.Exists(path))
            {
                return Task.FromResult<Stream?>(File.OpenRead(filePath));
            }
        }
        catch (Exception)
        {
            //TODO: Add logging.
        }
        
        return Task.FromResult<Stream?>(default);
    }
}