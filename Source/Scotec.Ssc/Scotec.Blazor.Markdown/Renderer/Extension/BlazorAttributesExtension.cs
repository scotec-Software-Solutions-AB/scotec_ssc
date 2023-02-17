using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace Scotec.Blazor.Markdown.Renderer.Extension;

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