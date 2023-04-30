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
        var query = httpContext.Request.Query;

        int? width = null;
        int? height = null;
        if(query.ContainsKey("width"))
        {
            width = int.Parse(query["width"]);
        }
        if(query.ContainsKey("height"))
        {
            height = int.Parse(query["height"]);
        }

        var imageData = await imageServer.GetImageInfoAsync(httpContext.Request.Path.Value!, width, height);
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
