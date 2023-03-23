﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Scotec.Blazor.Markdown.Tests.Renderer
{
    public class TestFencedCodeBlock
    {
        [Theory]
        [InlineData("```\nc\n```")]
        [InlineData("```\nc\n```\n")]
        [InlineData("\n```\nc\n```")]
        [InlineData("\n\n```\nc\n```")]
        [InlineData("```\nc\n```\n\n")]
        [InlineData("\n```\nc\n```\n")]
        [InlineData("\n```\nc\n```\n\n")]
        [InlineData("\n\n```\nc\n```\n")]
        [InlineData("\n\n```\nc\n```\n\n")]

        [InlineData(" ```\nc\n````")]
        [InlineData("```\nc\n````")]
        [InlineData("p\n\n```\nc\n```")]

        [InlineData("```\n c\n```")]
        [InlineData("```\nc \n```")]
        [InlineData("```\n c \n```")]

        [InlineData(" ``` \n c \n ``` ")]
        [InlineData("\t```\t\n\tc\t\n\t```\t")]
        [InlineData("\v```\v\n\vc\v\n\v```\v")]
        [InlineData("\f```\f\n\fc\f\n\f```\f")]
        public void Test(string markdown)
        {
            MarkdownTest.Run(markdown);
        }

        [Theory]
        [InlineData("~~~ aa ``` ~~~\nfoo\n~~~")]
        [InlineData("~~~ aa ``` ~~~\nfoo\n~~~ ")]
        public void TestTilde(string markdown)
        {
            MarkdownTest.Run(markdown);
        }

        [Theory]
        [InlineData("```\n c \n```")]
        [InlineData("```\n c \r```")]
        [InlineData("```\n c \r\n```")]
        [InlineData("```\r c \n```")]
        [InlineData("```\r c \r```")]
        [InlineData("```\r c \r\n```")]
        [InlineData("```\r\n c \n```")]
        [InlineData("```\r\n c \r```")]
        [InlineData("```\r\n c \r\n```")]

        [InlineData("```\n c \n```\n")]
        [InlineData("```\n c \r```\n")]
        [InlineData("```\n c \r\n```\n")]
        [InlineData("```\r c \n```\n")]
        [InlineData("```\r c \r```\n")]
        [InlineData("```\r c \r\n```\n")]
        [InlineData("```\r\n c \n```\n")]
        [InlineData("```\r\n c \r```\n")]
        [InlineData("```\r\n c \r\n```\n")]

        [InlineData("```\n c \n```\r")]
        [InlineData("```\n c \r```\r")]
        [InlineData("```\n c \r\n```\r")]
        [InlineData("```\r c \n```\r")]
        [InlineData("```\r c \r```\r")]
        [InlineData("```\r c \r\n```\r")]
        [InlineData("```\r\n c \n```\r")]
        [InlineData("```\r\n c \r```\r")]
        [InlineData("```\r\n c \r\n```\r")]

        [InlineData("```\n c \n```\r\n")]
        [InlineData("```\n c \r```\r\n")]
        [InlineData("```\n c \r\n```\r\n")]
        [InlineData("```\r c \n```\r\n")]
        [InlineData("```\r c \r```\r\n")]
        [InlineData("```\r c \r\n```\r\n")]
        [InlineData("```\r\n c \n```\r\n")]
        [InlineData("```\r\n c \r```\r\n")]
        [InlineData("```\r\n c \r\n```\r\n")]
        public void TestNewline(string markdown)
        {
            MarkdownTest.Run(markdown);
        }

        [Theory]
        [InlineData("```i a\n```")]
        [InlineData("```i a a2\n```")]
        [InlineData("```i a a2 a3\n```")]
        [InlineData("```i a a2 a3 a4\n```")]

        [InlineData("```i\ta\n```")]
        [InlineData("```i\ta a2\n```")]
        [InlineData("```i\ta a2 a3\n```")]
        [InlineData("```i\ta a2 a3 a4\n```")]

        [InlineData("```i\ta \n```")]
        [InlineData("```i\ta a2 \n```")]
        [InlineData("```i\ta a2 a3 \n```")]
        [InlineData("```i\ta a2 a3 a4 \n```")]
        public void TestInfoArguments(string markdown)
        {
            MarkdownTest.Run(markdown);
        }
    }
}
