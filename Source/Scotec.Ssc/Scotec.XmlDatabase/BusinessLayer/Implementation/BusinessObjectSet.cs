#region

using System;
using System.Collections;
using System.Collections.Generic;

#endregion


namespace Scotec.XMLDatabase
{
    internal class BusinessObjectSet : IBusinessObjectSet
    {
        #region IBusinessObjectSet Members

        public int Add( IBusinessObject businessObject )
        {
            try
            {
                if( businessObject is BusinessObject == false )
                    throw new BusinessException( EBusinessError.Document, "Invalid implementation for IBusinessObject." );

                return DataSet.Add( ((BusinessObject)businessObject).DataObject );
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


        public bool Contains( IBusinessObject businessObject )
        {
            try
            {
                if( businessObject is BusinessObject == false )
                    throw new BusinessException( EBusinessError.Document, "Invalid implementation for IBusinessObject." );

                return DataSet.Contains( ((BusinessObject)businessObject).DataObject );
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
                    return DataSet.Count;
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


        public IBusinessObjectSet Difference( IBusinessObjectSet businessObjectSet )
        {
            try
            {
                if( businessObjectSet is BusinessObjectSet == false )
                    throw new BusinessException( EBusinessError.Document, "Invalid implementation for IBusinessObject." );

                var dataSet = DataSet.Difference( ((BusinessObjectSet)businessObjectSet).DataSet );

                var set = new BusinessObjectSet();
                set.Initialize( BusinessSession, dataSet );

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


        public IBusinessObjectSet Intersect( IBusinessObjectSet businessObjectSet )
        {
            try
            {
                if( businessObjectSet is BusinessObjectSet == false )
                    throw new BusinessException( EBusinessError.Document, "Invalid implementation for IBusinessObject." );

                var dataSet = DataSet.Intersect( ((BusinessObjectSet)businessObjectSet).DataSet );

                var set = new BusinessObjectSet();
                set.Initialize( BusinessSession, dataSet );

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


        public int RemoveAt( int index )
        {
            try
            {
                return DataSet.RemoveAt( index );
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


        public IBusinessObjectSet SymmetricDifference( IBusinessObjectSet businessObjectSet )
        {
            try
            {
                if( businessObjectSet is BusinessObjectSet == false )
                    throw new BusinessException( EBusinessError.Document, "Invalid implementation for IBusinessObject." );

                var dataSet = DataSet.SymmetricDifference( ((BusinessObjectSet)businessObjectSet).DataSet );

                var set = new BusinessObjectSet();
                set.Initialize( BusinessSession, dataSet );

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


        public IBusinessObjectSet Union( IBusinessObjectSet businessObjectSet )
        {
            try
            {
                if( businessObjectSet is BusinessObjectSet == false )
                    throw new BusinessException( EBusinessError.Document, "Invalid implementation for IBusinessObject." );

                var dataSet = DataSet.Union( ((BusinessObjectSet)businessObjectSet).DataSet );

                var set = new BusinessObjectSet();
                set.Initialize( BusinessSession, dataSet );

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


        public IBusinessObject this[ int index ]
        {
            get
            {
                try
                {
                    return BusinessSession.Factory.GetBusinessObject( DataSet[index] );
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


        public IEnumerator<IBusinessObject> GetEnumerator()
        {
            try
            {
                return new Enumerator( BusinessSession, DataSet.GetEnumerator() );
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


        internal void Initialize( BusinessSession session, IDataSet dataSet )
        {
            _session = session;
            _dataSet = dataSet;
        }


        #region Internal Properties

        private BusinessSession BusinessSession
        {
            get { return _session; }
        }

        #endregion Internal Properties


        #region Protected Properties

        private IDataSet DataSet
        {
            get { return _dataSet; }
        }

        #endregion Protected Properties


        #region Nested type: Enumerator

        private class Enumerator : IEnumerator<IBusinessObject>
        {
            #region Private Attributes

            private readonly IEnumerator _dataEnumerator;
            private readonly BusinessSession _session;

            #endregion Private Attributes


            #region Constructor

            public Enumerator( BusinessSession session, IEnumerator dataEnumerator )
            {
                _session = session;
                _dataEnumerator = dataEnumerator;
            }

            #endregion Constructor


            #region IEnumerator<IBusinessObject> Members

            public IBusinessObject Current
            {
                get { return _session.Factory.GetBusinessObject( (IDataObject)_dataEnumerator.Current ); }
            }


            public void Dispose()
            {
            }


            object IEnumerator.Current
            {
                get { return _session.Factory.GetBusinessObject( (IDataObject)_dataEnumerator.Current ); }
            }


            public bool MoveNext()
            {
                return _dataEnumerator.MoveNext();
            }


            public void Reset()
            {
                _dataEnumerator.Reset();
            }

            #endregion
        }

        #endregion


        #region Private Attributes

        private IDataSet _dataSet;
        private BusinessSession _session;

        #endregion Private Attributes
    }
}
