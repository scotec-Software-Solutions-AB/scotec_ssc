using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scotec.Web.Robots.Sitemap
{
    public class PriorityAttribute : Attribute
    {
        public PriorityAttribute()
        {

        }

        public PriorityAttribute(double priority)
        {
            Priority = priority;
        }

        public double Priority { get; set; } = 0.5;

    }
}
