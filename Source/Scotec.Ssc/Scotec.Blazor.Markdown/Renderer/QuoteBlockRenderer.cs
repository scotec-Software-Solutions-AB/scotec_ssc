using Markdig.Syntax;

namespace Scotec.Blazor.Markdown.Renderer;

/// <summary>
///     A HTML renderer for a <see cref="QuoteBlock" />.
/// </summary>
/// <seealso cref="BlazorObjectRenderer{QuoteBlock}" />
public class QuoteBlockRenderer : BlazorObjectRenderer<QuoteBlock>
{
    protected override void Write(BlazorRenderer renderer, QuoteBlock obj)
    {
        if (renderer.EnableHtmlForBlock)
        {
            renderer.OpenElement("blockquote");
            renderer.AddAttributes(obj);
        }

        var savedImplicitParagraph = renderer.ImplicitParagraph;
        renderer.ImplicitParagraph = false;
        renderer.WriteChildren(obj);
        renderer.ImplicitParagraph = savedImplicitParagraph;

        if (renderer.EnableHtmlForBlock)
        {
            renderer.CloseElement();
        }
    }
}
