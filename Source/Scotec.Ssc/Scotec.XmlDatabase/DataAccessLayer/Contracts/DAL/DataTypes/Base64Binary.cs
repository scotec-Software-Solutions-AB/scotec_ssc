#region

using System.Text;

#endregion


namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public class Base64Binary : DataType<Base64Binary, System.Byte[]>
    {
        public static implicit operator Base64Binary( string value )
        {
            if( value == null )
                return null;

            var bytes = new System.Byte[value.Length];
            value.ToCharArray().CopyTo( bytes, 0 );

            return ConvertToType( bytes );
        }


        public static implicit operator Base64Binary( System.Byte[] value )
        {
            return ConvertToType( value );
        }


        public static implicit operator System.Byte[]( Base64Binary value )
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
