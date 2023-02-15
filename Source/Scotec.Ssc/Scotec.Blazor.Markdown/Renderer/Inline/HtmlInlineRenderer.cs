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
    /// A HTML renderer for a <see cref="HtmlInline"/>.
    /// </summary>
    /// <seealso cref="BlazorObjectRenderer{HtmlInline}" />
    public class HtmlInlineRenderer : BlazorObjectRenderer<HtmlInline>
    {
        protected override void Write(BlazorRenderer renderer, HtmlInline obj)
        {
            if (renderer.EnableHtmlForInline)
            {
                renderer.AddContent(new MarkupString(obj.Tag));
            }
        }
    }
}
