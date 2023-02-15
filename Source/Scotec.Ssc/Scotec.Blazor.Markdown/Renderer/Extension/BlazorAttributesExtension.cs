using Markdig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Renderers.Html;

namespace Scotec.Blazor.Markdown.Renderer.Extension
{
    public class BlazorAttributesExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            renderer.ObjectWriteBefore += RendererOnObjectWriteBefore;
        }

        private void RendererOnObjectWriteBefore(IMarkdownRenderer arg1, MarkdownObject arg2)
        {
            var attributes = arg2.TryGetAttributes();
            if (attributes != null)
            {

            }
        }
    }
}
