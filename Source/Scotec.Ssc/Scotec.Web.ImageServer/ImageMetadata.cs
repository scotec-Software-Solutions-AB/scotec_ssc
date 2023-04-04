namespace Scotec.Web.ImageServer
{
    public struct ImageMetadata
    {
        public ImageType ImageType { get; set; }

        public string Path { get; set; }

        public Stream ImageStream { get; set; }
    }
}
