#region

using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units;

[Serializable]
public sealed class Numeric : UnitValue<Numeric.Types, Numeric>
{
    public enum Types
    {
        Any,
        Ratio,
        Factor,
        Coefficient,
        Percent
    }

    private static readonly Dictionary<Types, Func<double, double>> ConvertionToBaseUnit = new();

    private static readonly Dictionary<Types, Func<double, double>> ConvertionFromBaseUnit = new();

    static Numeric()
    {
        var valueTypes = Enum.GetValues(typeof(Types)).Cast<Types>();
        foreach (var valueType in valueTypes)
            switch (valueType)
            {
                case Types.Percent:
                    ConvertionToBaseUnit.Add(valueType, v => v / 100);
                    ConvertionFromBaseUnit.Add(valueType, v => v * 100);
                    break;
                default:
                    ConvertionToBaseUnit.Add(valueType, v => v);
                    ConvertionFromBaseUnit.Add(valueType, v => v);
                    break;
            }
    }

    public Numeric()
        : this(DefaultType, 0.0)
    {
    }

    public Numeric(double value)
        : this(DefaultType, value)
    {
    }

    public Numeric(Types unit, double value)
        : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public Numeric(UnitValue<Types, Numeric> rhs)
        : base(rhs)
    {
    }

    private Numeric(SerializationInfo info, StreamingContext context)
        : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }


    public static Types DefaultType => Types.Any;

    protected override Types GetDefaultUnit()
    {
        return DefaultType;
    }
}