#region

using System;

#endregion


namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public class Id : DataType<Id, System.Guid?>
    {
        public static implicit operator Id( string value )
        {
            if( value == null )
                return null;

            if( value.Length == 0 )
                return ConvertToType( System.Guid.NewGuid() );

            return ConvertToType( new System.Guid( value ) );
        }


        public static implicit operator Id( System.Guid? value )
        {
            return ConvertToType( value );
        }


        public static implicit operator System.Guid?( Id value )
        {
            return ConvertToValue( value );
        }


        public static implicit operator Id( System.Guid value )
        {
            return ConvertToType( value );
        }


        public static implicit operator System.Guid( Id value )
        {
            return ConvertToValue( value ) ?? default(System.Guid);
        }


        public override string ToString()
        {
            if( Value != null )
                return Value.Value.ToString( "D" );

            throw new Exception( "Invalid value for type 'Id'." );
        }
    }
}
