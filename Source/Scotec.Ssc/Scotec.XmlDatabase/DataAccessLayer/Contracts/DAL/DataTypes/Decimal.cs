#region

using System;
using System.Globalization;

#endregion


namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public class Decimal : DataType<Decimal, System.Decimal?>
    {
        public static implicit operator Decimal( string value )
        {
            if( value == null )
                return null;

            if( value.Length == 0 )
                return ConvertToType( default(System.Decimal) );

            return ConvertToType( System.Decimal.Parse( value, NumberFormatInfo.InvariantInfo ) );
        }


        public static implicit operator Decimal( System.Decimal value )
        {
            return ConvertToType( value );
        }


        public static implicit operator System.Decimal?( Decimal value )
        {
            return ConvertToValue( value );
        }


        public override string ToString()
        {
            if( Value != null )
                return Value.Value.ToString( NumberFormatInfo.InvariantInfo );

            throw new Exception( "Invalid value for type 'Decimal'." );
        }
    }
}
