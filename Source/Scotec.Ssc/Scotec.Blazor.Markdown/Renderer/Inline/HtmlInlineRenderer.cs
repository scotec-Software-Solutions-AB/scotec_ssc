using System.Text.RegularExpressions;
using Markdig.Syntax.Inlines;

namespace Scotec.Blazor.Markdown.Renderer.Inline;

/// <summary>
///     A HTML renderer for a <see cref="HtmlInline" />.
/// </summary>
/// <seealso cref="BlazorObjectRenderer{HtmlInline}" />
public class HtmlInlineRenderer : BlazorObjectRenderer<HtmlInline>
{
    protected override void Write(BlazorRenderer renderer, HtmlInline obj)
    {
        if (renderer.EnableHtmlForInline)
        {
            renderer.WriteHtmlInlineTag(obj);
        }
    }
}
