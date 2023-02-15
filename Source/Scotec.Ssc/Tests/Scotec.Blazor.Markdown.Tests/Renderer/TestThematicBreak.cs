using Xunit;

namespace Scotec.Blazor.Markdown.Tests.Renderer;

public class TestThematicBreak
{
    [Theory]
    [InlineData("---")]
    [InlineData(" ---")]
    [InlineData("  ---")]
    [InlineData("   ---")]
    [InlineData("--- ")]
    [InlineData(" --- ")]
    [InlineData("  --- ")]
    [InlineData("   --- ")]
    [InlineData("- - -")]
    [InlineData(" - - -")]
    [InlineData(" - - - ")]
    [InlineData("-- -")]
    [InlineData("---\n")]
    [InlineData("---\n\n")]
    [InlineData("---\np")]
    [InlineData("---\n\np")]
    [InlineData("---\n# h")]
    [InlineData("p\n\n---")]
    // Note: "p\n---" is parsed as setext heading
    public void Test(string markdown)
    {
        MarkdownTest.Run(markdown);
    }

    [Theory]
    [InlineData("\n---")]
    [InlineData("\r---")]
    [InlineData("\r\n---")]
    [InlineData("\n---\n")]
    [InlineData("\r---\n")]
    [InlineData("\r\n---\n")]
    [InlineData("\n---\r")]
    [InlineData("\r---\r")]
    [InlineData("\r\n---\r")]
    [InlineData("\n---\r\n")]
    [InlineData("\r---\r\n")]
    [InlineData("\r\n---\r\n")]
    public void TestNewline(string markdown)
    {
        MarkdownTest.Run(markdown);
    }
}