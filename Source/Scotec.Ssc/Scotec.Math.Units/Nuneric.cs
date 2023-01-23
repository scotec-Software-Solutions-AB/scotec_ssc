#region

using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units;

[Serializable]
public sealed class Numeric : UnitValue<Numeric.Units, Numeric>
{
    public enum Units
    {
        Any,
        Ratio,
        Factor,
        Coefficient,
        Percent
    }

    private static readonly Dictionary<Units, Func<double, double>> ConvertionToBaseUnit = new();

    private static readonly Dictionary<Units, Func<double, double>> ConvertionFromBaseUnit = new();

    static Numeric()
    {
        var valueTypes = Enum.GetValues(typeof(Units)).Cast<Units>();
        foreach (var valueType in valueTypes)
            switch (valueType)
            {
                case Units.Percent:
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
        : this(DefaultUnit, 0.0)
    {
    }

    public Numeric(double value)
        : this(DefaultUnit, value)
    {
    }

    public Numeric(Units unit, double value)
        : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public Numeric(UnitValue<Units, Numeric> rhs)
        : base(rhs)
    {
    }

    private Numeric(SerializationInfo info, StreamingContext context)
        : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }


    public static Units DefaultUnit => Units.Any;

    protected override Units GetDefaultUnit()
    {
        return DefaultUnit;
    }
}