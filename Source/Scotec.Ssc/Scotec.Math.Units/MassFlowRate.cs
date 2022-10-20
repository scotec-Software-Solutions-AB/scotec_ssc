#region

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units
{
    [Serializable]
    public sealed class MassFlowRate : UnitValue<MassFlowRate.Units, MassFlowRate>
    {
        public enum Units
        {
            KilogramPerSecond,
            KilogramPerHour,
            KilogramPerMinute,
            TonnePerHour,
            PoundPerHour,
            PoundPerMinute,
            PoundPerSecond,
        }

        private const double FactorKilogramPerHour = 1 / 3600.0;
        private const double FactorKilogramPerMinute = 1 / 60.0;
        private const double FactorPoundPerSecond = 0.4535924;
        private const double FactorPoundPerMinute = FactorPoundPerSecond / 60.0;
        private const double FactorPoundPerHour = FactorPoundPerSecond / 3600.0;
        private const double FactorTonnePerHour = 1000.0 / 3600.0;

        private static readonly Dictionary<Units, Func<double, double>> ConvertionToBaseUnit = new Dictionary<Units, Func<double, double>>
        {
            {Units.KilogramPerSecond, v => v},
            {Units.KilogramPerHour, v => v * FactorKilogramPerHour},
            {Units.KilogramPerMinute, v => v * FactorKilogramPerMinute},
            {Units.PoundPerHour, v => v * FactorPoundPerHour},
            {Units.PoundPerMinute, v => v * FactorPoundPerMinute},
            {Units.PoundPerSecond, v => v * FactorPoundPerSecond},
            {Units.TonnePerHour, v => v * FactorTonnePerHour}
        };

        private static readonly Dictionary<Units, Func<double, double>> ConvertionFromBaseUnit = new Dictionary<Units, Func<double, double>>
        {
            {Units.KilogramPerSecond, v => v},
            {Units.KilogramPerHour, v => v / FactorKilogramPerHour},
            {Units.KilogramPerMinute, v => v / FactorKilogramPerMinute},
            {Units.PoundPerHour, v => v / FactorPoundPerHour},
            {Units.PoundPerMinute, v => v / FactorPoundPerMinute},
            {Units.PoundPerSecond, v => v / FactorPoundPerSecond},
            {Units.TonnePerHour, v => v / FactorTonnePerHour}
        };

        public MassFlowRate()
            : this(SIUnit, 0.0)
        {
        }

        public MassFlowRate(double value)
            : this(SIUnit, value)
        {
        }

        public MassFlowRate(Units unit, double value)
            : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
        {
        }

        public MassFlowRate(UnitValue<Units, MassFlowRate> rhs)
            : base(rhs)
        {
        }

        private MassFlowRate(SerializationInfo info, StreamingContext context)
            : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
        {
        }

        public static Units SIUnit
        {
            get { return Units.KilogramPerSecond; }
        }

        protected override Units GetDefaultUnit()
        {
            return SIUnit;
        }
    }
}