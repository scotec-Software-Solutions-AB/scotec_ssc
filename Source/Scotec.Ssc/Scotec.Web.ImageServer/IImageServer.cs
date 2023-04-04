namespace Scotec.Web.ImageServer
{
    public interface IImageServer
    {
        Stream GetImage(string path);
    }
}
