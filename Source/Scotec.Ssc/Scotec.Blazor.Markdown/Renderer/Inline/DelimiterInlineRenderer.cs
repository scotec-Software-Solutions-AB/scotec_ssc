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
    /// A HTML renderer for a <see cref="DelimiterInline"/>.
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
}
