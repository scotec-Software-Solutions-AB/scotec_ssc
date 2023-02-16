using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Scotec.Blazor.Markdown.Tests.Renderer
{
    public class TestLinkReferenceDefinition
    {
        [Theory]
        [InlineData(@"[a]: /r")]
        [InlineData(@" [a]: /r")]
        [InlineData(@"  [a]: /r")]
        [InlineData(@"   [a]: /r")]

        [InlineData(@"[a]:  /r")]
        [InlineData(@" [a]:  /r")]
        [InlineData(@"  [a]:  /r")]
        [InlineData(@"   [a]:  /r")]

        [InlineData(@"[a]:  /r ")]
        [InlineData(@" [a]:  /r ")]
        [InlineData(@"  [a]:  /r ")]
        [InlineData(@"   [a]:  /r ")]

        [InlineData(@"[a]: /r ""l""")]
        [InlineData(@"[a]:  /r ""l""")]
        [InlineData(@"[a]: /r  ""l""")]
        [InlineData(@"[a]: /r ""l"" ")]
        [InlineData(@"[a]:  /r  ""l""")]
        [InlineData(@"[a]:  /r  ""l"" ")]

        [InlineData(@" [a]: /r ""l""")]
        [InlineData(@" [a]:  /r ""l""")]
        [InlineData(@" [a]: /r  ""l""")]
        [InlineData(@" [a]: /r ""l"" ")]
        [InlineData(@" [a]:  /r  ""l""")]
        [InlineData(@" [a]:  /r  ""l"" ")]

        [InlineData(@"  [a]: /r ""l""")]
        [InlineData(@"  [a]:  /r ""l""")]
        [InlineData(@"  [a]: /r  ""l""")]
        [InlineData(@"  [a]: /r ""l"" ")]
        [InlineData(@"  [a]:  /r  ""l""")]
        [InlineData(@"  [a]:  /r  ""l"" ")]

        [InlineData(@"   [a]: /r ""l""")]
        [InlineData(@"   [a]:  /r ""l""")]
        [InlineData(@"   [a]: /r  ""l""")]
        [InlineData(@"   [a]: /r ""l"" ")]
        [InlineData(@"   [a]:  /r  ""l""")]
        [InlineData(@"   [a]:  /r  ""l"" ")]

        [InlineData("[a]:\t/r")]
        [InlineData("[a]:\t/r\t")]
        [InlineData("[a]:\t/r\t\"l\"")]
        [InlineData("[a]:\t/r\t\"l\"\t")]

        [InlineData("[a]: \t/r")]
        [InlineData("[a]: \t/r\t")]
        [InlineData("[a]: \t/r\t\"l\"")]
        [InlineData("[a]: \t/r\t\"l\"\t")]

        [InlineData("[a]:\t /r")]
        [InlineData("[a]:\t /r\t")]
        [InlineData("[a]:\t /r\t\"l\"")]
        [InlineData("[a]:\t /r\t\"l\"\t")]

        [InlineData("[a]: \t /r")]
        [InlineData("[a]: \t /r\t")]
        [InlineData("[a]: \t /r\t\"l\"")]
        [InlineData("[a]: \t /r\t\"l\"\t")]

        [InlineData("[a]:\t/r \t")]
        [InlineData("[a]:\t/r \t\"l\"")]
        [InlineData("[a]:\t/r \t\"l\"\t")]

        [InlineData("[a]: \t/r")]
        [InlineData("[a]: \t/r \t")]
        [InlineData("[a]: \t/r \t\"l\"")]
        [InlineData("[a]: \t/r \t\"l\"\t")]

        [InlineData("[a]:\t /r")]
        [InlineData("[a]:\t /r \t")]
        [InlineData("[a]:\t /r \t\"l\"")]
        [InlineData("[a]:\t /r \t\"l\"\t")]

        [InlineData("[a]: \t /r")]
        [InlineData("[a]: \t /r \t")]
        [InlineData("[a]: \t /r \t\"l\"")]
        [InlineData("[a]: \t /r \t\"l\"\t")]

        [InlineData("[a]:\t/r\t ")]
        [InlineData("[a]:\t/r\t \"l\"")]
        [InlineData("[a]:\t/r\t \"l\"\t")]

        [InlineData("[a]: \t/r")]
        [InlineData("[a]: \t/r\t ")]
        [InlineData("[a]: \t/r\t \"l\"")]
        [InlineData("[a]: \t/r\t \"l\"\t")]

        [InlineData("[a]:\t /r")]
        [InlineData("[a]:\t /r\t ")]
        [InlineData("[a]:\t /r\t \"l\"")]
        [InlineData("[a]:\t /r\t \"l\"\t")]

        [InlineData("[a]: \t /r")]
        [InlineData("[a]: \t /r\t ")]
        [InlineData("[a]: \t /r\t \"l\"")]
        [InlineData("[a]: \t /r\t \"l\"\t")]

        [InlineData("[a]:\t/r \t ")]
        [InlineData("[a]:\t/r \t \"l\"")]
        [InlineData("[a]:\t/r \t \"l\"\t")]

        [InlineData("[a]: \t/r")]
        [InlineData("[a]: \t/r \t ")]
        [InlineData("[a]: \t/r \t \"l\"")]
        [InlineData("[a]: \t/r \t \"l\"\t")]

        [InlineData("[a]:\t /r")]
        [InlineData("[a]:\t /r \t ")]
        [InlineData("[a]:\t /r \t \"l\"")]
        [InlineData("[a]:\t /r \t \"l\"\t")]

        [InlineData("[a]: \t /r")]
        [InlineData("[a]: \t /r \t ")]
        [InlineData("[a]: \t /r \t \"l\"")]
        [InlineData("[a]: \t /r \t \"l\"\t")]
        public void Test(string markdown)
        {
            MarkdownTest.Run(markdown);
        }

        [Theory]
        [InlineData("[a]: /r\n[b]: /r\n")]
        [InlineData("[a]: /r\n[b]: /r\n[c] /r\n")]
        public void TestMultiple(string markdown)
        {
            MarkdownTest.Run(markdown);
        }

        [Theory]
        [InlineData("[a]:\f/r\f\"l\"")]
        [InlineData("[a]:\v/r\v\"l\"")]
        public void TestUncommonWhitespace(string markdown)
        {
            MarkdownTest.Run(markdown);
        }

        [Theory]
        [InlineData("[a]:\n/r\n\"t\"")]
        [InlineData("[a]:\n/r\r\"t\"")]
        [InlineData("[a]:\n/r\r\n\"t\"")]

        [InlineData("[a]:\r/r\n\"t\"")]
        [InlineData("[a]:\r/r\r\"t\"")]
        [InlineData("[a]:\r/r\r\n\"t\"")]

        [InlineData("[a]:\r\n/r\n\"t\"")]
        [InlineData("[a]:\r\n/r\r\"t\"")]
        [InlineData("[a]:\r\n/r\r\n\"t\"")]

        [InlineData("[a]:\n/r\n\"t\nt\"")]
        [InlineData("[a]:\n/r\n\"t\rt\"")]
        [InlineData("[a]:\n/r\n\"t\r\nt\"")]

        [InlineData("[a]:\r\n  /r\t \n \t \"t\r\nt\"   ")]
        [InlineData("[a]:\n/r\n\n[a],")]
        [InlineData("[a]: /r\n[b]: /r\n\n[a],")]
        public void TestNewlines(string markdown)
        {
            MarkdownTest.Run(markdown);
        }

        [Theory]
        [InlineData("[ a]: /r")]
        [InlineData("[a ]: /r")]
        [InlineData("[ a ]: /r")]
        [InlineData("[  a]: /r")]
        [InlineData("[  a ]: /r")]
        [InlineData("[a  ]: /r")]
        [InlineData("[ a  ]: /r")]
        [InlineData("[  a  ]: /r")]
        [InlineData("[a a]: /r")]
        [InlineData("[a\va]: /r")]
        [InlineData("[a\fa]: /r")]
        [InlineData("[a\ta]: /r")]
        [InlineData("[\va]: /r")]
        [InlineData("[\fa]: /r")]
        [InlineData("[\ta]: /r")]
        [InlineData(@"[\]]: /r")]
        public void TestLabel(string markdown)
        {
            MarkdownTest.Run(markdown);
        }

        [Theory]
        [InlineData("[a]: /r ()")]
        [InlineData("[a]: /r (t)")]
        [InlineData("[a]: /r ( t)")]
        [InlineData("[a]: /r (t )")]
        [InlineData("[a]: /r ( t )")]

        [InlineData("[a]: /r ''")]
        [InlineData("[a]: /r 't'")]
        [InlineData("[a]: /r ' t'")]
        [InlineData("[a]: /r 't '")]
        [InlineData("[a]: /r ' t '")]
        public void Test_Title(string markdown)
        {
            MarkdownTest.Run(markdown);
        }

        [Theory]
        [InlineData("[a]: /r\n===\n[a]")]
        public void TestSetextHeader(string markdown)
        {
            MarkdownTest.Run(markdown);
        }
    }
}
