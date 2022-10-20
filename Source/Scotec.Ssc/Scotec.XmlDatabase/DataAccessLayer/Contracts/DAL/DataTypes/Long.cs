#region

using System;
using System.Globalization;

#endregion


namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public class Long : DataType<Long, Int64?>
    {
        public static implicit operator Long( string value )
        {
            if( value == null )
                return null;

            if( value.Length == 0 )
                return ConvertToType( default(Int64) );

            return ConvertToType( Int64.Parse( value, NumberFormatInfo.InvariantInfo ) );
        }


        public static implicit operator Long( Int64 value )
        {
            return ConvertToType( value );
        }


        public static implicit operator Int64?( Long value )
        {
            return ConvertToValue( value );
        }


        public override string ToString()
        {
            if( Value != null )
                return Value.Value.ToString( NumberFormatInfo.InvariantInfo );

            throw new Exception( "Invalid value for type 'Long'." );
        }
    }
}
