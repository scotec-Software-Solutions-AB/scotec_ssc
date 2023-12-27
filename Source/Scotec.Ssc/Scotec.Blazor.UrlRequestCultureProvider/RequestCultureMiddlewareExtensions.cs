using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
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

        var requestCultureProvider = builder.ApplicationServices.GetRequiredService<UriRequestCultureProvider>();
        options.AddInitialRequestCultureProvider(requestCultureProvider);

        // Do not pass the options to UseMiddleware(). Instead, register them as a singleton during service registration. Passing the options as parameters
        // to UseMiddleware() does not register the options as a service and makes them available only to the middleware.
        // IOptions<> is registered as a singleton by default and can be injected into any service lifetime. However, if it has not been
        // registered before, the service locator returns a default instance of the options.
        return builder.UseMiddleware<UrlLocalizationAwareWebSocketsMiddleware>(Options.Create(options))
        //.UseMiddleware<UrlLocalizationAwareWebSocketsMiddleware>(Options.Create(options))
        //.UseRequestLocalization(options);
        .UseRequestLocalization(options); 
    }

    public static IServiceCollection AddUrlRequestLocalization(this IServiceCollection services, LanguageOptions languageOptions)
    {
        services.AddTransient<UriRequestCultureProvider>();

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture(languageOptions.DefaultLanguage);
            options.SupportedCultures = languageOptions.Languages;
            options.SupportedUICultures = languageOptions.Languages;
            //options.AddInitialRequestCultureProvider(new UriRequestCultureProvider());
            //options.ApplyCurrentCultureToResponseHeaders = true;
        });

        return services;
    }
}

