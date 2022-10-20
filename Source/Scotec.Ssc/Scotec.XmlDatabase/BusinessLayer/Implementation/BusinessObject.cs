#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

#endregion


namespace Scotec.XMLDatabase
{
    public class BusinessObject : IBusinessObject, IBusinessObjectQuery
    {
        #region Private Attributes

        private readonly Guid _key = Guid.NewGuid();
        private IDataObject _dataObject;
        private int? _hashCode;
        private BusinessSession _session;

        #endregion Private Attributes


        #region IBusinessObject Members

        public bool DataAvailable
        {
            get
            {
                try
                {
                    return Session.Mode != EBusinessSessionMode.Closed && DataObject.DataAvailable;
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

        public IBusinessSession Session
        {
            get
            {
                try
                {
                    return _session;
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

        public Guid Key
        {
            get
            {
                try
                {
                    return _key;
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


        public bool IsSameAs( IBusinessObject obj )
        {
            try
            {
                if( obj is BusinessObject == false )
                    return false;

                return DataObject.IsSameAs( ((BusinessObject)obj).DataObject );
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


        public string ObjectName
        {
            get
            {
                try
                {
                    return DataObject.Name;
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

        public IBusinessObject Parent
        {
            get
            {
                try
                {
                    return BusinessSession.Factory.GetBusinessObject( DataObject.Parent );
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

        public TBusinessObject GetReverseLink<TBusinessObject>( ESearchType searchType = ESearchType.Flat )
        {
            var reverseLinks = GetReverseLinks<TBusinessObject>( searchType );

            if( reverseLinks.Count > 1 )
                throw new BusinessException( EBusinessError.ReadError, "Found more than one reverse link. Use GetReverseLink<>() instead." );

            return reverseLinks.FirstOrDefault();
        }

        public IList<TBusinessObject> GetReverseLinks<TBusinessObject>( ESearchType searchType = ESearchType.Flat )
        {
            return GetReverseLinks( new List<Type> {typeof( TBusinessObject )}, searchType ).Cast<TBusinessObject>().ToList();
        }

        public IList<IBusinessObject> GetReverseLinks( IList<Type> types, ESearchType searchType = ESearchType.Flat )
        {
            try
            {
                IList<string> dataTypes = null;

                if( types != null )
                    dataTypes = new List<string>();

                if( dataTypes != null )
                {
                    foreach( var t in types )
                    {
                        var type = new StringBuilder();
                        type.Append( t.Namespace );
                        type.Append( "." );
                        type.Append( t.Name.Substring( 1 ) );
                        type.Append( "Type" );
                        dataTypes.Add( type.ToString() );
                    }
                }

                var dataSet = ((IDataQuery)DataObject).GetDirectReverseLinks( dataTypes, (EDataSearchType)searchType );
                var set = dataSet.Select( data => BusinessSession.Factory.GetBusinessObject( data ) ).ToList();

                return set;
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

        public T FindParent<T>() where T : IBusinessObject
        {
            try
            {
                var type = typeof( T );
                if( !type.IsInterface )
                    throw new BusinessException( EBusinessError.NotSupported, "The Type must be an interface." );

                var parent = Parent;
                while( parent != null && !(parent is T) )
                    parent = parent.Parent;

                return (T)parent;
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


        public void Reload( bool forceNotification )
        {
            try
            {
                DataObject.Reload( forceNotification );
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


        #region IBusinessObjectQuery Members

        public IList<IBusinessObject> Execute( string query )
        {
            try
            {
                var dataSet = ((IDataQuery)DataObject).Execute( query );
                var set = dataSet.Select( data => BusinessSession.Factory.GetBusinessObject( data ) ).ToList();

                return set;
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


        private readonly Dictionary<string, object> _modifiedProperties = new Dictionary<string, object>();
        public event PropertyChangedEventHandler PropertyChanged;

        internal void Initialize( BusinessSession session, IDataObject dataObject )
        {
            _session = session;
            _dataObject = dataObject;

            OnInitialize();
        }


        protected virtual void OnInitialize()
        {
        }

        public override bool Equals( object obj )
        {
            // obj is null, thus cannot be equal.
            if( obj == null )
                return false;

            // obj is not a business object, thus cannot be equal.
            if( obj is BusinessObject == false )
                return false;

            // Check if the references refer to the same object.
            if( base.Equals( obj ) )
                return true;

            // Compare the content.

            return DataObject.IsSameAs( ((BusinessObject)obj).DataObject );
        }


        public override int GetHashCode()
        {
#if DEBUG
            if( DataObject == null )
                throw new BusinessException( EBusinessError.Document,
                    "Data object does not contain data. Cannot generate hash value." );
#endif

            if( _hashCode == null )
                _hashCode = DataObject.GetHashCode();

            return (int)_hashCode;
        }


        public static bool InterfaceFilter( Type type, Object criteria )
        {
            return type.ToString() == criteria.ToString();
        }


        // Used in generated code.
        protected IBusinessObject Choose( string name )
        {
            var dataObject = DataObject.HasDataObject( name ) ? DataObject.GetDataObject( name ) : DataObject.CreateDataObject( name );

            return BusinessSession.Factory.GetBusinessObject( dataObject );
        }

        internal void NotifyPropertyChanged()
        {
            if( _modifiedProperties.Count == 0 )
                return;

            var localCopy = _modifiedProperties.ToList();
            _modifiedProperties.Clear();

            foreach (var property in localCopy)
            {
                if( PropertyChanged != null )
                    PropertyChanged( this, new PropertyChangedEventArgs( property.Key ) );
            }

        }

        protected void AddModifiedProperty( string property )
        {
            if( !_modifiedProperties.ContainsKey( property ) )
                _modifiedProperties.Add( property, null );
        }


        #region public Properties

        public BusinessSession BusinessSession
        {
            get
            {
                try
                {
                    return _session;
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


        public IDataObject DataObject
        {
            get
            {
                try
                {
                    return _dataObject;
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

        #endregion public Properties
    }
}