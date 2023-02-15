using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bunit;
using Markdig;
using Xunit;

namespace Scotec.Blazor.Markdown.Tests.Renderer
{
    public class TestUnorderedList
    {
        [Theory]
        [InlineData("- i1")]
        [InlineData("- i1 ")]
        [InlineData("- i1\n")]
        [InlineData("- i1\n\n")]
        [InlineData("- i1\n- i2")]
        [InlineData("- i1\n    - i2")]
        [InlineData("- i1\n    - i1.1\n    - i1.2")]
        [InlineData("- i1 \n- i2 \n")]
        [InlineData("- i1  \n- i2  \n")]
        [InlineData(" - i1")]
        [InlineData("  - i1")]
        [InlineData("   - i1")]
        [InlineData("- i1\n\n- i1")]
        [InlineData("- i1\n\n\n- i1")]
        [InlineData("- i1\n    - i1.1\n        - i1.1.1\n")]
        [InlineData("-\ti1")]
        [InlineData("-\ti1\n-\ti2")]
        [InlineData("-\ti1\n-  i2\n-\ti3")]
        public void Test(string markdown)
        {
            MarkdownTest.Run(markdown);
        }

        [Theory]
        [InlineData("- > q")]
        [InlineData(" - > q")]
        [InlineData("  - > q")]
        [InlineData("   - > q")]
        [InlineData("-  > q")]
        [InlineData(" -  > q")]
        [InlineData("  -  > q")]
        [InlineData("   -  > q")]
        [InlineData("-   > q")]
        [InlineData(" -   > q")]
        [InlineData("  -   > q")]
        [InlineData("   -   > q")]
        [InlineData("-    > q")]
        [InlineData(" -    > q")]
        [InlineData("  -    > q")]
        [InlineData("   -    > q")]
        [InlineData("   -    > q1\n   -    > q2")]
        public void TestBlockQuote(string markdown)
        {
            MarkdownTest.Run(markdown);
        }

        [Theory]
        [InlineData("-     i1\n\np\n")] // TODO: listblock should render newline, apparently last paragraph of last listitem dont have newline
        [InlineData("-     i1\n\n\np\n")]
        [InlineData("- i1\n\np")]
        [InlineData("- i1\n\np\n")]
        public void TestParagraph(string markdown)
        {
            MarkdownTest.Run(markdown);
        }

        [Theory]
        [InlineData("- i1\n\n---\n")]
        [InlineData("- i1\n\n\n---\n")]
        public void TestThematicBreak(string markdown)
        {
            MarkdownTest.Run(markdown);
        }

    }
}
