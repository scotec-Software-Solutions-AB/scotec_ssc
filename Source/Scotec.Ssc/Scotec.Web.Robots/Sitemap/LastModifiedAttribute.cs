using System;
using System.Collections.Generic;

namespace Scotec.Web.Robots.Sitemap
{
    public class LastModifiedAttribute : Attribute
    {
        public LastModifiedAttribute()
        {
            LastModified = DateTime.UtcNow.ToString("yyyy-MM-dd");
        }

        public LastModifiedAttribute(string lastModified)
        {
            LastModified = lastModified;
        }

        public string LastModified { get; set; }

        public DateTime AsDateTime() => DateTime.Parse(LastModified);
    }
}
