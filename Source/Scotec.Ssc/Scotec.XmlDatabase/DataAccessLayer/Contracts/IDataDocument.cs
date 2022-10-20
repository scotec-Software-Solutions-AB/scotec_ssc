#region

using System;
using System.Collections.Generic;
using System.IO;
using Scotec.Transactions;

#endregion


namespace Scotec.XMLDatabase
{
    public interface IDataDocument
    {
        /// <summary>
        ///   Indicates whether the document has been changed.
        /// </summary>
        bool Dirty { get; }

        /// <summary>
        ///   The transaction handler used by the data document.
        /// </summary>
        bool UseDefaultTransactionHandler { get; set; }

        /// <summary>
        ///   Gets or sets the transaction manager.
        /// </summary>
        ITransactionManager TransactionManager { get; set; }

        /// <summary>
        ///   The transaction handler used by the data document.
        /// </summary>
        ITransactionHandler TransactionHandler { get; set; }

        /// <summary>
        ///   Returns the data schema.
        /// </summary>
        IDataSchema Schema { get; }

        /// <summary>
        ///   The OnDirty events will be fired if the document content has changed.
        /// </summary>
        event OnDirtyDataEventHandler OnDirty;

        /// <summary>
        ///   BevorOpdate events will be fired bevor the document will be updated.
        /// </summary>
        event OnBevorDataUpdateEventHandler BevorUpdate;

        /// <summary>
        ///   AfterUpdate events will be fired after the document has been updated.
        /// </summary>
        event OnAfterDataUpdateEventHandler AfterUpdate;


        /// <summary>
        ///   Opens a document at the specified location.
        /// </summary>
        /// <param name = "source">The file name of a xml database.</param>
        /// <param name = "schema"></param>
        void OpenDocument( string source, Uri schema );


        /// <summary>
        ///   Opens a document from a stream.
        /// </summary>
        /// <param name = "source">The file name of a xml database.</param>
        /// <param name="schema"></param>
        /// <remarks>SaveDocument does not work if the database has been opened from a stream. Use SavaDocument(Stream) instead.</remarks>
        void OpenDocument(Stream source, Uri schema);


        /// <summary>
        ///   Creates a document at the specified location.
        /// </summary>
        /// <param name = "source">The file name of a xml database.</param>
        /// <param name="root">The root element name.</param>
        /// <param name = "schema"></param>
        void CreateDocument( string source, string root, Uri schema );


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
        void SaveDocumentAs(string target);


        /// <summary>
        ///   Closes the document. Does not save the document.
        /// </summary>
        void Close();


        /// <summary>
        ///   Creates a new session.
        /// </summary>
        /// <param name = "mode"></param>
        /// <returns></returns>
        IDataSession CreateSession( EDataSessionMode mode );


        /// <summary>
        ///   Returns a session by its id.
        /// </summary>
        /// <param name = "id"></param>
        /// <returns></returns>
        IDataSession GetSession( Guid id );

        bool IsTriggerEnabled { get; set; }

        void RegisterUpdaters( IEnumerable<IDataUpdater> updaters );
    }
}
