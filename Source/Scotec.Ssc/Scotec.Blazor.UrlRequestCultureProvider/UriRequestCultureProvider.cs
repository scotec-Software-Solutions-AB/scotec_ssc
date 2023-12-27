using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Globalization;

namespace Scotec.Blazor.UrlRequestCultureProvider
{
    public class UriRequestCultureProvider : RequestCultureProvider
    {
        protected internal static readonly ConcurrentDictionary<string, string> CultureByConnectionTokens = new();

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
            else if (segments.Length >= 2 && segments[0] == "_blazor" && segments[1] == "negotiate" && httpContext.Request.Method == "POST")
            {
                requestCulture = GetCultureFromReferer(httpContext);
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

        private static CultureInfo? GetCultureFromReferer(HttpContext httpContext)
        {
            var referer = httpContext.Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referer))
            {
                return null;
            }
            var uri = new Uri(referer);

            return new CultureInfo(GetCultureFromPath(uri.LocalPath));
        }

        private static string GetCultureFromPath(string? path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var segments = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                if (segments.Length >= 1 && segments[0].Length == 2)
                {
                    var currentCulture = segments[0];
                    return currentCulture;
                }
            }

            return string.Empty;
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
