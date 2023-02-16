using Xunit;

namespace Scotec.Blazor.Markdown.Tests.Renderer.Inline;

public class TestBackslashEscapeInline
{
    [Theory]
    [InlineData(@"\!")]
    [InlineData(@"\""")]
    [InlineData(@"\#")]
    [InlineData(@"\$")]
    [InlineData(@"\&")]
    [InlineData(@"\'")]
    [InlineData(@"\(")]
    [InlineData(@"\)")]
    [InlineData(@"\*")]
    [InlineData(@"\+")]
    [InlineData(@"\,")]
    [InlineData(@"\-")]
    [InlineData(@"\.")]
    [InlineData(@"\/")]
    [InlineData(@"\:")]
    [InlineData(@"\;")]
    [InlineData(@"\<")]
    [InlineData(@"\=")]
    [InlineData(@"\>")]
    [InlineData(@"\?")]
    [InlineData(@"\@")]
    [InlineData(@"\[")]
    [InlineData(@"\\")]
    [InlineData(@"\]")]
    [InlineData(@"\^")]
    [InlineData(@"\_")]
    [InlineData(@"\`")]
    [InlineData(@"\{")]
    [InlineData(@"\|")]
    [InlineData(@"\}")]
    [InlineData(@"\~")]

    // below test breaks visual studio
    //[InlineData(@"\!\""\#\$\%\&\'\(\)\*\+\,\-\.\/\:\;\<\=\>\?\@\[\\\]\^\_\`\{\|\}\~")]
    public void Test(string markdown)
    {
        MarkdownTest.Run(markdown);
    }

    [Theory]
    [InlineData(@"# \#\#h1")]
    [InlineData(@"# \#\#h1\#")]
    public void TestHeading(string markdown)
    {
        MarkdownTest.Run(markdown);
    }

    [Theory]
    [InlineData(@"`\``")]
    [InlineData(@"` \``")]
    [InlineData(@"`\` `")]
    [InlineData(@"` \` `")]
    [InlineData(@" ` \` `")]
    [InlineData(@"` \` ` ")]
    [InlineData(@" ` \` ` ")]
    public void TestCodeSpanInline(string markdown)
    {
        MarkdownTest.Run(markdown);
    }
}