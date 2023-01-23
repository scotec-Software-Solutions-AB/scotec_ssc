#region

using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units;

[Serializable]
public sealed class Current : UnitValue<Current.Units, Current>
{
    public enum Units
    {
        Ampere,
        KiloAmpere,
        MilliAmpere,
        MicroAmpere,
        NanoAmpere
    }


    private static readonly Dictionary<Units, Func<double, double>> ConvertionToBaseUnit = new()
    {
        { Units.Ampere, a => a },
        { Units.KiloAmpere, a => a * 1000.0 },
        { Units.MilliAmpere, v => v * 0.001 },
        { Units.MicroAmpere, v => v * 0.000001 },
        { Units.NanoAmpere, v => v * 0.000000001 },
    };

    private static readonly Dictionary<Units, Func<double, double>> ConvertionFromBaseUnit = new()
    {
        { Units.Ampere, v => v },
        { Units.KiloAmpere, v => v / 1000.0 },
        { Units.MilliAmpere, v => v / 0.001 },
        { Units.MicroAmpere, v => v / 0.000001 },
        { Units.NanoAmpere, v => v / 0.000000001 },
    };

    public Current()
        : this(SIUnit, 0.0)
    {
    }

    public Current(double value)
        : this(SIUnit, value)
    {
    }

    public Current(Units unit, double value)
        : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public Current(UnitValue<Units, Current> rhs)
        : base(rhs)
    {
    }

    private Current(SerializationInfo info, StreamingContext context)
        : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public static Units SIUnit => Units.Ampere;

    protected override Units GetDefaultUnit()
    {
        return SIUnit;
    }
}