namespace Scotec.XMLDatabase.DAL
{
    public interface IDatabaseObject
    {
        bool DataAvailable { get; }
        string Name { get; }
        IDatabaseObject Parent { get; }
        IDatabaseSession Session { get; }
        object RawObject { get; }


        void Persist();


        IDatabaseAttribute CreateAttribute( string name, object value );


        IDatabaseObject CreateDataObject( string name );


        IDatabaseObject CreateDataObject( string name, string type );


        void DeleteAttribute( string name );


        void DeleteDataObject( string name );


        IDatabaseAttribute GetAttribute( string name );


        IDatabaseObject GetDataObject( string name );


        IDatabaseObject GetReference( string name );


        bool HasAttribute( string name );


        bool HasDataObject( string name );


        bool IsAttribute( string name );


        bool IsDataObject( string name );


        bool IsSameAs( IDatabaseObject obj );


        void SetReference( string name, IDatabaseObject reference );
    }
}
