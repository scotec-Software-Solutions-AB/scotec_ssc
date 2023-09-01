using System.Globalization;

namespace Scotec.Web.Robots.Sitemap;

public interface ISitemapProvider
{
    /// <summary>
    /// Builds a sitemap.
    /// </summary>
    SitemapResult Build();

    /// <summary>
    /// Builds a sitemap for the given language.
    /// </summary>
    /// <param name="culture">The culture for which to build a sitemap</param>
    SitemapResult Build(string culture);

    /// <summary>
    /// Builds a sitemap for the given language.
    /// </summary>
    /// <param name="culture">The culture for which to build a sitemap</param>
    SitemapResult Build(CultureInfo culture);

    /// <summary>
    /// Builds a sitemap index for the given languages.
    /// </summary>
    /// <param name="cultures">The cultures for which to build a sitemap index</param>
    SitemapResult Build(CultureInfo[] cultures);
}