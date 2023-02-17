using Markdig.Syntax.Inlines;

namespace Scotec.Blazor.Markdown.Renderer.Inline;

/// <summary>
///     A HTML renderer for a <see cref="DelimiterInline" />.
/// </summary>
/// <seealso cref="BlazorObjectRenderer{DelimiterInline}" />
public class DelimiterInlineRenderer : BlazorObjectRenderer<DelimiterInline>
{
    protected override void Write(BlazorRenderer renderer, DelimiterInline obj)
    {
        renderer.AddContent(obj.ToLiteral());
        renderer.WriteChildren(obj);
    }
}
