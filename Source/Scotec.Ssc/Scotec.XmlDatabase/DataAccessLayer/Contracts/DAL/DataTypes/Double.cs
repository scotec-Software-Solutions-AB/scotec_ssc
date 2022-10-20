#region

using System;
using System.Globalization;

#endregion


namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public class Double : DataType<Double, System.Double?>
    {
        public static implicit operator Double( string value )
        {
            if( value == null )
                return null;

            if( value.Length == 0 )
                return ConvertToType( default(System.Double) );

            return ConvertToType( System.Double.Parse( value, NumberFormatInfo.InvariantInfo ) );
        }


        public static implicit operator Double( System.Double value )
        {
            return ConvertToType( value );
        }


        public static implicit operator System.Double?( Double value )
        {
            return ConvertToValue( value );
        }


        public override string ToString()
        {
            if( Value != null )
                return Value.Value.ToString( NumberFormatInfo.InvariantInfo );

            throw new Exception( "Invalid value for type 'Double'." );
        }
    }
}
