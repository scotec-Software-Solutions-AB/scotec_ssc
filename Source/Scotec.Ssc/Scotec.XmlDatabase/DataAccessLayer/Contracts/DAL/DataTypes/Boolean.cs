#region

using System;

#endregion


namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public class Boolean : DataType<Boolean, System.Boolean?>
    {
        public static implicit operator Boolean( string value )
        {
            if( value == null )
                return null;

            if( value.Length == 0 )
                return ConvertToType( default(System.Boolean) );

            return ConvertToType( System.Boolean.Parse( value ) );
        }


        public static implicit operator Boolean( System.Boolean value )
        {
            return ConvertToType( value );
        }


        public static implicit operator System.Boolean?( Boolean value )
        {
            return ConvertToValue( value );
        }


        public override string ToString()
        {
            if( Value != null )
                return (Value.Value) ? "true" : "false";

            throw new Exception( "Invalid value for type 'Boolean'." );
        }
    }
}
