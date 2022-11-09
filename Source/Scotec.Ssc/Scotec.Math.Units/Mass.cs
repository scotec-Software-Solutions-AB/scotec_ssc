#region

using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units;

[Serializable]
public sealed class Mass : UnitValue<Mass.Units, Mass>
{
    public enum Units
    {
        Kilogram,
        Gram,
        Milligram,
        Tonne,
        Ounce,
        Pound
    }

    private const double FactorGram = 1 / 1000.0;
    private const double FactorMilligram = 1 / 1000000.0;
    private const double FactorTonne = 1000.0;
    private const double FactorOunce = 0.0283495;
    private const double FactorPound = 0.4535924;

    private static readonly Dictionary<Units, Func<double, double>> ConvertionToBaseUnit = new()
    {
        { Units.Kilogram, v => v },
        { Units.Gram, v => v * FactorGram },
        { Units.Milligram, v => v * FactorMilligram },
        { Units.Tonne, v => v * FactorTonne },
        { Units.Ounce, v => v * FactorOunce },
        { Units.Pound, v => v * FactorPound }
    };

    private static readonly Dictionary<Units, Func<double, double>> ConvertionFromBaseUnit = new()
    {
        { Units.Kilogram, v => v },
        { Units.Gram, v => v / FactorGram },
        { Units.Milligram, v => v / FactorMilligram },
        { Units.Tonne, v => v / FactorTonne },
        { Units.Ounce, v => v / FactorOunce },
        { Units.Pound, v => v / FactorPound }
    };

    public Mass()
        : this(SIUnit, 0.0)
    {
    }

    public Mass(double value)
        : this(SIUnit, value)
    {
    }

    public Mass(Units unit, double value)
        : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public Mass(UnitValue<Units, Mass> rhs)
        : base(rhs)
    {
    }

    private Mass(SerializationInfo info, StreamingContext context)
        : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public static Units SIUnit => Units.Kilogram;

    protected override Units GetDefaultUnit()
    {
        return SIUnit;
    }
}