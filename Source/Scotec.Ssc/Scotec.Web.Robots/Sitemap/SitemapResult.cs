using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Scotec.Web.Robots.Sitemap;

public class SitemapResult
{
    private readonly XDocument _sitemap;

    public SitemapResult(XDocument sitemap)
    {
        _sitemap = sitemap;
    }

    public Memory<byte> Content
    {
        get
        {
            using var memory = new MemoryStream();
            using var writer = new XmlTextWriter(memory, Encoding.UTF8);
            writer.Formatting = Formatting.None;
            _sitemap.WriteTo(writer);
            writer.Flush();
            memory.Flush();
            memory.Seek(0, SeekOrigin.Begin);

            using var reader = new StreamReader(memory);
            var xml = reader.ReadToEnd();
            return Encoding.UTF8.GetBytes(xml).AsMemory();
        }
    }
}