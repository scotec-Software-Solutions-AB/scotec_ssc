#region

using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units;

[Serializable]
public sealed class Loudness : UnitValue<Loudness.Units, Loudness>
{
    public enum Units
    {
        // ReSharper disable once InconsistentNaming
        dB
    }

    private static readonly Dictionary<Units, Func<double, double>> ConvertionToBaseUnit = new() { { Units.dB, v => v } };

    private static readonly Dictionary<Units, Func<double, double>> ConvertionFromBaseUnit = new()
    {
        { Units.dB, v => v }
    };

    public Loudness()
        : this(SIUnit, 0.0)
    {
    }

    public Loudness(double value)
        : this(SIUnit, value)
    {
    }

    public Loudness(Units unit, double value)
        : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public Loudness(UnitValue<Units, Loudness> rhs)
        : base(rhs)
    {
    }

    private Loudness(SerializationInfo info, StreamingContext context)
        : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public static Units SIUnit => Units.dB;

    protected override Units GetDefaultUnit()
    {
        return SIUnit;
    }
}