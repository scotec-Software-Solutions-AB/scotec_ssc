﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Scotec.Blazor.Markdown.Tests.Renderer
{
    public class TestQuoteBlock
    {
        [Theory]
        [InlineData(">q")]
        [InlineData(" >q")]
        [InlineData("  >q")]
        [InlineData("   >q")]
        [InlineData("> q")]
        [InlineData(" > q")]
        [InlineData("  > q")]
        [InlineData("   > q")]
        [InlineData(">  q")]
        [InlineData(" >  q")]
        [InlineData("  >  q")]
        [InlineData("   >  q")]

        [InlineData(">q\n>q")]
        [InlineData(">q\n >q")]
        [InlineData(">q\n  >q")]
        [InlineData(">q\n   >q")]
        [InlineData(">q\n> q")]
        [InlineData(">q\n > q")]
        [InlineData(">q\n  > q")]
        [InlineData(">q\n   > q")]
        [InlineData(">q\n>  q")]
        [InlineData(">q\n >  q")]
        [InlineData(">q\n  >  q")]
        [InlineData(">q\n   >  q")]

        [InlineData(" >q\n>q")]
        [InlineData(" >q\n >q")]
        [InlineData(" >q\n  >q")]
        [InlineData(" >q\n   >q")]
        [InlineData(" >q\n> q")]
        [InlineData(" >q\n > q")]
        [InlineData(" >q\n  > q")]
        [InlineData(" >q\n   > q")]
        [InlineData(" >q\n>  q")]
        [InlineData(" >q\n >  q")]
        [InlineData(" >q\n  >  q")]
        [InlineData(" >q\n   >  q")]

        [InlineData("  >q\n>q")]
        [InlineData("  >q\n >q")]
        [InlineData("  >q\n  >q")]
        [InlineData("  >q\n   >q")]
        [InlineData("  >q\n> q")]
        [InlineData("  >q\n > q")]
        [InlineData("  >q\n  > q")]
        [InlineData("  >q\n   > q")]
        [InlineData("  >q\n>  q")]
        [InlineData("  >q\n >  q")]
        [InlineData("  >q\n  >  q")]
        [InlineData("  >q\n   >  q")]

        [InlineData("> q\n>q")]
        [InlineData("> q\n >q")]
        [InlineData("> q\n  >q")]
        [InlineData("> q\n   >q")]
        [InlineData("> q\n> q")]
        [InlineData("> q\n > q")]
        [InlineData("> q\n  > q")]
        [InlineData("> q\n   > q")]
        [InlineData("> q\n>  q")]
        [InlineData("> q\n >  q")]
        [InlineData("> q\n  >  q")]
        [InlineData("> q\n   >  q")]

        [InlineData(" > q\n>q")]
        [InlineData(" > q\n >q")]
        [InlineData(" > q\n  >q")]
        [InlineData(" > q\n   >q")]
        [InlineData(" > q\n> q")]
        [InlineData(" > q\n > q")]
        [InlineData(" > q\n  > q")]
        [InlineData(" > q\n   > q")]
        [InlineData(" > q\n>  q")]
        [InlineData(" > q\n >  q")]
        [InlineData(" > q\n  >  q")]
        [InlineData(" > q\n   >  q")]

        [InlineData("  > q\n>q")]
        [InlineData("  > q\n >q")]
        [InlineData("  > q\n  >q")]
        [InlineData("  > q\n   >q")]
        [InlineData("  > q\n> q")]
        [InlineData("  > q\n > q")]
        [InlineData("  > q\n  > q")]
        [InlineData("  > q\n   > q")]
        [InlineData("  > q\n>  q")]
        [InlineData("  > q\n >  q")]
        [InlineData("  > q\n  >  q")]
        [InlineData("  > q\n   >  q")]

        [InlineData("   > q\n>q")]
        [InlineData("   > q\n >q")]
        [InlineData("   > q\n  >q")]
        [InlineData("   > q\n   >q")]
        [InlineData("   > q\n> q")]
        [InlineData("   > q\n > q")]
        [InlineData("   > q\n  > q")]
        [InlineData("   > q\n   > q")]
        [InlineData("   > q\n>  q")]
        [InlineData("   > q\n >  q")]
        [InlineData("   > q\n  >  q")]
        [InlineData("   > q\n   >  q")]

        [InlineData(">  q\n>q")]
        [InlineData(">  q\n >q")]
        [InlineData(">  q\n  >q")]
        [InlineData(">  q\n   >q")]
        [InlineData(">  q\n> q")]
        [InlineData(">  q\n > q")]
        [InlineData(">  q\n  > q")]
        [InlineData(">  q\n   > q")]
        [InlineData(">  q\n>  q")]
        [InlineData(">  q\n >  q")]
        [InlineData(">  q\n  >  q")]
        [InlineData(">  q\n   >  q")]

        [InlineData(" >  q\n>q")]
        [InlineData(" >  q\n >q")]
        [InlineData(" >  q\n  >q")]
        [InlineData(" >  q\n   >q")]
        [InlineData(" >  q\n> q")]
        [InlineData(" >  q\n > q")]
        [InlineData(" >  q\n  > q")]
        [InlineData(" >  q\n   > q")]
        [InlineData(" >  q\n>  q")]
        [InlineData(" >  q\n >  q")]
        [InlineData(" >  q\n  >  q")]
        [InlineData(" >  q\n   >  q")]

        [InlineData("  >  q\n>q")]
        [InlineData("  >  q\n >q")]
        [InlineData("  >  q\n  >q")]
        [InlineData("  >  q\n   >q")]
        [InlineData("  >  q\n> q")]
        [InlineData("  >  q\n > q")]
        [InlineData("  >  q\n  > q")]
        [InlineData("  >  q\n   > q")]
        [InlineData("  >  q\n>  q")]
        [InlineData("  >  q\n >  q")]
        [InlineData("  >  q\n  >  q")]
        [InlineData("  >  q\n   >  q")]

        [InlineData("   >  q\n>q")]
        [InlineData("   >  q\n >q")]
        [InlineData("   >  q\n  >q")]
        [InlineData("   >  q\n   >q")]
        [InlineData("   >  q\n> q")]
        [InlineData("   >  q\n > q")]
        [InlineData("   >  q\n  > q")]
        [InlineData("   >  q\n   > q")]
        [InlineData("   >  q\n>  q")]
        [InlineData("   >  q\n >  q")]
        [InlineData("   >  q\n  >  q")]
        [InlineData("   >  q\n   >  q")]

        [InlineData(">q\n>q\n>q")]
        [InlineData(">q\n>\n>q")]
        [InlineData(">q\np\n>q")]
        [InlineData(">q\n>\n>\n>q")]
        [InlineData(">q\n>\n>\n>\n>q")]
        [InlineData(">q\n>\n>q\n>\n>q")]
        [InlineData("p\n\n> **q**\n>p\n")]

        [InlineData("> q\np\n> q")] // lazy
        [InlineData("> q\n> q\np")] // lazy

        [InlineData(">>q")]
        [InlineData(" >  >   q")]

        [InlineData("> **q**\n>p\n")]
        [InlineData("> **q**")]
        public void Test(string markdown)
        {
            MarkdownTest.Run(markdown);
        }

        [Theory]
        [InlineData(">     q")] // 5
        [InlineData(">      q")] // 6
        [InlineData(" >     q")] //5
        [InlineData(" >      q")] //6
        [InlineData(" > \tq")]
        [InlineData(">     q\n>     q")] // 5, 5
        [InlineData(">     q\n>      q")] // 5, 6
        [InlineData(">      q\n>     q")] // 6, 5
        [InlineData(">      q\n>      q")] // 6, 6
        [InlineData(">     q\n\n>     5")] // 5, 5
        public void TestIndentedCodeBlock(string markdown)
        {
            MarkdownTest.Run(markdown);
        }

        [Theory]
        [InlineData("\n> q")]
        [InlineData("\n> q\n")]
        [InlineData("\n> q\n\n")]
        [InlineData("> q\n\np")]
        [InlineData("p\n\n> q\n\n# h")]

        //https://github.com/lunet-io/markdig/issues/480
        //[InlineData(">\np")]
        //[InlineData(">**b**\n>\n>p\n>\np\n")]
        public void TestParagraph(string markdown)
        {
            MarkdownTest.Run(markdown);
        }

        [Theory]
        [InlineData("> q\n\n# h\n")]
        public void TestAtxHeader(string markdown)
        {
            MarkdownTest.Run(markdown);
        }

        [Theory]
        [InlineData(">- i")]
        [InlineData("> - i")]
        [InlineData(">- i\n>- i")]
        [InlineData(">- >p")]
        [InlineData("> - >p")]
        [InlineData(">- i1\n>- i2\n")]
        [InlineData("> **p** p\n>- i1\n>- i2\n")]
        public void TestUnorderedList(string markdown)
        {
            MarkdownTest.Run(markdown);
        }

        [Theory]
        [InlineData("> *q*\n>p\n")]
        [InlineData("> *q*")]
        public void TestEmphasis(string markdown)
        {
            MarkdownTest.Run(markdown);
        }

        [Theory]
        [InlineData("> **q**\n>p\n")]
        [InlineData("> **q**")]
        public void TestStrongEmphasis(string markdown)
        {
            MarkdownTest.Run(markdown);
        }

        [Theory]
        [InlineData(">p\n")]
        [InlineData(">p\r")]
        [InlineData(">p\r\n")]

        [InlineData(">p\n>p")]
        [InlineData(">p\r>p")]
        [InlineData(">p\r\n>p")]

        [InlineData(">p\n>p\n")]
        [InlineData(">p\r>p\n")]
        [InlineData(">p\r\n>p\n")]

        [InlineData(">p\n>p\r")]
        [InlineData(">p\r>p\r")]
        [InlineData(">p\r\n>p\r")]

        [InlineData(">p\n>p\r\n")]
        [InlineData(">p\r>p\r\n")]
        [InlineData(">p\r\n>p\r\n")]
        public void TestNewline(string markdown)
        {
            MarkdownTest.Run(markdown);
        }

        [Theory]
        [InlineData(">\n>q")]
        [InlineData(">\n>\n>q")]
        [InlineData(">q\n>\n>q")]
        [InlineData(">q\n>\n>\n>q")]
        [InlineData(">q\n> \n>q")]
        [InlineData(">q\n>  \n>q")]
        [InlineData(">q\n>   \n>q")]
        [InlineData(">q\n>\t\n>q")]
        [InlineData(">q\n>\v\n>q")]
        [InlineData(">q\n>\f\n>q")]
        public void TestEmptyLines(string markdown)
        {
            MarkdownTest.Run(markdown);
        }
    }
}
