#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using Scotec.Transactions;
using Scotec.XMLDatabase.Rules;

#endregion


namespace Scotec.XMLDatabase
{
    public class BusinessDocument : IBusinessDocument, IBusinessRuleTrigger
    {
        #region Private Attributes

        /// <summary>
        ///   The underlying data document
        /// </summary>
        private readonly IDataDocument _dataDocument;

        /// <summary>
        ///   The business rule trigger.
        /// </summary>
        private readonly IBusinessRuleTrigger _ruleTrigger;


        private readonly IDictionary<Guid, IBusinessSession> _sessions;

        #endregion Private Attributes


        #region Constructors

        public BusinessDocument(IDataDocument dataDocument)
        {
            _sessions = new Dictionary<Guid, IBusinessSession>();

            _dataDocument = dataDocument;
            _ruleTrigger = new BusinessRuleTrigger( this, DataDocument );
        }

        #endregion Constructors


        #region IBusinessDocument Members

        private Uri _schema;
        public Uri Schema
        {
            get { return _schema; }
            set
            {
                if(_schema == null)
                    _schema = value;
                else
                    throw new BusinessException( EBusinessError.Document, "Schema can only be set once." );
            }
        }


        private string _root;
        public string Root
        {
            get { return _root; }
            set
            {
                if (_root == null)
                    _root = value;
                else
                    throw new BusinessException(EBusinessError.Document, "Root can only be set once.");
            }
        }

        public bool IsTriggerEnabled
        {
            get { return _dataDocument.IsTriggerEnabled; }
            set { _dataDocument.IsTriggerEnabled = value; }
        }


        public void Close()
        {
            try
            {
                var sessions = Sessions.Select( kvp => kvp.Value ).ToList();
                foreach(var session in sessions)
                    session.Close();

                DataDocument.OnDirty -= OnDataDocumentDirty;

                UnloadRules();
                DataDocument.Close();
            }
            catch( DataException e )
            {
                throw new BusinessException( (EBusinessError)e.DataError, e.Message, e );
            }
            catch( Exception e )
            {
                throw new BusinessException( EBusinessError.Document, "Caught unhandled exception.", e );
            }
        }


        public void CreateDocument( string source )
        {
            if (string.IsNullOrEmpty(Root))
                throw new BusinessException( EBusinessError.Document, "Root must be set before creating a new document." );

            if(Schema == null)
                throw new BusinessException(EBusinessError.Document, "Path to schema must be set before creating a new document.");

            try
            {
                // Create a local session. 
                var session = CreateSession( EBusinessSessionMode.ReadExclusive );

                using( session.CreateNotificationLock() )
                {
                    DataDocument.CreateDocument( source, Root, Schema );
                    OpenInternal();
                }

                session.Close();
            }
            catch( DataException e )
            {
                throw new BusinessException( (EBusinessError)e.DataError, e.Message, e );
            }
            catch( Exception e )
            {
                throw new BusinessException( EBusinessError.Document, "Caught unhandled exception.", e );
            }
        }


        public bool Dirty
        {
            get
            {
                try
                {
                    return DataDocument.Dirty;
                }
                catch( DataException e )
                {
                    throw new BusinessException( (EBusinessError)e.DataError, e.Message, e );
                }
                catch( Exception e )
                {
                    throw new BusinessException( EBusinessError.Document, "Caught unhandled exception.", e );
                }
            }
        }


        public void OpenDocument( string source)
        {
            if (string.IsNullOrEmpty(Root))
                throw new BusinessException(EBusinessError.Document, "Root must be set before opening a document.");

            if (Schema == null)
                throw new BusinessException(EBusinessError.Document, "Path to schema must be set before opening a document.");

            try
            {
                DataDocument.BevorUpdate += BevorUpdateDoc;
                DataDocument.AfterUpdate += AfterUpdateDoc;
                DataDocument.OpenDocument( source, Schema );

                OpenInternal();
            }
            catch( DataException e )
            {
                throw new BusinessException( (EBusinessError)e.DataError, e.Message, e );
            }
            catch( Exception e )
            {
                throw new BusinessException( EBusinessError.Document, "Caught unhandled exception.", e );
            }
            finally
            {
                DataDocument.BevorUpdate -= BevorUpdateDoc;
                DataDocument.AfterUpdate -= AfterUpdateDoc;
            }
        }


        public void OpenDocument(Stream source)
        {
            if (string.IsNullOrEmpty(Root))
                throw new BusinessException(EBusinessError.Document, "Root must be set before opening a document.");

            if (Schema == null)
                throw new BusinessException(EBusinessError.Document, "Path to schema must be set before opening a document.");

            try
            {
                DataDocument.BevorUpdate += BevorUpdateDoc;
                DataDocument.AfterUpdate += AfterUpdateDoc;
                DataDocument.OpenDocument(source, Schema);

                OpenInternal();
            }
            catch (DataException e)
            {
                throw new BusinessException((EBusinessError)e.DataError, e.Message, e);
            }
            catch (Exception e)
            {
                throw new BusinessException(EBusinessError.Document, "Caught unhandled exception.", e);
            }
            finally
            {
                DataDocument.BevorUpdate -= BevorUpdateDoc;
                DataDocument.AfterUpdate -= AfterUpdateDoc;
            }
        }


        public void SaveDocument()
        {
            try
            {
                DataDocument.SaveDocument();
            }
            catch( DataException e )
            {
                throw new BusinessException( (EBusinessError)e.DataError, e.Message, e );
            }
            catch( Exception e )
            {
                throw new BusinessException( EBusinessError.Document, "Caught unhandled exception.", e );
            }
        }


        public void SaveDocument( Stream target )
        {
            try
            {
                DataDocument.SaveDocument(target);
            }
            catch (DataException e)
            {
                throw new BusinessException((EBusinessError)e.DataError, e.Message, e);
            }
            catch (Exception e)
            {
                throw new BusinessException(EBusinessError.Document, "Caught unhandled exception.", e);
            }
        }


        public void SaveDocumentAs( string target )
        {
            if (string.IsNullOrEmpty(Root))
                throw new BusinessException(EBusinessError.Document, "Root must be set before saving a document.");

            if (Schema == null)
                throw new BusinessException(EBusinessError.Document, "Path to schema must be set before saving a document.");

            try
            {
                DataDocument.SaveDocumentAs( target );
            }
            catch( DataException e )
            {
                throw new BusinessException( (EBusinessError)e.DataError, e.Message, e );
            }
            catch( Exception e )
            {
                throw new BusinessException( EBusinessError.Document, "Caught unhandled exception.", e );
            }
        }


        public bool UseDefaultTransactionHandler
        {
            get { return DataDocument.UseDefaultTransactionHandler; }
            set { DataDocument.UseDefaultTransactionHandler = value; }
        }

        public ITransactionHandler TransactionHandler
        {
            get
            {
                try
                {
                    return DataDocument.TransactionHandler ;
                }
                catch( DataException e )
                {
                    throw new BusinessException( (EBusinessError)e.DataError, e.Message, e );
                }
                catch( Exception e )
                {
                    throw new BusinessException( EBusinessError.Document, "Caught unhandled exception.", e );
                }
            }
            set
            {
                try
                {
                    DataDocument.TransactionHandler = value;
                }
                catch( DataException e )
                {
                    throw new BusinessException( (EBusinessError)e.DataError, e.Message, e );
                }
                catch( Exception e )
                {
                    throw new BusinessException( EBusinessError.Document, "Caught unhandled exception.", e );
                }
            }
        }


        public IBusinessSession CreateSession( EBusinessSessionMode mode )
        {
            try
            {
                var dataSession = DataDocument.CreateSession((EDataSessionMode)mode);
                IBusinessSession businessSession = new BusinessSession(this, dataSession);

                businessSession.OnDirty += OnSessionDirty;
                businessSession.OnClose += OnCloseSession;
                businessSession.RegisterModificationHandlers( _modificationHandlers.ToList() );
                
                Sessions.Add( businessSession.Id, businessSession );

                return businessSession;
            }
            catch( DataException e )
            {
                throw new BusinessException( (EBusinessError)e.DataError, e.Message, e );
            }
            catch( Exception e )
            {
                throw new BusinessException( EBusinessError.Document, "Caught unhandled exception.", e );
            }
        }

        public IBusinessSession GetSession( Guid id )
        {
            try
            {
                IBusinessSession session;

                return Sessions.TryGetValue( id, out session ) ? session : null;
            }
            catch( DataException e )
            {
                throw new BusinessException( (EBusinessError)e.DataError, e.Message, e );
            }
            catch( Exception e )
            {
                throw new BusinessException( EBusinessError.Document, "Caught unhandled exception.", e );
            }
        }


        public event OnDirtyDocumentEventHandler OnDirty;
        public event OnBevorUpdateEventHandler BevorUpdate;
        public event OnAfterUpdateEventHandler AfterUpdate;

        #endregion


        #region Private Properties

        private IDataDocument DataDocument
        {
            get { return _dataDocument; }
        }

        private IDictionary<Guid, IBusinessSession> Sessions
        {
            get { return _sessions; }
        }

        #endregion Protected Properties


        #region Private Methods

        private void OpenInternal()
        {
            //LoadRules();
            DataDocument.OnDirty += OnDataDocumentDirty;
        }


        private void OnDataDocumentDirty( IDataDocument document )
        {
            if( OnDirty != null )
                OnDirty( this );
        }


        private void OnSessionDirty( IBusinessSession session )
        {
            if( OnDirty != null )
                OnDirty( this );
        }


        private void OnCloseSession( IBusinessSession session )
        {
            session.OnDirty -= OnSessionDirty;
            session.OnClose -= OnCloseSession;
            Sessions.Remove( session.Id );
        }


        //private void LoadRules()
        //{
        //    (this as IBusinessRuleTrigger).RegisterRules( m_rulesExtensionPath );
        //}


        private void UnloadRules()
        {
            (this as IBusinessRuleTrigger).UnregisterAllRules();
        }


        private void BevorUpdateDoc( XmlDocument xmlDoc, CancelEventArgs cancelArg )
        {
            if( BevorUpdate != null )
                BevorUpdate( xmlDoc, cancelArg );
        }


        private void AfterUpdateDoc( IDataDocument xmlDoc )
        {
            if( AfterUpdate != null )
                AfterUpdate( this );
        }

        #endregion Private Methods


        #region IBusinessRuleTrigger Members

        void IBusinessRuleTrigger.RegisterRule<T>( IBusinessRule<T> rule )
        {
            try
            {
                _ruleTrigger.RegisterRule( rule );
            }
            catch( DataException e )
            {
                throw new BusinessException( (EBusinessError)e.DataError, e.Message, e );
            }
            catch( Exception e )
            {
                throw new BusinessException( EBusinessError.Document, "Caught unhandled exception.", e );
            }
        }


        void IBusinessRuleTrigger.UnregisterRule<T>( IBusinessRule<T> rule )
        {
            try
            {
                _ruleTrigger.UnregisterRule( rule );
            }
            catch( DataException e )
            {
                throw new BusinessException( (EBusinessError)e.DataError, e.Message, e );
            }
            catch( Exception e )
            {
                throw new BusinessException( EBusinessError.Document, "Caught unhandled exception.", e );
            }
        }


        void IBusinessRuleTrigger.RegisterRules( IEnumerable<IBusinessRule> businessRules )
        {
            try
            {
                _ruleTrigger.RegisterRules( businessRules );
            }
            catch( DataException e )
            {
                throw new BusinessException( (EBusinessError)e.DataError, e.Message, e );
            }
            catch( Exception e )
            {
                throw new BusinessException( EBusinessError.Document, "Caught unhandled exception.", e );
            }
        }


        void IBusinessRuleTrigger.UnregisterAllRules()
        {
            try
            {
                _ruleTrigger.UnregisterAllRules();
            }
            catch( DataException e )
            {
                throw new BusinessException( (EBusinessError)e.DataError, e.Message, e );
            }
            catch( Exception e )
            {
                throw new BusinessException( EBusinessError.Document, "Caught unhandled exception.", e );
            }
        }

        private IList<IBusinessSessionModificationHandler> _modificationHandlers = new List<IBusinessSessionModificationHandler>();
        public void RegisterModificationHandler(IBusinessSessionModificationHandler handler)
        {
            _modificationHandlers.Add(handler);
        }

        public void RegisterModificationHandlers(IEnumerable<IBusinessSessionModificationHandler> handlers)
        {
            foreach (var handler in handlers)
                RegisterModificationHandler(handler);
        }

        #endregion
    }
}
