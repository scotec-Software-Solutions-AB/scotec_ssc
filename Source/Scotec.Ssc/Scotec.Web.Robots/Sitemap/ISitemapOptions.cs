using System.Globalization;

namespace Scotec.Web.Robots.Sitemap;

public interface ISitemapOptions
{
    ChangeFrequency? ChangeFrequency { get; set; }

    double? Priority { get; set; }

    DateTime? LastModified { get; set; }

    string Protocoll { get; set; }

    CultureInfo[] SupportedCultures { get; set; }
}