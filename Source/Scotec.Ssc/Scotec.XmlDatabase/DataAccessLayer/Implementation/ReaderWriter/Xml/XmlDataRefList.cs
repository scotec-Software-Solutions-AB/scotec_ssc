#region

using System;
using System.Xml;
using System.Xml.Schema;
using Scotec.XMLDatabase.DAL.DataTypes;
using Guid = System.Guid;

#endregion


namespace Scotec.XMLDatabase.ReaderWriter.Xml
{
    /// <summary>
    ///   Summary description for XmlDataRefList.
    /// </summary>
    internal sealed class XmlDataRefList : XmlDataObject, IDataRefList
    {
        #region IDataRefList Members

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

                // Get the reference object
                var referenceObject = Document.DataObjectFactory.GetDataObject( element );

                // All objects in a reference list must contain references.
                // On an object contains a null reference, it would be invalid.
                var idrefAttribute = referenceObject.GetAttribute( "idRef" );
                Guid id = (Idref)idrefAttribute.Value;

                // Get the referenced object.
                return Document.DataObjectFactory.GetDataObjectById( id );
            }
        }


        public int IndexOf( IDataObject dataObject )
        {
            // TODO: Check for type before iterating through the list.

            var count = Count;
            for( var i = 0; i < count; i++ )
            {
                if( dataObject.IsSameAs( this[i] ) )
                    return i;
            }
            return -1;
        }


        public int Append( IDataObject dataObject )
        {
            return InsertAt( Count, dataObject );
        }


        public int InsertAt( int index, IDataObject dataObject )
        {
            if( ObjectType != XmlDataObjectType.Sequence )
                throw new Exception( "Invalid list type. List must be a sequence of elements." );

            // In reflists can be one description only.
            var schemaElement = (XmlSchemaElement)ElementDescriptions[0];

            var referenceObject = Document.DataObjectFactory.CreateDataObject( Document, schemaElement, null, this, null,
                                                                               index );
            var idrefAttribute = referenceObject.CreateAttribute( "idRef" );

            // Get the objects identifier.
            var idAttribute = dataObject.GetAttribute( "id" );

            // Set the reference
            idrefAttribute.Value = (Idref)(Guid)(Id)idAttribute.Value;

            return Count;
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
            var pos = IndexOf( dataObject );
            if( pos < 0 )
                throw new Exception( "Cannot move data object. Object is not a member of the list." );

            Move( pos, index );
        }


        public void MoveAfter( IDataObject sourceObject, IDataObject targetObject )
        {
            var sourcePos = IndexOf( sourceObject );
            if( sourcePos < 0 )
                throw new Exception( "Cannot move data object. Source object is not a member of the list." );

            // targetObject might be null. In this case the target index will be 0.
            var target = targetObject as XmlDataObject;
            var targetPos = (target != null) ? IndexOf( targetObject ) : 0;
            if( targetPos < 0 )
                throw new Exception( "Cannot move data object. Target object is not a member of the list." );

            if( sourcePos > targetPos )
                targetPos++;

            Move( sourcePos, targetPos );
        }


        public int RemoveAt( int index )
        {
            if( index < 0 || index >= Element.ChildNodes.Count )
                throw new IndexOutOfRangeException( "Index out of range." );

            var element = (XmlElement)Element.ChildNodes[index];
            Document.DataObjectFactory.RemoveElement( Element, element );

            return Count;
        }


        public void RemoveAll()
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
            var idrefAttribute = dataObject.GetAttribute( "idRef" );
            Guid id = (Idref)idrefAttribute.Value;

            // Get the referenced object.
            var query = ".[@idref='" + Utils.IdFromGuid( id ) + "']";

            var resultSet = (this as IDataQuery).Execute( query );
            return (resultSet.Count > 0);
        }

        #endregion
    }
}
