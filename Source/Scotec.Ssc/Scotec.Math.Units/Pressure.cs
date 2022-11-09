#region

using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units;

[Serializable]
public sealed class Pressure : UnitValue<Pressure.Units, Pressure>
{
    public enum Units
    {
        Bar,
        Pascal,
        KiloPascal,
        MegaPascal,
        PoundPerSquareInch,
        PoundPerSquareFeet,
        InchesOfMercury,
        MillimeterOfMercury,
        InchesOfWater,
        FeetOfWater,
        MillimeterOfWater,
        Atmosphere,
        TechnicalAtmosphere,
        Torr,
        KilogramPerSquareCentimeter,
        KipsPerSquareInch
    }

    private const double FactorKiloPascal = 1.0e3;
    private const double FactorMegaPascal = 1.0e6;
    private const double FactorBar = 1.0e5;
    private const double FactorPoundPerSquareInch = 6894.757293168;
    private const double FactorPoundPerSquareFeet = 47.88026;
    private const double FactorInchesOfMercury = 3386.389;
    private const double FactorMillimeterOfMercury = 1 / 0.0075006168;
    private const double FactorTorr = 1 / 0.0075006168;
    private const double FactorInchesOfWater = 249.08891;
    private const double FactorFeetOfWater = 2989.067;
    private const double FactorAtmosphere = 101325;
    private const double FactorTechnicalAtmosphere = 98066.5;
    private const double FactorKilogramPerSquareCentimeter = 98066.5;
    private const double FactorKipsPerSquareInch = 6894757.293168;
    private const double FactorMillimeterOfWater = 9.80665;

    private static readonly Dictionary<Units, Func<double, double>> ConvertionToBaseUnit = new()
    {
        { Units.Pascal, v => v },
        { Units.KiloPascal, v => v * FactorKiloPascal },
        { Units.MegaPascal, v => v * FactorMegaPascal },
        { Units.Bar, v => v * FactorBar },
        { Units.PoundPerSquareInch, v => v * FactorPoundPerSquareInch },
        { Units.PoundPerSquareFeet, v => v * FactorPoundPerSquareFeet },
        { Units.InchesOfMercury, v => v * FactorInchesOfMercury },
        { Units.MillimeterOfMercury, v => v * FactorMillimeterOfMercury },
        { Units.Torr, v => v * FactorTorr },
        { Units.InchesOfWater, v => v * FactorInchesOfWater },
        { Units.FeetOfWater, v => v * FactorFeetOfWater },
        { Units.Atmosphere, v => v * FactorAtmosphere },
        { Units.TechnicalAtmosphere, v => v * FactorTechnicalAtmosphere },
        { Units.KilogramPerSquareCentimeter, v => v * FactorKilogramPerSquareCentimeter },
        { Units.KipsPerSquareInch, v => v * FactorKipsPerSquareInch },
        { Units.MillimeterOfWater, v => v * FactorMillimeterOfWater }
    };

    private static readonly Dictionary<Units, Func<double, double>> ConvertionFromBaseUnit = new()
    {
        { Units.Pascal, v => v },
        { Units.KiloPascal, v => v / FactorKiloPascal },
        { Units.MegaPascal, v => v / FactorMegaPascal },
        { Units.Bar, v => v / FactorBar },
        { Units.PoundPerSquareInch, v => v / FactorPoundPerSquareInch },
        { Units.PoundPerSquareFeet, v => v / FactorPoundPerSquareFeet },
        { Units.InchesOfMercury, v => v / FactorInchesOfMercury },
        { Units.MillimeterOfMercury, v => v / FactorMillimeterOfMercury },
        { Units.Torr, v => v / FactorTorr },
        { Units.InchesOfWater, v => v / FactorInchesOfWater },
        { Units.FeetOfWater, v => v / FactorFeetOfWater },
        { Units.Atmosphere, v => v / FactorAtmosphere },
        { Units.TechnicalAtmosphere, v => v / FactorTechnicalAtmosphere },
        { Units.KilogramPerSquareCentimeter, v => v / FactorKilogramPerSquareCentimeter },
        { Units.KipsPerSquareInch, v => v / FactorKipsPerSquareInch },
        { Units.MillimeterOfWater, v => v / FactorMillimeterOfWater }
    };


    public Pressure()
        : this(SIUnit, 0.0)
    {
    }

    public Pressure(double value)
        : this(SIUnit, value)
    {
    }

    public Pressure(Units unit, double value)
        : base(unit, value, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public Pressure(UnitValue<Units, Pressure> rhs)
        : base(rhs)
    {
    }

    private Pressure(SerializationInfo info, StreamingContext context)
        : base(info, context, ConvertionFromBaseUnit, ConvertionToBaseUnit)
    {
    }

    public static Units SIUnit => Units.Pascal;

    protected override Units GetDefaultUnit()
    {
        return SIUnit;
    }
}