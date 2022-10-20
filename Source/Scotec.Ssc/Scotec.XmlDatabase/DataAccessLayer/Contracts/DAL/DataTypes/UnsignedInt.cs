#region

using System;
using System.Globalization;

#endregion


namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public class UnsignedInt : DataType<UnsignedInt, UInt32?>
    {
        public static implicit operator UnsignedInt( string value )
        {
            if( value == null )
                return null;

            if( value.Length == 0 )
                return ConvertToType( default(UInt32) );

            return ConvertToType( UInt32.Parse( value, NumberFormatInfo.InvariantInfo ) );
        }


        public static implicit operator UnsignedInt( UInt32 value )
        {
            return ConvertToType( value );
        }


        public static implicit operator UInt32?( UnsignedInt value )
        {
            return ConvertToValue( value );
        }


        public override string ToString()
        {
            if( Value != null )
                return Value.Value.ToString( NumberFormatInfo.InvariantInfo );

            throw new Exception( "Invalid value for type 'UnsignedInt'." );
        }
    }
}
