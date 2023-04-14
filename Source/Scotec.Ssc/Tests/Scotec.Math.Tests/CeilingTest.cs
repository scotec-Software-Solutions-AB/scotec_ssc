
using Xunit;
using Xunit.Sdk;

namespace Scotec.Math.Tests
{
    public class CeilingTest
    {
        [Theory]
        [InlineData(13.0, 1, 13.0)]
        [InlineData(-13.0, 1, -13.0)]
        [InlineData(13.1, 1, 14.0)]
        [InlineData(-13.1, 1, -14.0)]
        [InlineData(13.0, 2, 14.0)]
        [InlineData(-13.0, 2, -14.0)]
        [InlineData(13.0, 2.3, 13.8)]
        [InlineData(1.234, 0.01, 1.24)]
        [InlineData(1.234, 0.1, 1.3)]
        [InlineData(0.234, 0.1, 0.3)]
        [InlineData(0.034, 0.01, 0.04)]
        [InlineData(0.034, 0.1, 0.1)]
        [InlineData(0.0, 1, 0)]
        [InlineData(0.0, 2, 0)]
        [InlineData(0.0, 0.1, 0)]
        void Test(double value, double significance, double expectedResult)
        {
            var result = value.Ceiling(significance);

            Assert.Equal(result, expectedResult, 8);
        }
    }
}