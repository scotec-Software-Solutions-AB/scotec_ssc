
using Xunit;
using Xunit.Sdk;

namespace Scotec.Math.Tests
{
    public class IsEqualTest
    {
        [Theory]
        [InlineData(1.0, 1.0, true)]
        [InlineData(2.0, 1.0, false)]
        [InlineData(1.0, 2.0, false)]
        [InlineData(1.00000001, 1.00000001, true)]
        [InlineData(1.00000002, 1.00000001, false)]
        [InlineData(1.00000001, 1.00000002, false)]
        [InlineData(1.000000001, 1.000000001, true)]
        [InlineData(1.000000002, 1.000000001, true)]
        [InlineData(1.000000001, 1.000000002, true)]
        public void Test(double first, double second, bool expectedResult)
        {
            var result = first.IsEqual(second);

            Assert.Equal(result, expectedResult);
        }
    }
}