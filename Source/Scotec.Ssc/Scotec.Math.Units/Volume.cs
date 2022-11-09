#region

using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units;

[Serializable]
public sealed class Volume : UnitValue<Volume.Units, Volume>
{
    public enum Units
    {
        CubicMeter,
        Liter,
        Deciliter,
        Centiliter,
        Milliliter,
        CubicFeet,
        CubicInch,
        CubicYard,
        GallonUS,
        GallonUK
    }

    private const double FactorLiter = 0.001;
    private const double FactorDeciliter = 0.001 / 10.0;
    private const double FactorCentiliter = 0.001 / 100.0;
    private const double FactorMilliliter = 0.001 / 1000.0;
    private const double FactorCubicFeet = 0.028316846592;
    private const double FactorCubicInch = 0.000016387064;
    private const double CubicYard = 0.764554857984;
    private const double GallonUS = 0.003785412;
    private const double GallonUK = 0.00454609;

    private static readonly Dictionary<Units, Func<double, double>> ConvertionToBaseUnit = new()
    {
        { Units.CubicMeter, v => v },
        { Units.Liter, v => v * FactorLiter },
        { Units.Deciliter, v => v * FactorDeciliter },
        { Units.Centiliter, v => v * FactorCentiliter },
        { Units.Milliliter, v => v * FactorMilliliter },
        { Units.CubicFeet, v => v * FactorCubicFeet },
        { Units.CubicInch, v => v * FactorCubicInch },
        { Units.CubicYard, v => v * CubicYard },
        { Units.GallonUS, v => v * GallonUS },
        { Units.GallonUK, v => v * GallonUK }
    };

    private static readonly Dictionary<Units, Func<double, double>> ConvertionFromBaseUnit = new()
    {
        { Units.CubicMeter, v => v },
        { Units.Liter, v => v / FactorLiter },
        { Units.Deciliter, v => v / FactorDeciliter },
        { Units.Centiliter, v => v / FactorCentiliter },
        { Units.Milliliter, v => v / FactorMilliliter },
        { Units.CubicFeet, v => v / FactorCubicFeet },
        { Units.CubicInch, v => v / FactorCubicInch },
        { Units.CubicYard, v => v / CubicYard },
        { Units.GallonUS, v => v / GallonUS },
        { Units.GallonUK, v => v / GallonUK }
    };

    public Volume()
        : this(SIUnit, 0.0)
    {
    }

    public Volume(double value)
        : this(SIUnit, value)
    {
    }

    public Volume(Units unit, double value)
        : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public Volume(UnitValue<Units, Volume> rhs)
        : base(rhs)
    {
    }

    private Volume(SerializationInfo info, StreamingContext context)
        : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public static Units SIUnit => Units.CubicMeter;

    protected override Units GetDefaultUnit()
    {
        return SIUnit;
    }
}