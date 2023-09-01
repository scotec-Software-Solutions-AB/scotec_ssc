using Xunit;

namespace Scotec.Extensions.Utilities.Tests;

public class StringFormatTest
{
    [Theory]
    [InlineData("_{value}_", StringComparison.Ordinal, "A", "_A_")]
    [InlineData("_{Value}_", StringComparison.Ordinal, "A", "_{Value}_")]
    [InlineData("_{Value}_", StringComparison.OrdinalIgnoreCase, "A", "_A_")]
    public void Test(string template, StringComparison comparison, string data, string expected)
    {
        var result = template.Format(comparison, value => data);
        Assert.Equal(expected, result);
    }
}
