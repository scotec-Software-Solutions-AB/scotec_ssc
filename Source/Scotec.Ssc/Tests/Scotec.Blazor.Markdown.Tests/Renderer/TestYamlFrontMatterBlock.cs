using Xunit;

namespace Scotec.Blazor.Markdown.Tests.Renderer;

public class TestYamlFrontMatterBlock
{
    [Theory]
    [InlineData("---\nkey1: value1\nkey2: value2\n---\n\nContent\n")]
    [InlineData("No front matter")]
    [InlineData("Looks like front matter but actually is not\n---\nkey1: value1\nkey2: value2\n---")]
    public void FrontMatterBlockIsPreserved(string markdown)
    {
        MarkdownTest.Run(markdown);
    }
}