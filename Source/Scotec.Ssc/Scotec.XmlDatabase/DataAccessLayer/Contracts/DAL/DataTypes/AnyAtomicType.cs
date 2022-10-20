namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public class AnyAtomicType : DataType<AnyAtomicType, System.String>
    {
        public static implicit operator AnyAtomicType( string value )
        {
            return value == null ? null : ConvertToType( value );
        }


        public static implicit operator string( AnyAtomicType value )
        {
            return ConvertToValue( value );
        }


        public override string ToString()
        {
            return Value;
        }
    }
}
