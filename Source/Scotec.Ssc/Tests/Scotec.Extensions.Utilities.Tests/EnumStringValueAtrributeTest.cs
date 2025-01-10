using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Scotec.Extensions.Utilities.Tests
{
    public enum TestEnum
    {
        [EnumStringValue("test1")] Test1,

        Test2,
        
    }


    public class EnumStringValueAtrributeTest
    {
        [Theory]
        [InlineData(TestEnum.Test1, "test1")]
        [InlineData(TestEnum.Test2, "Test2")]
        public void TestEnumToString(TestEnum value, string expectedValue)
        {
            Assert.Equal(expectedValue, value.ToStringValue());
        }

        [Theory]
        [InlineData("test1", TestEnum.Test1)] 
        [InlineData("Test2", TestEnum.Test2)]
        public void TestStringToEnum(string value, TestEnum expectedValue)
        {
            Assert.Equal(expectedValue, value.ToEnumValue<TestEnum>());
        }

        [Theory]
        [InlineData(TestEnum.Test1, "\"test1\"")]
        [InlineData(TestEnum.Test2, "\"Test2\"")]

        
        public void TestEnumSerializer(TestEnum value, string expectedValue)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new EnumStringValueJsonConverter() },
            };

            var jsonString = JsonSerializer.Serialize(value, options);

            Assert.Equal(expectedValue, jsonString);

        }

        [Theory]
        [InlineData("\"test1\"", TestEnum.Test1)]
        [InlineData("\"Test2\"", TestEnum.Test2)]

        public void TestEnumDeserializer(string value, TestEnum expectedValue)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new EnumStringValueJsonConverter() },
            };

            var enumValue = JsonSerializer.Deserialize<TestEnum>(value, options);

            Assert.Equal(expectedValue, enumValue);
        }
    }
}
