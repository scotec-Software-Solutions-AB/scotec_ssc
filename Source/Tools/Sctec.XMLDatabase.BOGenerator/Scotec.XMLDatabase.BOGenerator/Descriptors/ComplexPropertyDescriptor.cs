#region

using System.Xml.Schema;

#endregion


namespace Scotec.XMLDatabase.BOGenerator.Descriptors
{
    public class ComplexPropertyDescriptor : PropertyDescriptor
    {
        public ComplexPropertyDescriptor( XmlSchemaElement schemaElement, bool isChoise, ProjectDescriptor projectDescriptor )
                : base( projectDescriptor )
        {
            SchemaElement = schemaElement;

            IsChoise = isChoise;

            ReadComplexProperty();
        }


        public bool IsRefType { get; private set; }

        public bool IsChoise { get; private set; }


        private XmlSchemaElement SchemaElement { get; set; }


        private void ReadComplexProperty()
        {
            string returnType;
            string name;
            string fieldName;
            bool isRefType;

            Utility.AnalyzeSchemaElement( SchemaElement, out returnType, out name, out fieldName, out isRefType );

            Name = name;
            FieldName = fieldName;
            IsRefType = isRefType;

            if( SchemaElement.MinOccurs == 0 )
                IsOptional = true;

            SetReturnType( returnType );
        }
    }
}
