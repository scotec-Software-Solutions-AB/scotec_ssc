using Microsoft.AspNetCore.Components;
using Scotec.Extensions.Linq;

namespace Scotec.Web.Robots.Sitemap;

public class SitemapEntryProvider : ISitemapEntryProvider
{
    private readonly ISitemapOptions _options;

    public SitemapEntryProvider(ISitemapOptions options)
    {
        _options = options;
    }

    public IEnumerable<ISitemapEntry> Entries => CollectSitemapEntries();

    private IEnumerable<ISitemapEntry> CollectSitemapEntries()
    {
        var pages = GetPages();

        foreach (var page in pages)
        {
            if (page.HasCustomAttribute<NoSitemapAttribute>(true))
                continue;

            var route = page.GetCustomAttribute<RouteAttribute>(true)?.Template;
            if (route == null)
                continue;

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