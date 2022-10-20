#region

using System;
using System.Globalization;

#endregion


namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public class Int : DataType<Int, Int32?>
    {
        public static implicit operator Int( string value )
        {
            if( value == null )
                return null;

            if( value.Length == 0 )
                return ConvertToType( default(Int32) );

            return ConvertToType( Int32.Parse( value, NumberFormatInfo.InvariantInfo ) );
        }


        public static implicit operator Int( Int32 value )
        {
            return ConvertToType( value );
        }


        public static implicit operator Int32?( Int value )
        {
            return ConvertToValue( value );
        }


        public override string ToString()
        {
            if( Value != null )
                return Value.Value.ToString( NumberFormatInfo.InvariantInfo );

            throw new Exception( "Invalid value for type 'Int'." );
        }
    }
}
