#region

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units
{
    [Serializable]
    public sealed class Time : UnitValue<Time.Units, Time>
    {
        public enum Units
        {
            Microsecond,
            Millisecond,
            Second,
            Minute,
            Hour,
        }


        private static readonly Dictionary<Units, Func<double, double>> ConvertionToBaseUnit = new Dictionary<Units, Func<double, double>>
        {
            {Units.Microsecond, v => v / 1.0E6},
            {Units.Millisecond, v => v / 1000.0},
            {Units.Second, v => v},
            {Units.Minute, v => v * 60.0},
            {Units.Hour, v => v * 3600.0}
        };

        private static readonly Dictionary<Units, Func<double, double>> ConvertionFromBaseUnit = new Dictionary<Units, Func<double, double>>
        {
            {Units.Microsecond, v => v * 1.0E6},
            {Units.Millisecond, v => v * 1000.0},
            {Units.Second, v => v},
            {Units.Minute, v => v / 60.0},
            {Units.Hour, v => v / 3600.0}
        };

        public Time()
            : this(SIUnit, 0.0)
        {
        }

        public Time(double value)
            : this(SIUnit, value)
        {
        }

        public Time(Units unit, double value)
            : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
        {
        }

        public Time(UnitValue<Units, Time> rhs)
            : base(rhs)
        {
        }

        private Time(SerializationInfo info, StreamingContext context)
            : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
        {
        }

        public static Units SIUnit
        {
            get { return Units.Second; }
        }

        protected override Units GetDefaultUnit()
        {
            return SIUnit;
        }
    }
}