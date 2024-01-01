namespace Scotec.Web.ImageServer;

public struct ImageRequest
{
    public string Path { get; init; }

    public int? Width { get; init; }

    public int? Height { get; init; }

    public ImageFormat? Format { get; init; }
}