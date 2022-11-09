namespace Scotec.Web.Robots.Sitemap;

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