#region

using System;
using System.Globalization;

#endregion


namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public class DateTime : DataType<DateTime, System.DateTime?>
    {
        public static implicit operator DateTime( string value )
        {
            if( value == null )
                return null;

            if( value.Length == 0 )
                return ConvertToType( System.DateTime.Now );

            return ConvertToType( System.DateTime.Parse( value, DateTimeFormatInfo.InvariantInfo ).ToLocalTime() );
        }


        public static implicit operator DateTime( System.DateTime value )
        {
            return ConvertToType( value );
        }


        public static implicit operator System.DateTime?( DateTime value )
        {
            return ConvertToValue( value );
        }


        public override string ToString()
        {
            if( Value != null )
                return Value.Value.ToUniversalTime().ToString( "yyyy-MM-ddTHH:mm:ss.fffZ",
                                                               DateTimeFormatInfo.InvariantInfo );

            throw new Exception( "Invalid value for type 'DateTime'." );
        }
    }
}
