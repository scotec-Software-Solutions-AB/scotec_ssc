using Markdig.Syntax.Inlines;

namespace Scotec.Blazor.Markdown.Renderer.Inline;

/// <summary>
///     A HTML renderer for a <see cref="CodeInline" />.
/// </summary>
/// <seealso cref="BlazorObjectRenderer{CodeInline}" />
public class CodeInlineRenderer : BlazorObjectRenderer<CodeInline>
{
    protected override void Write(BlazorRenderer renderer, CodeInline obj)
    {
        if (renderer.EnableHtmlForInline)
        {
            renderer.OpenElement("code");
            renderer.AddAttributes(obj);
        }

        if (renderer.EnableHtmlEscape)
        {
            renderer.AddContent(obj.ContentSpan);
        }
        else
        {
            renderer.AddMarkupContent(obj.ContentSpan);
        }

        if (renderer.EnableHtmlForInline)
        {
            renderer.CloseElement();
        }
    }
}
