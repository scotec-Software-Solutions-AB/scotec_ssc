using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Scotec.Blazor.UrlRequestCultureProvider;

/// <remarks>
///     Microsoft recommends the use of a cookie to ensures that the WebSocket connection can
///     correctly propagate the culture.
///     If localization schemes are based on the URL path or query string, the scheme might not
///     be able to work with WebSockets, thus fail to persist the culture.
///     Therefore, use of a localization culture cookie is the recommended approach.
///     This is not the approach we want as people can have multiple browser screens (browser tab or
///     iframe) using different languages.
/// </remarks>
public class UrlLocalizationAwareWebSocketsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IOptions<RequestLocalizationOptions> _options;

    /// <summary>
    ///     Initializes a new instance of the <see cref="UrlLocalizationAwareWebSocketsMiddleware" /> class.
    /// </summary>
    /// <param name="next">
    ///     The delegate representing the remaining middleware in the request pipeline.
    /// </param>
    /// <param name="options">Language options</param>
    public UrlLocalizationAwareWebSocketsMiddleware(RequestDelegate next, IOptions<RequestLocalizationOptions> options)
    {
        _next = next;
        _options = options;
    }

    /// <summary>
    ///     Handles the requests and returns a Task that represents the execution of the middleware.
    /// </summary>
    /// <param name="httpContext">
    ///     The HTTP context representing the request.
    /// </param>
    public async Task InvokeAsync(HttpContext httpContext)
    {
        var segments = httpContext
                       .Request
                       .Path
                       .Value!
                       .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

        var nextAction = segments switch
        {
            // Files under the path _framework are not handled by the StaticFileMiddleware.
            // However, we won't handle such files here as well.
            string[] { Length: > 0 } x
                when x.Contains("_framework")
                => _next,

            ["_blazor", "negotiate"] when httpContext.Request.Method == "POST"
                => BlazorNegotiate,

            // No path provided, so we need to redirect to a language specific uri such /de or /en. 
            string[] { Length: 0 } x
                => Redirect,

            // Check if the path points to a supported language. If not, redirect.
            string[] { Length: > 0 } x
                when x[0] != "_blazor"
                     && !_options.Value.SupportedUICultures!.Select(info => info.Name)
                                 .Contains(x[0])
                => Redirect,

            // Nothing to do here.
            _ => _next
        };

        await nextAction(httpContext);
    }

    private Task Redirect(HttpContext context)
    {
        //Get preferred language
        var preferredLanguage = GetPreferredLanguage(context.Request.Headers.AcceptLanguage!);

        var segments = context
                       .Request
                       .Path
                       .Value!
                       .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length > 0 && segments[0].Length == 0) // IsValidCulture(segments[0]))
        {
            segments[0] = preferredLanguage.Name;
        }
        else
        {
            segments = new[] { preferredLanguage.Name }.Concat(segments).ToArray();
        }

        var protocol = context.Request.IsHttps ? "https" : "http";
        context.Response.Redirect($"{protocol}://{context.Request.Host}/{string.Join('/', segments)}", false);

        return Task.CompletedTask;
    }

    private CultureInfo GetPreferredLanguage(string acceptLanguage)
    {
        if (!string.IsNullOrEmpty(acceptLanguage))
        {
            var supportedLanguages = _options.Value.SupportedCultures;
            var culture = acceptLanguage.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(language => new AcceptLanguage(language))
                                        .OrderByDescending(language => language.Quality)
                                        .Select(language => language.Culture)
                                        .FirstOrDefault(culture => supportedLanguages!.Contains(culture),
                                            _options.Value.DefaultRequestCulture.UICulture);

            return culture;
        }

        return _options.Value.DefaultRequestCulture.UICulture;
    }

    /// <summary>
    ///     On blazor negotiate, set the culture based on the referer and save it to a dictionary.
    /// </summary>
    private async Task BlazorNegotiate(HttpContext httpContext)
    {
        var currentCulture = httpContext.GetCultureFromReferer();
        if (currentCulture == null)
        {
            return;
        }

        // Enable the rewinding of the body after the action has been called
        httpContext.Request.EnableBuffering();

        // Save the reference of the response body
        var originalResponseBodyStream = httpContext.Response.Body;
        using var responseBody = new MemoryStream();
        httpContext.Response.Body = responseBody;

        await _next(httpContext);

        // Temporary unwrap the response body to get the connectionToken
        var responseBodyContent = await ReadResponseBodyAsync(httpContext.Response);

        if (httpContext.Response.ContentType == "application/json")
        {
            var root = JsonSerializer
                .Deserialize<BlazorNegotiateBody>(responseBodyContent);
            CultureByConnectionTokens.AddToken(root!.ConnectionToken, currentCulture);
        }

        // Rewind the response body as if we hadn't unwrap-it
        await responseBody.CopyToAsync(originalResponseBodyStream);
        return;

        static async Task<string> ReadResponseBodyAsync(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(response.Body, leaveOpen: true);
            var bodyAsText = await reader.ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }
    }

    private class AcceptLanguage
    {
        public AcceptLanguage(string acceptLanguage)
        {
            var parts = acceptLanguage.Split(new[] { ';', '=' }, StringSplitOptions.RemoveEmptyEntries);
            Culture = CultureInfo.GetCultureInfo(parts[0]);
            if (parts.Length == 3)
            {
                double.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var value);
                Quality = value;
            }
            else
            {
                Quality = 1.0;
            }
        }

        public CultureInfo Culture { get; }
        public double Quality { get; }
    }
}
