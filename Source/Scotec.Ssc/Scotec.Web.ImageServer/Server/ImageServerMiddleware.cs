using System.Net;
using Microsoft.Extensions.Options;

namespace Scotec.Web.ImageServer.Server;

public class ImageServerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IOptions<ImageServerOptions> _options;

    public ImageServerMiddleware(RequestDelegate next, IOptions<ImageServerOptions> options)
    {
        _next = next;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext httpContext, IImageServer imageServer)
    {
        var query = httpContext.Request.Query;
        var path = httpContext.Request.Path.Value ?? string.Empty;

        int? width = null;
        int? height = null;

        if (!imageServer.CanProcessImage(path))
        {
            // Could not find an image under the given path. Let the next middleware handle the request.
            await _next(httpContext);
            return;
        }

        if (query.TryGetValue("width", out var widthString) && !string.IsNullOrEmpty(widthString))
            width = int.Parse(widthString!);

        if (query.TryGetValue("height", out var heightString) && !string.IsNullOrEmpty(heightString))
            height = int.Parse(heightString!);


        var imageData = await imageServer.GetImageAsync(path, width, height);
        var response = httpContext.Response;
        response.ContentType = $"image/{imageData.Format.ToString().ToLower()}";
        response.StatusCode = (int)HttpStatusCode.OK;

        await response.BodyWriter.WriteAsync(imageData.Image);
    }
}