using System.Reflection;

namespace Scotec.Extensions.Utilities.Enums;

/// <summary>
/// Provides extension methods for working with enums, including converting between enums and their associated string values.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Converts an enum field of type <typeparamref name="TEnum" /> to its associated string value.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="enumValue">The enum value to convert.</param>
    /// <returns>
    /// The associated string value if the enum field is decorated with an 
    /// <see cref="EnumStringValueAttribute" />, or the name of the enum field if no attribute is found.
    /// </returns>
    public static string ToStringValue<TEnum>(this TEnum enumValue)
        where TEnum : Enum
    {
        var stringValue = enumValue.ToString();
        var fieldInfo = enumValue.GetType().GetField(stringValue);
        var attribute = fieldInfo?.GetCustomAttribute<EnumStringValueAttribute>();

        return attribute?.StringValue ?? stringValue;
    }

    /// <summary>
    /// Converts an enum value to its associated string value.
    /// </summary>
    /// <param name="enumValue">The enum value to convert.</param>
    /// <returns>
    /// The associated string value if the enum field is decorated with an 
    /// <see cref="EnumStringValueAttribute" />, or the name of the enum field if no attribute is found.
    /// </returns>
    public static string ToStringValue(this Enum enumValue)
    {
        var stringValue = enumValue.ToString();
        var fieldInfo = enumValue.GetType().GetField(stringValue);
        var attribute = fieldInfo?.GetCustomAttribute<EnumStringValueAttribute>();

        return attribute?.StringValue ?? stringValue;
    }

    /// <summary>
    ///     Converts a string value to its corresponding enum field.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="stringValue">The string value to convert.</param>
    /// <returns>
    ///     The corresponding enum field of type <typeparamref name="TEnum" />.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     Thrown when no enum field with the specified string value is found in the enum type.
    /// </exception>
    public static TEnum ToEnumValue<TEnum>(this string stringValue)
        where TEnum : Enum
    {
        return (TEnum)ToEnumValue(stringValue, typeof(TEnum));
    }

    /// <summary>
    ///     Converts a string value to its corresponding enum field.
    /// </summary>
    /// <param name="stringValue">The string value to convert.</param>
    /// <param name="enumType">The type of the enum.</param>
    /// <returns>
    ///     The corresponding enum field as an <see cref="Enum" />.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     Thrown when no enum field with the specified string value is found in the specified enum type.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="stringValue" /> or <paramref name="enumType" /> is <c>null</c>.
    /// </exception>
    public static Enum ToEnumValue(this string stringValue, Type enumType)
    {
        var fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (var field in fields)
        {
            var attribute = field.GetCustomAttribute<EnumStringValueAttribute>();
            if (attribute?.StringValue == stringValue)
            {
                return (Enum)field.GetValue(null)!;
            }
        }

#if NETSTANDARD2_1_OR_GREATER
        if (Enum.TryParse(enumType, stringValue, out var value))
        {
            return (Enum)value;
        }
#else
        return (Enum)Enum.Parse(enumType, stringValue);

#endif
        throw new ArgumentException($"No enum field with string value '{stringValue}' found in {enumType.Name}.");
    }
}
