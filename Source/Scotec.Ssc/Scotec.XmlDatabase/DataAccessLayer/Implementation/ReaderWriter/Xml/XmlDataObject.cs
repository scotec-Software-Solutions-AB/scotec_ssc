#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using Scotec.XMLDatabase.Attributes;
using Scotec.XMLDatabase.DAL.DataTypes;
using Scotec.XMLDatabase.ReaderWriter.Xml.Attributes;
using Guid = System.Guid;

#endregion


namespace Scotec.XMLDatabase.ReaderWriter.Xml
{
    internal class XmlDataObject : XmlBaseObject, IDataObject, IDataQuery

    {
        private readonly IDictionary<string, XPathExpression> _queries = new Dictionary<string, XPathExpression>();
        private XmlSchemaObjectCollection _attributeDescriptions;
        private XmlDataDocument _dataDocument;
        private XmlSchemaObjectCollection _elementDescriptions;
        private XPathNavigator _navigator;
        private XmlDataObjectType _objectType = XmlDataObjectType.Empty;
        private XmlSchemaType _schemaType;
        private XmlElement _xmlElement;


        #region Contructor and Initialization

        /// <summary>
        ///   This method is used to initialize existing elements.
        /// </summary>
        /// <param name = "document"></param>
        /// <param name = "element"></param>
        /// <param name="create"></param>
        public virtual void Initialize( XmlDataDocument document, XmlElement element, bool create )
        {
            _dataDocument = document;
            _xmlElement = element;

            InitializeSchema();

            if( create )
                Create();
            else
                Document.DataObjectFactory.AddElementToCache( this );
        }


        public virtual void OnInitialize()
        {
        }


        /// <summary>
        ///   This method is used to initialize a new element and to add XML node into the XML document.
        /// </summary>
        /// <param name = "document"></param>
        /// <param name = "element"></param>
        /// <param name="typeName"></param>
        /// <param name="parent"></param>
        /// <param name="attributes"></param>
        public override void Initialize(
                XmlDataDocument document, XmlElement element,
                XmlQualifiedName typeName, XmlNode parent, ListDictionary attributes )
        {
            _dataDocument = document;
            Create( element, typeName, parent, attributes );
        }

        #endregion Contructor


        #region IDataQuery Members

        /// <summary>
        ///   Executes a XPath query and returns a result set containing all found data objects and attributes.
        /// </summary>
        /// <param name = "query"></param>
        /// <returns></returns>
        public IDataSet Execute( string query )
        {
            var it = Select( query );
            var list = new List<IDataObject>();
            while( it.MoveNext() )
            {
                var node = (XmlNode)it.Current.UnderlyingObject;
                if( node is XmlElement )
                    list.Add( Document.DataObjectFactory.GetDataObject( (XmlElement)node ) );
                else if( node is XmlAttribute )
                    list.Add( (IDataObject)Document.DataObjectFactory.GetAttribute( (XmlAttribute)node ) );
            }

            return new XmlDataSet( list );
        }

        #endregion


        internal XPathNodeIterator Select( string query )
        {
            if( _navigator == null )
                _navigator = Element.CreateNavigator();

            XPathExpression expression;
            if( _queries.TryGetValue( query, out expression ) == false )
            {
                expression = _navigator.Compile( query) ;
                expression.SetContext( Document.NamespaceManager as IXmlNamespaceResolver );
                _queries.Add( query, expression );
            }

            return _navigator.Select( expression );
        }


        public IDataSet GetDirectReverseLinks( IList<string> types, EDataSearchType searchType )
        {
            // If the object doesn't have an id, it cannot be referenced.
            // In such case just return an empty set.
            if( !HasAttribute( "id" ) )
                return new XmlDataSet();

            var id = GetAttribute( "id" );
            var query = new StringBuilder();

            query.Append( "//*[(" );

            if( types == null )
                searchType = EDataSearchType.Flat;
            else
            {
                var or = false;
                foreach( var type in types )
                {
                    if( or )
                        query.Append( " or " );
                    or = true;

                    query.Append( "@xsi:type='" );
                    query.Append( type );
                    query.Append( "'" );
                }

                query.Append( ") and (" );
            }

            query.Append( "./" );
            if( searchType == EDataSearchType.Deep )
                query.Append( "/" );
            query.Append( "*[@idRef='" );
            query.Append( Utils.IdFromGuid( (Id)id.Value ) );
            query.Append( "'])]" );

            return Execute( query.ToString() );
        }


        #region IDataObject Implementation

        string IDataObject.Name
        {
            get { return Name; }
        }

        IDataObject IDataObject.Parent
        {
            get { return Parent; }
        }

        bool IDataObject.DataAvailable
        {
            get { return DataAvailable; }
        }

        IDataFactoryInfo IDataObject.DataFactoryInfo
        {
            get { return DataFactoryInfo; }
        }

        IDataSession IDataObject.Session
        {
            get { return _dataDocument; }
        }


        bool IDataObject.IsAttribute( string name )
        {
            return IsAttribute( name );
        }


        bool IDataObject.HasAttribute( string name )
        {
            return HasAttribute( name );
        }


        IDataAttribute IDataObject.GetAttribute( string name )
        {
            return GetAttribute( name );
        }


        void IDataObject.DeleteAttribute( string name )
        {
            DeleteAttribute( name );
        }


        void IDataObject.SetReference( string name, IDataObject reference )
        {
            SetReference( name, reference );
        }


        IDataObject IDataObject.GetReference( string name )
        {
            return GetReference( name );
        }


        IDataAttribute IDataObject.CreateAttribute( string name )
        {
            return CreateAttribute( name, true );
        }


        bool IDataObject.IsDataObject( string name )
        {
            return IsElement( name );
        }


        bool IDataObject.HasDataObject( string name )
        {
            return HasElement( name );
        }


        IDataObject IDataObject.GetDataObject( string name )
        {
            return GetElement( name );
        }


        bool IDataObject.IsSameAs( IDataObject obj )
        {
            return IsSameAs( obj );
        }


        IDataObject IDataObject.CreateDataObject( string name )
        {
            return CreateElement( name );
        }


        IDataObject IDataObject.CreateDataObject( string name, string type )
        {
            return CreateElement( name, type );
        }


        void IDataObject.DeleteDataObject( string name )
        {
            DeleteElement( name );
        }


        //IDataDocument IDataObject.Document
        //{
        //    get { return (IDataDocument)this.Document; }
        //}


        void IDataObject.Reload( bool forceNotification )
        {
            // Xml objects do not need to be reloaded.
            if( forceNotification )
                Document.DataObjectFactory.AddModifiedElementToChangeNotifier( Element );
        }

        #endregion Interface Implementation


        #region Property Implementation

        public XmlDataDocument Document
        {
            get { return _dataDocument; }
        }

        public XmlElement Element
        {
            get { return _xmlElement; }
        }

        public string Name
        {
            get { return Element.Name; }
        }

        public IDataObject Parent
        {
            get
            {
                var element = Element.ParentNode as XmlElement;
                while( element != null )
                {
                    // Only data objects can be parents of other data objects.
                    // Lists and reference list are intermediate objects only.
                    var type = element.Attributes["xsi:type"].Value;
                    if( type.EndsWith( "ListType" ) || type.EndsWith( "RefListType" ) )
                        element = (XmlElement)element.ParentNode;
                    else
                        break;
                }

                if( element == null )
                    return null;

                return Document.DataObjectFactory.GetDataObject( element );
            }
        }

        public XmlSchemaType SchemaType
        {
            get
            {
                if( _schemaType == null )
                {
                    var type = Element.Attributes["xsi:type"].Value;
                    //_schemaType = Document.GetSchemaType( new XmlQualifiedName( type, Element.NamespaceURI ) );
                    _schemaType = Document.GetSchemaType(new XmlQualifiedName(type, Document.NamespaceManager.DefaultNamespace));
                }
                return _schemaType;
            }
        }

        public XmlSchemaObjectCollection SchemaAttributes
        {
            get {
                return _attributeDescriptions
                       ??
                       (_attributeDescriptions = XmlAttributeDescriptionProvider.GetAttributeDescription( SchemaType ));
            }
        }

        public XmlSchemaObjectCollection ElementDescriptions
        {
            get { return _elementDescriptions; }
        }

        public XmlDataObjectType ObjectType
        {
            get { return _objectType; }
        }

        public IDataFactoryInfo DataFactoryInfo { get; set; }

        #endregion Property Implementation


        #region Attribute implementation

        public bool IsAttribute( string name )
        {
            return SchemaAttributes.Cast<XmlSchemaAttribute>().Any( a => a.Name == name );
        }


        public bool HasAttribute( string name )
        {
            return (Element.Attributes.GetNamedItem( name ) != null);
        }


        public IDataAttribute GetAttribute( string name )
        {
            return GetAttributeInternal( name );
        }


        private IDataAttribute CreateAttribute( string name, bool notify )
        {
            //Check if the attribute name is valid and if the attribute already exists,
            if( HasAttribute( name ) )
                throw new Exception( "Attribute " + name + " already exists." );

            // Find the description and create the new attribute.
            foreach( XmlSchemaAttribute a  in SchemaAttributes )
            {
                if( a.Name == name )
                    return CreateAttribute( name, a, notify );
            }
            throw new Exception( "Creating attribute " + name + "failed." );
        }


        private IDataAttribute CreateAttribute( string name, XmlSchemaAttribute schemaAttribute, bool notify )
        {
            //Check if the attribute name is valid and if the attribute already exists,
            if( HasAttribute( name ) )
                throw new Exception( "Attribute " + name + " already exists." );

            // Find the description and create the new attribute.
            foreach( var a in from XmlSchemaAttribute a in SchemaAttributes where a.Name == name select a )
            {
                return Document.DataObjectFactory.CreateAttribute( this, a, notify );
            }

            throw new Exception( "Creating attribute " + name + "failed." );
        }


        public void DeleteAttribute( string name )
        {
#if DEBUG
            if( !IsAttribute( name ) )
                throw new Exception( name + " is not a valid attribute name." );
#endif

            if( HasAttribute( name ) == false )
                return;

            var attribute = GetAttributeInternal( name ) as XmlDataAttribute;

            var description = attribute.Description;
            if( description.Use == XmlSchemaUse.Required || (description.DefaultValue != null) )
                throw new Exception( "Cannot remove required attribute." );

            var xmlAttribute = attribute.Attribute;
            Document.DataObjectFactory.RemoveAttribute( Element, xmlAttribute );
        }

        #endregion Attribute implementation


        #region Element Implementation

        protected bool DataAvailable
        {
            get
            {
                XmlNode document = Element.OwnerDocument;
                var parent = Element.ParentNode;
                while( parent != null && parent != document )
                    parent = parent.ParentNode;

                return (parent != null);
            }
        }

        protected string DataType
        {
            get { return Element.GetAttribute( "xsi:type" ); }
        }


        public override bool Equals( object obj )
        {
            // obj is null, thus cannot be equal.
            if( obj == null )
                return false;

            // obj is not a IDataObject, thus cannot be equal.
            if( obj is IDataObject == false )
                return false;

            // Check if the references refer to the same object.
            if( base.Equals( obj ) )
                return true;

            // Compare the content.
            return IsSameAs( obj as IDataObject );
        }


        public override int GetHashCode()
        {
            if( _xmlElement == null )
                throw new Exception( "Data object does not contain data. Cannot generate hash value." );

            return _xmlElement.GetHashCode();
        }


        /// <summary>
        ///   This method is called from the contructor or the Create() methods.
        ///   Could be overwritten by derived classes for better schama data handling.
        ///   There could be static descriptions in derived classes.
        ///   See also property "AttributeDescriptions" and "ElementDescription"
        /// </summary>
        protected virtual void InitializeSchema()
        {
            if( _elementDescriptions == null )
            {
                XmlElementDescriptionProvider.GetElementDescription( SchemaType, out _elementDescriptions,
                                                                     out _objectType );
            }
        }


        protected void Create()
        {
            OnCreate();
            Document.DataObjectFactory.AddElementToCache( this );
            Document.DataObjectFactory.AddAddedElementToChangeNotifier( Element );
        }


        protected virtual void OnCreate()
        {
            // Create all mandatory attributes and elements.
            CreateAttributes();
            CreateElements();
        }


        protected void CreateAttributes()
        {
            OnCreateAttributes();
        }


        protected void CreateElements()
        {
            OnCreateElements();
        }


        /// <summary>
        ///   Called from the parent when a element is created initially.
        /// </summary>
        protected void Create(
                XmlElement element, XmlQualifiedName typeName, XmlNode parent, ListDictionary attributes )
        {
            var type = Document.GetSchemaType( typeName );

            _xmlElement = element;

            if( type.GetType().Name == "XmlSchemaComplexType" )
            {
                // The following attributes may be needed for internal initialization.
                // Thus they are not created via CreateAttribute.

                // Create the schemaType attribute.
                if( attributes != null )
                {
                    foreach( DictionaryEntry de in attributes )
                    {
                        var attrib = Document.Document.CreateAttribute( (string)de.Key /*, typeName.Namespace*/ );

                        attrib.Value = (string)de.Value;
                        Element.Attributes.SetNamedItem( attrib );
                    }
                }

                // Create the schemaType attribute
                var schemaTypeAttrib = Document.Document.CreateAttribute( "xsi:type",
                                                                          Document.NamespaceManager.LookupNamespace(
                                                                                  "xsi" ) );
                schemaTypeAttrib.Value = typeName.Name;
                Element.Attributes.SetNamedItem( schemaTypeAttrib );

                // Initialize with schema data
                InitializeSchema();

                // Finally create all mandatory attributes and elements.
                Create();
            }
        }


        protected void OnCreateAttributes()
        {
            foreach( XmlSchemaAttribute a in SchemaAttributes )
            {
                // Create mandatory attributes only
                if( a.Use == XmlSchemaUse.Required || (a.DefaultValue != null) )
                {
                    if( !HasAttribute( a.Name ) )
                        CreateAttribute( a.Name, a, false );
                }
            }
        }


        protected IDataAttribute GetAttributeInternal( string name )
        {
            var attrib = (XmlAttribute)Element.Attributes.GetNamedItem( name );
            if( attrib != null )
                return Document.DataObjectFactory.GetAttribute( attrib );

            throw new Exception( "The attribute " + name + " has not been created." );
        }


        protected void OnCreateElements()
        {
            if( ObjectType == XmlDataObjectType.Sequence )
            {
                foreach( XmlSchemaElement e in ElementDescriptions )
                {
                    // if max == 1, min can be 0 or 1
                    // if max > 1, min must be 0. It is a list.
                    if( e.MinOccurs > 0 )
                    {
                        // Get the schema type name. Then get the type description and check if it is abstract.
                        // Abstract types must be optional.
                        var type = Document.GetSchemaType( e.SchemaTypeName ) as XmlSchemaComplexType;

                        // Required elements cannot be abstract classe because 
                        // the loader cannot now the appropriate derived type at runtime.
                        if( type.IsAbstract )
                            throw new Exception( "Required type cannot be abstract." );

                        Document.DataObjectFactory.CreateDataObject( Document, e, null, this, null, null );
                    }
                }
            }
            else if( ObjectType == XmlDataObjectType.Choice )
            {
                // If there is at least one element with min==0, do not create any.
                // Otherwise create the first element.
                var needsElement = ElementDescriptions.Cast<XmlSchemaElement>().All( e => e.MinOccurs != 0 );
                if( needsElement )
                {
                    // Find the first non abstract class
                    foreach( XmlSchemaElement e in ElementDescriptions )
                    {
                        if( !((XmlSchemaComplexType)e.ElementSchemaType).IsAbstract )
                        {
                            Document.DataObjectFactory.CreateDataObject( Document, e, null, this, null, null );
                            return;
                        }
                    }
                    // This point must never be reached.
                    throw new Exception( "The choice must contain atleast one non abstract element." );
                }
            }
        }


        protected bool IsElement( string name )
        {
            var col = ElementDescriptions;
            return col.Cast<XmlSchemaElement>().Any( e => e.Name == name );
        }


        protected bool HasElement( string name )
        {
            var it = Select( string.Format("dm:{0}", name ));
            if( it.Count != 0 )
                return true;

#if DEBUG
            if( !IsElement( name ) )
                throw new Exception( name + " is not an element." );
#endif

            return false;
        }


        protected IDataObject GetElement( string name )
        {
            return Document.DataObjectFactory.GetDataObject( name, this );
        }


        protected virtual IDataObject CreateElement( string name )
        {
            return CreateElement( name, null );
        }


        protected virtual IDataObject CreateElement( string name, string type )
        {
            // The object can be created only once.
            if( HasElement( name ) )
                throw new Exception( "Data object \"" + name + "\" can be created only once." );

            // The element does not exists
            // Check if this object is a sequence or a choice. If it is a sequence we can just
            // insert the new element. If it is a choice, we need to remove all child objects.
            if( ObjectType == XmlDataObjectType.Sequence )
            {
                var position = 0;
                var insertAfter = new string[ElementDescriptions.Count];
                foreach( XmlSchemaElement e in ElementDescriptions )
                {
                    if( e.Name == name )
                    {
                        XmlSchemaComplexType derivedType;
                        var complex = (e.ElementSchemaType as XmlSchemaComplexType);
                        if( type == null )
                        {
                            if( complex.IsAbstract )
                                throw new Exception( "Cannot instantiate abstract type." );
                        }
                        else
                        {
                            if( complex.Name == type )
                            {
                                // Base class shall be instantiated
                                if( complex.IsAbstract )
                                    throw new Exception( "Cannot instantiate abstract type." );
                            }
                            else
                            {
                                // A derived class shall be instantiated.
                                // Get the element type and check if it is really derived from the base.
                                derivedType =
                                        (XmlSchemaComplexType)
                                        Document.GetSchemaType( new XmlQualifiedName( type, Element.NamespaceURI ) );
                                while( derivedType != null )
                                {
                                    if( derivedType.QualifiedName == e.SchemaTypeName )
                                        break;

                                    derivedType = (XmlSchemaComplexType)derivedType.BaseXmlSchemaType;
                                }
                                if( derivedType == null )
                                    throw new Exception( "Type is not derived from base type" );
                            }
                        }

                        return Document.DataObjectFactory.CreateDataObject( Document, e, type, this, null, insertAfter );
                    }
                    insertAfter[position] = e.Name;
                    position++;
                }
            }
            else if( _objectType == XmlDataObjectType.Choice )
            {
                // Remove the child elements. There should be one only.
                // However, we do this in a loop.
                foreach( XmlElement e in Element.ChildNodes )
                    Document.DataObjectFactory.RemoveElement( Element, e );
                foreach( XmlSchemaElement e in ElementDescriptions )
                {
                    if( e.Name == name )
                    {
                        if( (e.ElementSchemaType as XmlSchemaComplexType).IsAbstract )
                            throw new Exception( "Cannot instantiate abstract type." );

                        return Document.DataObjectFactory.CreateDataObject( Document, e, type, this, null, null );
                    }
                }
            }
            return null;
        }


        protected void DeleteElement( string name )
        {
            var canRemove = false;

            if( _objectType == XmlDataObjectType.Sequence )
            {
                foreach( XmlSchemaElement e in from XmlSchemaElement e in ElementDescriptions where e.Name == name select e )
                {
                    if( e.MinOccurs > 0 )
                        throw new Exception( "Date object cannot be deleted because it is a mandatory object." );
                    
                    canRemove = true;
                    break;
                }
            }
            else if( _objectType == XmlDataObjectType.Choice )
            {
                // If there is at least one element with min==0, the element can be deleted.
                if( ElementDescriptions.Cast<XmlSchemaElement>().Any( e => e.MinOccurs == 0 ) )
                {
                    canRemove = true;
                }
            }

            if( canRemove )
            {
                var it = Select(string.Format("dm:{0}", name));
                XmlElement element = null;
                // (XmlElement)Element.SelectSingleNode(/*"systecs:" +*/ name, Document.NamespaceManager);
                if( it.MoveNext() )
                    element = it.Current.UnderlyingObject as XmlElement;

                if( element != null )
                    Document.DataObjectFactory.RemoveElement( Element, element );
            }
        }


        protected void SetReference( string name, IDataObject reference )
        {
            // Get the reference object
            var referenceObject = GetElement( name );

            if( referenceObject == reference )
                return; // Reference already set. Nothing to do.

            IDataAttribute idrefAttribute;
            if( referenceObject.HasAttribute( "idRef" ) )
                idrefAttribute = referenceObject.GetAttribute( "idRef" );
            else
            {
                if( reference == null )
                    return;
                idrefAttribute = referenceObject.CreateAttribute( "idRef" );
            }


            if( reference != null )
            {
                // Get the objects identifier.
                var idAttribute = reference.GetAttribute( "id" );
                // Set the reference
                idrefAttribute.Value = (Idref)(Guid)(Id)idAttribute.Value;
            }
            else
                referenceObject.DeleteAttribute( "idRef" );
        }


        protected IDataObject GetReference( string name )
        {
            // Get the reference object
            var referenceObject = GetElement( name );

            if( !referenceObject.HasAttribute( "idRef" ) )
                return null; // Object contains a null reference.

            var idrefAttribute = referenceObject.GetAttribute( "idRef" );
            Guid id = (Idref)idrefAttribute.Value;

            return Document.DataObjectFactory.GetDataObjectById( id );
        }


        protected bool IsSameAs( IDataObject obj )
        {
            if( !(obj is XmlDataObject) )
                return false;

            return (Element == ((XmlDataObject)obj).Element);
        }

        #endregion Element Implementation


        #region IDataQuery Member

        #endregion
    }
}
