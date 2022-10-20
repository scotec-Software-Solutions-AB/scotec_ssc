#region

using System;
using System.Globalization;

#endregion


namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public class Date : DataType<Date, System.DateTime?>
    {
        public static implicit operator Date( string value )
        {
            if( value == null )
                return null;

            if( value.Length == 0 )
                return ConvertToType( System.DateTime.Today );

            return ConvertToType( System.DateTime.Parse( value, DateTimeFormatInfo.InvariantInfo ).ToLocalTime() );
        }


        public static implicit operator Date( System.DateTime value )
        {
            return ConvertToType( value );
        }


        public static implicit operator System.DateTime?( Date value )
        {
            return ConvertToValue( value );
        }


        public override string ToString()
        {
            if( Value != null )
                return Value.Value.ToUniversalTime().ToString( "yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo );

            throw new Exception( "Invalid value for type 'Date'." );
        }
    }
}
