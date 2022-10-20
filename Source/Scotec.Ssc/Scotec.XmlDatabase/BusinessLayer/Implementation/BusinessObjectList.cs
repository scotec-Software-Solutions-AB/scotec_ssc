#region

using System;
using System.Collections;
using System.Collections.Generic;

#endregion


namespace Scotec.XMLDatabase
{
    public class BusinessObjectList<TBusinessObjectType> : BusinessObject, IBusinessObjectList<TBusinessObjectType>
            where TBusinessObjectType : IBusinessObject
    {
        #region IBusinessObjectList<IBusinessObjectType> Members

        public TBusinessObjectType Append( TBusinessObjectType businessObject )
        {
            try
            {
                if( businessObject is BusinessObject == false )
                    throw new BusinessException( EBusinessError.Document, "Invalid implementation for IBusinessObject." );

                // ReSharper disable PossibleNullReferenceException
                var newObject = DataList.Append( (businessObject as BusinessObject).DataObject );
                // ReSharper restore PossibleNullReferenceException
                return (TBusinessObjectType)BusinessSession.Factory.GetBusinessObject( newObject );
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


        public bool Contains( TBusinessObjectType businessObject )
        {
            try
            {
                if( businessObject is BusinessObject == false )
                    return false;

                // ReSharper disable PossibleNullReferenceException
                return DataList.Contains( (businessObject as BusinessObject).DataObject );
                // ReSharper restore PossibleNullReferenceException
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


        public int Count
        {
            get
            {
                try
                {
                    return DataList.Count;
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


        public T Create<T>() where T : TBusinessObjectType
        {
            try
            {
                var type = typeof( T );
                var typeName = string.Format( "{0}.{1}Type", type.Namespace, type.Name.Substring( 1 ) );


                var newObject = DataList.CreateDataObject( typeName );

                return (T)BusinessSession.Factory.GetBusinessObject( newObject );
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


        public TBusinessObjectType Create()
        {
            try
            {
                var newObject = DataList.CreateDataObject();

                return (TBusinessObjectType)BusinessSession.Factory.GetBusinessObject( newObject );
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


        public T CreateAt<T>( int index ) where T : TBusinessObjectType
        {
            try
            {
                var type = typeof( T );
                var typeName = string.Format( "{0}.{1}Type", type.Namespace, type.Name.Substring( 1 ) );


                var newObject = DataList.CreateDataObjectAt( index, typeName );

                return (T)BusinessSession.Factory.GetBusinessObject( newObject );
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


        public TBusinessObjectType CreateAt( int index )
        {
            try
            {
                var newObject = DataList.CreateDataObjectAt( index );

                return (TBusinessObjectType)BusinessSession.Factory.GetBusinessObject( newObject );
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


        public void DeleteAll()
        {
            try
            {
                DataList.DeleteAll();
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


        public int Delete( TBusinessObjectType businessObject )
        {
            try
            {
                var index = IndexOf( businessObject );
                return DeleteAt( index );
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


        public int DeleteAt( int index )
        {
            try
            {
                return DataList.DeleteAt( index );
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


        public int IndexOf( TBusinessObjectType businessObject )
        {
            try
            {
                if( businessObject is BusinessObject == false )
                    return -1;

                // ReSharper disable PossibleNullReferenceException
                return DataList.IndexOf( (businessObject as BusinessObject).DataObject );
                // ReSharper restore PossibleNullReferenceException
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


        public TBusinessObjectType InsertAt( int index, TBusinessObjectType businessObject )
        {
            try
            {
                if( businessObject is BusinessObject == false )
                    throw new BusinessException( EBusinessError.Document, "Invalid implementation for IBusinessObject." );

                // ReSharper disable PossibleNullReferenceException
                var newObject = DataList.InsertAt( index, (businessObject as BusinessObject).DataObject );
                // ReSharper restore PossibleNullReferenceException

                return (TBusinessObjectType)BusinessSession.Factory.GetBusinessObject( newObject );
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


        public void Move( int from, int to )
        {
            try
            {
                DataList.Move( from, to );
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


        public void MoveAfter( TBusinessObjectType sourceObject, TBusinessObjectType targetObject )
        {
            try
            {
                if( sourceObject is BusinessObject == false )
                    throw new BusinessException( EBusinessError.Document,
                                                 "Invalid implementation for IBusinessObject (source)." );

                if( targetObject is BusinessObject == false )
                    throw new BusinessException( EBusinessError.Document,
                                                 "Invalid implementation for IBusinessObject (target)." );

                // ReSharper disable PossibleNullReferenceException
                DataList.MoveAfter( (sourceObject as BusinessObject).DataObject,
                                    (targetObject as BusinessObject).DataObject );
                // ReSharper restore PossibleNullReferenceException
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


        public void MoveTo( int index, TBusinessObjectType businessObject )
        {
            try
            {
                if( businessObject is BusinessObject == false )
                    throw new BusinessException( EBusinessError.Document,
                                                 "Invalid implementation for IBusinessObject (target)." );

                // ReSharper disable PossibleNullReferenceException
                DataList.MoveTo( index, (businessObject as BusinessObject).DataObject );
                // ReSharper restore PossibleNullReferenceException
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


        public TBusinessObjectType this[ int index ]
        {
            get
            {
                try
                {
                    return (TBusinessObjectType)BusinessSession.Factory.GetBusinessObject( DataList[index] );
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


        public IEnumerator<TBusinessObjectType> GetEnumerator()
        {
            try
            {
                return new Enumerator( BusinessSession, DataList );
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


        IEnumerator IEnumerable.GetEnumerator()
        {
            try
            {
                return GetEnumerator();
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


        #region Public Properties

        public IDataList DataList
        {
            get { return (DataObject as IDataList); }
        }

        #endregion Protected Properties


        #region Nested type: Enumerator

        private class Enumerator : IEnumerator<TBusinessObjectType>
        {
            #region Private Attributes

            private readonly IDataList _dataList;
            private readonly BusinessSession _session;
            private int _index = -1;

            #endregion Private Attributes


            #region Constructor

            public Enumerator( BusinessSession session, IDataList dataList )
            {
                _session = session;
                _dataList = dataList;
            }

            #endregion Constructor


            #region IEnumerator<IBusinessObjectType> Members

            public TBusinessObjectType Current
            {
                get { return (TBusinessObjectType)_session.Factory.GetBusinessObject( _dataList[_index] ); }
            }


            public void Dispose()
            {
            }


            object IEnumerator.Current
            {
                get { return Current; }
            }


            public bool MoveNext()
            {
                _index++;

                return (_index < _dataList.Count);
            }


            public void Reset()
            {
                _index = -1;
            }

            #endregion
        }

        #endregion


        #region Private Attributes

        #endregion Private Attributes
    }
}
