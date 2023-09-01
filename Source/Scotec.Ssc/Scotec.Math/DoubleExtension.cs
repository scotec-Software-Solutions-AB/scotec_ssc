using System.Runtime.CompilerServices;

namespace Scotec.Math;

public static class DoubleExtension
{
    private const int DefaultPrecision = 8;

    public static bool IsEqual(this double leftValue, double rightValue, int precision = DefaultPrecision)
    {
        return System.Math.Abs(GetDifference(leftValue, rightValue)) < GetEpsilon(precision);
    }
    
    public static bool IsGreater(this double leftValue, double rightValue, int precision = DefaultPrecision)
    {
        return GetDifference(leftValue, rightValue) > GetEpsilon(precision);
    }

    public static bool IsLower(this double leftValue, double rightValue, int precision = DefaultPrecision)
    {
        return GetDifference(rightValue, leftValue) > GetEpsilon(precision);
    }

    public static bool IsGreaterOrEqual(this double leftValue, double rightValue, int precision = DefaultPrecision)
    {
        var epsilon = GetEpsilon(precision);
        var difference = GetDifference(leftValue, rightValue);

        return System.Math.Abs(difference) < epsilon || difference > epsilon;
    }

    public static bool IsLowerOrEqual(this double leftValue, double rightValue, int precision = DefaultPrecision)
    {
        var epsilon = System.Math.Pow(10, -precision);
        var difference = GetDifference(rightValue, leftValue);

        return System.Math.Abs(difference) < epsilon || difference > epsilon;
    }

    public static string ToFraction(this double value)
    {
        return new Fraction(value).ToString();
    }

    public static double Sqr(this double value)
    {
        return System.Math.Pow(value, 2);
    }

    public static int GetExponent(double value)
    {
        var exponent = System.Math.Ceiling(System.Math.Log10(System.Math.Abs(value))) - 1;

        return double.IsNaN(exponent) || double.IsInfinity(exponent) ? 0 : (int)exponent;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double GetDifference(double leftValue, double rightValue)
    {
        return leftValue - rightValue;
    }

    /// <summary>
    /// Returns the smallest multiple of significance that is greater than or equal to the specified value.
    /// </summary>
    public static double Ceiling(this double value, double significance, int precision = DefaultPrecision)
    {
        if (significance.IsLowerOrEqual(0.0))
        {
            throw new Exception("significance must be greater than 0");
        }

        var sign = value.IsLower(0.0, precision) ? -1.0 : 1.0;

        var x = System.Math.Round((int)(value / significance) * significance, precision);
        
        return System.Math.Round(x.Equals(value) ? x : x + significance * sign, precision);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double GetEpsilon(int precision)
    {
        return System.Math.Pow(10, -precision);
    }
}