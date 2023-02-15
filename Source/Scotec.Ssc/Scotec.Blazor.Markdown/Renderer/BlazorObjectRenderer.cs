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
    /// A base class for Blazor rendering <see cref="Block"/> and <see cref="Syntax.Inlines.Inline"/> Markdown objects.
    /// </summary>
    /// <typeparam name="TObject">The type of the object.</typeparam>
    /// <seealso cref="IMarkdownObjectRenderer" />
    public abstract class BlazorObjectRenderer<TObject> : MarkdownObjectRenderer<BlazorRenderer, TObject>
        where TObject : MarkdownObject
    {
    }
}
