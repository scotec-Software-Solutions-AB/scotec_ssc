namespace Scotec.XMLDatabase.BOGenerator.Descriptors
{
    public class PropertyDescriptor : Descriptor
    {
        private string _returnType;


        public PropertyDescriptor( ProjectDescriptor projectDescriptor )
        {
            ProjectDescriptor = projectDescriptor;
        }


        public string Name { get; protected set; }


        public string FieldName { get; protected set; }


        public bool IsOptional { get; protected set; }


        public string Namespace
        {
            get { return ProjectDescriptor.DomainDescriptor.FindDescriptor( Name ).Namespace; }
        }


        public TypeDescriptor ReturnType
        {
            get { return ProjectDescriptor.DomainDescriptor.FindDescriptor( _returnType ); }
        }


        protected ProjectDescriptor ProjectDescriptor { get; private set; }


        protected void SetReturnType( string returnType )
        {
            _returnType = returnType;
        }
    }
}
