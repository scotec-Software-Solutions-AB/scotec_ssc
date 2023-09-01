using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Scotec.Blazor.Markdown.Tests.Renderer.Inline
{
    public class TestCodeInline
    {
        [Theory]
        [InlineData("``")]
        [InlineData(" ``")]
        [InlineData("`` ")]
        [InlineData(" `` ")]

        [InlineData("`c`")]
        [InlineData(" `c`")]
        [InlineData("`c` ")]
        [InlineData(" `c` ")]

        [InlineData("` c`")]
        [InlineData(" ` c`")]
        [InlineData("` c` ")]
        [InlineData(" ` c` ")]

        [InlineData("`c `")]
        [InlineData(" `c `")]
        [InlineData("`c ` ")]
        [InlineData(" `c ` ")]

        [InlineData("`c``")] // 1, 2
        [InlineData("``c`")] // 2, 1
        [InlineData("``c``")] // 2, 2

        [InlineData("```c``")] // 2, 3
        [InlineData("``c```")] // 3, 2
        [InlineData("```c```")] // 3, 3

        [InlineData("```c````")] // 3, 4
        [InlineData("````c```")] // 4, 3
        [InlineData("````c````")] // 4, 4

        [InlineData("```a``` p")]
        [InlineData("```a`b`c```")]
        [InlineData("```a``` p\n```a``` p")]

        [InlineData("` a `")]
        [InlineData(" ` a `")]
        [InlineData("` a ` ")]
        [InlineData(" ` a ` ")]
        public void Test(string markdown)
        {
            MarkdownTest.Run(markdown);
        }

        [Theory]
        [InlineData("p `a` p")]
        [InlineData("p ``a`` p")]
        [InlineData("p ```a``` p")]
        [InlineData("p\n\n```a``` p")]
        public void TestParagraph(string markdown)
        {
            MarkdownTest.Run(markdown);
        }

        [Theory]
        [InlineData("`\na\n`")]
        [InlineData("`\na\r`")]
        [InlineData("`\na\r\n`")]
        [InlineData("`\ra\r`")]
        [InlineData("`\ra\n`")]
        [InlineData("`\ra\r\n`")]
        [InlineData("`\r\na\n`")]
        [InlineData("`\r\na\r`")]
        [InlineData("`\r\na\r\n`")]
        public void Test_Newlines(string markdown)
        {
            MarkdownTest.Run(markdown);
        }
    }
}
