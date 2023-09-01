using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Scotec.Blazor.Markdown.Tests.Renderer.Inline
{
    public class TestImageInline
    {
        [Theory]
        [InlineData("![](a)")]
        [InlineData(" ![](a)")]
        [InlineData("![](a) ")]
        [InlineData(" ![](a) ")]
        [InlineData("   ![description](http://example.com)")]
        public void Test(string markdown)
        {
            MarkdownTest.Run(markdown);
        }

        [Theory]
        [InlineData("paragraph   ![description](http://example.com)")]
        public void TestParagraph(string markdown)
        {
            MarkdownTest.Run(markdown);
        }
    }
}
