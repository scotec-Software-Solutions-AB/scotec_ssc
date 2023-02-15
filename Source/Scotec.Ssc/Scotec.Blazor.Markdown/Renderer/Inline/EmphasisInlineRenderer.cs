using Markdig.Renderers.Html;
using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scotec.Blazor.Markdown.Renderer.Inline
{
    /// <summary>
    /// A HTML renderer for an <see cref="EmphasisInline"/>.
    /// </summary>
    /// <seealso cref="BlazorObjectRenderer{EmphasisInline}" />
    public class EmphasisInlineRenderer : BlazorObjectRenderer<EmphasisInline>
    {
        /// <summary>
        /// Delegates to get the tag associated to an <see cref="EmphasisInline"/> object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The HTML tag associated to this <see cref="EmphasisInline"/> object</returns>
        public delegate string GetTagDelegate(EmphasisInline obj);

        /// <summary>
        /// Initializes a new instance of the <see cref="EmphasisInlineRenderer"/> class.
        /// </summary>
        public EmphasisInlineRenderer()
        {
            GetTag = GetDefaultTag;
        }

        /// <summary>
        /// Gets or sets the GetTag delegate.
        /// </summary>
        public GetTagDelegate GetTag { get; set; }

        protected override void Write(BlazorRenderer renderer, EmphasisInline obj)
        {
            if (renderer.EnableHtmlForInline)
            {
                renderer.OpenElement(GetTag(obj));
            }

            renderer.WriteChildren(obj);

            if (renderer.EnableHtmlForInline)
            {
                renderer.CloseElement();
            }
        }

        /// <summary>
        /// Gets the default HTML tag for ** and __ emphasis.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static string GetDefaultTag(EmphasisInline obj)
        {
            if (obj.DelimiterChar is '*' or '_')
            {
                Debug.Assert(obj.DelimiterCount <= 2);
                return obj.DelimiterCount == 2 ? "strong" : "em";
            }
            return null;
        }
    }
}
