#region

using System;

#endregion


namespace Scotec.XMLDatabase.DAL.DataTypes
{
    public class AnyUri : DataType<AnyUri, Uri>
    {
        public static implicit operator AnyUri( string value )
        {
            if( value == null )
                return null;

            Uri uri;
            Uri.TryCreate( value, UriKind.RelativeOrAbsolute, out uri );

            return ConvertToType( uri );
        }


        public static implicit operator AnyUri( Uri value )
        {
            return ConvertToType( value );
        }


        public static implicit operator Uri( AnyUri value )
        {
            return ConvertToValue( value );
        }


        public static implicit operator System.String( AnyUri value )
        {
            return value == null ? null : ConvertToType( value ).ToString();
        }


        public override string ToString()
        {
            return Value.OriginalString;
        }
    }
}
