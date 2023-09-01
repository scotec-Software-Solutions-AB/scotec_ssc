using System.Diagnostics;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

namespace Scotec.Blazor.UrlRequestCultureProvider;

public static class RequestCultureMiddlewareExtensions
{
    public static IApplicationBuilder UseUrlRequestLocalization(this IApplicationBuilder builder)
    {
        var options = builder.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>()!.Value;
        // Do not use the default culture providers.
        options.RequestCultureProviders.Clear();
        options.ApplyCurrentCultureToResponseHeaders = true;

        options.AddInitialRequestCultureProvider(new CustomRequestCultureProvider(async context =>
        {
            var currentCulture = context.GetCultureFromRequest();

            var requestCulture = new ProviderCultureResult(currentCulture, currentCulture);

            return await Task.FromResult(requestCulture);
        }));

        // Do not pass the options to UseMiddleware(). Instead, register them as a singleton during service registration. Passing the options as parameters
        // to UseMiddleware() does not register the options as a service and makes them available only to the middleware.
        // IOptions<> is registered as a singleton by default and can be injected into any service lifetime. However, if it has not been
        // registered before, the service locator returns a default instance of the options.
        return builder.UseMiddleware<UrlLocalizationAwareWebSocketsMiddleware>();
        //.UseMiddleware<UrlLocalizationAwareWebSocketsMiddleware>(Options.Create(options))
        //.UseRequestLocalization(options);
    }
}
