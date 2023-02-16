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
    /// A HTML renderer for a <see cref="ListBlock"/>.
    /// </summary>
    /// <seealso cref="BlazorObjectRenderer{ListBlock}" />
    public class ListRenderer : BlazorObjectRenderer<ListBlock>
    {
        protected override void Write(BlazorRenderer renderer, ListBlock listBlock)
        {
            renderer.AddLineBreak();

            if (renderer.EnableHtmlForBlock)
            {
                if (listBlock.IsOrdered)
                {
                    renderer.OpenElement("ol");

                    if (listBlock.BulletType != '1')
                    {
                        var attributes = listBlock.GetAttributes();
                        attributes.AddProperty("type", listBlock.BulletType.ToString());
                    }

                    if (listBlock.OrderedStart is not null && listBlock.OrderedStart != "1")
                    {
                        var attributes = listBlock.GetAttributes();
                        attributes.AddProperty("start", listBlock.OrderedStart);
                    }
                    renderer.AddAttributes(listBlock);
                }
                else
                {
                    renderer.OpenElement("ul");
                    renderer.AddAttributes(listBlock);
                }
            }

            foreach (var item in listBlock)
            {
                var listItem = (ListItemBlock)item;
                var previousImplicit = renderer.ImplicitParagraph;
                renderer.ImplicitParagraph = !listBlock.IsLoose;

                if (renderer.EnableHtmlForBlock)
                {
                    renderer.OpenElement("li");
                    renderer.AddAttributes(item);
                }

                renderer.WriteChildren(listItem);

                if (renderer.EnableHtmlForBlock)
                {
                    renderer.CloseElement();
                }

                renderer.ImplicitParagraph = previousImplicit;
            }

            if (renderer.EnableHtmlForBlock)
            {
                renderer.CloseElement();
            }
        }
    }
}
