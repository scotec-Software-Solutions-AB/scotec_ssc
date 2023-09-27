namespace Scotec.Web.ImageServer
{
    public class ImageServerException : ApplicationException
    {
        public ImageServerException(string message)
        : base(message)
        {
        }

        public ImageServerException(string message, Exception innerException)
        : base(message, innerException)
        {
        }
    }
}
