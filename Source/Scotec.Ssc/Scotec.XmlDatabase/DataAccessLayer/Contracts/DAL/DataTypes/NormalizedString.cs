namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public class NormalizedString : DataType<NormalizedString, System.String>
    {
        public static implicit operator NormalizedString( System.String value )
        {
            if( value == null )
                return null;

            return ConvertToType( value );
        }


        public static implicit operator System.String( NormalizedString value )
        {
            return ConvertToValue( value );
        }


        public override string ToString()
        {
            return Value;
        }
    }
}
