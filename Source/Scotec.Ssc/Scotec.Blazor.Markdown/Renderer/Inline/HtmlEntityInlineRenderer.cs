﻿using Markdig.Renderers.Html;
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
    /// A HTML renderer for a <see cref="HtmlEntityInline"/>.
    /// </summary>
    /// <seealso cref="BlazorObjectRenderer{HtmlEntityInline}" />
    public class HtmlEntityInlineRenderer : BlazorObjectRenderer<HtmlEntityInline>
    {
        protected override void Write(BlazorRenderer renderer, HtmlEntityInline obj)
        {
            if (renderer.EnableHtmlEscape)
            {
                renderer.AddContent(obj.Transcoded);
            }
            else
            {
                renderer.AddMarkupContent(obj.Transcoded);
            }
        }
    }
}
