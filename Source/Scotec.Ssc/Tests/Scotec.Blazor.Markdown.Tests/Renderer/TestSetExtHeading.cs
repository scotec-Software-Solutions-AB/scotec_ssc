using Xunit;

namespace Scotec.Blazor.Markdown.Tests.Renderer;

public class TestSetExtHeading
{
    [Theory]
    [InlineData("h1\n===")] //3
    [InlineData("h1\n ===")] //3
    [InlineData("h1\n  ===")] //3
    [InlineData("h1\n   ===")] //3
    [InlineData("h1\n=== ")] //3
    [InlineData("h1 \n===")] //3
    [InlineData("h1\\\n===")] //3
    [InlineData("h1\n === ")] //3
    [InlineData("h1\nh1 l2\n===")] //3
    [InlineData("h1\n====")] // 4
    [InlineData("h1\n ====")] // 4
    [InlineData("h1\n==== ")] // 4
    [InlineData("h1\n ==== ")] // 4
    [InlineData("h1\n===\nh1\n===")] //3
    [InlineData("\\>h1\n===")] //3
    public void Test(string markdown)
    {
        MarkdownTest.Run(markdown);
    }

    [Theory]
    [InlineData("h1\r===")]
    [InlineData("h1\n===")]
    [InlineData("h1\r\n===")]
    [InlineData("h1\r===\r")]
    [InlineData("h1\n===\r")]
    [InlineData("h1\r\n===\r")]
    [InlineData("h1\r===\n")]
    [InlineData("h1\n===\n")]
    [InlineData("h1\r\n===\n")]
    [InlineData("h1\r===\r\n")]
    [InlineData("h1\n===\r\n")]
    [InlineData("h1\r\n===\r\n")]
    [InlineData("h1\n===\n\n\nh2---\n\n")]
    [InlineData("h1\r===\r\r\rh2---\r\r")]
    [InlineData("h1\r\n===\r\n\r\n\r\nh2---\r\n\r\n")]
    public void TestNewline(string markdown)
    {
        MarkdownTest.Run(markdown);
    }
}