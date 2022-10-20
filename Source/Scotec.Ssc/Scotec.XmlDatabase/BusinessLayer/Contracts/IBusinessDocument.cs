#region

using System;
using System.Collections.Generic;
using System.IO;
using Scotec.Transactions;

#endregion


namespace Scotec.XMLDatabase
{
    /// <summary>
    ///   A business document contains a collection of business objects. 
    ///   An implementation of the IBusinessDocument interfaces would forward most calls
    ///   to an underlying implementation of the IDataDocument interface.
    /// </summary>
    public interface IBusinessDocument
    {
        /// <summary>
        ///   Indicates whether the document has been changed.
        /// </summary>
        bool Dirty { get; }


        bool UseDefaultTransactionHandler { get; set; }

        /// <summary>
        ///   The transaction handler used by the data document.
        /// </summary>
        ITransactionHandler TransactionHandler { get; set; }

        /// <summary>
        ///   The OnDirty events will be fired if the document content has changed.
        /// </summary>
        event OnDirtyDocumentEventHandler OnDirty;

        /// <summary>
        ///   BevorOpdate events will be fired bevor the document will be updated.
        /// </summary>
        event OnBevorUpdateEventHandler BevorUpdate;

        /// <summary>
        ///   AfterUpdate events will be fired after the document has been updated.
        /// </summary>
        event OnAfterUpdateEventHandler AfterUpdate;


        /// <summary>
        ///   Opens a document at the specified location.
        /// </summary>
        /// <param name = "source">The file name of a xml database.</param>
        void OpenDocument( string source );


        /// <summary>
        ///   Opens a document from a stream.
        /// </summary>
        /// <param name = "source">The file name of a xml database.</param>
        /// <remarks>SaveDocument does not work if the database has been opened from a stream. Use SavaDocument(Stream) instead.</remarks>
        void OpenDocument( Stream source  );


        /// <summary>
        ///   Creates a document at the specified location.
        /// </summary>
        /// <param name = "source">The file name of a xml database.</param>
        void CreateDocument( string source );


        /// <summary>
        ///   Saves all changes.
        /// </summary>
        /// <remarks>SaveDocument does not work if the database has been opened from a stream. Use SavaDocument(Stream) instead.</remarks>
        void SaveDocument();


        /// <summary>
        ///   Saves all changes to a stream.
        /// </summary>
        void SaveDocument(Stream target);


        /// <summary>
        ///   Save the document to a new location.
        /// </summary>
        /// <param name = "target">The new location for the copy of the xml database.</param>
        void SaveDocumentAs( string target );


        /// <summary>
        ///   Closes the document. Does not save the document. The application logic is responsible to save
        ///   the document before closing.
        /// </summary>
        void Close();


        /// <summary>
        ///   Creates a new session for an open database.
        /// </summary>
        /// <param name = "mode"></param>
        /// <returns>The new sessions mode.</returns>
        IBusinessSession CreateSession( EBusinessSessionMode mode );


        /// <summary>
        ///   Returns a session by its session id.
        /// </summary>
        /// <param name = "id">The sessions id.</param>
        /// <returns></returns>
        IBusinessSession GetSession( Guid id );


        /// <summary>
        /// Gets or sets the uri to the schema file. 
        /// </summary>
        Uri Schema { get; set; }


        /// <summary>
        /// Gets or sets the name of the root node.
        /// </summary>
        string Root { get; set; }

        /// <summary>
        /// Enables/disables business rules. 
        /// </summary>
        bool IsTriggerEnabled { get; set; }

        void RegisterModificationHandler( IBusinessSessionModificationHandler handler );

        void RegisterModificationHandlers( IEnumerable<IBusinessSessionModificationHandler> handlers );
    }
}
