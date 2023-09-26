namespace Scotec.Web.ImageServer;

public record struct ImageResponse
{
    private readonly ImageFormat _format;
    public string Path { get; init; }

    public int? Width { get; init; }

    public int? Height { get; init; }

    public readonly ImageFormat Format
    {
        get => Image != null ? _format : ImageFormat.None;
        init => _format = value;
    }

    public Stream? Image { get; set; }

    public Guid Id { get; set; }
}