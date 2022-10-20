#region

using System;
using System.Collections.Generic;
using System.IO;
using Scotec.Transactions;

#endregion


namespace Scotec.XMLDatabase
{
    public enum EBusinessSessionMode
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
    public enum EBusinessSessionWriteMode
    {
        Default = 0,

        ImmediateWrite = 1,

        UseLocks = 2,
    }


    [Flags]
    public enum EChangeType
    {
        None = 0,

        Added = 1,

        Modified = 2,

        Deleted = 4
    }


    public interface IBusinessSession
    {
        /// <summary>
        ///   Returns the session's id.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        ///   Returns the current session mode. See enaum EDatabaseSessionMode.
        /// </summary>
        EBusinessSessionMode Mode { get; }

        EBusinessSessionWriteMode WriteMode { get; set; }


        /// <summary>
        ///   Returns the business document.
        /// </summary>
        IBusinessDocument Document { get; }


        /// <summary>
        ///   Returns true if the session's data has changed.
        /// </summary>
        bool Dirty { get; }

        /// <summary>
        ///   Gets The root object. There must be exactly 1 Root object in the document.
        ///   0 root objects are allowed for empty documents.
        /// </summary>
        TRoot GetRoot<TRoot>() where TRoot : IBusinessObject;

        /// <summary>
        ///   The OnClose events will be fired if the session has closed.
        /// </summary>
        event OnCloseSessionEventHandler OnClose;

        /// <summary>
        ///   The OnDirty events will be fired if the session content has changed.
        /// </summary>
        event OnDirtySessionEventHandler OnDirty;


        event EventHandler<DataChangedEventArgs> DataChanged;

        /// <summary>
        ///   Saves all changes inside the session to the document. Dependend on the underlying 
        ///   data access layer, the changes are merged into the document without saving physically
        ///   or they are written to the xml database immediately. For information how the data access 
        ///   layer behaves, see the appropriate documentation.
        /// </summary>
        void Save();


        /// <summary>
        ///   Saves all changes inside the session to the document stream.
        /// </summary>
        void Save( Stream target );


        /// <summary>
        ///   Closes the session without saving. Call Save() in order to apply the changes to the document.
        ///   This method does not close the document.
        /// </summary>
        void Close();


        /// <summary>
        ///   Deletes an object from database
        /// </summary>
        /// <param name="businessObject"> The object to delete. </param>
        void DeleteObject( IBusinessObject businessObject );


        /// <summary>
        ///   Creates a new transaction. If there is already a running transaction
        ///   belonging to the same thread, a nested transaction will becreated.
        ///   If the CreateTransaction() has been called from a different thread,
        ///   the thread will be blocked until the current running transaction has 
        ///   been committed, rolled back, or set into the pending state.
        /// </summary>
        /// <param name="mode"> The mode for the new trx. Default is ETransactionMode.Write. </param>
        /// <returns> Returns a new transaction created from the documents transaction handler. If no transaction handler has been assigned to the document, CreateTransaction() throws an exception of type BusinessExection </returns>
        IBusinessTransaction CreateTransaction( ETransactionMode mode = ETransactionMode.Write );


        /// <summary>
        ///   Creates a notification lock and prevents the session from sending change
        ///   notifications.
        /// </summary>
        /// <returns> Returns a notification lock. </returns>
        INotificationLock CreateNotificationLock();


        /// <summary>
        /// Copies a business object and appends it to a list.
        /// </summary>
        /// <param name="businessObject">The business object to copy.</param>
        /// <param name="businessObjectList">The target list.</param>
        /// <returns>The copied business object.</returns>
        TBusinessObject CopyTo<TBusinessObject, TBusinessObjectList>(TBusinessObject businessObject, TBusinessObjectList businessObjectList)
            where TBusinessObject : IBusinessObject 
            where TBusinessObjectList : IBusinessObjectList<TBusinessObject>;


        /// <summary>
        /// Copies a business object and appends it to a list.
        /// </summary>
        /// <param name="businessObject">The business object to copy.</param>
        /// <param name="businessObjectList">The target list.</param>
        /// <param name="index">The position of the new business object. If index is -1, the business object will be appendend at the end of the list</param>
        /// <returns>The copied business object.</returns>
        TBusinessObject CopyTo<TBusinessObject, TBusinessObjectList>(TBusinessObject businessObject, TBusinessObjectList businessObjectList, int index)
            where TBusinessObject : IBusinessObject
            where TBusinessObjectList : IBusinessObjectList<TBusinessObject>;

        void RegisterModificationHandler(IBusinessSessionModificationHandler handler);

        void RegisterModificationHandlers(IEnumerable<IBusinessSessionModificationHandler> handlers);
    }
}