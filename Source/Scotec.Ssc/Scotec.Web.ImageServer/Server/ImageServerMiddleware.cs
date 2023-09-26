using System.Diagnostics;
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

        int? width = null;
        int? height = null;

        if (query.TryGetValue("width", out var widthString) && !string.IsNullOrEmpty(widthString))
        {
            width = int.Parse(widthString!);
        }

        if (query.TryGetValue("height", out var heightString) && !string.IsNullOrEmpty(heightString))
        {
            height = int.Parse(heightString!);
        }

        var imageData = await imageServer.GetImageAsync(httpContext.Request.Path.Value!, width, height);
        if (imageData.Format == ImageFormat.None)
        {
            // Could not find an image under the given path. Let the next middleware handle the request.
            await _next(httpContext);
            return;
        }

        var response = httpContext.Response;
        response.ContentType = $"image/{imageData.Format.ToString().ToLower()}";
        response.StatusCode = (int)HttpStatusCode.OK;

        Debug.Assert(imageData.Image != null, "imageData.Image != null");
        await imageData.Image.CopyToAsync(response.BodyWriter.AsStream());
    }
}