
namespace Scotec.Web.Robots.Sitemap
{
    public class ChangeFrequencyAttribute : Attribute
    {
        public ChangeFrequencyAttribute()
        {

        }

        public ChangeFrequencyAttribute(ChangeFrequency changeFrequency)
        {
            ChangeFrequency = changeFrequency;
        }

        public ChangeFrequency ChangeFrequency { get; set; } = ChangeFrequency.Weekly;
    }
}
