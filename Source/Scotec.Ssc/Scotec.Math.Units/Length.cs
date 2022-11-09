#region

using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units;

[Serializable]
public sealed class Length : UnitValue<Length.Units, Length>
{
    public enum Units
    {
        Meter,
        Millimeter,
        Centimeter,
        Micrometer,
        Feet,
        Inch,
        Yard
    }

    private const double FactorMicrometer = 0.000001;
    private const double FactorMillimeter = 0.001;
    private const double FactorCentimeter = 0.01;
    private const double FactorFeet = 0.3048;
    private const double FactorInch = 0.0254;
    private const double FactorYard = 0.9144;

    private static readonly Dictionary<Units, Func<double, double>> ConvertionToBaseUnit = new()
    {
        { Units.Meter, v => v },
        { Units.Micrometer, v => v * FactorMicrometer },
        { Units.Millimeter, v => v * FactorMillimeter },
        { Units.Centimeter, v => v * FactorCentimeter },
        { Units.Feet, v => v * FactorFeet },
        { Units.Inch, v => v * FactorInch },
        { Units.Yard, v => v * FactorYard }
    };

    private static readonly Dictionary<Units, Func<double, double>> ConvertionFromBaseUnit = new()
    {
        { Units.Meter, v => v },
        { Units.Micrometer, v => v / FactorMicrometer },
        { Units.Millimeter, v => v / FactorMillimeter },
        { Units.Centimeter, v => v / FactorCentimeter },
        { Units.Feet, v => v / FactorFeet },
        { Units.Inch, v => v / FactorInch },
        { Units.Yard, v => v / FactorYard }
    };

    public Length()
        : this(SIUnit, 0.0)
    {
    }

    public Length(double value)
        : this(SIUnit, value)
    {
    }

    public Length(Units unit, double value)
        : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public Length(UnitValue<Units, Length> rhs)
        : base(rhs)
    {
    }

    private Length(SerializationInfo info, StreamingContext context)
        : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public static Units SIUnit => Units.Meter;

    public static Area operator *(Length value1, Length value2)
    {
        return new Area(value1[SIUnit] * value2[SIUnit]) { Unit = Area.SIUnit };
    }

    protected override Units GetDefaultUnit()
    {
        return SIUnit;
    }
}