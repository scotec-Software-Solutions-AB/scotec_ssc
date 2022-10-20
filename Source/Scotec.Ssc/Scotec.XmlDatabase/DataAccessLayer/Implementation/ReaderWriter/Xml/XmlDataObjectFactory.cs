#region

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using Scotec.Transactions;
using Scotec.XMLDatabase.Attributes;
using Scotec.XMLDatabase.ReaderWriter.Transactions;
using Scotec.XMLDatabase.ReaderWriter.Xml.Attributes;

#endregion


namespace Scotec.XMLDatabase.ReaderWriter.Xml
{
    internal sealed class XmlDataObjectFactory : IDisposable
    {
        private readonly XmlChangeNotifier _changeNotifier;
        private readonly IDictionary<XmlElement, XmlElement> _deletedObjects = new Dictionary<XmlElement, XmlElement>();

        /// <directed>False</directed>
        private readonly XmlDataDocument _document;

        private readonly XmlRecreator _recreator = new XmlRecreator();

        private readonly IDictionary<XmlSchema, IDictionary<string, string>> _schemaReflectionInfo =
            new Dictionary<XmlSchema, IDictionary<string, string>>();

        private XmlDataObjectCache _dataCache = new XmlDataObjectCache();
        private long _deletedCount;


        internal XmlDataObjectFactory( XmlDataDocument document, ITransactionManager transactionManager )
        {
            _document = document;
            _changeNotifier = new XmlChangeNotifier( document, _dataCache );

            TransactionManager = transactionManager;
        }


        private IDictionary<XmlSchema, IDictionary<string, string>> SchemaReflectionInfo
        {
            get { return _schemaReflectionInfo; }
        }

        private XmlDataDocument Document
        {
            get { return _document; }
        }

        private ITransactionManager TransactionManager { get; set; }


        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if( _dataCache != null )
            {
                _dataCache.Clear();
                ((IDisposable)_dataCache).Dispose();
                _dataCache = null;
            }
        }

        #endregion


        internal void AddSchemaReflectionInfo( XmlSchema schema, IDictionary<string, string> reflectionInfo )
        {
            SchemaReflectionInfo.Add( schema, reflectionInfo );
        }


        internal IDictionary<string, string> GetSchemaReflectionInfo( XmlSchema schema )
        {
            return SchemaReflectionInfo[schema];
        }


        internal void MoveElement( XmlElement parent, int from, int to )
        {
            lock( this )
            {
                InternalMoveElement( parent, from, to, true );
                AddModifiedElementToChangeNotifier( parent );
            }
        }

        internal IDataObject CloneDataObject( XmlElement element, XmlElement newParent, XmlSchemaType schemaType, int index )
        {
            var clone = CloneElement( element, newParent, schemaType, index );

            return GetDataObject( clone );
        }

        private XmlElement CloneElement( XmlElement element, XmlElement newParent, XmlSchemaType schemaType, int index )
        {
            lock( this )
            {
                var clone = (XmlElement)RecreateObject( schemaType.Name );

                if( clone == null )
                {
// ReSharper disable PossibleNullReferenceException
// ReSharper disable AssignNullToNotNullAttribute
                    clone = (XmlElement)newParent.OwnerDocument.ImportNode( element, true );

                    var ids = clone.SelectNodes( "//@id" ).Cast<XmlAttribute>();
                    var references = clone.SelectNodes( "//@idRef" ).Cast<XmlAttribute>().ToList();

                    foreach( var id in ids )
                    {
                        var inClosureId = id;
                        var newId = Utils.IdFromGuid( Guid.NewGuid() );

                        foreach( var reference in references.Where( a => a.Value == inClosureId.Value ) )
                            reference.Value = newId;

                        id.Value = newId;
                    }
                }
// ReSharper restore AssignNullToNotNullAttribute
// ReSharper restore PossibleNullReferenceException

                InternalAddElement( newParent, clone, index, true );
                AddAddedElementToChangeNotifier( clone );

                return clone;
            }
        }


        internal void RemoveElement( XmlElement parent, XmlElement element )
        {
            lock( this )
            {
                RemoveElementInternal( parent, element, true );
            }
        }


        private void NotifyRemovingElement( XmlElement element )
        {
            AddRemovingElementToChangeNotifier( element );

            foreach (var n in element.ChildNodes.OfType<XmlNode>())
            {
                NotifyRemovingElement( n as XmlElement );
            }
        }


        private void RemoveElementInternal( XmlElement parent, XmlElement element, bool notifyDeleting )
        {
            try
            {
                ++_deletedCount;

                if( _deletedObjects.ContainsKey( element ) )
                    return; // Do not delete twice.

                _deletedObjects.Add( element, null );

                if( notifyDeleting ) // Avoid recursive "deleting" notifications
                {
                    // Notify all observers, that the object is going to be deleted.
                    // This notification will happen immediatly.
                    var notifier = Document as IDataChangeNotifier;

                    try
                    {
                        // Create a new lock. This is needed to create a new notification list
                        // containing elements that are marked as "deleting" only.
                        // The list wont't contain elements marked as "added", "modified, etc.
                        notifier.Lock();
                        NotifyRemovingElement( element );
                    }
                    finally
                    {
                        notifier.Unlock();
                    }
                }
                foreach( var node in element.ChildNodes.OfType<XmlElement>() )
                    RemoveElementInternal( element, node, false );

                AddRemovedElementToChangeNotifier( element );

                // Notify parent as "modified" if we are a list element

                var type = parent.Attributes["xsi:type"].Value;
                if( type.EndsWith( "ListType" ) || type.EndsWith( "RefListType" ) )
                    AddModifiedElementToChangeNotifier( parent );

                // Really remove the element
                InternalRemoveElement( parent, element, true );
            }
            finally
            {
                if( --_deletedCount == 0 )
                    _deletedObjects.Clear();
            }
        }


        internal void RemoveAttribute( XmlElement parent, XmlAttribute attribute )
        {
            lock( this )
            {
                if( _deletedObjects.ContainsKey( parent ) )
                    return; // Do not delete attributes of already deleted elements.

                AddRemovedAttributeToChangeNotifier( attribute );
                InternalRemoveAttribute( parent, attribute, true );
            }
        }


        private void CleanupReferences( XmlElement element )
        {
            var query = "//@id";

            // Get all "id" attributes up from the current node.
// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable PossibleNullReferenceException
            var ids = element.SelectNodes( query ).OfType<XmlAttribute>();

            foreach( var id in ids )
            {
                // Get all nodes of the appropriate type in the document referencing the current node.
                query = "//*[@idRef='" + id.Value + "']";
                var references = element.OwnerDocument.SelectNodes( query ).OfType<XmlElement>();

                // Set all references to "". The reference objects won't be removed!
                // Removing empty reference is in the responsibility of the program flow.
                // Trying to access an empty reference object will return null.
                foreach( var reference in references )
                {
                    var parent = (XmlElement)reference.ParentNode;
                    if( parent.Attributes["xsi:type"].Value.EndsWith( "RefListType" ) )
                    {
                        // If the reference is within a reference list, the element must
                        // be removed from the list. 
                        // !!!The "Count" property of the reference list will change in that case!!!

                        RemoveElement( parent, reference );
                        //this.InternalRemoveElement(parent, reference, true);
                    }
                    else
                    {
                        // Just remove the reference.
                        RemoveAttribute( reference, reference.Attributes["idRef"] );
                        //this.InternalRemoveAttribute(reference, reference.Attributes["idRef"], true);
                    }
                    //this.AddModifiedElementToChangeNotifier(parent);
                }
            }
// ReSharper restore PossibleNullReferenceException
// ReSharper restore AssignNullToNotNullAttribute
        }


        internal void AddElementToCache( XmlDataObject dataObject )
        {
            _dataCache.AddDataObject( dataObject.Element, dataObject );
        }


        private void RemoveElementFromCache( XmlElement key )
        {
            _dataCache.RemoveDataObject( key );
        }


        private void RemoveAttributeFromCache( XmlAttribute key )
        {
            _dataCache.RemoveAttribute( key );
        }


        /// <summary>
        ///     Creates a new data object. Called from XmlDataDocument.CreateDocument() only.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        internal IDataObject CreateDataObject( XmlDataDocument document, XmlElement element )
        {
            lock( this )
            {
                var typeName = element.Attributes["xsi:type"].Value;

                return InstantiateDataElement( document, typeName, element, true );
            }
        }


        /// <summary>
        ///     Insert a new object after any object given in the insertAfter parameter. If insertAfter is null
        ///     or does not contain any element, the new element is appended as the very last element.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="description"></param>
        /// <param name="parent"></param>
        /// <param name="attributes"></param>
        /// <param name="insertAfter"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal IDataObject CreateDataObject(
            XmlDataDocument document, XmlSchemaElement description,
            string type, XmlDataObject parent, ListDictionary attributes, string[] insertAfter )
        {
            lock( this )
            {
                var index = -1;
                if( insertAfter != null && insertAfter.Length > 0 && insertAfter[0] != null )
                {
                    var query = "";
                    var size = insertAfter.Length;
                    for( var i = 0; i < size && insertAfter[i] != null; i++ )
                    {
                        if( query.Length > 0 )
                            query += " | ";
                        query += string.Format( "dm:{0}", insertAfter[i] );
                    }

                    var it = parent.Select( query );
                    index = it.Count;
                }

                return CreateDataObject( document, description, type, parent, attributes, index );
            }
        }


        /// <summary>
        ///     Insert a new object at the specified index. If index is -1
        ///     or this element does not contain any child element, the new element is
        ///     appended as the very last element.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="description"></param>
        /// <param name="parent"></param>
        /// <param name="attributes"></param>
        /// <param name="index"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal IDataObject CreateDataObject(
            XmlDataDocument document, XmlSchemaElement description,
            string type, XmlDataObject parent, ListDictionary attributes, int index )
        {
            lock( this )
            {
                // First create the XML element
                string typeName = type ?? description.SchemaTypeName.Name;

                var element = (XmlElement)RecreateObject( typeName )
                              ??
                              Document.Document.CreateElement( description.Name, description.SchemaTypeName.Namespace );
                InternalAddElement( parent.Element, element, index, true );

                //Create a new instance of the XmlDataObject.
                return InstantiateDataElement( document, element,
                    new XmlQualifiedName( typeName, description.SchemaTypeName.Namespace ),
                    parent.Element, attributes );
            }
        }


        private XmlDataObject InstantiateDataElement(
            XmlDataDocument document, string typeName, XmlElement element, bool create )
        {
            XmlDataObject dataObject;

            if( _dataCache.TryGetDataObject( element, out dataObject ) )
                return dataObject;

            dataObject = GetNewInstance( typeName );

            dataObject.Initialize( document, element, create );
            dataObject.DataFactoryInfo = GetDataFactoryInfo( dataObject );
            if(create)
                AddNewElementToNotifier( element );

            return dataObject;
        }

        private void AddNewElementToNotifier( XmlElement element )
        {
            var elementType = element.Attributes["xsi:type"].Value;
            if( !elementType.EndsWith( "ListType" ) && !elementType.EndsWith( "RefListType" ) && !elementType.EndsWith( "RefType" ) )
            {
                var notifier = Document as IDataChangeNotifier;
                try
                {
                    notifier.Lock();
                    _changeNotifier.AddElement( element, EDataChangeType.Adding );
                }
                finally
                {
                    notifier.Unlock();
                }
            }
        }


        private XmlDataObject InstantiateDataElement(
            XmlDataDocument document, XmlElement element, XmlQualifiedName typeName, XmlNode parent,
            ListDictionary attributes )
        {
            XmlDataObject dataObject;

            if( _dataCache.TryGetDataObject( element, out dataObject ) )
                return dataObject;

            dataObject = GetNewInstance( typeName.Name );

            dataObject.Initialize( document, element, typeName, parent, attributes );
            dataObject.DataFactoryInfo = GetDataFactoryInfo( dataObject );
            AddNewElementToNotifier(element);

            return dataObject;
        }


        private static XmlDataObject GetNewInstance( string typeName )
        {
            if( typeName.EndsWith( "RefListType" ) )
                return new XmlDataRefList();
            return typeName.EndsWith( "ListType" ) ? new XmlDataList() : new XmlDataObject();
        }


        private IDataFactoryInfo GetDataFactoryInfo( XmlDataObject dataObject )
        {
            var typeName = dataObject.Element.GetAttribute( "xsi:type" );

            var info = _dataCache.GetFactoryInfo( typeName );

            if( info != null )
                return info;
            return GetDataFactoryInfo( dataObject.SchemaType, typeName );
        }


        private IDataFactoryInfo GetDataFactoryInfo( XmlDataAttribute dataAttribute )
        {
            var type = (XmlSchemaSimpleType)dataAttribute.SchemaType;
            var typeName = type.Name;

            var info = _dataCache.GetFactoryInfo( typeName );

            if( info != null )
                return info;
            return GetDataFactoryInfo( type, typeName );
        }


        private IDataFactoryInfo GetDataFactoryInfo( XmlSchemaObject schemaObject, string typeName )
        {
            while( !(schemaObject is XmlSchema) )
                schemaObject = schemaObject.Parent;

            var reflectionInfo = SchemaReflectionInfo[(XmlSchema)schemaObject];
            IDataFactoryInfo info = new XmlDataFactoryInfo {{"Schema.TypeName", typeName}};

            foreach( var pair in reflectionInfo )
                info.Add( "Schema." + pair.Key, pair.Value );

            _dataCache.AddFactoryInfo( typeName, info );

            return info;
        }


        internal IDataObject GetDataObject( string name, XmlDataObject parent )
        {
            lock( this )
            {
                XmlElement element = null;

                if( parent != null )
                {
                    var it = parent.Select( string.Format( "dm:{0}", name ) );
                    if( it.MoveNext() )
                        element = it.Current.UnderlyingObject as XmlElement;
                }
                else
                    element = Document.Document.DocumentElement;

                if( element == null )
                    throw new Exception( "Element " + name + " not found." );

                return GetDataObject( element );
            }
        }


        internal IDataObject GetDataObject( XmlElement element )
        {
            lock( this )
            {
                XmlDataObject dataObject;

                if( _dataCache.TryGetDataObject( element, out dataObject ) )
                    return dataObject;

                return GetInternalDataObject( element );
            }
        }


        private XmlDataObject GetInternalDataObject( XmlElement element )
        {
            var typeName = element.Attributes["xsi:type"].Value;

            return InstantiateDataElement( Document, typeName, element, false );
        }


        internal IDataAttribute CreateAttribute( XmlDataObject parent, XmlSchemaAttribute description, bool notify )
        {
            lock( this )
            {
                var attribute = (XmlAttribute)RecreateObject( description.Name ) ?? parent.Document.Document.CreateAttribute( description.Name );
                attribute.Value = description.DefaultValue;

                return AddAttribute( parent, attribute, description, true, notify );
            }
        }


        internal IDataAttribute AddAttribute(
            XmlDataObject parent, XmlAttribute attribute, XmlSchemaAttribute description, bool inTransaction,
            bool notify )
        {
            lock( this )
            {
                InternalAddAttribute( parent.Element, attribute, -1, true );

                IDataAttribute newAttribute = InstantiateAttribute( parent, attribute, description, true );

                if( notify )
                    AddAddedAttributeToChangeNotifier( attribute );

                return newAttribute;
            }
        }


        internal IDataAttribute GetAttribute( XmlDataObject parent, XmlSchemaAttribute description )
        {
            lock( this )
            {
                var attrib = parent.Element.Attributes[description.Name];

                XmlDataAttribute attribute;
                if( _dataCache.TryGetAttribute( attrib, out attribute ) )
                    return attribute;

                return InstantiateAttribute( parent, attrib, description, false );
            }
        }


        internal IDataAttribute GetAttribute( XmlAttribute attribute )
        {
            lock( this )
            {
                XmlDataAttribute dataAttribute;

                if( _dataCache.TryGetAttribute( attribute, out dataAttribute ) )
                    return dataAttribute;

                var parent = (XmlDataObject)GetDataObject( attribute.OwnerElement );

                var name = attribute.Name;
                foreach( var a in parent.SchemaAttributes.OfType<XmlSchemaAttribute>() )
                {
                    if( a.Name == name )
                        return GetAttribute( parent, a );
                }
                throw new Exception( "Invalid attribute." );
            }
        }


        internal void SetAttributeValue( XmlAttribute attribute, string value, bool notify )
        {
            lock( this )
            {
                if( InternalModifyAttribute( attribute, value, true ) && notify )
                    _changeNotifier.AddAttribute( attribute, EDataChangeType.Modified );
            }
        }


        private XmlDataAttribute InstantiateAttribute(
            XmlDataObject parent, XmlAttribute attrib, XmlSchemaAttribute description, bool initialize )
        {
            var attribute = InstantiateSimpleAttribute( parent, attrib, description, initialize );

            attribute.DataFactoryInfo = GetDataFactoryInfo( attribute );
            _dataCache.AddAttribute( attribute.Attribute, attribute );

            return attribute;
        }


        private static XmlDataAttribute InstantiateSimpleAttribute(
            XmlDataObject parent, XmlAttribute attrib, XmlSchemaAttribute description, bool initialize )
        {
            // Try to create the attribute. If not possible, just create the default attribute type.
            // Do not try to instantiate the base class.

            var type = description.AttributeSchemaType;
            return InstantiateBaseType( type, parent, attrib, description, initialize );
        }


        private static XmlDataAttribute InstantiateBaseType(
            XmlSchemaSimpleType type, XmlDataObject parent, XmlAttribute attrib, XmlSchemaAttribute description,
            bool initialize )
        {
            //TODO: Implement a more generic solution.
            var typeCode = type.Name.EndsWith( ".GuidType" ) ? "Guid" : type.Datatype.TypeCode.ToString();

            var dataType = "Scotec.XMLDatabase.DAL.DataTypes." + typeCode;

            return new XmlDataAttribute( parent, attrib, dataType, description, initialize );
        }


        private ITransactionParticle GetParticle( Type type, EParticleMode mode )
        {
            var trxHandler = Document.TransactionHandler;
            ITransaction transaction = null;

            if( trxHandler == null )
            {
                if( Document.UseDefaultTransactionHandler )
                    transaction = TransactionManager.RunningTransaction;
            }
            else
                transaction = trxHandler.RunningTransaction;

            if( transaction != null )
                return transaction.CreateParticle( type, mode );

            return null;
        }


        internal void InternalAddAttribute( XmlElement parent, XmlAttribute attribute, int index, bool inTransaction )
        {
            lock( this )
            {
                using( var particle = inTransaction
                    ? GetParticle(
                                  typeof( XmlTransactionAttributeAdded ),
                        EParticleMode.Write )
                    : null )
                {
                    parent.Attributes.Append( attribute );

                    if( particle != null )
                    {
                        var p = (XmlTransactionAttributeAdded)particle;
                        p.Document = Document;
                        p.Parent = parent;
                        p.Attribute = attribute;
                    }
                    Document.OnXmlNodeChanged( parent );
                }
            }
        }


        internal void InternalRemoveAttribute( XmlElement parent, XmlAttribute attribute, bool inTransaction )
        {
            lock( this )
            {
                // Add the attribute to the transaction handler.
                var index = 0;
                var previousAttribute = (XmlAttribute)attribute.PreviousSibling;
                while( previousAttribute != null )
                {
                    previousAttribute = (XmlAttribute)previousAttribute.PreviousSibling;
                    index++;
                }

                using( var particle = inTransaction
                    ? GetParticle(
                                  typeof( XmlTransactionAttributeDeleted ),
                        EParticleMode.Write )
                    : null )
                {
                    if( particle != null )
                    {
                        var p = (XmlTransactionAttributeDeleted)particle;
                        p.Document = Document;
                        //p.Parent = (XmlElement)attribute.ParentNode;
                        p.Parent = parent;
                        p.Attribute = attribute;
                        //p.Description = description;
                        p.Index = index;
                    }
                    // Delete the attribute
                    parent.Attributes.Remove( attribute );
                    RemoveAttributeFromCache( attribute );
                    Document.OnXmlNodeChanged( parent );
                }
            }
        }


        internal bool InternalModifyAttribute( XmlAttribute attribute, string value, bool inTransaction )
        {
            lock( this )
            {
                if( attribute.Value == value )
                    return false; // Nothing to do.

                using( var particle = inTransaction
                    ? GetParticle( typeof( XmlTransactionAttributeModified ), EParticleMode.Write )
                    : null )
                {
                    if( particle != null )
                    {
                        var p = (XmlTransactionAttributeModified)particle;
                        p.Document = Document;
                        p.Attribute = attribute;
                        p.OldValue = attribute.Value;
                    }
                    attribute.Value = value;
                    Document.OnXmlNodeChanged( attribute );
                }
            }

            return true;
        }


        internal void InternalRemoveElement( XmlElement parent, XmlElement element, bool inTransaction )
        {
            lock( this )
            {
                var index = 0;
                var previousElement = (XmlElement)element.PreviousSibling;
                while( previousElement != null )
                {
                    previousElement = (XmlElement)previousElement.PreviousSibling;
                    index++;
                }

                using( var particle = inTransaction
                    ? GetParticle( typeof( XmlTransactionElementDeleted ), EParticleMode.Write )
                    : null )
                {
                    if( particle != null )
                    {
                        var p = (XmlTransactionElementDeleted)particle;
                        p.Document = Document;
                        p.Parent = parent;
                        p.Element = element;
                        p.Position = index;
                    }
                    // Remove the element from its parent
                    parent.RemoveChild( element );

                    Document.OnXmlNodeChanged( parent );
                }


                // Cleanup references.
                CleanupReferences( element );

                // Remove the element from the cache.
                RemoveElementFromCache( element );
            }
        }


        internal void InternalAddElement( XmlElement parent, XmlElement element, int index, bool inTransaction )
        {
            lock( this )
            {
                using( var particle = inTransaction
                    ? GetParticle( typeof( XmlTransactionElementCreated ), EParticleMode.Write )
                    : null )
                {
                    if( particle != null )
                    {
                        var p = (XmlTransactionElementCreated)particle;
                        p.Document = Document;
                        p.Parent = parent;
                        p.Element = element;
                    }
                    if( index < 0 || index >= parent.ChildNodes.Count )
                        parent.AppendChild( element );
                    else
                        parent.InsertBefore( element, parent.ChildNodes.Item( index ) );
                    Document.OnXmlNodeChanged( parent );
                }
            }
        }


        internal void InternalMoveElement( XmlElement parent, int from, int to, bool inTransaction )
        {
            lock( this )
            {
                if( from == to )
                    return; // Just do nothing.

                using( var particle = inTransaction
                    ? GetParticle(typeof( XmlTransactionElementMoved ), EParticleMode.Write )
                    : null )
                {
                    if( particle != null )
                    {
                        var p = (XmlTransactionElementMoved)particle;
                        p.Document = Document;
                        p.Parent = parent;
                        p.From = from;
                        p.To = to;
                    }
                    var movedElement = (XmlElement)parent.ChildNodes[from];
                    parent.RemoveChild( movedElement );

                    if( to >= parent.ChildNodes.Count )
                        parent.AppendChild( movedElement );
                    else
                        parent.InsertBefore( movedElement, parent.ChildNodes[to] );

                    Document.OnXmlNodeChanged( parent );
                }
            }
        }


        internal void AddAddedAttributeToChangeNotifier( XmlAttribute attribute )
        {
            lock( this )
            {
                _changeNotifier.AddAttribute( attribute, EDataChangeType.Added );
            }
        }


        internal void AddModifiedAttributeToChangeNotifier( XmlAttribute attribute )
        {
            lock( this )
            {
                _changeNotifier.AddAttribute( attribute, EDataChangeType.Modified );
            }
        }


        internal void AddRemovedAttributeToChangeNotifier( XmlAttribute attribute )
        {
            lock( this )
            {
                _changeNotifier.AddAttribute( attribute, EDataChangeType.Deleted );
            }
        }


        internal void AddAddedElementToChangeNotifier( XmlElement element )
        {
            lock( this )
            {
                _changeNotifier.AddElement( element, EDataChangeType.Added );

                var parent = element.ParentNode as XmlElement;
                if( parent != null )
                {
                    var type = parent.Attributes["xsi:type"].Value;
                    if( type.EndsWith( "ListType" ) || type.EndsWith( "RefListType" ) )
                        AddModifiedElementToChangeNotifier( parent );
                }
            }
        }


        internal void AddModifiedElementToChangeNotifier( XmlElement element )
        {
            lock( this )
            {
                _changeNotifier.AddElement( element, EDataChangeType.Modified );
            }
        }


        internal void AddRemovingElementToChangeNotifier( XmlElement element )
        {
            lock( this )
            {
                _changeNotifier.AddElement( element, EDataChangeType.Deleting );
            }
        }


        internal void AddRemovedElementToChangeNotifier( XmlElement element )
        {
            lock( this )
            {
                _changeNotifier.AddElement( element, EDataChangeType.Deleted );
            }
        }


        internal void AddRecreateObject( string type, XmlNode node )
        {
            _recreator.AddNode( type, node );
        }


        internal XmlNode RecreateObject( string type )
        {
            return _recreator.RecreateNode( type );
        }


        internal IDataObject GetDataObjectById( Guid id )
        {
            lock( this )
            {
                IDataObject dataObject = _dataCache.GetDataObjectById( id );
                if( dataObject == null )
                {
                    var query = "//*[@id='" + Utils.IdFromGuid( id ) + "'][position()=1]";

                    var root = (IDataQuery)(Document as IDataSession).Root;
                    var resultSet = root.Execute( query );
                    if( resultSet.Count == 0 )
                        throw new Exception( "Invalid reference. Identifier not found." );
                    if( resultSet.Count > 1 )
                        throw new Exception( "Invalid reference. Multiple identifiers found." );

                    dataObject = resultSet[0]; // Use the first element. There con be one only.
                }
                return dataObject;
            }
        }
    }
}