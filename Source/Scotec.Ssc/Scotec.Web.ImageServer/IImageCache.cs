namespace Scotec.Web.ImageServer
{
    public interface IImageCache
    {
        public void AddImage(string name, string format, int width, int height);

        public bool TryGetImage(string name, string format, int width, int height, out Stream imageStream);
    }
}
