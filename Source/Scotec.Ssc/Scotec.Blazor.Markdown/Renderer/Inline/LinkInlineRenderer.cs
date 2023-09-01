using Markdig.Renderers.Html;
using Markdig.Syntax.Inlines;

namespace Scotec.Blazor.Markdown.Renderer.Inline;

/// <summary>
///     A HTML renderer for a <see cref="LinkInline" />.
/// </summary>
/// <seealso cref="BlazorObjectRenderer{LinkInline}" />
public class LinkInlineRenderer : BlazorObjectRenderer<LinkInline>
{
    /// <summary>
    ///     Gets or sets a value indicating whether to always add rel="nofollow" for links or not.
    /// </summary>
    [Obsolete("AutoRelNoFollow is obsolete. Please write \"nofollow\" into Property Rel.")]
    public bool AutoRelNoFollow
    {
        get => Rel is not null && Rel.Contains("nofollow");
        set
        {
            const string rel = "nofollow";
            if (value)
            {
                if (string.IsNullOrEmpty(Rel))
                {
                    Rel = rel;
                }
                else if (!Rel!.Contains(rel))
                {
                    Rel += $" {rel}";
                }
            }
            else
            {
                Rel = Rel?.Replace(rel, string.Empty);
            }
        }
    }

    /// <summary>
    ///     Gets or sets the literal string in property rel for links
    /// </summary>
    public string Rel { get; set; }

    protected override void Write(BlazorRenderer renderer, LinkInline link)
    {
        if (link.IsImage)
        {
            WriteImage(renderer, link);
        }
        else
        {
            WriteAnchor(renderer, link);
        }
    }

    private void WriteAnchor(BlazorRenderer renderer, LinkInline link)
    {
        renderer.OpenElement("a");

        var attributes = link.GetAttributes();
        attributes.AddProperty("href", renderer.UrlEncode(GetUrl(link)));

        if (renderer.EnableHtmlForInline && !string.IsNullOrEmpty(link.Title))
        {
            attributes.AddProperty("title", link.Title);
        }

        if (renderer.EnableHtmlForInline)
        {
            if (!string.IsNullOrWhiteSpace(Rel))
            {
                attributes.AddProperty("rel", Rel);
            }
        }

        renderer.AddAttributes(link);
        renderer.WriteChildren(link);

        if (renderer.EnableHtmlForInline)
        {
            renderer.CloseElement();
        }
    }

    private static string GetUrl(LinkInline link)
    {
        return link.GetDynamicUrl != null ? link.GetDynamicUrl() ?? link.Url : link.Url;
    }

    private void WriteImage(BlazorRenderer renderer, LinkInline link)
    {
        renderer.OpenElement("img");
        var attributes = link.GetAttributes();
        attributes.AddProperty("src", GetUrl(link));

        if (renderer.EnableHtmlForInline)
        {
            attributes.AddProperty("alt", link.FirstChild?.ToString() ?? string.Empty);
        }

        if (renderer.EnableHtmlForInline && !string.IsNullOrEmpty(link.Title))
        {
            attributes.AddProperty("title", link.Title);
        }

        renderer.AddAttributes(link);

        if (renderer.EnableHtmlForInline)
        {
            renderer.CloseElement();
        }
    }
}
