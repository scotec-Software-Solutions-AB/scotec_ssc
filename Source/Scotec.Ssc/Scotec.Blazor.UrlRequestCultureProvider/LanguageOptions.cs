using System.Globalization;

namespace Scotec.Blazor.UrlRequestCultureProvider
{
    public class LanguageOptions(CultureInfo[] languages, CultureInfo defaultLanguage)
    {
        public CultureInfo[] Languages { get; } = languages;

        public CultureInfo DefaultLanguage { get; } = defaultLanguage;
    }
}
