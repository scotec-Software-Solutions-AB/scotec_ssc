#region

using System;
using System.Globalization;

#endregion


namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public class UnsignedLong : DataType<UnsignedLong, UInt64?>
    {
        public static implicit operator UnsignedLong( string value )
        {
            if( value == null )
                return null;

            if( value.Length == 0 )
                return ConvertToType( default(UInt64) );

            return ConvertToType( UInt64.Parse( value, NumberFormatInfo.InvariantInfo ) );
        }


        public static implicit operator UnsignedLong( UInt64 value )
        {
            return ConvertToType( value );
        }


        public static implicit operator UInt64?( UnsignedLong value )
        {
            return ConvertToValue( value );
        }


        public override string ToString()
        {
            if( Value != null )
                return Value.Value.ToString( NumberFormatInfo.InvariantInfo );

            throw new Exception( "Invalid value for type 'UnsignedLong'." );
        }
    }
}
