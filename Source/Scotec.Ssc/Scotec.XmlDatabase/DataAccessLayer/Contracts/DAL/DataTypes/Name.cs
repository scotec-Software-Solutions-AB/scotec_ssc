namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public class Name : DataType<Name, System.String>
    {
        public static implicit operator Name( System.String value )
        {
            if( value == null )
                return null;

            return ConvertToType( value );
        }


        public static implicit operator System.String( Name value )
        {
            return ConvertToValue( value );
        }


        public override string ToString()
        {
            return Value;
        }
    }
}
