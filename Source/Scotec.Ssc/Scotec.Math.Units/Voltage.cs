#region

using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units;

[Serializable]
public sealed class Voltage : UnitValue<Voltage.Units, Voltage>
{
    public enum Units
    {
        Volt,
        Millivolt,
        Kilovolt
    }


    private static readonly Dictionary<Units, Func<double, double>> ConvertionToBaseUnit = new()
    {
        { Units.Volt, v => v },
        { Units.Kilovolt, v => v * 1000.0 },
        { Units.Millivolt, v => v * 0.001 }
    };

    private static readonly Dictionary<Units, Func<double, double>> ConvertionFromBaseUnit = new()
    {
        { Units.Volt, v => v },
        { Units.Kilovolt, v => v / 1000.0 },
        { Units.Millivolt, v => v / 0.001 }
    };

    public Voltage()
        : this(SIUnit, 0.0)
    {
    }

    public Voltage(double value)
        : this(SIUnit, value)
    {
    }

    public Voltage(Units unit, double value)
        : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public Voltage(UnitValue<Units, Voltage> rhs)
        : base(rhs)
    {
    }

    private Voltage(SerializationInfo info, StreamingContext context)
        : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public static Units SIUnit => Units.Volt;

    protected override Units GetDefaultUnit()
    {
        return SIUnit;
    }
}