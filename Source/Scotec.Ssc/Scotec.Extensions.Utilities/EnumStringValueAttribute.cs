namespace Scotec.Extensions.Utilities;

/// <summary>
/// An attribute used to associate a string value with an enum field.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class EnumStringValueAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EnumStringValueAttribute" /> class.
    /// </summary>
    /// <param name="stringValue">The string value to associate with the enum field.</param>
    public EnumStringValueAttribute(string stringValue)
    {
        StringValue = stringValue;
    }

    /// <summary>
    /// Gets the string value associated with the enum field.
    /// </summary>
    public string StringValue { get; }
}
