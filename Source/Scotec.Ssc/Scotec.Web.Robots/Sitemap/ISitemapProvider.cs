using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Scotec.Web.Robots.Sitemap
{
    public interface ISitemapProvider
    {
        SitemapResult Build();
    }
}
