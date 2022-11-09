#region

using System.Globalization;

#endregion


namespace Scotec.Math;

internal sealed class Fraction
{
    private const double Epsilon = 0.00000001;

    private long _denominator;
    private long _integer;
    private long _numerator;

    public Fraction(double value)
    {
        ConvertToFraction(value);
    }

    private void ConvertToFraction(double value)
    {
        // For further information see: 
        // Algorithm To Convert A Decimal To A Fraction
        // by John Kennedy
        // Mathematics Department
        // Santa Monica College

        if (Equals(value, System.Math.Round(value)))
        {
            _integer = (long)value;
            return;
        }


        var valueLocal = value;

        var negative = valueLocal < 0.0;
        if (negative) valueLocal = System.Math.Abs(valueLocal);


        var z = new List<double> { double.NaN, valueLocal };
        var dominator = new List<int> { 0, 1 };
        var nominator = new List<int> { int.MinValue, (int)System.Math.Round(valueLocal) };

        var i = 1;
        do
        {
            if (z[i] - (int)z[i] < Epsilon)
                break;

            z.Add(1.0 / (z[i] - (int)z[i]));
            dominator.Add(dominator[i] * (int)z[i + 1] + dominator[i - 1]);
            nominator.Add((int)System.Math.Round(valueLocal * dominator[i + 1]));

            i++;
        } while (i < 20);

        _denominator = dominator.Last();
        _numerator = nominator.Last();
        _integer = 0;
        while (_numerator > _denominator) // numerator to integer
        {
            _integer++;
            _numerator = _numerator - _denominator;
        }

        if (negative)
            _integer = -1 * _integer;
    }

    public double ToDouble()
    {
        var doubleValue = _numerator / (double)_denominator;

        if (_integer < 0) return -1.0 * (System.Math.Abs(_integer) + doubleValue);

        return _integer + doubleValue;
    }

    public override string ToString()
    {
        return ToString(CultureInfo.InvariantCulture);
    }

    public string ToString(CultureInfo cultureInfo)
    {
        if (_numerator == 0) return _integer.ToString(cultureInfo);

        if (_integer == 0 && _numerator != 0) return $"{_numerator}/{_denominator}";

        return $"{_integer} {_numerator}/{_denominator}";
    }
}