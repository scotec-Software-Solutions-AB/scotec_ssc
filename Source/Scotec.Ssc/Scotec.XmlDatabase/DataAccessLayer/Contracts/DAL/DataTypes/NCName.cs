namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public class NCName : DataType<NCName, System.String>
    {
        public static implicit operator NCName( System.String value )
        {
            if( value == null )
                return null;

            return ConvertToType( value );
        }


        public static implicit operator System.String( NCName value )
        {
            return ConvertToValue( value );
        }


        public override string ToString()
        {
            return Value;
        }
    }
}
