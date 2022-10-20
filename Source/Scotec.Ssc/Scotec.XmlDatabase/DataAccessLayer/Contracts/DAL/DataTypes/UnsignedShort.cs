#region

using System;
using System.Globalization;

#endregion


namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public class UnsignedShort : DataType<UnsignedShort, UInt16?>
    {
        public static implicit operator UnsignedShort( string value )
        {
            if( value == null )
                return null;

            if( value.Length == 0 )
                return ConvertToType( default(UInt16) );

            return ConvertToType( UInt16.Parse( value, NumberFormatInfo.InvariantInfo ) );
        }


        public static implicit operator UnsignedShort( UInt16 value )
        {
            return ConvertToType( value );
        }


        public static implicit operator UInt16?( UnsignedShort value )
        {
            return ConvertToValue( value );
        }


        public override string ToString()
        {
            if( Value != null )
                return Value.Value.ToString( NumberFormatInfo.InvariantInfo );

            throw new Exception( "Invalid value for type 'UnsignedShort'." );
        }
    }
}
