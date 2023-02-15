using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Scotec.Blazor.Markdown.Renderer.Extension;

namespace Scotec.Blazor.Markdown;

public class Markdown : ComponentBase
{
    private RenderTreeBuilder _builder;
    [Parameter] public string Content { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        _builder = builder;

        var pipelineBuilder = new BlazorPipelineBuilder();
        pipelineBuilder.Extensions.AddIfNotAlready<BlazorAttributesExtension>();

        var pipeline = pipelineBuilder.Build();

        var renderer = new BlazorRenderer(builder);
        pipeline.Setup(renderer);

        //var document = MarkdownParser.Parse(Content, pipeline, null);
        var document = Markdig.Markdown.Parse(Content, pipeline);
        renderer.Render(document);
    }

    private void RenderFragment(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "hr");
        builder.CloseElement();
    }
}