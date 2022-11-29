#region

using System;
using System.Xml.Schema;

#endregion


namespace Scotec.XMLDatabase.BOGenerator.Descriptors
{
    public abstract class TypeDescriptor : Descriptor
    {
        private string _namespace;
        private string _typeName;
        private string _typeNameImplementation;


        protected TypeDescriptor( XmlSchemaType schemaType, ProjectDescriptor projectDescriptor )
        {
            SchemaType = schemaType;

            ProjectDescriptor = projectDescriptor;
        }


        public ProjectDescriptor ProjectDescriptor { get; private set; }

        public string Namespace
        {
            get
            {
                if( string.IsNullOrEmpty( _namespace ) )
                {
                    var index = SchemaType.Name.LastIndexOf( '.' );

                    if( index >= 0 )
                        _namespace = SchemaType.Name.Substring( 0, index );
                    else
                        throw new Exception( "Missing namespace." );
                }

                return _namespace;
            }
        }

        public string TypeName
        {
            get
            {
                if( string.IsNullOrEmpty( _typeName ) )
                    _typeName = BuildInterfaceTypeName( SchemaType.Name );

                return _typeName;
            }
        }

        public string TypeNameImplementation
        {
            get
            {
                if( string.IsNullOrEmpty( _typeNameImplementation ) )
                    _typeNameImplementation = BuildImplementationTypeName( SchemaType.Name );

                return _typeNameImplementation;
            }
        }


        public string FullName
        {
            get { return string.Format( "{0}.{1}", Namespace, TypeName ); }
        }

        public string FullNameImplementation
        {
            get { return BuildFullImplementationTypeName( SchemaType.Name ); }
        }

        protected XmlSchemaType SchemaType { get; private set; }


        protected string BuildInterfaceTypeName( string typeName )
        {
            return "I" + BuildImplementationTypeName( typeName );
        }


        protected string BuildImplementationTypeName( string typeName )
        {
            var index = typeName.LastIndexOf( '.' );
            var type = typeName.Substring( index + 1 );

            return type.Remove( type.Length - 4, 4 ); // Remove "Type"
        }


        protected string BuildFullInterfaceTypeName( string typeName )
        {
            var index = typeName.LastIndexOf( '.' );
            var type = typeName.Substring( index + 1 );

            return typeName.Substring( 0, index ) + ".I" + type.Remove( type.Length - 4, 4 ); // Remove "Type"
        }


        protected string BuildFullImplementationTypeName( string typeName )
        {
            return typeName.Remove( typeName.Length - 4, 4 ); // Remove "Type"
        }
    }
}
