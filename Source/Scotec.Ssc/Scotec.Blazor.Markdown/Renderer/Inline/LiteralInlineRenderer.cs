using Markdig.Renderers.Html;
using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Scotec.Blazor.Markdown.Renderer.Inline
{
    /// <summary>
    /// A HTML renderer for a <see cref="LiteralInline"/>.
    /// </summary>
    /// <seealso cref="BlazorObjectRenderer{LiteralInline}" />
    public class LiteralInlineRenderer : BlazorObjectRenderer<LiteralInline>
    {
        protected override void Write(BlazorRenderer renderer, LiteralInline obj)
        {
            if (renderer.EnableHtmlEscape)
            {
                renderer.AddContent(obj.Content.AsSpan().ToString());
            }
            else
            {
                renderer.AddMarkupContent(obj.Content);
            }
        }
    }
}
