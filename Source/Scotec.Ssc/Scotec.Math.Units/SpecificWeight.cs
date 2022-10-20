#region

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units
{
    [Serializable]
    public sealed class SpecificWeight : UnitValue<SpecificWeight.Units, SpecificWeight>
    {
        public enum Units
        {
            NewtonPerCubicMeter,
            PoundForcePerCubicFeet,
        }

        private static readonly Dictionary<Units, Func<double, double>> ConvertionToBaseUnit = new Dictionary<Units, Func<double, double>>
        {
            {Units.NewtonPerCubicMeter, v => v}
        };

        private static readonly Dictionary<Units, Func<double, double>> ConvertionFromBaseUnit = new Dictionary<Units, Func<double, double>>
        {
            {Units.NewtonPerCubicMeter, v => v}
        };

        public SpecificWeight()
            : this(SIUnit, 0.0)
        {
        }

        public SpecificWeight(double value)
            : this(SIUnit, value)
        {
        }

        public SpecificWeight(Units unit, double value)
            : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
        {
        }

        public SpecificWeight(UnitValue<Units, SpecificWeight> rhs)
            : base(rhs)
        {
        }

        private SpecificWeight(SerializationInfo info, StreamingContext context)
            : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
        {
        }

        public static Units SIUnit
        {
            get { return Units.NewtonPerCubicMeter; }
        }

        protected override Units GetDefaultUnit()
        {
            return SIUnit;
        }
    }
}