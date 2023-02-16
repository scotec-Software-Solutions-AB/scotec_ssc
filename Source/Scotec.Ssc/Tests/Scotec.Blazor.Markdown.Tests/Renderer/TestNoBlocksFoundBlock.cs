using Xunit;

namespace Scotec.Blazor.Markdown.Tests.Renderer;

public class TestNoBlocksFoundBlock
{
    [Theory]
    [InlineData("\r")]
    [InlineData("\n")]
    [InlineData("\r\n")]
    [InlineData("\t")]
    [InlineData("\v")]
    [InlineData("\f")]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData("   ")]
    public void Test(string markdown)
    {
        MarkdownTest.Run(markdown);
    }
}