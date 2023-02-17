using Markdig.Syntax;

namespace Scotec.Blazor.Markdown.Renderer;

/// <summary>
///     A HTML renderer for a <see cref="ParagraphBlock" />.
/// </summary>
/// <seealso cref="BlazorObjectRenderer{ParagraphBlock}" />
public class ParagraphRenderer : BlazorObjectRenderer<ParagraphBlock>
{
    protected override void Write(BlazorRenderer renderer, ParagraphBlock obj)
    {
        if (!renderer.ImplicitParagraph && renderer.EnableHtmlForBlock)
        {
            renderer.OpenElement("p");
            renderer.AddAttributes(obj);
        }

        renderer.WriteLeafInline(obj);

        if (!renderer.ImplicitParagraph)
        {
            if (renderer.EnableHtmlForBlock)
            {
                renderer.CloseElement();
            }
        }
    }
}