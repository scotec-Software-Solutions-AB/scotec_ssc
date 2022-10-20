#region

using System;

#endregion


namespace Scotec.XMLDatabase.DAL
{
    public delegate void OnClosingDatabaseSessionEventHandler( IDatabaseSession session );

    [Flags]
    public enum EDatabaseWriteMode
    {
        Default = 0,
        ImmediateWrite = 1,
        UseLocks = 2,
    }

    public enum EDatabaseSessionMode
    {
        // Session is closed and cannot be used.
        Closed,

        // Session is in read only mode.
        Read,

        // Session is in exclusive read only mode.
        // Neither session can read or write data.
        ReadExclusive,

        // Session is in write mode. The behaviour can depend on the implementation.
        // The database manager may not allow multiple sessions in write mode.
        Write,

        // Session is in exclusive write mode.
        // Neither session can read or write data.
        WriteExcusive,
    }

    public interface IDatabaseSession
    {
        // Returns the session's id.
        Guid Id { get; }

        // Return the internal database representation of the session.
        object Tag { get; }

        // Returns the current session mode. See enaum EDatabaseSessionMode.
        EDatabaseSessionMode Mode { get; }

        EDatabaseWriteMode WriteMode { get; set; }

        // Saves all changes inside the session to the database.


        // Returns an instance of the database manager.
        IDatabaseManager DatabaseManager { get; }

        // Returns the root object
        IDatabaseObject Root { get; }
        event OnClosingDatabaseSessionEventHandler OnClose;


        void Save();


        // Closes the session without saving. Call Save() in order to apply the changes.
        void Close();
    }
}
