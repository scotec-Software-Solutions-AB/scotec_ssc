namespace Scotec.Web.ImageServer
{
    internal class ImageServer : IImageServer
    {
        private readonly IWebHostEnvironment _environment;

        public ImageServer(IWebHostEnvironment environment)
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
}
