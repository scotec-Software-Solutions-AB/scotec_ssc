using Markdig.Renderers.Html;
using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scotec.Blazor.Markdown.Renderer.Inline
{
    /// <summary>
    /// A HTML renderer for a <see cref="CodeInline"/>.
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
                renderer.AddContent(obj.ContentSpan);
            }
            if (renderer.EnableHtmlForInline)
            {
                renderer.CloseElement();
            }
        }
    }
}
