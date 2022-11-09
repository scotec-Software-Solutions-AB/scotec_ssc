namespace Scotec.Math;

public class InterpolationLinear : InterpolationBase
{
    /// <summary>
    ///     Interpolates between the point (x1,y1) and (x2,y2) to the new (unknown) point (x,y). The x-value of this point must
    ///     be given.
    /// </summary>
    /// <param name="x">The x-value for the point, the y-value is searched for.</param>
    /// <param name="x1">The x-value for the (x1,y1) point </param>
    /// <param name="y1">The y-value for the (x1,y1) point</param>
    /// <param name="x2">The x-value for the (x2,y2) point</param>
    /// <param name="y2">The y-value for the (x2,y2) point</param>
    /// <returns>The interpolates y-value for the unknown point (x,y)</returns>
    public double Interpolate(double x, double x1, double y1, double x2, double y2)
    {
        return y2 + (y2 - y1) * (x - x1) / (x2 - x1);
    }
}