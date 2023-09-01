using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Scotec.Blazor.Markdown.Tests.Renderer.Inline
{
    public class TestHtmlEntityInline
    {
        [Theory]
        [InlineData("&gt;")]
        [InlineData("&lt;")]
        [InlineData("&nbsp;")]
        [InlineData("&heartsuit;")]
        [InlineData("&#42;")]
        [InlineData("&#0;")]
        [InlineData("&#1234;")]
        [InlineData("&#xcab;")]

        [InlineData(" &gt;")]
        [InlineData(" &lt;")]
        [InlineData(" &nbsp;")]
        [InlineData(" &heartsuit;")]
        [InlineData(" &#42;")]
        [InlineData(" &#0;")]
        [InlineData(" &#1234;")]
        [InlineData(" &#xcab;")]

        [InlineData("&gt; ")]
        [InlineData("&lt; ")]
        [InlineData("&nbsp; ")]
        [InlineData("&heartsuit; ")]
        [InlineData("&#42; ")]
        [InlineData("&#0; ")]
        [InlineData("&#1234; ")]
        [InlineData("&#xcab; ")]

        [InlineData(" &gt; ")]
        [InlineData(" &lt; ")]
        [InlineData(" &nbsp; ")]
        [InlineData(" &heartsuit; ")]
        [InlineData(" &#42; ")]
        [InlineData(" &#0; ")]
        [InlineData(" &#1234; ")]
        [InlineData(" &#xcab; ")]
        public void Test(string markdown)
        {
            MarkdownTest.Run(markdown);
        }
    }
}
