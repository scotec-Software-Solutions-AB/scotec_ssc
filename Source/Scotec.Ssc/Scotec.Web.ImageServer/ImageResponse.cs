namespace Scotec.Web.ImageServer;

public class ImageResponse
{
    public ImageResponse(string path)
    {
        Path = path;
        Format = ImageFormat.None;
        Image = Array.Empty<byte>();
    }

    //public ImageResponse(ImageResponse response)
    //{
    //    Path = response.Path;
    //    Width = response.Width;
    //    Height = response.Height;
    //    Format = response.Format;
    //    Id = response.Id;
    //    Image = CloneStream(response.Image);
    //    Timestamp = response.Timestamp;

    //}

    public string Path { get; }

    public int? Width { get; init; }

    public int? Height { get; init; }

    public ImageFormat Format { get; init; }

    public byte[] Image { get; set; }

    public Guid Id { get; set; }

    public DateTime Timestamp { get; set; }

    //private static Stream? CloneStream(Stream? stream)
    //{
    //    if (stream == null)
    //    {
    //        return null;
    //    }
    //    stream.Position = 0;
    //    var memoryStream = new MemoryStream();
    //    stream.CopyTo(memoryStream);
    //    memoryStream.Position = 0;
    //    stream.Position = 0;

    //    return memoryStream;
    //}
}