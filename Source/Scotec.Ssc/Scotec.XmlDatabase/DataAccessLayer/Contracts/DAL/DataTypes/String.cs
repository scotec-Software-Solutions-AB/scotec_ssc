namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public class String : DataType<String, System.String>
    {
        public static implicit operator String( System.String value )
        {
            if( value == null )
                return null;
            return ConvertToType( value );
        }


        public static implicit operator System.String( String value )
        {
            return ConvertToValue( value );
        }


        public override string ToString()
        {
            return Value;
        }
    }
}
