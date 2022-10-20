#region

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units
{
    [Serializable]
    public sealed class Temperature : UnitValue<Temperature.Units, Temperature>
    {
        public enum Units
        {
            Kelvin,
            DegreeCelsius,
            DegreeFahrenheit,
            DegreeRankine,
        }

        private const double OffsetDegreeCelsius = 273.15;
        private const double FactorDegreeRankine = 5.0 / 9.0;

        private static readonly Dictionary<Units, Func<double, double>> ConvertionToBaseUnit = new Dictionary<Units, Func<double, double>>
        {
            {Units.Kelvin, v => v},
            {Units.DegreeCelsius, v => v + OffsetDegreeCelsius},
            {Units.DegreeFahrenheit, v => 5.0 / 9.0 * (v - 32.0) + 273.15},
            {Units.DegreeRankine, v => v * FactorDegreeRankine},
        };

        private static readonly Dictionary<Units, Func<double, double>> ConvertionFromBaseUnit = new Dictionary<Units, Func<double, double>>
        {
            {Units.Kelvin, v => v},
            {Units.DegreeCelsius, v => v - OffsetDegreeCelsius},
            {Units.DegreeFahrenheit, v => 9.0 / 5.0 * (v - 273.15) + 32.0},
            {Units.DegreeRankine, v => v / FactorDegreeRankine}
        };

        public Temperature()
            : this(SIUnit, 0.0)
        {
        }

        public Temperature(double value)
            : this(SIUnit, value)
        {
        }

        public Temperature(Units unit, double value)
            : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
        {
        }

        public Temperature(UnitValue<Units, Temperature> rhs)
            : base(rhs)
        {
        }

        private Temperature(SerializationInfo info, StreamingContext context)
            : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
        {
        }

        public static Units SIUnit
        {
            get { return Units.Kelvin; }
        }

        protected override Units GetDefaultUnit()
        {
            return SIUnit;
        }
    }
}