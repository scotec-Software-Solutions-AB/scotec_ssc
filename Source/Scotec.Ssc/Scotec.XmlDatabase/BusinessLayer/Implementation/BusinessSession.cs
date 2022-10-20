#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Scotec.Transactions;
using Scotec.XMLDatabase.ChangeNotification;
using Scotec.Events.WeakEvents;

#endregion


namespace Scotec.XMLDatabase
{
    public class BusinessSession : IBusinessSession, IChangeNotifier
    {
        #region private fields

        private readonly IDataSession _dataSession;
        private readonly IBusinessDocument _document;
        private BusinessObjectFactory _businessObjectFactory;

        #endregion private fields


        #region constructor

        internal BusinessSession( IBusinessDocument document, IDataSession dataSession )
        {
            _document = document;
            _dataSession = dataSession;
            _businessObjectFactory = new BusinessObjectFactory( this );

            _dataSession.OnClose += OnDataSessionClose;
            _dataSession.OnDirty += OnDataSessionDirty;
            ((IDataChangeNotifier)DataSession).OnDataChange += OnDataSessionChange;
        }

        #endregion constructor


        #region IBusinessSession Members

        public void Close()
        {
            try
            {
                if( _businessObjectFactory != null )
                    (_businessObjectFactory as IDisposable).Dispose();
                _businessObjectFactory = null;

                DataSession.Close();
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


        public IBusinessDocument Document
        {
            get
            {
                try
                {
                    return _document;
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

        public Guid Id
        {
            get
            {
                try
                {
                    return DataSession.Id;
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

        public EBusinessSessionMode Mode
        {
            get
            {
                try
                {
                    return (EBusinessSessionMode)DataSession.Mode;
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

        public EBusinessSessionWriteMode WriteMode
        {
            get
            {
                try
                {
                    return (EBusinessSessionWriteMode)DataSession.WriteMode;
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
                    DataSession.WriteMode = (EDataWriteMode)value;
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

        public TRoot GetRoot<TRoot>() where TRoot : IBusinessObject
        {
            try
            {
                return (TRoot)Factory.GetBusinessObject( DataSession.Root );
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

        event EventHandler<DataChangedEventArgs> IBusinessSession.DataChanged
        {
            add { DataChanged += value.MakeWeak(eh => DataChanged -= eh); }
            remove { throw new NotSupportedException(); }
        }

        private event EventHandler<DataChangedEventArgs> DataChanged;

        public void Save()
        {
            try
            {
                DataSession.Save();
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


        public void Save( Stream target )
        {
            try
            {
                DataSession.Save( target );
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

        bool IBusinessSession.Dirty
        {
            get { return DataSession.Dirty; }
        }


        void IBusinessSession.DeleteObject( IBusinessObject businessObject )
        {
            try
            {
                DataSession.DeleteObject( ((BusinessObject)businessObject).DataObject );
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


        IBusinessTransaction IBusinessSession.CreateTransaction( ETransactionMode mode )
        {
            try
            {
                var transactionHandler = Document.TransactionHandler;

                if( transactionHandler == null )
                    throw new BusinessException( EBusinessError.Schema, "Could not create transaction: Missing transaction handler." );

                return new BusinessTransaction( this, transactionHandler.CreateTransaction( this, mode, "" ) );
            }
            catch( DataException e )
            {
                throw new BusinessException( (EBusinessError)e.DataError, e.Message, e );
            }
            catch( Exception e )
            {
                throw new BusinessException( EBusinessError.Session, "Creating transaction failed.", e );
            }
        }

        public INotificationLock CreateNotificationLock()
        {
            return new NotificationLock( this );
        }

        public TBusinessObject CopyTo<TBusinessObject, TBusinessObjectList>( TBusinessObject businessObject, TBusinessObjectList businessObjectList ) 
            where TBusinessObject : IBusinessObject
            where TBusinessObjectList : IBusinessObjectList<TBusinessObject>
        {
            return CopyTo( businessObject, businessObjectList, -1 );
        }

        public TBusinessObject CopyTo<TBusinessObject, TBusinessObjectList>(TBusinessObject businessObject, TBusinessObjectList businessObjectList, int index)
            where TBusinessObject : IBusinessObject
            where TBusinessObjectList : IBusinessObjectList<TBusinessObject>
        {
            var dataObject = ((BusinessObject)(IBusinessObject)businessObject).DataObject;
            var dataList = ((BusinessObjectList<TBusinessObject>)(IBusinessObjectList<TBusinessObject>)businessObjectList).DataList;

            var copy = DataSession.CopyTo(dataObject, dataList, index);

            return (TBusinessObject)Factory.GetBusinessObject(copy);
        }

        private IList<IBusinessSessionModificationHandler> _modificationHandlers = new List<IBusinessSessionModificationHandler>();
        public void RegisterModificationHandler( IBusinessSessionModificationHandler handler )
        {
            _modificationHandlers.Add( handler );
        }

        public void RegisterModificationHandlers(IEnumerable<IBusinessSessionModificationHandler> handlers)
        {
            foreach(var handler in handlers)
                RegisterModificationHandler( handler );
        }

        private void NotifyModificationHandlers( DataChangedEventArgs dataChangedEventArgs )
        {
            using( CreateNotificationLock() )
            {
                using(var transaction = This.CreateTransaction( ETransactionMode.Write ))
                {
                    foreach( var handler in _modificationHandlers )
                    handler.Run( this, dataChangedEventArgs );

                    transaction.Commit();
                }
            }
        }


        public event OnCloseSessionEventHandler OnClose;
        public event OnDirtySessionEventHandler OnDirty;

        #endregion


        #region IChangeNotifier Members

        public void AddChangedObject( IBusinessObject obj, EChangeNotificationType type )
        {
            try
            {
                var dataType = ChangeTypeConverter.Convert( type );
                ((IDataChangeNotifier)DataSession).AddChangedObject( ((BusinessObject)obj).DataObject, dataType );
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


        public void Lock()
        {
            try
            {
                ((IDataChangeNotifier)DataSession).Lock();
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


        public void Unlock()
        {
            try
            {
                ((IDataChangeNotifier)DataSession).Unlock();
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

        #endregion


        #region internal properties

        public BusinessObjectFactory Factory
        {
            get { return _businessObjectFactory; }
        }

        #endregion internal properties


        #region private properties

        private IDataSession DataSession
        {
            get { return _dataSession; }
        }

        #endregion private properties


        #region private methods

        private void OnDataSessionClose( IDataSession session )
        {
            ((IDataChangeNotifier)DataSession).OnDataChange -= OnDataSessionChange;
            session.OnClose -= OnDataSessionClose;
            session.OnDirty -= OnDataSessionDirty;

            if( OnClose != null )
                OnClose( this );
        }


        private void OnDataSessionDirty( IDataSession session )
        {
            if( OnDirty != null )
                OnDirty( this );
        }


        private void OnDataSessionChange( IDataSession session, IDictionary<IDataObject, IDataChangedObject> changedObjects )
        {
            var notificationList = BuildNotificationList( changedObjects );
            var eventArgs = new DataChangedEventArgs(this, notificationList);

            if (DataChanged != null)
                DataChanged(this, eventArgs);

            //Notify property changes
            foreach (var bo in notificationList.Select( n => (BusinessObject)n.Key ))
                bo.NotifyPropertyChanged(  );

            // Notify all modification Handlers. This will run in its own notification lock.
            NotifyModificationHandlers( eventArgs );

            // Remove all deleted Objects from Cache.
            foreach( var changedObject in changedObjects.Values )
            {
                if( changedObject.ChangeType == EDataChangeType.Deleted )
                    _businessObjectFactory.ReleaseBusinessObject( changedObject.DataObject );
            }
        }

        private IDictionary<IBusinessObject, IChangeNotificationObject> BuildNotificationList( IDictionary<IDataObject, IDataChangedObject> changedObjects )
        {
            return (from c in changedObjects.Values.AsParallel()
                    select (IChangeNotificationObject)new ChangeNotificationObject( Factory.GetBusinessObject( c.DataObject ),
                                                                                    ChangeTypeConverter.Convert( c.ChangeType ) )).ToDictionary( (c) => c.BusinessObject, (c) => c );
        }

        private IBusinessSession This
        {
            get {return this; }
        }

        #endregion private methods
    }
}