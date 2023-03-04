namespace Scotec.Web.Robots.Sitemap;

public interface ISitemapProvider
{
    /// <summary>
    /// Builds a sitemap.
    /// </summary>
    SitemapResult Build();
    
    /// <summary>
    /// Builds a sitemap for the ´given language.
    /// </summary>
    /// <param name="language">The language fpr which to build a sitemap</param>
    SitemapResult Build(string? language);
}