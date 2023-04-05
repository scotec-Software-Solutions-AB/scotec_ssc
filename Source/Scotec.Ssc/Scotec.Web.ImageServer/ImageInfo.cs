namespace Scotec.Web.ImageServer
{
    public struct ImageInfo
    {
        public ImageFormat ImageSourceFormat { get; set; }

        public ImageFormat ImageTargetFormat { get; set; }

        public string Path { get; set; }

        public Stream ImageStream { get; set; }

        public int? Width { get; set; } 

        public int? Height { get; set; } 

    }
}
