#region

using System;
using System.Linq;
using System.Xml;
using System.Xml.Schema;

#endregion


namespace Scotec.XMLDatabase.ReaderWriter.Xml
{
    internal sealed class XmlDataList : XmlDataObject, IDataList
    {
        #region IDataList Members

        public int Count
        {
            get { return Element.ChildNodes.Count; }
        }


        public IDataObject this[ int index ]
        {
            get
            {
                if( index < 0 || index >= Element.ChildNodes.Count )
                    throw new IndexOutOfRangeException( "Index out of range." );

                var element = (XmlElement)Element.ChildNodes[index];

                return Document.DataObjectFactory.GetDataObject( element );
            }
        }


        public int IndexOf( IDataObject dataObject )
        {
            // TODO: Check for type before iterating through the list.
            var element = (dataObject as XmlDataObject).Element;

            var index = Element.ChildNodes.Cast<XmlElement>().TakeWhile( e => element != e ).Count();
            
            if( index >= Element.ChildNodes.Count )
                index = -1;

            return index;
        }


        public IDataObject CreateDataObject()
        {
            return CreateDataObjectAt( Count, null );
        }


        public IDataObject CreateDataObject( string type )
        {
            return CreateDataObjectAt( Count, type );
        }


        public IDataObject CreateDataObjectAt( int index )
        {
            if( index < 0 || index > Element.ChildNodes.Count )
                throw new IndexOutOfRangeException( "Index out of range." );


            return CreateElementAt( index, null );
        }


        public IDataObject CreateDataObjectAt( int index, string type )
        {
            if( index < 0 || index > Element.ChildNodes.Count )
                throw new IndexOutOfRangeException( "Index out of range." );


            return CreateElementAt( index, type );
        }


        public IDataObject Append( IDataObject dataObject )
        {
            throw new NotImplementedException( "Method not implemented!" );
        }


        public IDataObject InsertAt( int index, IDataObject dataObject )
        {
            throw new NotImplementedException( "Method not implemented!" );
        }


        public void Move( int from, int to )
        {
            if( from != to )
            {
                var element = Element;
                Document.DataObjectFactory.MoveElement( element, from, to );
            }
        }


        public void MoveTo( int index, IDataObject dataObject )
        {
            var pos = GetElementPosition( (dataObject as XmlDataObject).Element );
            if( pos < 0 )
                throw new Exception( "Cannot move data object. Object is not a member of the list." );

            Move( pos, index );
        }


        public void MoveAfter( IDataObject sourceObject, IDataObject targetObject )
        {
            var source = sourceObject as XmlDataObject;
            var sourcePos = GetElementPosition( source.Element );
            if( sourcePos < 0 )
                throw new Exception( "Cannot move data object. Source object is not a member of the list." );

            // targetObject might be null. In this case the target index will be 0.
            var target = targetObject as XmlDataObject;
            var targetPos = (target != null) ? GetElementPosition( target.Element ) : 0;
            if( targetPos < 0 )
                throw new Exception( "Cannot move data object. Target object is not a member of the list." );

            if( sourcePos > targetPos )
                targetPos++;

            Move( sourcePos, targetPos );
        }


        public int DeleteAt( int index )
        {
            var dataObject = (XmlDataObject)this[index];

            var element = dataObject.Element;
            Document.DataObjectFactory.RemoveElement( Element, element );

            return Count;
        }


        public void DeleteAll()
        {
            var element = Element;

            var count = element.ChildNodes.Count;

            for( var i = 0; i < count; i++ )
            {
                var toRemove = (XmlElement)element.ChildNodes[0];
                Document.DataObjectFactory.RemoveElement( element, toRemove );
            }
        }


        public bool Contains( IDataObject dataObject )
        {
            return IsSameAs( dataObject.Parent );
        }

        #endregion


        protected override void OnCreate()
        {
            // Create all mandatory attributes and elements.
            CreateAttributes();
        }


        protected override IDataObject CreateElement( string name )
        {
            // Overwrittem n just to avoid conflicts with the base implementation.

            // Create the element at the end.
            return CreateElementAt( Count, null );
        }


        private IDataObject CreateElementAt( int index, string type )
        {
            if( ObjectType != XmlDataObjectType.Sequence )
                throw new Exception( "Invalid list type. List must be a sequence of elements." );

            // In lists can be one description only.
            var schemaElement = (XmlSchemaElement)ElementDescriptions[0];

            XmlSchemaComplexType derivedType;
            var complex = (schemaElement.ElementSchemaType as XmlSchemaComplexType);
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
                        if( derivedType.QualifiedName == schemaElement.SchemaTypeName )
                            break;

                        derivedType = (XmlSchemaComplexType)derivedType.BaseXmlSchemaType;
                    }
                    if( derivedType == null )
                        throw new Exception( "Type is not derived from base type" );
                }
            }

            return Document.DataObjectFactory.CreateDataObject( Document, schemaElement, type, this, null, index );
        }


        private int GetElementPosition( XmlElement element )
        {
            var compareElement = (XmlElement)Element.FirstChild;

            var pos = 0;
            while( compareElement != null && element != compareElement )
            {
                compareElement = (XmlElement)compareElement.NextSibling;
                ++pos;
            }
            return (compareElement != null) ? pos : -1;
        }
    }
}
