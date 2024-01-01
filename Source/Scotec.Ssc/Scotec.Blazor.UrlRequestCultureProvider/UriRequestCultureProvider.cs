using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

namespace Scotec.Blazor.UrlRequestCultureProvider;

public class UriRequestCultureProvider : RequestCultureProvider
{
    public UriRequestCultureProvider(IOptions<RequestLocalizationOptions> options)
    {
        Options = options.Value;
    }

    public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        var segments = httpContext
            .Request
            .Path
            .Value!
            .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);


        CultureInfo? requestCulture;
        if (segments.Length == 0)
            requestCulture = Thread.CurrentThread.CurrentUICulture; //Options?.DefaultRequestCulture.UICulture;
        else if (segments is ["_blazor", "negotiate", ..] && httpContext.Request.Method == "POST")
            requestCulture = httpContext.GetCultureFromReferer();
        else if (segments is ["_blazor"])
            requestCulture = httpContext.Request.Query.TryGetValue("id", out var id)
                ? CultureByConnectionTokens.GetCulture(id.ToString())
                : null;
        else
            requestCulture = Options?.SupportedCultures?.FirstOrDefault(culture =>
                                 culture.TwoLetterISOLanguageName == segments[0])
                             ?? Thread.CurrentThread.CurrentUICulture;

        if (requestCulture == null)
            return Task.FromResult<ProviderCultureResult?>(default);
        return Task.FromResult(new ProviderCultureResult(requestCulture.Name, requestCulture.Name))!;
    }
}