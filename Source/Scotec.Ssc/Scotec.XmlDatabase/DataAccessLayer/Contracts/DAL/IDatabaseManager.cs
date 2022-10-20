#region

using System;

#endregion


namespace Scotec.XMLDatabase.DAL
{
    public interface IDatabaseManager
    {
        // Creates a new database
        ITypeMapper TypeMapper { get; }


        void CreateDatabase( string source, string database, string user, string password, IDataSchema schema );


        // Opens an existing database.
        void OpenDatabase( string source, string database, string user, string password, IDataSchema schema );


        // Creates a new database containing all content incl. changes.
        // Changes won't be applied to the original database.
        void SaveDatabaseAs( string target, string database, string user, string password );


        // Saves all sessions
        void Save();


        // Closes the database. No changes will be applied. Save() must be call before 
        // in order to save all changes.
        void CloseDatabase();


        // Creates a new session for an open database.
        IDatabaseSession CreateSession( EDatabaseSessionMode mode );


        // Returns a session by its session id.
        IDatabaseSession GetSession( Guid id );
    }
}
