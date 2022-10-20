#region

using System.Xml;

#endregion


namespace Scotec.XMLDatabase.ReaderWriter.Xml
{
    internal sealed class XmlChangeNotifier
    {
        private readonly XmlDataDocument _document;
        private readonly XmlDataObjectCache _dataCache;


        public XmlChangeNotifier( XmlDataDocument document, XmlDataObjectCache dataCache )
        {
            _document = document;
            _dataCache = dataCache;
        }


        public void AddAttribute( XmlAttribute attribute, EDataChangeType changeType )
        {
            var owner = attribute.OwnerElement;

            try
            {
                // Each attribue change (adding, modifying or deleting) causes
                // an immediate "modifying" notification that will be handled by rules.
                // 
                ChangeNotifier.Lock();
                // The attribute will be added to the change list as well.
                AddElement( owner, EDataChangeType.Modifying );
#if ADVANCED_NOTIFY
                AddAttributeDefault(attribute, changeType);
#endif
            }
            finally
            {
                ChangeNotifier.Unlock();
            }

            AddElement( owner, EDataChangeType.Modified );
#if ADVANCED_NOTIFY
            AddAttributeDefault(attribute, changeType);
#endif
        }


        public void AddElement( XmlElement element, EDataChangeType changeType )
        {
            switch( changeType )
            {
                case EDataChangeType.Adding:
                case EDataChangeType.Added:
                case EDataChangeType.Modifying:
                case EDataChangeType.Modified:
                case EDataChangeType.Deleting:
                    AddElementDefault( element, changeType );
                    break;
                case EDataChangeType.Deleted:
                    AddElementDeleted( element, changeType );
                    break;
            }
        }


        private void AddElementDefault( XmlElement element, EDataChangeType changeType )
        {
            ChangeNotifier.AddChangedObject( _document.DataObjectFactory.GetDataObject( element ), changeType );
        }


#if ADVANCED_NOTIFY
        private void AddAttributeDefault( XmlAttribute attribute, EDataChangeType changeType)
        {
            ChangeNotifier.AddChangedObject(_document.DataObjectFactory.GetAttribute(attribute)
                as IDataObject, changeType);
        }
#endif


        private void AddElementDeleted( XmlElement element, EDataChangeType changeType )
        {
            XmlDataObject deletedObject = null;
            if( _dataCache.TryGetDataObject( element, out deletedObject ) )
            {
                var parent = deletedObject.Parent;

                if( parent != null )
                    ChangeNotifier.AddChangedObject( parent, EDataChangeType.Modified );
                ChangeNotifier.AddChangedObject( deletedObject, changeType );
            }

            var children = element.SelectNodes( ".//*", _document.NamespaceManager );
            foreach( XmlNode n in children )
            {
                if( n is XmlElement )
                {
                    XmlDataObject childData = null;
                    if( _dataCache.TryGetDataObject( (XmlElement)n, out childData ) )
                        ChangeNotifier.AddChangedObject( childData, EDataChangeType.Deleted );
                }
                //else if (n is XmlAttribute)
                //{
                //    if (m_dataCache.ContainsAttribute((XmlAttribute)n))
                //        ChangeNotifier.AddChangedObject(m_dataCache.GetAttribute((XmlAttribute)n), EDataChangeType.Deleted);
                //}
            }
        }


        private IDataChangeNotifier ChangeNotifier
        {
            get { return _document; }
        }
    }
}
