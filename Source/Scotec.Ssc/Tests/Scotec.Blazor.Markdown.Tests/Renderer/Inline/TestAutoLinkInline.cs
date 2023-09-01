using Xunit;

namespace Scotec.Blazor.Markdown.Tests.Renderer.Inline;

public class TestAutoLinkInline
{
    [Theory]
    [InlineData("<http://a>")]
    [InlineData(" <http://a>")]
    [InlineData("<http://a> ")]
    [InlineData(" <http://a> ")]
    [InlineData("<example@example.com>")]
    [InlineData(" <example@example.com>")]
    [InlineData("<example@example.com> ")]
    [InlineData(" <example@example.com> ")]
    [InlineData("p http://a p")]
    public void Test(string markdown)
    {
        MarkdownTest.Run(markdown);
    }
}