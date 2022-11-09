namespace Scotec.Web.Robots.Sitemap;

public interface ISitemapEntryProvider
{
    IEnumerable<ISitemapEntry> Entries { get; }
}