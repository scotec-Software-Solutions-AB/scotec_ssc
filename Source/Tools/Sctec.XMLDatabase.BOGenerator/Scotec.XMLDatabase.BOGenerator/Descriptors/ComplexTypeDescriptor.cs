#region

using System.Xml.Schema;

#endregion


namespace Scotec.XMLDatabase.BOGenerator.Descriptors
{
    public class ComplexTypeDescriptor : TypeDescriptor
    {
        private ComplexTypeDescriptor _baseType;


        public ComplexTypeDescriptor( XmlSchemaType schemaType, ProjectDescriptor projectDescriptor )
                : base( schemaType, projectDescriptor )
        {
            var complex = (XmlSchemaComplexType)SchemaType;
            IsAbstract = complex.IsAbstract;
            IsSealed = (complex.Final == XmlSchemaDerivationMethod.All);
        }


        public bool IsAbstract { get; private set; }

        public bool IsSealed { get; private set; }

        public ComplexTypeDescriptor BaseType
        {
            get
            {
                if( _baseType == null )
                {
                    var complex = SchemaType as XmlSchemaComplexType;

                    if( complex.ContentModel != null )
                    {
                        var baseType = (complex.ContentModel.Content as XmlSchemaComplexContentExtension).BaseTypeName.Name;

                        var baseTypeName = BuildFullInterfaceTypeName( baseType );
                        _baseType = (ComplexTypeDescriptor)ProjectDescriptor.DomainDescriptor.FindDescriptor( baseTypeName );
                    }
                }

                return _baseType;
            }
        }
    }
}
