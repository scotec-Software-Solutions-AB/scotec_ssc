#region

using System;

#endregion


namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public class Guid : DataType<Guid, System.Guid?>
    {
        public static implicit operator Guid( string value )
        {
            if( value == null )
                return null;

            if( value.Length == 0 )
                return ConvertToType( default(System.Guid) );

            return ConvertToType( new System.Guid( value ) );
        }


        public static implicit operator Guid( System.Guid value )
        {
            return ConvertToType( value );
        }


        public static implicit operator System.Guid?( Guid value )
        {
            return ConvertToValue( value );
        }


        public override string ToString()
        {
            if( Value != null )
                return Value.Value.ToString( "D" );

            throw new Exception( "Invalid value for type 'Guid'." );
        }
    }
}
