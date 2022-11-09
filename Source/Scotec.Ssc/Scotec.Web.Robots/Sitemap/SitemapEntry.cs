namespace Scotec.Web.Robots.Sitemap;

internal class SitemapEntry : ISitemapEntry
{
    private string _location = string.Empty;

    public string Location
    {
        get
        {
            if (string.IsNullOrEmpty(_location))
                throw new MemberAccessException("Location must not be empty.");
            return _location;
        }
        set
        {
            if (string.IsNullOrEmpty(value))
                throw new MemberAccessException("Location must not be empty.");

            _location = value;
        }
    }

    public DateTime? LastModified { get; set; }

    public ChangeFrequency? ChangeFrequency { get; set; }

    public double? Priority { get; set; }
}