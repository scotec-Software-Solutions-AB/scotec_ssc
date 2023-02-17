using Markdig.Syntax.Inlines;

namespace Scotec.Blazor.Markdown.Renderer.Inline;

/// <summary>
///     A HTML renderer for a <see cref="LineBreakInline" />.
/// </summary>
/// <seealso cref="BlazorObjectRenderer{LineBreakInline}" />
public class LineBreakInlineRenderer : BlazorObjectRenderer<LineBreakInline>
{
    /// <summary>
    ///     Gets or sets a value indicating whether to render this softline break as a HTML hardline break tag (&lt;br /&gt;)
    /// </summary>
    public bool RenderAsHardlineBreak { get; set; }

    protected override void Write(BlazorRenderer renderer, LineBreakInline obj)
    {
        if (renderer.IsLastInContainer)
        {
            return;
        }

        if (renderer.EnableHtmlForInline)
        {
            if (obj.IsHard || RenderAsHardlineBreak)
            {
                renderer.OpenElement("br");
                renderer.CloseElement();
            }
        }

        renderer.AddLineBreak();
    }
}
