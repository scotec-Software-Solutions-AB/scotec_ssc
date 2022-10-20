using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scotec.Web.Robots.Sitemap
{
    public interface ISitemapEntryProvider
    {
        IEnumerable<ISitemapEntry> Entries { get; }
    }
}
