using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdig.Renderers;
using Markdig.Syntax;

namespace Scotec.Blazor.Markdown.Renderer
{
    /// <summary>
    /// An HTML renderer for a <see cref="HeadingBlock"/>.
    /// </summary>
    /// <seealso cref="BlazorObjectRenderer{TObject}" />
    public class HeadingRenderer : BlazorObjectRenderer<HeadingBlock>
    {
        protected override void Write(BlazorRenderer renderer, HeadingBlock obj)
        {
            string headingText = $"h{obj.Level}";

            if (renderer.EnableHtmlForBlock)
            {
                renderer.OpenElement(headingText);
                renderer.AddAttributes(obj);
            }

            renderer.WriteLeafInline(obj);

            if (renderer.EnableHtmlForBlock)
            {
                renderer.CloseElement();
            }
        }
    }
}
