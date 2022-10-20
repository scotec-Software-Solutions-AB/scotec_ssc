#region

using System.Runtime.Serialization;

#endregion


namespace Scotec.Math.Units
{
    public interface IUnitValue : ISerializable
    {
    }

    public interface IUnitValue<TUnit> : IUnitValue
    {
        double this[TUnit unit] { get; set; }

        TUnit Unit { get; set; }
    }
}