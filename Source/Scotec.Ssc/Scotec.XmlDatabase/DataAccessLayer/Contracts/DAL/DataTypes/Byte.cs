#region

using System;
using System.Globalization;

#endregion


namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public class Byte : DataType<Byte, SByte?>
    {
        public static implicit operator Byte( string value )
        {
            if( value == null )
                return null;

            if( value.Length == 0 )
                return ConvertToType( default(SByte) );

            return ConvertToType( SByte.Parse( value, NumberFormatInfo.InvariantInfo ) );
        }


        public static implicit operator Byte( SByte value )
        {
            return ConvertToType( value );
        }


        public static implicit operator SByte?( Byte value )
        {
            return ConvertToValue( value );
        }


        public override string ToString()
        {
            if( Value != null )
                return Value.Value.ToString( NumberFormatInfo.InvariantInfo );

            throw new Exception( "Invalid value for type 'Byte'." );
        }
    }
}
