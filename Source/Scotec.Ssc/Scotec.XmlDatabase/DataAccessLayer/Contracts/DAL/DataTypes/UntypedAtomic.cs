namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public class UntypedAtomic : DataType<UntypedAtomic, System.String>
    {
        public static implicit operator UntypedAtomic( System.String value )
        {
            if( value == null )
                return null;

            return ConvertToType( value );
        }


        public static implicit operator System.String( UntypedAtomic value )
        {
            return ConvertToValue( value );
        }


        public override string ToString()
        {
            return Value;
        }
    }
}
