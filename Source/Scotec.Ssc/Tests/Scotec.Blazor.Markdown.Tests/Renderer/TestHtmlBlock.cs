using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Scotec.Blazor.Markdown.Tests.Renderer
{
    public class TestHtmlBlock
    {
        [Theory]
        [InlineData("<br>")]
        [InlineData("<br>\n")]
        [InlineData("<br>\n\n")]
        [InlineData("<div></div>\n\n# h")]
        [InlineData("p\n\n<div></div>\n")]
        public void Test(string markdown)
        {
            MarkdownTest.Run(markdown);
        }
    }
}
