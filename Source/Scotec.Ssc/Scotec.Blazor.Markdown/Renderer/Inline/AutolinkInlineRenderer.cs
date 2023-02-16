using Markdig.Renderers.Html;
using Markdig.Syntax.Inlines;

namespace Scotec.Blazor.Markdown.Renderer.Inline;

/// <summary>
///     A HTML renderer for an <see cref="AutolinkInline" />.
/// </summary>
/// <seealso cref="BlazorObjectRenderer{AutolinkInline}" />
public class AutolinkInlineRenderer : BlazorObjectRenderer<AutolinkInline>
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
            const string noFollow = "nofollow";

            if (value)
            {
                if (string.IsNullOrEmpty(Rel))
                {
                    Rel = noFollow;
                }
                else if (!Rel!.Contains(noFollow))
                {
                    Rel += $" {noFollow}";
                }
            }
            else
            {
                Rel = Rel?.Replace(noFollow, string.Empty);
            }
        }
    }

    /// <summary>
    ///     Gets or sets the literal string in property rel for links
    /// </summary>
    public string Rel { get; set; }

    protected override void Write(BlazorRenderer renderer, AutolinkInline obj)
    {
        if (renderer.EnableHtmlForInline)
        {
            renderer.OpenElement("a");

            var link = obj.IsEmail ? $"mailto:{obj.Url}" : obj.Url;

            var attributes = obj.GetAttributes();
            attributes.AddProperty("href", link);

            renderer.AddAttributes(obj);

            if (!obj.IsEmail && !string.IsNullOrWhiteSpace(Rel))
            {
                attributes.AddProperty("rel", Rel);
            }
        }

        renderer.AddContent(obj.Url);

        if (renderer.EnableHtmlForInline)
        {
            renderer.CloseElement();
        }
    }
}