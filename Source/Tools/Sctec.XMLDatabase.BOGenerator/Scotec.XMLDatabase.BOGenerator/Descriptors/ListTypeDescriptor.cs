#region

using System.Xml.Schema;

#endregion


namespace Scotec.XMLDatabase.BOGenerator.Descriptors
{
    public class ListTypeDescriptor : ComplexTypeDescriptor
    {
        private ObjectTypeDescriptor _itemType;


        public ListTypeDescriptor( XmlSchemaType schemaType, ProjectDescriptor projectDescriptor )
                : base( schemaType, projectDescriptor )
        {
        }


        public ObjectTypeDescriptor ItemType
        {
            get
            {
                if( _itemType == null )
                {
                    var complex = SchemaType as XmlSchemaComplexType;
                    var sequence = complex.ContentTypeParticle as XmlSchemaSequence;
                    var complexItem = (XmlSchemaComplexType)((XmlSchemaElement)sequence.Items[0]).ElementSchemaType;

                    var baseType = complexItem.Name;
                    var baseTypeName = BuildFullInterfaceTypeName( baseType );
                    _itemType = (ObjectTypeDescriptor)ProjectDescriptor.DomainDescriptor.FindDescriptor( baseTypeName );
                }
                return _itemType;
            }
        }
    }
}
