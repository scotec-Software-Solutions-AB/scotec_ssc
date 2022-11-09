#region

using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units;

[Serializable]
public sealed class Angle : UnitValue<Angle.Units, Angle>
{
    public enum Units
    {
        Radiant,
        Degree,
        Gon
    }

    private const double FactorDegree = 180.0 / System.Math.PI;
    private const double FactorGon = 200 / System.Math.PI;

    private static readonly Dictionary<Units, Func<double, double>> ConvertionToBaseUnit = new()
    {
        { Units.Radiant, v => v },
        { Units.Degree, v => v / FactorDegree },
        { Units.Gon, v => v / FactorGon }
    };

    private static readonly Dictionary<Units, Func<double, double>> ConvertionFromBaseUnit = new()
    {
        { Units.Radiant, v => v },
        { Units.Degree, v => v * FactorDegree },
        { Units.Gon, v => v * FactorGon }
    };

    public Angle()
        : this(SIUnit, 0.0)
    {
    }

    public Angle(double value)
        : this(SIUnit, value)
    {
    }

    public Angle(Units unit, double value)
        : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public Angle(UnitValue<Units, Angle> rhs)
        : base(rhs)
    {
    }

    private Angle(SerializationInfo info, StreamingContext context)
        : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public static Units SIUnit => Units.Radiant;

    protected override Units GetDefaultUnit()
    {
        return SIUnit;
    }
}