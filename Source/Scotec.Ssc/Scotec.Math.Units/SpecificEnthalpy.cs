#region

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units
{
    [Serializable]
    public sealed class SpecificEnthalpy : UnitValue<SpecificEnthalpy.Units, SpecificEnthalpy>
    {
        public enum Units
        {
            JoulePerKilogram,
            KiloJoulePerKilogram,
            BtuPerPound,
            CaloriePerGram,
        }

        private const double FactorKiloJoulePerKilogram = 1000.0;
        private const double FactorBtuPerPound = 2326.0;
        private const double FactorCaloriePerGram = 4186.8;

        private static readonly Dictionary<Units, Func<double, double>> ConvertionToBaseUnit = new Dictionary<Units, Func<double, double>>
        {
            {Units.JoulePerKilogram, v => v},
            {Units.KiloJoulePerKilogram, v => v * FactorKiloJoulePerKilogram},
            {Units.BtuPerPound, v => v * FactorBtuPerPound},
            {Units.CaloriePerGram, v => v * FactorCaloriePerGram}
        };

        private static readonly Dictionary<Units, Func<double, double>> ConvertionFromBaseUnit = new Dictionary<Units, Func<double, double>>
        {
            {Units.JoulePerKilogram, v => v},
            {Units.KiloJoulePerKilogram, v => v / FactorKiloJoulePerKilogram},
            {Units.BtuPerPound, v => v / FactorBtuPerPound},
            {Units.CaloriePerGram, v => v / FactorCaloriePerGram}
        };

        public SpecificEnthalpy()
            : this(SIUnit, 0.0)
        {
        }

        public SpecificEnthalpy(double value)
            : this(SIUnit, value)
        {
        }

        public SpecificEnthalpy(Units unit, double value)
            : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
        {
        }

        public SpecificEnthalpy(UnitValue<Units, SpecificEnthalpy> rhs)
            : base(rhs)
        {
        }

        private SpecificEnthalpy(SerializationInfo info, StreamingContext context)
            : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
        {
        }

        public static Units SIUnit
        {
            get { return Units.JoulePerKilogram; }
        }

        protected override Units GetDefaultUnit()
        {
            return SIUnit;
        }
    }
}