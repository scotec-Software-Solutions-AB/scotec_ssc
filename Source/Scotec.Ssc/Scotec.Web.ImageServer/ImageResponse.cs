namespace Scotec.Web.ImageServer
{
    public struct ImageResponse
    {
        public string Path { get; init; }

        public int? Width { get; init; }

        public int? Height { get; init; }

        public ImageFormat Format { get; init; } 

        public Stream Image { get; init; }
    }
}
