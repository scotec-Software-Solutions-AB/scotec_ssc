using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Scotec.Extensions.Linq;
using RouteAttribute = Microsoft.AspNetCore.Components.RouteAttribute;

namespace Scotec.Web.Robots.Sitemap;

public class SitemapEntryProvider : ISitemapEntryProvider
{
    private readonly ISitemapOptions _options;

    public SitemapEntryProvider(ISitemapOptions options)
    {
        _options = options;
    }

    public IEnumerable<ISitemapEntry> Entries => CollectSitemapEntries(CultureInfo.InvariantCulture);
    public IEnumerable<ISitemapEntry> GetEntries(CultureInfo culture)
    {
        return CollectSitemapEntries(culture);
    }

    private IEnumerable<ISitemapEntry> CollectSitemapEntries(CultureInfo culture)
    {
        var pages = GetPages();

        foreach (var page in pages)
        {
            if (page.HasCustomAttribute<NoSitemapAttribute>(true))
                continue;

            var route = page.GetCustomAttribute<RouteAttribute>(true)?.Template;
            if (route == null)
                continue;


            if(!culture.Equals(CultureInfo.InvariantCulture))
            {
                route = route.Replace("{language}", culture.Name);
            }

            var changeFrequency = page.GetCustomAttribute<ChangeFrequencyAttribute>(true)?.ChangeFrequency;
            var lastModified = page.GetCustomAttribute<LastModifiedAttribute>(true)?.AsDateTime();
            var priority = page.GetCustomAttribute<PriorityAttribute>(true)?.Priority;

            yield return new SitemapEntry
            {
                Location = route,
                ChangeFrequency = changeFrequency ?? _options.ChangeFrequency,
                LastModified = lastModified ?? _options.LastModified,
                Priority = priority ?? _options.Priority
            };
        }
    }

    private IEnumerable<Type> GetPages()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic);

        var pages = assemblies.SelectMany(assembly => assembly.ExportedTypes
                .Where(p => p.IsSubclassOf(typeof(ComponentBase)) /* && p.Namespace == "Scotec.WebApp.Pages"*/))
            .ToList();

        return pages;
    }
}