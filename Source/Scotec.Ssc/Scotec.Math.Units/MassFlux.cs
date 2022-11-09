#region

using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units;

[Serializable]
public sealed class MassFlux : UnitValue<MassFlux.Units, MassFlux>
{
    public enum Units
    {
        KilogramPerSecondSquareMeter,
        PoundPerSecondSquareInch
    }

    private static readonly double FactorPoundPerSecondSquareInch = 1.0 / 2.204622622 /*lb->kg*/ * System.Math.Pow(3.280839895, 2.0) /*ft²->m²*/;

    private static readonly Dictionary<Units, Func<double, double>> ConvertionToBaseUnit = new()
    {
        { Units.KilogramPerSecondSquareMeter, v => v },
        { Units.PoundPerSecondSquareInch, v => v * FactorPoundPerSecondSquareInch }
    };

    private static readonly Dictionary<Units, Func<double, double>> ConvertionFromBaseUnit = new()
    {
        { Units.KilogramPerSecondSquareMeter, v => v },
        { Units.PoundPerSecondSquareInch, v => v / FactorPoundPerSecondSquareInch }
    };

    public MassFlux()
        : this(SIUnit, 0.0)
    {
    }

    public MassFlux(double value)
        : this(SIUnit, value)
    {
    }

    public MassFlux(Units unit, double value)
        : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public MassFlux(UnitValue<Units, MassFlux> rhs)
        : base(rhs)
    {
    }

    private MassFlux(SerializationInfo info, StreamingContext context)
        : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public static Units SIUnit => Units.KilogramPerSecondSquareMeter;

    protected override Units GetDefaultUnit()
    {
        return SIUnit;
    }
}