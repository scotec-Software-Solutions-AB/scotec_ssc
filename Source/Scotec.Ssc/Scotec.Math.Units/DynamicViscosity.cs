#region

using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units;

[Serializable]
public sealed class DynamicViscosity : UnitValue<DynamicViscosity.Units, DynamicViscosity>
{
    public enum Units
    {
        PascalSeconds,
        MilliPascalSeconds,
        Centipoise,
        Poise,
        KilogramPerMeterHour,
        PoundPerFeetHour
    }

    private const double FactorMilliPascalSeconds = 0.001;
    private const double FactorCentipoise = 0.001;
    private const double FactorPoise = 0.1;
    private const double FactorKilogramPerMeterHour = 0.000277777777777778;
    private const double FactorPoundPerFeetHour = 1.48816404199475;

    private static readonly Dictionary<Units, Func<double, double>> ConvertionToBaseUnit = new()
    {
        { Units.PascalSeconds, v => v },
        { Units.MilliPascalSeconds, v => v * FactorMilliPascalSeconds },
        { Units.Centipoise, v => v * FactorCentipoise },
        { Units.Poise, v => v * FactorPoise },
        { Units.KilogramPerMeterHour, v => v * FactorKilogramPerMeterHour },
        { Units.PoundPerFeetHour, v => v * FactorPoundPerFeetHour }
    };

    private static readonly Dictionary<Units, Func<double, double>> ConvertionFromBaseUnit = new()
    {
        { Units.PascalSeconds, v => v },
        { Units.MilliPascalSeconds, v => v / FactorMilliPascalSeconds },
        { Units.Centipoise, v => v / FactorCentipoise },
        { Units.Poise, v => v / FactorPoise },
        { Units.KilogramPerMeterHour, v => v / FactorKilogramPerMeterHour },
        { Units.PoundPerFeetHour, v => v / FactorPoundPerFeetHour }
    };

    public DynamicViscosity()
        : this(SIUnit, 0.0)
    {
    }

    public DynamicViscosity(double value)
        : this(SIUnit, value)
    {
    }

    public DynamicViscosity(Units unit, double value)
        : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public DynamicViscosity(UnitValue<Units, DynamicViscosity> rhs)
        : base(rhs)
    {
    }

    private DynamicViscosity(SerializationInfo info, StreamingContext context)
        : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public static Units SIUnit => Units.PascalSeconds;

    protected override Units GetDefaultUnit()
    {
        return SIUnit;
    }
}