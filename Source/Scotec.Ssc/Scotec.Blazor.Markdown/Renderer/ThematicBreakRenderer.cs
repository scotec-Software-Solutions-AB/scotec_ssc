using Markdig.Syntax;

namespace Scotec.Blazor.Markdown.Renderer;

/// <summary>
///     A HTML renderer for a <see cref="ThematicBreakBlock" />.
/// </summary>
/// <seealso cref="BlazorObjectRenderer{ThematicBreakBlock}" />
public class ThematicBreakRenderer : BlazorObjectRenderer<ThematicBreakBlock>
{
    protected override void Write(BlazorRenderer renderer, ThematicBreakBlock obj)
    {
        if (renderer.EnableHtmlForBlock)
        {
            renderer.OpenElement("hr");
            renderer.AddAttributes(obj);
            renderer.CloseElement();
        }
    }
}
