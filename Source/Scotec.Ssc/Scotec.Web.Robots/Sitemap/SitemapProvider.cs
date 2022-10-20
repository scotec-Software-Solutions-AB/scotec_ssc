using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Scotec.Extensions.Linq;
using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace Scotec.Web.Robots.Sitemap
{
    public class SitemapProvider : ISitemapProvider
    {
        IEnumerable<ISitemapEntryProvider> _entryProviders;
        IHttpContextAccessor _httpContextAccessor;

        public SitemapProvider(IEnumerable<ISitemapEntryProvider> entryProviders, IHttpContextAccessor httpContextAccessor)
        {
            _entryProviders = entryProviders;
            _httpContextAccessor = httpContextAccessor;
        }

        public SitemapResult Build()
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

                    url.Add(new XElement(xmlNamespace+ "loc", host + entry.Location));
                    if(entry.LastModified != null)
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
}
