namespace Scotec.Math;

public static class DoubleExtension
{
    private const int DefaultPrecision = 8;

    public static bool IsEqual(this double leftValue, double rightValue, int precision = DefaultPrecision)
    {
        var epsilon = System.Math.Pow(10, -precision);

        return System.Math.Abs(GetDifference(leftValue, rightValue)) < epsilon;
    }

    public static bool IsGreater(this double leftValue, double rightValue, int precision = DefaultPrecision)
    {
        var epsilon = System.Math.Pow(10, -precision);

        return GetDifference(leftValue, rightValue) > epsilon;
    }

    public static bool IsLower(this double leftValue, double rightValue, int precision = DefaultPrecision)
    {
        var epsilon = System.Math.Pow(10, -precision);

        return GetDifference(rightValue, leftValue) > epsilon;
    }

    public static bool IsGreaterOrEqual(this double leftValue, double rightValue, int precision = DefaultPrecision)
    {
        var epsilon = System.Math.Pow(10, -precision);

        return System.Math.Abs(GetDifference(leftValue, rightValue)) < epsilon || GetDifference(leftValue, rightValue) > epsilon;
    }

    public static bool IsLowerOrEqual(this double leftValue, double rightValue, int precision = DefaultPrecision)
    {
        var epsilon = System.Math.Pow(10, -precision);

        return System.Math.Abs(GetDifference(leftValue, rightValue)) < epsilon || GetDifference(rightValue, leftValue) > epsilon;
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

    /// <summary>
    ///     Devides both value by 10^exp and builds then the difference of the results.
    ///     This is needed if we want to compare values that are lower than epsilon.
    ///     WARNING: By using this method, very hight values are considered to be equal:
    ///     123456789123 will be equal to 123456789122
    ///     To treat the values as unequal, the precision must be set to 12 (default is 8).
    /// </summary>
    private static double GetDifference(double leftValue, double rightValue)
    {
        var exponent = GetExponent(leftValue);

        var devisor = System.Math.Pow(10, exponent);

        return leftValue / devisor - rightValue / devisor;
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
}