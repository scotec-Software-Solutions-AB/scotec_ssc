using System.Globalization;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Scotec.Extensions.Linq;

namespace Scotec.Web.Robots.Sitemap;

public class SitemapProvider : ISitemapProvider
{
    private readonly IEnumerable<ISitemapEntryProvider> _entryProviders;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISitemapOptions _options;

    public SitemapProvider(IEnumerable<ISitemapEntryProvider> entryProviders, ISitemapOptions options, IHttpContextAccessor httpContextAccessor)
    {
        _entryProviders = entryProviders;
        _options = options;
        _httpContextAccessor = httpContextAccessor;
    }

    public SitemapResult Build()
    {
        return Build(string.Empty);
    }


    public SitemapResult Build(string culture)
    {
        return Build(CultureInfo.GetCultureInfo(culture));
    }

    public SitemapResult Build(CultureInfo culture)
    {
        var document = new XDocument(new XDeclaration("1.0", "UTF-8", null));
        XNamespace xmlNamespace = @"http://www.sitemaps.org/schemas/sitemap/0.9";

        var urlSet = AddRoot(document, xmlNamespace, "urlset");

        var host = _httpContextAccessor.HttpContext!.Request.Host;

        if (_options.SupportedCultures.Contains(culture))
        {
            _entryProviders.SelectMany(provider => provider.GetEntries(culture))
                           .ForAll(entry =>
                           {
                               var url = new XElement(xmlNamespace + "url");
                               urlSet.Add(url);

                               //var path = culture.Equals(CultureInfo.InvariantCulture)
                               //    ? entry.Location
                               //    : entry.Location.Replace("{language}", culture.Name);

                               url.Add(new XElement(xmlNamespace + "loc", _options.Protocoll + host + entry.Location));
                               if (entry.LastModified != null)
                                   url.Add(new XElement(xmlNamespace + "lastmod", entry.LastModified.Value.ToString("yyyy-MM-dd")));
                               if (entry.ChangeFrequency != null)
                                   url.Add(new XElement(xmlNamespace + "changefreq", entry.ChangeFrequency.Value.ToString()));
                               if (entry.Priority != null)
                                   url.Add(new XElement(xmlNamespace + "priority", entry.Priority.Value.ToString("0.0", CultureInfo.InvariantCulture)));
                           });
        }

        return new SitemapResult(document);
    }

    public SitemapResult Build(CultureInfo[] cultures)
    {
        var document = new XDocument(new XDeclaration("1.0", "UTF-8", null));
        XNamespace xmlNamespace = @"http://www.sitemaps.org/schemas/sitemap/0.9";

        var urlSet = AddRoot(document, xmlNamespace, "sitemapindex");

        var host = _httpContextAccessor.HttpContext!.Request.Host;
        foreach(var culture in cultures) 
        {
           var sitemap = new XElement(xmlNamespace + "sitemap");
           urlSet.Add(sitemap);

            sitemap.Add(new XElement(xmlNamespace + "loc", $"{_options.Protocoll}{host}/{culture.Name}/sitemap.xml"));
        }

        return new SitemapResult(document);
    }

    private static XElement AddRoot(XDocument document, XNamespace xmlNamespace, string root)
    {
        var element = new XElement(xmlNamespace + root);

        document.Add(element);

        return element;
    }
}