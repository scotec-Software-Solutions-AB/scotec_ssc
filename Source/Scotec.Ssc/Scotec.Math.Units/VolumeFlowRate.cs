#region

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units
{
    [Serializable]
    public sealed class VolumeFlowRate : UnitValue<VolumeFlowRate.Units, VolumeFlowRate>
    {
        public enum Units
        {
            CubicMeterPerSecond,
            CubicMeterPerMinute,
            CubicMeterPerHour,
            LiterPerHour,
            LiterPerMinute,
            MilliliterPerMinute,
            MilliliterPerSecond,
            CubicFeetPerMinute,
            CubicFeetPerHour,
            CubicFeetPerSecond,
            GallonPerMinuteUS,
            GallonPerMinuteUK,
            GallonPerHourUS,
            GallonPerHourUK,
        }

        private const double FactorCubicMeterPerMinute = 1.0 / 60.0;
        private const double FactorCubicMeterPerHour = 1.0 / 3600.0;
        private const double FactorLiterPerHour = 0.001 / 3600.0;
        private const double FactorLiterPerMinute = 0.001 / 60.0;
        private const double FactorMilliliterPerMinute = 0.001 / 1000.0 / 60.0;
        private const double FactorMilliliterPerSecond = 0.001 / 1000.0;
        private const double FactorCubicFeetPerSecond = 0.02831685;
        private const double FactorCubicFeetPerMinute = FactorCubicFeetPerSecond / 60;
        private const double FactorCubicFeetPerHour = FactorCubicFeetPerSecond / 3600.0;
        private const double FactorGallonPerMinuteUK = 0.0000757681666666667;
        private const double FactorGallonPerMinuteUS = 0.0000630902;
        private const double FactorGallonPerHourUK = FactorGallonPerMinuteUK / 60.0;
        private const double FactorGallonPerHourUS = FactorGallonPerMinuteUS / 60.0;

        private static readonly Dictionary<Units, Func<double, double>> ConvertionToBaseUnit = new Dictionary<Units, Func<double, double>>
        {
            {Units.CubicMeterPerSecond, v => v},
            {Units.CubicMeterPerMinute, v => v * FactorCubicMeterPerMinute},
            {Units.CubicMeterPerHour, v => v * FactorCubicMeterPerHour},
            {Units.LiterPerHour, v => v * FactorLiterPerHour},
            {Units.LiterPerMinute, v => v * FactorLiterPerMinute},
            {Units.MilliliterPerMinute, v => v * FactorMilliliterPerMinute},
            {Units.MilliliterPerSecond, v => v * FactorMilliliterPerSecond},
            {Units.CubicFeetPerMinute, v => v * FactorCubicFeetPerMinute},
            {Units.CubicFeetPerHour, v => v * FactorCubicFeetPerHour},
            {Units.CubicFeetPerSecond, v => v * FactorCubicFeetPerSecond},
            {Units.GallonPerMinuteUK, v => v * FactorGallonPerMinuteUK},
            {Units.GallonPerMinuteUS, v => v * FactorGallonPerMinuteUS},
            {Units.GallonPerHourUK, v => v * FactorGallonPerHourUK},
            {Units.GallonPerHourUS, v => v * FactorGallonPerHourUS}
        };

        private static readonly Dictionary<Units, Func<double, double>> ConvertionFromBaseUnit = new Dictionary<Units, Func<double, double>>
        {
            {Units.CubicMeterPerSecond, v => v},
            {Units.CubicMeterPerMinute, v => v / FactorCubicMeterPerMinute},
            {Units.CubicMeterPerHour, v => v / FactorCubicMeterPerHour},
            {Units.LiterPerHour, v => v / FactorLiterPerHour},
            {Units.LiterPerMinute, v => v / FactorLiterPerMinute},
            {Units.MilliliterPerMinute, v => v / FactorMilliliterPerMinute},
            {Units.MilliliterPerSecond, v => v / FactorMilliliterPerSecond},
            {Units.CubicFeetPerMinute, v => v / FactorCubicFeetPerMinute},
            {Units.CubicFeetPerHour, v => v / FactorCubicFeetPerHour},
            {Units.CubicFeetPerSecond, v => v / FactorCubicFeetPerSecond},
            {Units.GallonPerMinuteUK, v => v / FactorGallonPerMinuteUK},
            {Units.GallonPerMinuteUS, v => v / FactorGallonPerMinuteUS},
            {Units.GallonPerHourUK, v => v / FactorGallonPerHourUK},
            {Units.GallonPerHourUS, v => v / FactorGallonPerHourUS}
        };

        public VolumeFlowRate()
            : this(SIUnit, 0.0)
        {
        }

        public VolumeFlowRate(double value)
            : this(SIUnit, value)
        {
        }

        public VolumeFlowRate(Units unit, double value)
            : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
        {
        }

        public VolumeFlowRate(UnitValue<Units, VolumeFlowRate> rhs)
            : base(rhs)
        {
        }

        private VolumeFlowRate(SerializationInfo info, StreamingContext context)
            : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
        {
        }

        public static Units SIUnit
        {
            get { return Units.CubicMeterPerSecond; }
        }

        protected override Units GetDefaultUnit()
        {
            return SIUnit;
        }
    }
}