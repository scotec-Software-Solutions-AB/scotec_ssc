namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public class Token : DataType<Token, System.String>
    {
        public static implicit operator Token( System.String value )
        {
            if( value == null )
                return null;

            return ConvertToType( value );
        }


        public static implicit operator System.String( Token value )
        {
            return ConvertToValue( value );
        }


        public override string ToString()
        {
            return Value;
        }
    }
}
