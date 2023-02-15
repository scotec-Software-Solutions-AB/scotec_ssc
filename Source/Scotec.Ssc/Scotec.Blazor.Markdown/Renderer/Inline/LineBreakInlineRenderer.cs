using Markdig.Renderers.Html;
using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdig.Helpers;

namespace Scotec.Blazor.Markdown.Renderer.Inline
{

    /// <summary>
    /// A HTML renderer for a <see cref="LineBreakInline"/>.
    /// </summary>
    /// <seealso cref="BlazorObjectRenderer{LineBreakInline}" />
    public class LineBreakInlineRenderer : BlazorObjectRenderer<LineBreakInline>
    {
        /// <summary>
        /// Gets or sets a value indicating whether to render this softline break as a HTML hardline break tag (&lt;br /&gt;)
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
}
