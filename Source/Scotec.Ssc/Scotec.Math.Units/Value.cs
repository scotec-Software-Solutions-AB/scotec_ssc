﻿#region

using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units;

[Serializable]
[Obsolete("This type is marked as depricated and will be removed in a future release. Use Scotec.Math.Units.Numeric instead.")]
public sealed class Value : UnitValue<Value.Types, Value>
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

    static Value()
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

    public Value()
        : this(DefaultType, 0.0)
    {
    }

    public Value(double value)
        : this(DefaultType, value)
    {
    }

    public Value(Types unit, double value)
        : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public Value(UnitValue<Types, Value> rhs)
        : base(rhs)
    {
    }

    private Value(SerializationInfo info, StreamingContext context)
        : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }


    public static Types DefaultType => Types.Any;

    protected override Types GetDefaultUnit()
    {
        return DefaultType;
    }
}