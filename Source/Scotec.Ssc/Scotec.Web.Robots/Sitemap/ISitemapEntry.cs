namespace Scotec.Web.Robots.Sitemap;

public interface ISitemapEntry
{
    string Location { get; set; }

    DateTime? LastModified { get; set; }

    ChangeFrequency? ChangeFrequency { get; set; }

    double? Priority { get; set; }
}