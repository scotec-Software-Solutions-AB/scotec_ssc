#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

//TODO: Use resources (or not?)
//using Scotec.Psc.Resources;
//using Scotec.Psc.ServiceLocation;

#endregion


namespace Scotec.Math.Units
{
    [Serializable]
    public abstract class UnitValue<TUnit, TImplementation> : IComparable, IComparable<UnitValue<TUnit, TImplementation>>, IUnitValue<TUnit>
        where TImplementation : UnitValue<TUnit, TImplementation>, new()
        where TUnit : struct
    {
        private static IList<TUnit> s_units;
        private readonly Dictionary<TUnit, Func<double, double>> _conversionFromBaseUnit;
        private readonly Dictionary<TUnit, Func<double, double>> _conversionToBaseUnit;
        private double _value;

        //TODO: Use resources
        //private static readonly IEnumLocalizer EnumLocalizer;

        static UnitValue()
        {
            //TODO: Use resources
            //ServiceLocator.Current?.TryGetInstance( out EnumLocalizer );
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        private UnitValue(Dictionary<TUnit, Func<double, double>> conversionFromBaseUnit, Dictionary<TUnit, Func<double, double>> conversionToBaseUnit)
        {
            _conversionFromBaseUnit = conversionFromBaseUnit;
            _conversionToBaseUnit = conversionToBaseUnit;

            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            Unit = GetDefaultUnit();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        // ReSharper disable once UnusedParameter.Local
        protected UnitValue(SerializationInfo info, StreamingContext context, Dictionary<TUnit, Func<double, double>> conversionFromBaseUnit,
            Dictionary<TUnit, Func<double, double>> conversionToBaseUnit)
            : this(conversionFromBaseUnit, conversionToBaseUnit)
        {
            _value = info.GetDouble("SIValue");
            Unit = (TUnit)Enum.Parse(typeof(TUnit), info.GetString("Unit") ?? string.Empty);
        }

        protected UnitValue(TUnit unit, double value, Dictionary<TUnit, Func<double, double>> conversionFromBaseUnit,
            Dictionary<TUnit, Func<double, double>> conversionToBaseUnit)
            : this(conversionFromBaseUnit, conversionToBaseUnit)
        {
            this[unit] = value;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        protected UnitValue(UnitValue<TUnit, TImplementation> rhs)
            : this(rhs._conversionFromBaseUnit, rhs._conversionToBaseUnit)
        {
            _value = rhs._value;
        }

        public bool IsNaN
        {
            get { return double.IsNaN(_value); }
        }

        public bool IsInfinity
        {
            get { return double.IsInfinity(_value); }
        }

        public bool IsNegativeInfinity
        {
            get { return double.IsNegativeInfinity(_value); }
        }

        public bool IsPositiveInfinity
        {
            get { return double.IsPositiveInfinity(_value); }
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(this, obj))
                return 0;

            if (obj == null)
                return 1;

            var other = obj as UnitValue<TUnit, TImplementation>;
            if (other == null)
                throw new InvalidCastException();
            
            return _value.IsGreater(other._value) ? 1 : _value.IsLower(other._value) ? -1 : 0;
        }

        public int CompareTo(UnitValue<TUnit, TImplementation> other)
        {
            return this > other ? 1 : this < other ? -1 : 0;
        }

        public double this[TUnit unit]
        {
            get { return System.Math.Round(_conversionFromBaseUnit[unit](_value), 15); }
            set { _value = System.Math.Round(_conversionToBaseUnit[unit](value), 15); }
        }

        public TUnit Unit { get; set; }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("SIValue", _value);
            info.AddValue("Unit", Unit.ToString());
        }

        public static IEnumerable<TUnit> GetUnits()
        {
            return s_units ??= Enum.GetValues(typeof(TUnit)).Cast<TUnit>().ToList().AsReadOnly();
        }

        public TImplementation Clone()
        {
            return new TImplementation { _value = _value, Unit = Unit };
        }

        public static TImplementation operator *(UnitValue<TUnit, TImplementation> value, double factor)
        {
            return new TImplementation { [value.Unit] = value[value.Unit] * factor, Unit = value.Unit };
        }

        public static TImplementation operator +(UnitValue<TUnit, TImplementation> value1, UnitValue<TUnit, TImplementation> value2)
        {
            return new TImplementation
            {
                // Use the same unit for value1 and value2. The result has the unit of value1.
                [value1.Unit] = value1[value1.Unit] + value2[value1.Unit]
            };
        }

        public static TImplementation operator -(UnitValue<TUnit, TImplementation> value1, UnitValue<TUnit, TImplementation> value2)
        {
            return new TImplementation
            {
                // Use the same unit for value1 and value2. The result has the unit of value1.
                [value1.Unit] = value1[value1.Unit] - value2[value1.Unit],
                Unit = value1.Unit
            };
        }

        public static double operator /(UnitValue<TUnit, TImplementation> value1, UnitValue<TUnit, TImplementation> value2)
        {
            // Use the same unit for value1 and value2. The result has the unit of value1.
            return value1[value1.Unit] / value2[value1.Unit];
        }

        public static TImplementation operator /(UnitValue<TUnit, TImplementation> value, double divisor)
        {
            return new TImplementation { [value.Unit] = value[value.Unit] / divisor, Unit = value.Unit };
        }

        public static bool operator ==(UnitValue<TUnit, TImplementation> value1, UnitValue<TUnit, TImplementation> value2)
        {
            if (ReferenceEquals(value1, value2))
                return true;

            if (ReferenceEquals(null, value1) || ReferenceEquals(null, value2))
                return false;

            return value1._value.IsEqual(value2._value);
        }

        public static bool operator !=(UnitValue<TUnit, TImplementation> value1, UnitValue<TUnit, TImplementation> value2)
        {
            return !(value1 == value2);
        }

        public static bool operator >(UnitValue<TUnit, TImplementation> value1, UnitValue<TUnit, TImplementation> value2)
        {
            if (ReferenceEquals(value1, value2))
                return false;

            if (ReferenceEquals(null, value2))
                return true;

            if (ReferenceEquals(null, value1))
                return false;

            return value1._value.IsGreater(value2._value);
        }

        public static bool operator <(UnitValue<TUnit, TImplementation> value1, UnitValue<TUnit, TImplementation> value2)
        {
            if (ReferenceEquals(value1, value2))
                return false;

            if (ReferenceEquals(null, value2))
                return false;

            if (ReferenceEquals(null, value1))
                return true;

            return value1._value.IsLower(value2._value);
        }

        public static bool operator >=(UnitValue<TUnit, TImplementation> value1, UnitValue<TUnit, TImplementation> value2)
        {
            return !(value1 < value2);
        }

        public static bool operator <=(UnitValue<TUnit, TImplementation> value1, UnitValue<TUnit, TImplementation> value2)
        {
            return !(value1 > value2);
        }

        //public static implicit operator double(UnitValue<TUnit, TImplementation> value)
        //{
        //    return value[value.Unit];
        //}

        public static TImplementation Min(TImplementation first, TImplementation second)
        {
            return first <= second ? first : second;
        }

        public static TImplementation Max(TImplementation first, TImplementation second)
        {
            return first >= second ? first : second;
        }

        public TImplementation Round(int precision)
        {
            return new TImplementation { [Unit] = System.Math.Round(this[Unit], precision), Unit = Unit };
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;

            return Equals((UnitValue<TUnit, TImplementation>)obj);
        }

        protected bool Equals(UnitValue<TUnit, TImplementation> other)
        {
            return other != null && this == other;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_conversionFromBaseUnit?.GetHashCode() ?? 0) * 397 ^ (_conversionToBaseUnit?.GetHashCode() ?? 0);
            }
        }

        protected abstract TUnit GetDefaultUnit();

        //TODO: Use resources
        //public string ToString(TUnit unit, CultureInfo culture = null)
        //{
        //    return ToString( unit, null, culture );
        //}

        //TODO: Use resources
        //public string ToString(TUnit unit, string formatKey, CultureInfo culture = null)
        //{
        //    return this[unit].ToString(EnumLocalizer?.GetString(unit as Enum, string.IsNullOrWhiteSpace(formatKey) ? "FormatReadMode" : $"{formatKey}.FormatReadMode") ?? "F6", culture ?? CultureInfo.CurrentCulture);
        //}

        //TODO: Use resources
        //public string ToString(TUnit unit, bool appendUnitString, CultureInfo culture = null) 
        //{
        //    return ToString(unit, null, appendUnitString, culture);
        //}

        //TODO: Use resources
        //public string ToString(TUnit unit, string formatKey, bool appendUnitString, CultureInfo culture = null)
        //{
        //    var valueString = ToString( unit, formatKey, culture );

        //    return appendUnitString ? $"{valueString} {EnumLocalizer?.GetString(unit as Enum) ?? ""}" : valueString;
        //}
    }
}