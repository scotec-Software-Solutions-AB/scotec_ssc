#region

using System;

#endregion


namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public class Idref : DataType<Idref, System.Guid?>
    {
        public static implicit operator Idref( string value )
        {
            if( value == null )
                return null;

            if( value.Length == 0 )
                return ConvertToType( default(System.Guid) );

            return ConvertToType( new System.Guid( value ) );
        }


        public static implicit operator Idref( System.Guid? value )
        {
            return ConvertToType( value );
        }


        public static implicit operator System.Guid?( Idref value )
        {
            return ConvertToValue( value );
        }


        public static implicit operator Idref( System.Guid value )
        {
            return ConvertToType( value );
        }


        public static implicit operator System.Guid( Idref value )
        {
            return ConvertToValue( value ) ?? default(System.Guid);
        }


        public override string ToString()
        {
            if( Value != null )
                return Value.Value.ToString( "D" );

            throw new Exception( "Invalid value for type 'Idref'." );
        }
    }
}
