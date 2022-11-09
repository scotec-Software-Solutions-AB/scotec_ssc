#region

using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units;

[Serializable]
public sealed class Area : UnitValue<Area.Units, Area>
{
    public enum Units
    {
        SquareMeter,
        SquareMillimeter,
        SquareCentimeter,
        SquareFeet,
        SquareInch,
        SquareYard
    }

    private const double FactorSquareCentimeter = 1 / 1.0e4;
    private const double FactorSquareMillimeter = 1 / 1.0e6;
    private const double FactorSquareFeet = 0.09290304;
    private const double FactorSquareInch = 0.00064516;
    private const double FactorSquareYard = 0.83612736;

    private static readonly Dictionary<Units, Func<double, double>> ConvertionToBaseUnit = new()
    {
        { Units.SquareMeter, v => v },
        { Units.SquareCentimeter, v => v * FactorSquareCentimeter },
        { Units.SquareMillimeter, v => v * FactorSquareMillimeter },
        { Units.SquareFeet, v => v * FactorSquareFeet },
        { Units.SquareInch, v => v * FactorSquareInch },
        { Units.SquareYard, v => v * FactorSquareYard }
    };

    private static readonly Dictionary<Units, Func<double, double>> ConvertionFromBaseUnit = new()
    {
        { Units.SquareMeter, v => v },
        { Units.SquareCentimeter, v => v / FactorSquareCentimeter },
        { Units.SquareMillimeter, v => v / FactorSquareMillimeter },
        { Units.SquareFeet, v => v / FactorSquareFeet },
        { Units.SquareInch, v => v / FactorSquareInch },
        { Units.SquareYard, v => v / FactorSquareYard }
    };

    public Area()
        : this(SIUnit, 0.0)
    {
    }

    public Area(double value)
        : this(SIUnit, value)
    {
    }

    public Area(Units unit, double value)
        : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public Area(UnitValue<Units, Area> rhs)
        : base(rhs)
    {
    }

    private Area(SerializationInfo info, StreamingContext context)
        : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public static Units SIUnit => Units.SquareMeter;

    protected override Units GetDefaultUnit()
    {
        return SIUnit;
    }
}