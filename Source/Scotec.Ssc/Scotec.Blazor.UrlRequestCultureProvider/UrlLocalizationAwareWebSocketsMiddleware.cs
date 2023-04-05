using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
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
    /*
     * Localization Extensibility
     * https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization-extensibility?view=aspnetcore-3.1
     *
     * Write custom ASP.NET Core middleware
     * https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/write?view=aspnetcore-3.1
     *
     *
     * https://docs.microsoft.com/en-us/aspnet/core/blazor/advanced-scenarios?view=aspnetcore-3.1#blazor-server-circuit-handler
     */

    protected internal static readonly ConcurrentDictionary<string, string> CultureByConnectionTokens = new();
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
            string[] { Length: 2 } x
                when x[0] == "_blazor" && x[1] == "negotiate"
                                       && httpContext.Request.Method == "POST"
                => BlazorNegotiate,

            string[] { Length: 1 } x
                when x[0] == "_blazor"
                     && httpContext.Request.QueryString.HasValue
                     && httpContext.Request.Method == "GET"
                => BlazorHeartbeat,

            string[] { Length: 0 } x
                => Redirect,

            string[] { Length: > 0 } x
                when x[0] != "_blazor"
                     && !_options.Value.SupportedUICultures!.Select(info => info.Name)
                                 .Contains(x[0])
                => Redirect,

            //_ => _next
            _ => (RequestDelegate)SetCulture
        };

        await nextAction(httpContext);
    }

    private async Task SetCulture(HttpContext httpContext)
    {
        var currentCulture = httpContext.GetCultureFromRequest();
        if (string.IsNullOrEmpty(currentCulture))
        {
            currentCulture = httpContext.GetCultureFromReferer();
        }

        if (!string.IsNullOrEmpty(currentCulture))
        {
            var culture = new CultureInfo(currentCulture);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }
        await _next(httpContext);
    }

    private Task Redirect(HttpContext context)
    {
        //Get preferred language
        var preferredLanguage = GetPreferredLanguage(context.Request.Headers.AcceptLanguage);

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

        context.Response.Redirect($"/{string.Join('/', segments)}", false);

        return Task.CompletedTask;
    }

    private bool IsValidCulture(string culture)
    {
        return new CultureInfo("culture").Name != "";
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
    ///     On blazor heartbeat, set the culture based on the dictionary entry created in
    ///     <see cref="BlazorNegotiate(HttpContext httpContext)" />.
    /// </summary>
    private async Task BlazorHeartbeat(HttpContext httpContext)
    {
        var components = QueryHelpers.ParseQuery(httpContext.Request.QueryString.Value);
        var connectionToken = components["id"];
        var currentCulture = CultureByConnectionTokens[connectionToken];

        var culture = new CultureInfo(currentCulture);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        await _next(httpContext);

        if (httpContext.Response.StatusCode == StatusCodes.Status101SwitchingProtocols)
        {
            // When "closing" the SignalR connection (websocket) clean-up the memory by removing the
            // token from the dictionary.
            CultureByConnectionTokens.TryRemove(connectionToken, out var _);
        }
    }

    /// <summary>
    ///     On blazor negotiate, set the culture based on the referer and save it to a
    ///     dictionary to be used by the Blazor heartbeat
    ///     <see cref="BlazorHeartbeat(HttpContext)" />.
    /// </summary>
    private async Task BlazorNegotiate(HttpContext httpContext)
    {
        var currentCulture = httpContext.GetCultureFromReferer();

        // Set the culture
        var culture = new CultureInfo(currentCulture);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        // Enable the rewinding of the body after the action has been called
        httpContext.Request.EnableBuffering();

        // Save the reference of the response body
        var originalResponseBodyStream = httpContext.Response.Body;
        using var responseBody = new MemoryStream();
        httpContext.Response.Body = responseBody;

        await _next(httpContext);

        // Temporary unwarp the response body to get the connectionToken
        var responseBodyContent = await ReadResponseBodyAsync(httpContext.Response);

        if (httpContext.Response.ContentType == "application/json")
        {
            var root = JsonSerializer
                .Deserialize<BlazorNegociateBody>(responseBodyContent);
            CultureByConnectionTokens[root!.ConnectionToken] = currentCulture;
        }

        // Rewind the response body as if we hadn't upwrap-it
        await responseBody.CopyToAsync(originalResponseBodyStream);

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
