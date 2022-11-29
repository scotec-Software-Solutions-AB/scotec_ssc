#region

using System.Xml.Schema;

#endregion


namespace Scotec.XMLDatabase.BOGenerator.Descriptors
{
    public class SimplePropertyDescriptor : PropertyDescriptor
    {
        public SimplePropertyDescriptor( XmlSchemaAttribute schemaAttribute, ProjectDescriptor projectDescriptor )
                : base( projectDescriptor )
        {
            SchemaAttribute = schemaAttribute;

            ReadSimpleProperty();
        }


        private XmlSchemaAttribute SchemaAttribute { get; set; }


        private void ReadSimpleProperty()
        {
            var name = SchemaAttribute.Name;
            FieldName = name;
            Name = name.Substring( 0, 1 ).ToUpper() + name.Substring( 1 );

            var typeName = SchemaAttribute.SchemaTypeName.Name;
            if( typeName.EndsWith( "Type" ) )
                typeName = typeName.Substring( 0, typeName.Length - "Type".Length );

            var index = typeName.LastIndexOf( '.' );
            SetReturnType( string.Format( "{0}.I{1}", typeName.Substring( 0, index ), typeName.Substring( index + 1 ) ) );

            if( ((SchemaAttribute.Use == XmlSchemaUse.Optional ||
                  SchemaAttribute.Use == XmlSchemaUse.None) &&
                 SchemaAttribute.DefaultValue == null) )
                IsOptional = true;
        }
    }
}
