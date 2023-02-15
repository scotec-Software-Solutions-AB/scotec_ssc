using Xunit;

namespace Scotec.Blazor.Markdown.Tests.Renderer.Inline;

public class TestLinkInline
{
    [Theory]
    [InlineData("[a]")] // TODO: this is not a link but a paragraph
    [InlineData("[a]()")]
    [InlineData("[](b)")]
    [InlineData(" [](b)")]
    [InlineData("[](b) ")]
    [InlineData(" [](b) ")]
    [InlineData("[a](b)")]
    [InlineData(" [a](b)")]
    [InlineData("[a](b) ")]
    [InlineData(" [a](b) ")]
    [InlineData("[ a](b)")]
    [InlineData(" [ a](b)")]
    [InlineData("[ a](b) ")]
    [InlineData(" [ a](b) ")]
    [InlineData("[a ](b)")]
    [InlineData(" [a ](b)")]
    [InlineData("[a ](b) ")]
    [InlineData(" [a ](b) ")]
    [InlineData("[ a ](b)")]
    [InlineData(" [ a ](b)")]
    [InlineData("[ a ](b) ")]
    [InlineData(" [ a ](b) ")]

    // below cases are required for a full roundtrip but not have low prio for impl
    [InlineData("[]( b)")]
    [InlineData(" []( b)")]
    [InlineData("[]( b) ")]
    [InlineData(" []( b) ")]
    [InlineData("[a]( b)")]
    [InlineData(" [a]( b)")]
    [InlineData("[a]( b) ")]
    [InlineData(" [a]( b) ")]
    [InlineData("[ a]( b)")]
    [InlineData(" [ a]( b)")]
    [InlineData("[ a]( b) ")]
    [InlineData(" [ a]( b) ")]
    [InlineData("[a ]( b)")]
    [InlineData(" [a ]( b)")]
    [InlineData("[a ]( b) ")]
    [InlineData(" [a ]( b) ")]
    [InlineData("[ a ]( b)")]
    [InlineData(" [ a ]( b)")]
    [InlineData("[ a ]( b) ")]
    [InlineData(" [ a ]( b) ")]
    [InlineData("[](b )")]
    [InlineData(" [](b )")]
    [InlineData("[](b ) ")]
    [InlineData(" [](b ) ")]
    [InlineData("[a](b )")]
    [InlineData(" [a](b )")]
    [InlineData("[a](b ) ")]
    [InlineData(" [a](b ) ")]
    [InlineData("[ a](b )")]
    [InlineData(" [ a](b )")]
    [InlineData("[ a](b ) ")]
    [InlineData(" [ a](b ) ")]
    [InlineData("[a ](b )")]
    [InlineData(" [a ](b )")]
    [InlineData("[a ](b ) ")]
    [InlineData(" [a ](b ) ")]
    [InlineData("[ a ](b )")]
    [InlineData(" [ a ](b )")]
    [InlineData("[ a ](b ) ")]
    [InlineData(" [ a ](b ) ")]
    [InlineData("[]( b )")]
    [InlineData(" []( b )")]
    [InlineData("[]( b ) ")]
    [InlineData(" []( b ) ")]
    [InlineData("[a]( b )")]
    [InlineData(" [a]( b )")]
    [InlineData("[a]( b ) ")]
    [InlineData(" [a]( b ) ")]
    [InlineData("[ a]( b )")]
    [InlineData(" [ a]( b )")]
    [InlineData("[ a]( b ) ")]
    [InlineData(" [ a]( b ) ")]
    [InlineData("[a ]( b )")]
    [InlineData(" [a ]( b )")]
    [InlineData("[a ]( b ) ")]
    [InlineData(" [a ]( b ) ")]
    [InlineData("[ a ]( b )")]
    [InlineData(" [ a ]( b )")]
    [InlineData("[ a ]( b ) ")]
    [InlineData(" [ a ]( b ) ")]
    public void Test(string markdown)
    {
        MarkdownTest.Run(markdown);
    }

    [Theory]
    [InlineData("[a](b \"t\") ")]
    [InlineData("[a](b \" t\") ")]
    [InlineData("[a](b \"t \") ")]
    [InlineData("[a](b \" t \") ")]
    [InlineData("[a](b  \"t\") ")]
    [InlineData("[a](b  \" t\") ")]
    [InlineData("[a](b  \"t \") ")]
    [InlineData("[a](b  \" t \") ")]
    [InlineData("[a](b \"t\" ) ")]
    [InlineData("[a](b \" t\" ) ")]
    [InlineData("[a](b \"t \" ) ")]
    [InlineData("[a](b \" t \" ) ")]
    [InlineData("[a](b  \"t\" ) ")]
    [InlineData("[a](b  \" t\" ) ")]
    [InlineData("[a](b  \"t \" ) ")]
    [InlineData("[a](b  \" t \" ) ")]
    [InlineData("[a](b 't') ")]
    [InlineData("[a](b ' t') ")]
    [InlineData("[a](b 't ') ")]
    [InlineData("[a](b ' t ') ")]
    [InlineData("[a](b  't') ")]
    [InlineData("[a](b  ' t') ")]
    [InlineData("[a](b  't ') ")]
    [InlineData("[a](b  ' t ') ")]
    [InlineData("[a](b 't' ) ")]
    [InlineData("[a](b ' t' ) ")]
    [InlineData("[a](b 't ' ) ")]
    [InlineData("[a](b ' t ' ) ")]
    [InlineData("[a](b  't' ) ")]
    [InlineData("[a](b  ' t' ) ")]
    [InlineData("[a](b  't ' ) ")]
    [InlineData("[a](b  ' t ' ) ")]
    [InlineData("[a](b (t)) ")]
    [InlineData("[a](b ( t)) ")]
    [InlineData("[a](b (t )) ")]
    [InlineData("[a](b ( t )) ")]
    [InlineData("[a](b  (t)) ")]
    [InlineData("[a](b  ( t)) ")]
    [InlineData("[a](b  (t )) ")]
    [InlineData("[a](b  ( t )) ")]
    [InlineData("[a](b (t) ) ")]
    [InlineData("[a](b ( t) ) ")]
    [InlineData("[a](b (t ) ) ")]
    [InlineData("[a](b ( t ) ) ")]
    [InlineData("[a](b  (t) ) ")]
    [InlineData("[a](b  ( t) ) ")]
    [InlineData("[a](b  (t ) ) ")]
    [InlineData("[a](b  ( t ) ) ")]
    public void Test_Title(string markdown)
    {
        MarkdownTest.Run(markdown);
    }

    [Theory]
    [InlineData("[a](<>)")]
    [InlineData("[a]( <>)")]
    [InlineData("[a](<> )")]
    [InlineData("[a]( <> )")]
    [InlineData("[a](< >)")]
    [InlineData("[a]( < >)")]
    [InlineData("[a](< > )")]
    [InlineData("[a]( < > )")]
    [InlineData("[a](<b>)")]
    [InlineData("[a](<b >)")]
    [InlineData("[a](< b>)")]
    [InlineData("[a](< b >)")]
    [InlineData("[a](<b b>)")]
    [InlineData("[a](<b b >)")]
    [InlineData("[a](< b b >)")]
    public void Test_PointyBrackets(string markdown)
    {
        MarkdownTest.Run(markdown);
    }

    [Theory]
    [InlineData("[*a*][a]")]
    [InlineData("[a][b]")]
    [InlineData("[a][]")]
    [InlineData("[a]")]
    public void Test_Inlines(string markdown)
    {
        MarkdownTest.Run(markdown);
    }

    // | [ a ]( b " t " ) |
    [Theory]
    [InlineData(" [ a ]( b \" t \" ) ")]
    [InlineData("\v[\va\v](\vb\v\"\vt\v\"\v)\v")]
    [InlineData("\f[\fa\f](\fb\f\"\ft\f\"\f)\f")]
    [InlineData("\t[\ta\t](\tb\t\"\tt\t\"\t)\t")]
    public void Test_UncommonWhitespace(string markdown)
    {
        MarkdownTest.Run(markdown);
    }

    [Theory]
    [InlineData("[x]: https://example.com\r\n")]
    public void Test_LinkReferenceDefinitionWithCarriageReturnLineFeed(string markdown)
    {
        MarkdownTest.Run(markdown);
    }
}