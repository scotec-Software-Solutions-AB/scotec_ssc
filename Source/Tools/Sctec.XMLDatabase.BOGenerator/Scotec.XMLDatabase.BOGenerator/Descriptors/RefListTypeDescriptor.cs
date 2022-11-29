#region

using System.Xml.Schema;

#endregion


namespace Scotec.XMLDatabase.BOGenerator.Descriptors
{
    public class RefListTypeDescriptor : ComplexTypeDescriptor
    {
        private ObjectTypeDescriptor _itemType;


        public RefListTypeDescriptor( XmlSchemaType schemaType, ProjectDescriptor projectDescriptor )
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
                    var complexRefItem = (XmlSchemaComplexType)((XmlSchemaElement)sequence.Items[0]).ElementSchemaType;

                    var baseType = complexRefItem.Name;
                    var baseTypeName = BuildFullInterfaceTypeName( baseType );
                    baseTypeName = baseTypeName.Remove( baseTypeName.Length - 3, 3 ); // Remove "Ref";
                    _itemType = (ObjectTypeDescriptor)ProjectDescriptor.DomainDescriptor.FindDescriptor( baseTypeName );
                }
                return _itemType;
            }
        }
    }
}
