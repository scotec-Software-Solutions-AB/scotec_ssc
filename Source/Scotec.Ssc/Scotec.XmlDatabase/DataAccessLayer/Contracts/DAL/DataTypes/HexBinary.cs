#region

using System;
using System.Text;

#endregion


namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public class HexBinary : DataType<HexBinary, System.Byte[]>
    {
        public static implicit operator HexBinary( string value )
        {
            if( value == null )
                return null;

            if( value.Length == 0 )
                return ConvertToType( default(System.Byte[]) );

            var chars = new Char[] { };
            var bytes = new System.Byte[] { };
            value.CopyTo( 0, chars, 0, value.Length );
            chars.CopyTo( bytes, 0 );
            return ConvertToType( bytes );
        }


        public static implicit operator HexBinary( System.Byte[] value )
        {
            return ConvertToType( value );
        }


        public static implicit operator System.Byte[]( HexBinary value )
        {
            return ConvertToValue( value );
        }


        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append( Value );

            return result.ToString();
        }
    }
}
