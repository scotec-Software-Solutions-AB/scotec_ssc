using Markdig.Syntax.Inlines;

namespace Scotec.Blazor.Markdown.Renderer.Inline;

/// <summary>
///     A HTML renderer for a <see cref="HtmlEntityInline" />.
/// </summary>
/// <seealso cref="BlazorObjectRenderer{HtmlEntityInline}" />
public class HtmlEntityInlineRenderer : BlazorObjectRenderer<HtmlEntityInline>
{
    protected override void Write(BlazorRenderer renderer, HtmlEntityInline obj)
    {
        if (renderer.EnableHtmlEscape)
        {
            renderer.AddContent(obj.Transcoded);
        }
        else
        {
            renderer.AddMarkupContent(obj.Transcoded);
        }
    }
}
