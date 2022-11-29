#region

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

#endregion


namespace Scotec.XMLDatabase.BOGenerator.Descriptors
{
    public class SimpleTypeDescriptor : TypeDescriptor
    {

        private string TypeCode { get; set; }
        private string FullTypeName { get; set; }

        public SimpleTypeDescriptor( XmlSchemaType schemaType, ProjectDescriptor projectDescriptor )
                : base( schemaType, projectDescriptor )
        {
            ReadConstraints();
        }


        public string ValueType
        {
            get
            {
                if( TypeCode == XmlTypeCode.Id.ToString() || TypeCode == XmlTypeCode.Idref.ToString() )
                    return "System.Guid";

                return FullTypeName;
            }
        }


        public string DataType
        {
            get { return "Scotec.XMLDatabase.DAL.DataTypes." + TypeCode; }
        }


        public IList<string> EnumValues { get; private set; }


        public bool IsEnum
        {
            get { return EnumValues != null; }
        }

        public virtual string EnumType
        {
            get { return "E" + TypeName.Substring( 1 ); }
        }


        private void ReadConstraints()
        {
            //TODO: Make this more generic
            if(SchemaType.Name.EndsWith(".GuidType"))
            {
                FullTypeName = "System.Guid";
                TypeCode = "Guid";
            }
            else
            {
                FullTypeName = SchemaType.Datatype.ValueType.FullName;
                TypeCode = SchemaType.Datatype.TypeCode.ToString();
            }


            
            var simpleType = SchemaType as XmlSchemaSimpleType;
            var content = simpleType.Content;

            if( content is XmlSchemaSimpleTypeRestriction )
            {
                var restriction = (XmlSchemaSimpleTypeRestriction)content;

                foreach( XmlSchemaFacet facet in restriction.Facets )
                {
                    if( facet is XmlSchemaEnumerationFacet )
                    {
                        if( EnumValues == null )
                            EnumValues = new List<string>();

                        EnumValues.Add( facet.Value );
                    }
                }
            }
        }
    }
}
