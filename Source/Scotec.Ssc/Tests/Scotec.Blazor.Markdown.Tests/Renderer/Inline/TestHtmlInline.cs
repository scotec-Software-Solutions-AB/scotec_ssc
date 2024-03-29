﻿using Xunit;

namespace Scotec.Blazor.Markdown.Tests.Renderer.Inline;

public class TestHtmlInline
{
    [Theory]
    [InlineData("<em>f</em>")]
    [InlineData("<em> f</em>")]
    [InlineData("<em>f </em>")]
    [InlineData("<em> f </em>")]
    [InlineData("<b>p</b>")]
    [InlineData("<b></b>")]
    [InlineData("<b> </b>")]
    [InlineData("<b>  </b>")]
    [InlineData("<b>   </b>")]
    [InlineData("<b>\t</b>")]
    [InlineData("<b> \t</b>")]
    [InlineData("<b>\t </b>")]
    [InlineData("<b> \t </b>")]
    [InlineData("Text with <b><i>inline</i></b> HTML")]
    public void Test(string markdown)
    {
        MarkdownTest.Run(markdown);
    }
}