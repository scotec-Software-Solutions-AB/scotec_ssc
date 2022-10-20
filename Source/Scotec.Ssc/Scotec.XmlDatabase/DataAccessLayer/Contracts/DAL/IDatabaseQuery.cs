#region

using System.Collections.Generic;

#endregion


namespace Scotec.XMLDatabase.DAL
{
    public interface IDatabaseQuery
    {
        IList<IDatabaseObject> Execute( string query );


        IList<IDatabaseObject> GetDirectReverseLinks( IList<string> dataTypes );
    }
}
