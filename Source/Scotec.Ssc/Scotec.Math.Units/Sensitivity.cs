#region

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units
{
    [Serializable]
    public sealed class Sensitivity : UnitValue<Sensitivity.Units, Sensitivity>
    {
        public enum Units
        {
            CoulombPerPascal,
            CoulombPerBar,
            PicoCoulombPerBar,
        }

        private const double FactorCoulombPerBar = 1.0e-5;
        private const double FactorPicoCoulombPerBar = 1.0e-17;

        private static readonly Dictionary<Units, Func<double, double>> ConvertionToBaseUnit = new Dictionary<Units, Func<double, double>>
        {
            {Units.CoulombPerPascal, v => v},
            {Units.CoulombPerBar, v => v * FactorCoulombPerBar},
            {Units.PicoCoulombPerBar, v => v * FactorPicoCoulombPerBar},
        };

        private static readonly Dictionary<Units, Func<double, double>> ConvertionFromBaseUnit = new Dictionary<Units, Func<double, double>>
        {
            {Units.CoulombPerPascal, v => v},
            {Units.CoulombPerBar, v => v / FactorCoulombPerBar},
            {Units.PicoCoulombPerBar, v => v / FactorPicoCoulombPerBar},
        };

        public Sensitivity()
            : this(SIUnit, 0.0)
        {
        }

        public Sensitivity(double value)
            : this(SIUnit, value)
        {
        }

        public Sensitivity(Units unit, double value)
            : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
        {
        }

        public Sensitivity(UnitValue<Units, Sensitivity> rhs)
            : base(rhs)
        {
        }

        private Sensitivity(SerializationInfo info, StreamingContext context)
            : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
        {
        }

        public static Units SIUnit
        {
            get { return Units.CoulombPerPascal; }
        }

        protected override Units GetDefaultUnit()
        {
            return SIUnit;
        }
    }
}