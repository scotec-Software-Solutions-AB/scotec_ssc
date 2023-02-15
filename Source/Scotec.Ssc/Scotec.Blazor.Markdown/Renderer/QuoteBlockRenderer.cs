using Markdig.Renderers.Html;
using Markdig.Renderers;
using Markdig.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scotec.Blazor.Markdown.Renderer
{
    /// <summary>
    /// A HTML renderer for a <see cref="QuoteBlock"/>.
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
}
