#region

using System;
using System.Globalization;

#endregion


namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public class Short : DataType<Short, Int16?>
    {
        public static implicit operator Short( string value )
        {
            if( value == null )
                return null;

            if( value.Length == 0 )
                return ConvertToType( default(Int16) );

            return ConvertToType( Int16.Parse( value, NumberFormatInfo.InvariantInfo ) );
        }


        public static implicit operator Short( Int16 value )
        {
            return ConvertToType( value );
        }


        public static implicit operator Int16?( Short value )
        {
            return ConvertToValue( value );
        }


        public override string ToString()
        {
            if( Value != null )
                return Value.Value.ToString( NumberFormatInfo.InvariantInfo );

            throw new Exception( "Invalid value for type 'Short'." );
        }
    }
}
