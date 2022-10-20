#region

using System;
using System.IO;
using System.Reflection;
using Scotec.XMLDatabase.Attributes;

#endregion


namespace Scotec.XMLDatabase
{
    public class BusinessObjectFactory : IDisposable
    {
        private readonly BusinessSession _session;
        private BusinessObjectCache _objectCache = new BusinessObjectCache();


        public BusinessObjectFactory( BusinessSession session )
        {
            _session = session;
            //macroService.Parse( "$AppDir", null );
        }


        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if( _objectCache != null )
                (_objectCache as IDisposable).Dispose();
            _objectCache = null;
        }

        #endregion


        public IBusinessObject GetBusinessObject( IDataObject dataObject )
        {
            if( dataObject == null )
                return null;

            lock( _objectCache )
            {
                var businessObject = _objectCache.GetBusinessObject( dataObject );

                if( businessObject != null )
                    return businessObject;

                return InstantiateBusinessObject( dataObject );
            }
        }


        public IBusinessAttribute GetBusinessAttribute( IDataAttribute dataAttribute )
        {
            lock( _objectCache )
            {
                var businessObject = _objectCache.GetBusinessObject( dataAttribute as IDataObject );

                if( businessObject != null )
                    return businessObject as IBusinessAttribute;

                return InstantiateBusinessObject( dataAttribute as IDataObject ) as IBusinessAttribute;
            }
        }


        private IBusinessObject InstantiateBusinessObject( IDataObject dataObject )
        {
            // Get the assembly used to instantiate the business object.

            var factoryInfo = dataObject.DataFactoryInfo;

            var typeName = (string)factoryInfo["Schema.TypeName"];
            // Remove trailing "Type".
            typeName = typeName.Remove( typeName.Length - 4 );

            Assembly assembly;
            object assemblyObject;
            if( factoryInfo.TryGetValue( "BusinessObject.Assembly", out assemblyObject ) )
                assembly = (Assembly)assemblyObject;
            else
            {
                // Try to create the business object if any.
                try
                {
                    // All generated implementation projects have the extension ".BLE". 
                    //assembly = Assembly.Load( (string)factoryInfo["Schema.Name"] + ".BLE" );
                    assembly = Assembly.Load( (string)factoryInfo["Schema.Name"] );
                    factoryInfo.Add( "BusinessObject.Assembly", assembly );
                }
                catch( FileNotFoundException e )
                {
                    throw new BusinessException( EBusinessError.Document, "Could not instantiate business object", e );
                }
            }
            var businessObject = (BusinessObject)assembly.CreateInstance( typeName, false,
                                                                          BindingFlags.CreateInstance, null,
                                                                          new object[] { }, null, null );

            // Object is still not initialized.
            _objectCache.AddBusinessObject( dataObject, businessObject );

            businessObject.Initialize(_session, dataObject);

            return businessObject;
        }


        public void ReleaseBusinessObject( IDataObject key )
        {
            lock( _objectCache )
            {
                _objectCache.RemoveBusinessObject( key );
            }
        }


        public void ReleaseBusinessAttribute( IDataAttribute key )
        {
            lock( _objectCache )
            {
                _objectCache.RemoveBusinessObject( key as IDataObject );
            }
        }


        public void ReleaseAll()
        {
            lock( _objectCache )
            {
                _objectCache.Clear();
            }
        }
    }
}
