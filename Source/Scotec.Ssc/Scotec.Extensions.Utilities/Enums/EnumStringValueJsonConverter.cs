using System.Text.Json;
using System.Text.Json.Serialization;

namespace Scotec.Extensions.Utilities.Enums;

public class EnumStringValueJsonConverter : JsonConverter<Enum?>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsEnum;
    }

    /// <summary>
    ///     Writes the JSON representation of the specified <see cref="Enum" /> value.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter" /> to write to.</param>
    /// <param name="value">The <see cref="Enum" /> value to write.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions" /> to use during serialization.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the <paramref name="writer" /> or <paramref name="value" /> is <c>null</c>.
    /// </exception>
    public override void Write(Utf8JsonWriter writer, Enum? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(value.ToStringValue());
        }
    }

    /// <summary>
    ///     Reads and converts the JSON representation of an enum value to its corresponding <see cref="Enum" /> object.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader" /> to read from.</param>
    /// <param name="typeToConvert">The type of the enum to convert to.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions" /> to use during deserialization.</param>
    /// <returns>The enum value corresponding to the string representation in the JSON.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the JSON string value is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when the string value does not match any field in the specified enum type.
    /// </exception>
    public override Enum? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringValue = reader.GetString();// ?? throw new ArgumentNullException("reader.GetString()");

        return stringValue?.ToEnumValue(typeToConvert);
    }
}
