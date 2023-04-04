using System.Net;
using Microsoft.Extensions.Options;

namespace Scotec.Web.ImageServer;

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
        var imageData = imageServer.GetImage(httpContext.Request.Path.Value!);
        if (imageData.ImageType == ImageType.None)
        {
            await _next(httpContext);
            return;
        }
        
        var response = httpContext.Response;
        response.ContentType = $"image/{imageData.ImageType}";
        response.StatusCode = (int)HttpStatusCode.OK;

        await imageData.ImageStream.CopyToAsync(response.BodyWriter.AsStream());
    }

}
