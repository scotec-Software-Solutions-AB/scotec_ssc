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
        return Build(null);
    }


    public SitemapResult Build(string? language)
    {
        var document = new XDocument(new XDeclaration("1.0", "UTF-8", null));
        XNamespace xmlNamespace = @"http://www.sitemaps.org/schemas/sitemap/0.9";

        var urlSet = AddUrlSet(document, xmlNamespace);

        var host = _httpContextAccessor.HttpContext!.Request.Host;
        _entryProviders.SelectMany(provider => provider.Entries)
            .ForAll(entry =>
            {
                var url = new XElement(xmlNamespace + "url");
                urlSet.Add(url);

                var path = string.IsNullOrEmpty(language)
                    ? entry.Location
                    : entry.Location.Replace("{language}", language);

                url.Add(new XElement(xmlNamespace + "loc", _options.Protocoll + host + path ));
                if (entry.LastModified != null)
                    url.Add(new XElement(xmlNamespace + "lastmod", entry.LastModified.Value.ToString("yyyy-MM-dd")));
                if (entry.ChangeFrequency != null)
                    url.Add(new XElement(xmlNamespace + "changefreq", entry.ChangeFrequency.Value.ToString()));
                if (entry.Priority != null)
                    url.Add(new XElement(xmlNamespace + "priority", entry.Priority.Value.ToString("0.0", CultureInfo.InvariantCulture)));
            });

        return new SitemapResult(document);
    }

    private XElement AddUrlSet(XDocument document, XNamespace xmlNamespace)
    {
        XNamespace ns = @"http://www.sitemaps.org/schemas/sitemap/0.9";

        var element = new XElement(ns + "urlset");

        document.Add(element);

        return element;
    }
}