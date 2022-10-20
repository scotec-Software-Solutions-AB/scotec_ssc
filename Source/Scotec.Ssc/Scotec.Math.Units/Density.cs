#region

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units
{
    [Serializable]
    public sealed class Density : UnitValue<Density.Units, Density>
    {
        public enum Units
        {
            KilogramPerCubicMeter,
            GramPerMilliliter,
            GramPerCubicCentimeter,
            GrainPerGallon,
            OuncePerCubicInch,
            PoundPerCubicInch,
            PoundPerCubicFeet,
            GramPerLiter,
        }

        private const double FactorGramPerMilliliter = 1000.0;
        private const double FactorGramPerCubicCentimeter = 1000.0;
        private const double FactorGrainPerGallon = 0.017118061045;
        private const double FactorOuncePerCubicInch = 1729.81097162982;
        private const double FactorPoundPerCubicInch = 27679.9065409154;
        private const double FactorPoundPerCubicFeet = 16.0184644334001;

        private static readonly Dictionary<Units, Func<double, double>> ConvertionToBaseUnit = new Dictionary<Units, Func<double, double>>
        {
            {Units.KilogramPerCubicMeter, v => v},
            {Units.GramPerLiter, v => v},
            {Units.GramPerMilliliter, v => v * FactorGramPerMilliliter},
            {Units.GramPerCubicCentimeter, v => v * FactorGramPerCubicCentimeter},
            {Units.GrainPerGallon, v => v * FactorGrainPerGallon},
            {Units.OuncePerCubicInch, v => v * FactorOuncePerCubicInch},
            {Units.PoundPerCubicInch, v => v * FactorPoundPerCubicInch},
            {Units.PoundPerCubicFeet, v => v * FactorPoundPerCubicFeet}
        };

        private static readonly Dictionary<Units, Func<double, double>> ConvertionFromBaseUnit = new Dictionary<Units, Func<double, double>>
        {
            {Units.KilogramPerCubicMeter, v => v},
            {Units.GramPerLiter, v => v},
            {Units.GramPerMilliliter, v => v / FactorGramPerMilliliter},
            {Units.GramPerCubicCentimeter, v => v / FactorGramPerCubicCentimeter},
            {Units.GrainPerGallon, v => v / FactorGrainPerGallon},
            {Units.OuncePerCubicInch, v => v / FactorOuncePerCubicInch},
            {Units.PoundPerCubicInch, v => v / FactorPoundPerCubicInch},
            {Units.PoundPerCubicFeet, v => v / FactorPoundPerCubicFeet}
        };

        public Density()
            : this(SIUnit, 0.0)
        {
        }

        public Density(double value)
            : this(SIUnit, value)
        {
        }

        public Density(Units unit, double value)
            : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
        {
        }

        public Density(UnitValue<Units, Density> rhs)
            : base(rhs)
        {
        }

        private Density(SerializationInfo info, StreamingContext context)
            : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
        {
        }

        public static Units SIUnit
        {
            get { return Units.KilogramPerCubicMeter; }
        }

        protected override Units GetDefaultUnit()
        {
            return SIUnit;
        }
    }
}