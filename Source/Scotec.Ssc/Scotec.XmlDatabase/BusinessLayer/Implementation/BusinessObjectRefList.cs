#region

using System;
using System.Collections;
using System.Collections.Generic;

#endregion


namespace Scotec.XMLDatabase
{
    public class BusinessObjectRefList<TBusinessObjectType> : BusinessObject,
                                                              IBusinessObjectRefList<TBusinessObjectType>
            where TBusinessObjectType : IBusinessObject
    {
        #region IBusinessObjectRefList<IBusinessObjectType> Members

        public int Append( TBusinessObjectType businessObject )
        {
            try
            {
                if( businessObject is BusinessObject == false )
                    throw new BusinessException( EBusinessError.Document, "Invalid implementation for IBusinessObject." );

// ReSharper disable PossibleNullReferenceException
                return DataRefList.Append( (businessObject as BusinessObject).DataObject );
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


        public bool Contains( TBusinessObjectType businessObject )
        {
            try
            {
                if( businessObject is BusinessObject == false )
                    throw new BusinessException( EBusinessError.Document, "Invalid implementation for IBusinessObject." );

// ReSharper disable PossibleNullReferenceException
                return DataRefList.Contains( (businessObject as BusinessObject).DataObject );
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
                    return DataRefList.Count;
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


        public int IndexOf( TBusinessObjectType businessObject )
        {
            try
            {
                if( businessObject is BusinessObject == false )
                    throw new BusinessException( EBusinessError.Document, "Invalid implementation for IBusinessObject." );

// ReSharper disable PossibleNullReferenceException
                return DataRefList.IndexOf( (businessObject as BusinessObject).DataObject );
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


        public int InsertAt( int index, TBusinessObjectType businessObject )
        {
            try
            {
                if( businessObject is BusinessObject == false )
                    throw new BusinessException( EBusinessError.Document, "Invalid implementation for IBusinessObject." );

// ReSharper disable PossibleNullReferenceException
                return DataRefList.InsertAt( index, (businessObject as BusinessObject).DataObject );
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


        public void Move( int from, int to )
        {
            try
            {
                DataRefList.Move( from, to );
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
                DataRefList.MoveAfter( (sourceObject as BusinessObject).DataObject,
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
                    throw new BusinessException( EBusinessError.Document, "Invalid implementation for IBusinessObject." );

// ReSharper disable PossibleNullReferenceException
                DataRefList.MoveTo( index, (businessObject as BusinessObject).DataObject );
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


        public void RemoveAll()
        {
            try
            {
                DataRefList.RemoveAll();
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


        public int RemoveAt( int index )
        {
            try
            {
                return DataRefList.RemoveAt( index );
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
                    return (TBusinessObjectType)BusinessSession.Factory.GetBusinessObject( DataRefList[index] );
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
            return new Enumerator( BusinessSession, DataRefList );
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion


        #region Public Properties

        public IDataRefList DataRefList
        {
            get { return (DataObject as IDataRefList); }
        }

        #endregion Protected Properties


        #region Nested type: Enumerator

        private class Enumerator : IEnumerator<TBusinessObjectType>
        {
            #region Private Attributes

            private readonly IDataRefList _dataList;
            private readonly BusinessSession _session;
            private int _index = -1;

            #endregion Private Attributes


            #region Constructor

            public Enumerator( BusinessSession session, IDataRefList dataList )
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
    }
}
