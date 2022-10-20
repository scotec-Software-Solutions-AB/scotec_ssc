#region

using System;
using System.IO;
using Scotec.XMLDatabase.DAL;

#endregion


namespace Scotec.XMLDatabase
{
    public enum EDataSessionMode
    {
        /// <summary>
        ///   Session is closed and cannot be used.
        /// </summary>
        Closed,

        /// <summary>
        ///   Session is in read only mode.
        /// </summary>
        Read,

        /// <summary>
        ///   Session is in exclusive read only mode.
        ///   Neither session can read or write data.
        /// </summary>
        ReadExclusive,

        /// <summary>
        ///   Session is in write mode. The behaviour can depend on the implementation.
        ///   The database manager may not allow multiple sessions in write mode.
        /// </summary>
        Write,

        /// <summary>
        ///   Session is in exclusive write mode.
        ///   Neither session can read or write data.
        /// </summary>
        WriteExcusive,
    }

    [Flags]
    public enum EDataWriteMode
    {
        Default = 0,
        ImmediateWrite = 1,
        UseLocks = 2,
    }

    public interface IDataSession
    {
        /// <summary>
        ///   Returns the session's id.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        ///   Returns the current session mode. See enaum EDatabaseSessionMode.
        /// </summary>
        EDataSessionMode Mode { get; }

        EDataWriteMode WriteMode { get; set; }


        /// <summary>
        ///   Returns the business document.
        /// </summary>
        IDataDocument Document { get; }

        /// <summary>
        ///   Gets The root object. There must be exactly 1 Root object in the document.
        ///   0 root objects are allowed for empty documents.
        /// </summary>
        IDataObject Root { get; }

        /// <summary>
        ///   Indicates whether the session has been changed.
        /// </summary>
        bool Dirty { get; }

        IDatabaseObjectHandling ObjectHandling { get; }

        IDataFactory DataFactory { get; }

        /// <summary>
        ///   The OnDirty events will be fired if the document content has changed.
        /// </summary>
        event OnDirtyDataSessionEventHandler OnDirty;

        /// <summary>
        ///   The OnClose events will be fired if the session has closed.
        /// </summary>
        event OnCloseDataSessionEventHandler OnClose;


        /// <summary>
        ///   Saves all changes inside the session to the document. Dependend on the underlying 
        ///   data access layer, the changes are merged into the document without saving physically
        ///   or they are written to the database immediately. For information how the data access 
        ///   layer behaves, see the appropriate documentation.
        /// </summary>
        void Save();


        /// <summary>
        ///   Saves all changes inside the session to the document stream.
        /// </summary>
        void Save(Stream target);


        /// <summary>
        ///   Closes the session without saving. Call Save() in order to apply the changes to the document.
        ///   This method does not close the document.
        /// </summary>
        void Close();


        void DeleteObject( IDataObject dataObject );


        /// <summary>
        /// Copies a data object and adds it to the given list at the specified position.
        /// </summary>
        IDataObject CopyTo( IDataObject dataObject, IDataList targetDataList, int index );
    }
}
