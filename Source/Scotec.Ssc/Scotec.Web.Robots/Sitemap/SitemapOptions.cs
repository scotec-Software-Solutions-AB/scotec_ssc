
namespace Scotec.Web.Robots.Sitemap
{
    public class SitemapOptions : ISitemapOptions
    {
        public SitemapOptions(Action<SitemapOptions>? options)
        {
            if(options != null)
                options(this);
        }

        public ChangeFrequency? ChangeFrequency { get; set; }

        public double? Priority { get; set; }

        public DateTime? LastModified { get; set; }

    }
}
