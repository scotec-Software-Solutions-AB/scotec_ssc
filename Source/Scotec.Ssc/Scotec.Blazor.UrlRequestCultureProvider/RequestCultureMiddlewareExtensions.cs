using Microsoft.AspNetCore.Components.Server.Circuits;
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

        var requestCultureProvider = builder.ApplicationServices.GetRequiredService<UriRequestCultureProvider>();
        options.AddInitialRequestCultureProvider(requestCultureProvider);

        return builder.UseMiddleware<UrlLocalizationAwareWebSocketsMiddleware>(Options.Create(options))
            .UseRequestLocalization(options);
    }

    public static IServiceCollection AddUrlRequestLocalization(this IServiceCollection services,
        LanguageOptions languageOptions)
    {
        services.AddTransient<UriRequestCultureProvider>();
        services.AddScoped<CircuitHandler, RequestCultureCircuitHandler>();
        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture(languageOptions.DefaultLanguage);
            options.SupportedCultures = languageOptions.Languages;
            options.SupportedUICultures = languageOptions.Languages;
        });

        return services;
    }
}