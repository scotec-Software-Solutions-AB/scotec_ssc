using System.Globalization;

namespace Scotec.Web.Robots.Sitemap;

public class SitemapOptions : ISitemapOptions
{
    public SitemapOptions(Action<SitemapOptions>? options)
    {
        SupportedCultures = new[] { CultureInfo.InvariantCulture };

        if (options != null)
        {
            options(this);
        }
    }

    public ChangeFrequency? ChangeFrequency { get; set; }

    public double? Priority { get; set; }

    public DateTime? LastModified { get; set; }

    public string Protocoll { get; set; } = "https://";

    public CultureInfo[] SupportedCultures { get; set; }
}
