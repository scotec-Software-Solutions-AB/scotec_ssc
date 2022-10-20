#region

using System;
using System.Globalization;

#endregion


namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public class Float : DataType<Float, Single?>
    {
        public static implicit operator Float( string value )
        {
            if( value == null )
                return null;

            if( value.Length == 0 )
                return ConvertToType( default(Single) );

            return ConvertToType( Single.Parse( value, NumberFormatInfo.InvariantInfo ) );
        }


        public static implicit operator Float( Single value )
        {
            return ConvertToType( value );
        }


        public static implicit operator Single?( Float value )
        {
            return ConvertToValue( value );
        }


        public override string ToString()
        {
            if( Value != null )
                return Value.Value.ToString( NumberFormatInfo.InvariantInfo );

            throw new Exception( "Invalid value for type 'Float'." );
        }
    }
}
