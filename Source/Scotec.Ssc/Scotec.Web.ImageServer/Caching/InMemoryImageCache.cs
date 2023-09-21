using System.Text;

namespace Scotec.Web.ImageServer.Caching;

public class InMemoryImageCache : IImageCache
{
    private readonly Dictionary<string, ImageResponse> _images = new();

    public ImageResponse AddImage(ImageResponse imageResponse)
    {
        var memoryStream = new MemoryStream();
        imageResponse.Image.CopyTo(memoryStream);

        memoryStream.Position = 0;
        imageResponse.Image = memoryStream;
        imageResponse.Id = Guid.NewGuid();

        _images.Add(BuildKey(imageResponse), imageResponse);

        return imageResponse;
    }

    public bool TryGetImage(ImageRequest imageRequest, out ImageResponse? imageResponse)
    {
        imageResponse = null;

        if (_images.TryGetValue(BuildKey(imageRequest), out var image))
        {
            image.Image = Clone(image.Image);
            imageResponse = image;
            return true;
        }

        return false;
    }

    private static Stream Clone(Stream stream)
    {
        stream.Position = 0;
        var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        memoryStream.Position = 0;

        return memoryStream;
    }

    private void ClearCache()
    {
        _images.Clear();
    }

    private static string BuildKey(ImageResponse response)
    {
        var builder = new StringBuilder();
        builder.Append(response.Path);
        builder.Append(response.Width);
        builder.Append(response.Height);

        return builder.ToString();
    }

    private static string BuildKey(ImageRequest request)
    {
        var builder = new StringBuilder();
        builder.Append(request.Path);
        builder.Append(request.Width);
        builder.Append(request.Height);

        return builder.ToString();
    }
}