#region

using System.Xml.Schema;

#endregion


namespace Scotec.XMLDatabase.BOGenerator.Descriptors
{
    public class RefTypeDescriptor : ComplexTypeDescriptor
    {
        public RefTypeDescriptor( XmlSchemaType schemaType, ProjectDescriptor projectDescriptor )
                : base( schemaType, projectDescriptor )
        {
        }
    }
}
