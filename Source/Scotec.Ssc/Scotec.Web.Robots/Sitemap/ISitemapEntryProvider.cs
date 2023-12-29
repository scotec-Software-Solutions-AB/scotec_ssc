using System.Globalization;

namespace Scotec.Web.Robots.Sitemap;

public interface ISitemapEntryProvider
{
    IEnumerable<ISitemapEntry> Entries { get; }
    IEnumerable<ISitemapEntry> GetEntries(CultureInfo culture);
}