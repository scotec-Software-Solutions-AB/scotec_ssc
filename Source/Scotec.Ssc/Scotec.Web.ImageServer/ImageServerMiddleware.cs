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
        var imageData = await imageServer.GetImageInfoAsync(httpContext.Request.Path.Value!);
        if (imageData == null)
        {
            await _next(httpContext);
            return;
        }
        
        var response = httpContext.Response;
        response.ContentType = $"image/{imageData.Value.Format.ToString().ToLower()}";
        response.StatusCode = (int)HttpStatusCode.OK;

        await imageData.Value.Image.CopyToAsync(response.BodyWriter.AsStream());
    }

}
