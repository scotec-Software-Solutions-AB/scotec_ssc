#region

using System;

#endregion


namespace Scotec.XMLDatabase
{
    internal class Utils
    {
        internal static Guid GuidFromId( string id )
        {
            return !string.IsNullOrEmpty( id ) ? new Guid( id.Substring( 2 ) ) : Guid.Empty;
        }


        internal static string IdFromGuid( Guid guid )
        {
            return "ID" + guid.ToString( "D" );
        }
    }
}
