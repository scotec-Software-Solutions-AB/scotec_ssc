using Markdig;
using Microsoft.AspNetCore.Components.Rendering;

namespace Scotec.Blazor.Markdown;

internal class MarkdownBlazor
{
    /// <summary>Converts a Markdown string to a FlowDocument.</summary>
    /// <param name="markdown">A Markdown text.</param>
    /// <param name="pipeline">The pipeline used for the conversion.</param>
    /// <param name="baseUri">Base uri for images and links.</param>
    /// <returns>The result of the conversion</returns>
    /// <exception cref="System.ArgumentNullException">if markdown variable is null</exception>
    public static void ToFlowDocument(string markdown, RenderTreeBuilder builder, MarkdownPipeline pipeline = null,
        Uri baseUri = null)
    {
        if (markdown == null)
        {
            throw new ArgumentNullException(nameof(markdown));
        }

        if (pipeline == null)
        {
            pipeline = new MarkdownPipelineBuilder().Build();
        }

        var renderer = new BlazorRenderer(builder);
        pipeline.Setup(renderer);

        //var document = Markdown.Parse(markdown, pipeline);
        //return renderer.Render(document);
    }
}