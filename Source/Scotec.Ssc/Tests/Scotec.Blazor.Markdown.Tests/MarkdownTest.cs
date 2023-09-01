using System.IO.Pipes;
using Bunit;
using Markdig;
using Markdig.Extensions.GenericAttributes;
using Scotec.Blazor.Markdown.Renderer.Extension;

namespace Scotec.Blazor.Markdown.Tests;

public static class MarkdownTest
{
    public static void Run(string markdown)
    {
        using var ctx = new TestContext();

        var cut = ctx.RenderComponent<Markdown>(parameters => parameters.Add(p => p.Content, markdown));
        
        var markdigHtml = ParseMarkdig(markdown);
        var componentHtml = cut.Markup;

        cut.MarkupMatches(markdigHtml);
    }

    private static string ParseMarkdig(string markdown)
    {
        var pipelineBuilder = new MarkdownPipelineBuilder();
        //pipelineBuilder.InlineParsers.AddIfNotAlready<GenericAttributesParser>();
        pipelineBuilder.UseAdvancedExtensions();//.UseGenericAttributes();

        var pipeline = pipelineBuilder.Build();
        return Markdig.Markdown.ToHtml(markdown, pipeline);
    }
}