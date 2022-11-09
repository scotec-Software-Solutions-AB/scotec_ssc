#region

using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units;

[Serializable]
public sealed class Velocity : UnitValue<Velocity.Units, Velocity>
{
    public enum Units
    {
        MeterPerSecond,
        KilometerPerHour
    }

    private const double KilometerPerHour = 1000.0 / 3600.0;

    private static readonly Dictionary<Units, Func<double, double>> ConvertionToBaseUnit = new()
    {
        { Units.MeterPerSecond, v => v },
        { Units.KilometerPerHour, v => v * KilometerPerHour }
    };

    private static readonly Dictionary<Units, Func<double, double>> ConvertionFromBaseUnit = new()
    {
        { Units.MeterPerSecond, v => v },
        { Units.KilometerPerHour, v => v / KilometerPerHour }
    };

    public Velocity()
        : this(SIUnit, 0.0)
    {
    }

    public Velocity(double value)
        : this(SIUnit, value)
    {
    }

    public Velocity(Units unit, double value)
        : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public Velocity(UnitValue<Units, Velocity> rhs)
        : base(rhs)
    {
    }

    private Velocity(SerializationInfo info, StreamingContext context)
        : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public static Units SIUnit => Units.MeterPerSecond;

    protected override Units GetDefaultUnit()
    {
        return SIUnit;
    }
}