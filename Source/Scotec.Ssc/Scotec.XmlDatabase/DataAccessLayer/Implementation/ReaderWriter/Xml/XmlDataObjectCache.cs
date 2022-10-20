#region

using System;
using System.Collections.Generic;
using System.Xml;
using OMS.Deep.Cache;
using Scotec.XMLDatabase.ReaderWriter.Xml.Attributes;

#endregion


namespace Scotec.XMLDatabase.ReaderWriter.Xml
{
    internal sealed class XmlDataObjectCache : IDisposable
    {
        private readonly IDynamicCache<XmlAttribute, XmlDataAttribute> _attributeCache =
                new DynamicCache<XmlAttribute, XmlDataAttribute>( 10000, "DA" );

        //IDictionary<XmlElement, XmlDataObject> _elementCache = new Dictionary<XmlElement, XmlDataObject>();
        //IDictionary<XmlAttribute, XmlDataAttribute> _attributeCache = new Dictionary<XmlAttribute, XmlDataAttribute>();

        private readonly Dictionary<string, IDataFactoryInfo> _dataFactoryInfos =
                new Dictionary<string, IDataFactoryInfo>();

        private readonly IDictionary<Guid, XmlDataObject> _elementByIdCache = new Dictionary<Guid, XmlDataObject>();

        private readonly IDynamicCache<XmlElement, XmlDataObject> _elementCache =
                new DynamicCache<XmlElement, XmlDataObject>( 10000, "DO" );


        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Clear();

            _attributeCache.Dispose();

            _elementCache.Dispose();
        }

        #endregion


        public void AddDataObject( XmlElement key, XmlDataObject dataObject )
        {
            if(_elementCache.Contains( key) )
                return;

            _elementCache.Add( key, dataObject );

            if( key.HasAttribute( "id" ) )
                _elementByIdCache.Add( Utils.GuidFromId( key.GetAttribute( "id" ) ), dataObject );
        }


        public void AddAttribute( XmlAttribute key, XmlDataAttribute attribute )
        {
            _attributeCache.Add( key, attribute );
        }


        //public bool ContainsDataObject(XmlElement key)
        //{
        //    XmlDataObject data = null;
        //    return _elementCache.TryGetValue(key, out data);
        //}

        //public bool ContainsAttribute(XmlAttribute key)
        //{
        //    return _attributeCache.ContainsKey(key);
        //}

        public bool TryGetDataObject( XmlElement key, out XmlDataObject dataObject )
        {
            return _elementCache.TryGetValue( key, out dataObject );
        }


        public XmlDataObject GetDataObjectById( Guid id )
        {
            XmlDataObject dataObject;
            _elementByIdCache.TryGetValue( id, out dataObject );

            return dataObject;
        }


        public bool TryGetAttribute( XmlAttribute key, out XmlDataAttribute dataAttribute )
        {
            return _attributeCache.TryGetValue( key, out dataAttribute );
        }


        public void RemoveDataObject( XmlElement key )
        {
            _elementCache.Remove( key );

            // Remove all attributes from cache.
            foreach( XmlAttribute a in key.Attributes )
                RemoveAttribute( a );

            // Remove all children from cache.
            foreach( XmlNode n in key.ChildNodes )
            {
                if( n is XmlElement )
                    RemoveDataObject( (XmlElement)n );
            }
            // Remove from ID cache.
            var id = Utils.GuidFromId( key.GetAttribute( "id" ) );
            if( id != Guid.Empty )
                _elementByIdCache.Remove( id );
        }


        public void RemoveAttribute( XmlAttribute key )
        {
            _attributeCache.Remove( key );
        }


        public void Clear()
        {
            _elementCache.Clear();
            _attributeCache.Clear();
            _elementByIdCache.Clear();
        }


        public void AddFactoryInfo( string key, IDataFactoryInfo factoryInfo )
        {
            _dataFactoryInfos.Add( key, factoryInfo );
        }


        public IDataFactoryInfo GetFactoryInfo( string key )
        {
            IDataFactoryInfo info;
            _dataFactoryInfos.TryGetValue( key, out info );

            return info;
        }
    }
}
