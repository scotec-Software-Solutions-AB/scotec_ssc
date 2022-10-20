#region

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units
{
    [Serializable]
    public sealed class MolarMass : UnitValue<MolarMass.Units, MolarMass>
    {
        public enum Units
        {
            KilogramPerKilomole,
            GramPerMole,
            PoundPerMole,
            PoundPerPoundmole,
        }

        private const double FactorPoundPerMole = 45.35924;

        private static readonly Dictionary<Units, Func<double, double>> ConvertionToBaseUnit = new Dictionary<Units, Func<double, double>>
        {
            {Units.KilogramPerKilomole, v => v},
            {Units.GramPerMole, v => v},
            {Units.PoundPerMole, v => v * FactorPoundPerMole},
            {Units.PoundPerPoundmole, v => v}
        };

        private static readonly Dictionary<Units, Func<double, double>> ConvertionFromBaseUnit = new Dictionary<Units, Func<double, double>>
        {
            {Units.KilogramPerKilomole, v => v},
            {Units.GramPerMole, v => v},
            {Units.PoundPerMole, v => v / FactorPoundPerMole},
            {Units.PoundPerPoundmole, v => v}
        };

        public MolarMass()
            : this(SIUnit, 0.0)
        {
        }

        public MolarMass(double value)
            : this(SIUnit, value)
        {
        }

        public MolarMass(Units unit, double value)
            : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
        {
        }

        public MolarMass(UnitValue<Units, MolarMass> rhs)
            : base(rhs)
        {
        }

        private MolarMass(SerializationInfo info, StreamingContext context)
            : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
        {
        }

        public static Units SIUnit
        {
            get { return Units.KilogramPerKilomole; }
        }

        protected override Units GetDefaultUnit()
        {
            return SIUnit;
        }
    }
}