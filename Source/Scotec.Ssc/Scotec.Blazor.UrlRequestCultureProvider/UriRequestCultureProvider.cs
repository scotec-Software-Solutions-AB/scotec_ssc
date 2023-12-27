using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Scotec.Blazor.UrlRequestCultureProvider
{
    public class UriRequestCultureProvider : RequestCultureProvider
    {
        public UriRequestCultureProvider(IOptions<RequestLocalizationOptions> options)
        {
            Options = options?.Value;
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
            {
                requestCulture = Thread.CurrentThread.CurrentUICulture;//Options?.DefaultRequestCulture.UICulture;
            }
            else
            {
                requestCulture =
                    Options?.SupportedCultures?.FirstOrDefault(culture =>
                        culture.TwoLetterISOLanguageName == segments[0])
                ?? Thread.CurrentThread.CurrentUICulture;

                //culture = GetPreferredLanguage(httpContext.Request.Headers.AcceptLanguage);
            }

            if (requestCulture == null)
            {
                return Task.FromResult<ProviderCultureResult?>(default);
            }
            return Task.FromResult(new ProviderCultureResult(requestCulture.Name, requestCulture.Name))!;
        }

        private CultureInfo GetPreferredLanguage(string? acceptLanguages)
        {
            if (Options == null || string.IsNullOrEmpty(acceptLanguages))
            {
                return CultureInfo.InvariantCulture;
            }

            if (!string.IsNullOrEmpty(acceptLanguages))
            {
                var supportedLanguages = Options.SupportedCultures;
                var culture = acceptLanguages.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(language => new AcceptLanguage(language))
                    .OrderByDescending(language => language.Quality)
                    .Select(language => language.Culture)
                    .FirstOrDefault(culture => supportedLanguages!.Contains(culture),
                        Options.DefaultRequestCulture.UICulture);

                return culture;
            }

            return Options.DefaultRequestCulture.UICulture;
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


}
