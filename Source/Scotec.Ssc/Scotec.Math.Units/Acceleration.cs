#region

using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units;

[Serializable]
public sealed class Acceleration : UnitValue<Acceleration.Units, Acceleration>
{
    public enum Units
    {
        MeterPerSquareSecond
    }

    private static readonly Dictionary<Units, Func<double, double>> ConvertionToBaseUnit = new()
    {
        { Units.MeterPerSquareSecond, v => v }
    };

    private static readonly Dictionary<Units, Func<double, double>> ConvertionFromBaseUnit = new()
    {
        { Units.MeterPerSquareSecond, v => v }
    };

    public Acceleration()
        : this(SIUnit, 0.0)
    {
    }

    public Acceleration(double value)
        : this(SIUnit, value)
    {
    }

    public Acceleration(Units unit, double value)
        : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public Acceleration(UnitValue<Units, Acceleration> rhs)
        : base(rhs)
    {
    }

    private Acceleration(SerializationInfo info, StreamingContext context)
        : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public static Units SIUnit => Units.MeterPerSquareSecond;

    protected override Units GetDefaultUnit()
    {
        return SIUnit;
    }
}