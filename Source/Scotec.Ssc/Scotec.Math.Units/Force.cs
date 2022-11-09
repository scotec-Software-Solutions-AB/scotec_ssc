#region

using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units;

[Serializable]
public sealed class Force : UnitValue<Force.Units, Force>
{
    public enum Units
    {
        Newton,
        Dyne,
        PoundForce,
        KilogramForce,
        KiloNewton,
        Kilopond,
        OunceForce
    }

    private const double FactorDyne = 0.00001;
    private const double FactorPoundForce = 4.448222;
    private const double FactorKilogramForce = 9.80655;
    private const double FactorKiloNewton = 1000.0;
    private const double FactorKilopond = 9.80655;
    private const double FactorOunceForce = 0.278010789225;

    private static readonly Dictionary<Units, Func<double, double>> ConvertionToBaseUnit = new()
    {
        { Units.Newton, v => v },
        { Units.Dyne, v => v * FactorDyne },
        { Units.PoundForce, v => v * FactorPoundForce },
        { Units.KilogramForce, v => v * FactorKilogramForce },
        { Units.KiloNewton, v => v * FactorKiloNewton },
        { Units.Kilopond, v => v * FactorKilopond },
        { Units.OunceForce, v => v * FactorOunceForce }
    };

    private static readonly Dictionary<Units, Func<double, double>> ConvertionFromBaseUnit = new()
    {
        { Units.Newton, v => v },
        { Units.Dyne, v => v / FactorDyne },
        { Units.PoundForce, v => v / FactorPoundForce },
        { Units.KilogramForce, v => v / FactorKilogramForce },
        { Units.KiloNewton, v => v / FactorKiloNewton },
        { Units.Kilopond, v => v / FactorKilopond },
        { Units.OunceForce, v => v / FactorOunceForce }
    };

    public Force()
        : this(SIUnit, 0.0)
    {
    }

    public Force(double value)
        : this(SIUnit, value)
    {
    }

    public Force(Units unit, double value)
        : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public Force(UnitValue<Units, Force> rhs)
        : base(rhs)
    {
    }

    private Force(SerializationInfo info, StreamingContext context)
        : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public static Units SIUnit => Units.Newton;

    protected override Units GetDefaultUnit()
    {
        return SIUnit;
    }
}