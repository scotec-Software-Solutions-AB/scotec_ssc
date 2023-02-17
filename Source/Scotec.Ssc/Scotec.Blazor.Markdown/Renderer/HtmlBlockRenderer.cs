using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace Scotec.Blazor.Markdown.Renderer;

/// <summary>
///     A HTML renderer for a <see cref="HtmlBlock" />.
/// </summary>
/// <seealso cref="HtmlObjectRenderer{HtmlBlock}" />
public class HtmlBlockRenderer : BlazorObjectRenderer<HtmlBlock>
{
    protected override void Write(BlazorRenderer renderer, HtmlBlock obj)
    {
        foreach (var line in obj.Lines.Lines)
        {
            renderer.AddMarkupContent(line.ToString());
        }
    }
}
