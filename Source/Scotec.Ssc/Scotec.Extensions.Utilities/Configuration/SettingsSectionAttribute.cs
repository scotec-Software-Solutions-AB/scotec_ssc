#if NET8_0_OR_GREATER

namespace Scotec.Extensions.Utilities.Configuration;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class SettingsSectionAttribute : Attribute
{
    public required string SectionName { get; set; }
}

#endif