using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Scotec.Blazor.Markdown.Tests.Renderer.Inline
{
    public class TestLineBreakInline
    {
        [Theory]
        [InlineData("p\n")]
        [InlineData("p\r\n")]
        [InlineData("p\r")]
        [InlineData("[]() ![]()  ``  ` `  `  `  ![]()   ![]()")]
        public void Test(string markdown)
        {
            MarkdownTest.Run(markdown);
        }
    }
}
