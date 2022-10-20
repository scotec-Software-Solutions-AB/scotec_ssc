#region

using Scotec.XMLDatabase.DAL;

#endregion


namespace Scotec.XMLDatabase
{
    public interface IDataFactory
    {
        IDataObject CreateObject( IDatabaseType databaseType, string type, string name );


        IDataFactoryInfo GetFactoryInfo( string type );


        bool TryGetObject( IDatabaseType key, out IDataObject dataObject );
    }
}
