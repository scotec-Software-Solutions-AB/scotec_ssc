#region

using System;
using System.Globalization;

#endregion


namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public class UnsignedByte : DataType<UnsignedByte, System.Byte?>
    {
        public static implicit operator UnsignedByte( string value )
        {
            if( value == null )
                return null;

            if( value.Length == 0 )
                return ConvertToType( default(System.Byte) );

            return ConvertToType( System.Byte.Parse( value, NumberFormatInfo.InvariantInfo ) );
        }


        public static implicit operator UnsignedByte( System.Byte value )
        {
            return ConvertToType( value );
        }


        public static implicit operator System.Byte?( UnsignedByte value )
        {
            return ConvertToValue( value );
        }


        public override string ToString()
        {
            if( Value != null )
                return Value.Value.ToString( NumberFormatInfo.InvariantInfo );

            throw new Exception( "Invalid value for type 'UnsignedByte'." );
        }
    }
}
