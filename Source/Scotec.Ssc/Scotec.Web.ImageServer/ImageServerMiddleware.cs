using System.Net;
using Microsoft.Extensions.Options;

namespace Scotec.Web.ImageServer;

public class ImageServerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _environment;
    private readonly IOptions<ImageServerOptions> _options;
    private readonly IImageServer _imageServer;

    public ImageServerMiddleware(RequestDelegate next, IOptions<ImageServerOptions> options
                                 , IImageServer imageServer)
    {
        _next = next;
        _options = options;
        _imageServer = imageServer;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (!IsImageRequest(httpContext))
        {
            await _next(httpContext);
            return;
        }

        var imageStream = _imageServer.GetImage(httpContext.Request.Path.Value!);

        var response = httpContext.Response;
        response.ContentType = "image/png";
        response.StatusCode = (int)HttpStatusCode.OK;

        await imageStream.CopyToAsync(response.BodyWriter.AsStream());
    }

    private bool IsImageRequest(HttpContext httpContext)
    {
        var extension = Path.GetExtension(httpContext.Request.Path.Value);
        if (extension == ".png")
        {
            return true;
        }

        return false;
    }
}
