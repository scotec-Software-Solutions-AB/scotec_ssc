namespace Scotec.Web.ImageServer;

public record struct ImageResponse
{
    public string Path { get; init; }

    public int? Width { get; init; }

    public int? Height { get; init; }

    public ImageFormat Format { get; init; }

    public Stream Image { get; set; }

    public Guid Id { get; set; }
}