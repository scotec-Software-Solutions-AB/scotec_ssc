#region

using System;
using OMS.Deep.Cache;
using Scotec.XMLDatabase.Attributes;

#endregion


namespace Scotec.XMLDatabase
{
    internal class BusinessObjectCache : IDisposable
    {
        private readonly IWeakReferenceCache<IDataAttribute, IBusinessAttribute> _businessAttributes
                = new WeakReferenceCache<IDataAttribute, IBusinessAttribute>( 30000, "BA" );

        private readonly IWeakReferenceCache<IDataObject, IBusinessObject> _businessObjects
                = new WeakReferenceCache<IDataObject, IBusinessObject>( 30000, "BO" );


        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Clear();

            if( _businessObjects != null )
                _businessObjects.Dispose();

            if( _businessAttributes != null )
                _businessAttributes.Dispose();
        }

        #endregion


        internal void Clear()
        {
            _businessAttributes.Clear();
            _businessObjects.Clear();
        }


        internal void AddBusinessObject( IDataObject key, IBusinessObject businessObject )
        {
            _businessObjects.Add( key, businessObject );
        }


        internal IBusinessObject GetBusinessObject( IDataObject key )
        {
            if( key == null )
                return null;

            IBusinessObject businessObject;
            _businessObjects.TryGetValue( key, out businessObject );

            return businessObject;
        }


        internal void RemoveBusinessObject( IDataObject key )
        {
            _businessObjects.Remove( key );
        }
    }
}
