using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bunit;
using Xunit;

namespace Scotec.Blazor.Markdown.Tests.Renderer
{
    public class TestAtxHeading
    {
        [Theory]
        [InlineData("# h")]
        [InlineData("# h ")]
        [InlineData("# h\n#h")]
        [InlineData("# h\n #h")]
        [InlineData("# h\n # h")]
        [InlineData("# h\n # h ")]
        [InlineData(" #  h   \n    #     h      ")]
        public void Test(string markdown)
        {
            MarkdownTest.Run(markdown);
        }

        [Theory]
        [InlineData("\n# h\n\np")]
        [InlineData("\n# h\n\np\n")]
        [InlineData("\n# h\n\np\n\n")]
        [InlineData("\n\n# h\n\np\n\n")]
        [InlineData("\n\n# h\np\n\n")]
        public void TestParagraph(string markdown)
        {
            MarkdownTest.Run(markdown);
        }

        [Theory]
        [InlineData("\n# h")]
        [InlineData("\n# h\n")]
        [InlineData("\n# h\r")]
        [InlineData("\n# h\r\n")]

        [InlineData("\r# h")]
        [InlineData("\r# h\n")]
        [InlineData("\r# h\r")]
        [InlineData("\r# h\r\n")]

        [InlineData("\r\n# h")]
        [InlineData("\r\n# h\n")]
        [InlineData("\r\n# h\r")]
        [InlineData("\r\n# h\r\n")]

        [InlineData("# h\n\n ")]
        [InlineData("# h\n\n  ")]
        [InlineData("# h\n\n   ")]
        public void TestNewline(string markdown)
        {
            MarkdownTest.Run(markdown);
        }
    }
}
