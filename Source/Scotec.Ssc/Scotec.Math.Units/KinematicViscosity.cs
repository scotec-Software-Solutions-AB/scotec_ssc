#region

using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units;

[Serializable]
public sealed class KinematicViscosity : UnitValue<KinematicViscosity.Units, KinematicViscosity>
{
    public enum Units
    {
        SquareMeterPerSecond,
        SquareMillimeterPerSecond,
        Stokes,
        Centistokes,
        SayboltUniversalSecond,
        SquareFeetPerSecond,
        SquareInchPerSecond
    }

    private const double FactorSquareMillimeterPerSecond = 0.000001;
    private const double FactorStokes = 0.0001;
    private const double FactorCentistokes = 0.000001;

    // ReSharper disable once UnusedMember.Local
    private const double FactorSayboltUniversalSecond = FactorCentistokes / 0.2162;
    //Näherungswert bei SUS > 100. Si-Tech 4 rechnet mit Definition ASTM D2161

    private const double FactorSquareFeetPerSecond = 0.0929034;
    private const double FactorSquareInchPerSecond = 0.00064516;

    private static readonly Dictionary<Units, Func<double, double>> ConvertionToBaseUnit = new()
    {
        { Units.SquareMeterPerSecond, v => v },
        { Units.SquareMillimeterPerSecond, v => v * FactorSquareMillimeterPerSecond },
        { Units.Stokes, v => v * FactorStokes },
        { Units.Centistokes, v => v * FactorCentistokes },
        // No conversion possible for SUS.
        { Units.SayboltUniversalSecond, v => v },
        { Units.SquareFeetPerSecond, v => v * FactorSquareFeetPerSecond },
        { Units.SquareInchPerSecond, v => v * FactorSquareInchPerSecond }
    };

    private static readonly Dictionary<Units, Func<double, double>> ConvertionFromBaseUnit = new()
    {
        { Units.SquareMeterPerSecond, v => v },
        { Units.SquareMillimeterPerSecond, v => v / FactorSquareMillimeterPerSecond },
        { Units.Stokes, v => v / FactorStokes },
        { Units.Centistokes, v => v / FactorCentistokes },
        { Units.SayboltUniversalSecond, v => v }, //Si-Tech 4 rechnet mit Definition ASTM D2161, daher hier keine Umwandlung.
        { Units.SquareFeetPerSecond, v => v / FactorSquareFeetPerSecond },
        { Units.SquareInchPerSecond, v => v / FactorSquareInchPerSecond }
    };

    public KinematicViscosity()
        : this(SIUnit, 0.0)
    {
    }

    public KinematicViscosity(double value)
        : this(SIUnit, value)
    {
    }

    public KinematicViscosity(Units unit, double value)
        : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public KinematicViscosity(UnitValue<Units, KinematicViscosity> rhs)
        : base(rhs)
    {
    }

    private KinematicViscosity(SerializationInfo info, StreamingContext context)
        : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public static Units SIUnit => Units.SquareMeterPerSecond;

    protected override Units GetDefaultUnit()
    {
        return SIUnit;
    }
}