#region

using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units;

[Serializable]
public sealed class SpecificVolume : UnitValue<SpecificVolume.Units, SpecificVolume>
{
    public enum Units
    {
        CubicMeterPerKilogram,
        CubicCentimeterPerGram,
        CubicInchPerOunce,
        CubicFeetPerPound,
        CubicInchPerPound
    }

    private const double FactorCubicCentimeterPerGram = 0.001;
    private const double FactorCubicInchPerOunce = 0.000578097847915606;
    private const double FactorCubicFeetPerPound = 0.0624279564472421;
    private const double FactorCubicInchPerPound = 0.0000361272896106725;

    private static readonly Dictionary<Units, Func<double, double>> ConvertionToBaseUnit = new()
    {
        { Units.CubicMeterPerKilogram, v => v },
        { Units.CubicCentimeterPerGram, v => v * FactorCubicCentimeterPerGram },
        { Units.CubicInchPerOunce, v => v * FactorCubicInchPerOunce },
        { Units.CubicFeetPerPound, v => v * FactorCubicFeetPerPound },
        { Units.CubicInchPerPound, v => v * FactorCubicInchPerPound }
    };

    private static readonly Dictionary<Units, Func<double, double>> ConvertionFromBaseUnit = new()
    {
        { Units.CubicMeterPerKilogram, v => v },
        { Units.CubicCentimeterPerGram, v => v / FactorCubicCentimeterPerGram },
        { Units.CubicInchPerOunce, v => v / FactorCubicInchPerOunce },
        { Units.CubicFeetPerPound, v => v / FactorCubicFeetPerPound },
        { Units.CubicInchPerPound, v => v / FactorCubicInchPerPound }
    };

    public SpecificVolume()
        : this(SIUnit, 0.0)
    {
    }

    public SpecificVolume(double value)
        : this(SIUnit, value)
    {
    }

    public SpecificVolume(Units unit, double value)
        : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public SpecificVolume(UnitValue<Units, SpecificVolume> rhs)
        : base(rhs)
    {
    }

    private SpecificVolume(SerializationInfo info, StreamingContext context)
        : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public static Units SIUnit => Units.CubicMeterPerKilogram;

    protected override Units GetDefaultUnit()
    {
        return SIUnit;
    }
}