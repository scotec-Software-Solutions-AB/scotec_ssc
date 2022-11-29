#region

using System.Collections.Generic;
using System.Xml.Schema;

#endregion


namespace Scotec.XMLDatabase.BOGenerator.Descriptors
{
    public class ObjectUnitTypeDescriptor : ObjectTypeDescriptor
    {
        public ObjectUnitTypeDescriptor( XmlSchemaType schemaType, ProjectDescriptor projectDescriptor )
                : base( schemaType, projectDescriptor )
        {
        }

        //protected override string BuildImplementationTypeName(string typeName)
        //{
        //    var index = typeName.LastIndexOf('.');
        //    var type = typeName.Substring(index + 1);

        //    return type.Remove(type.Length - 8, 8); // Remove "UnitType"
        //}

    }
}
