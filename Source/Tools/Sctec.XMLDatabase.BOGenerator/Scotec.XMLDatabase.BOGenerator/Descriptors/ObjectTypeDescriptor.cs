#region

using System.Collections.Generic;
using System.Xml.Schema;

#endregion


namespace Scotec.XMLDatabase.BOGenerator.Descriptors
{
    public class ObjectTypeDescriptor : ComplexTypeDescriptor
    {
        private SortedList<string, PropertyDescriptor> _properties;


        public ObjectTypeDescriptor( XmlSchemaType schemaType, ProjectDescriptor projectDescriptor )
                : base( schemaType, projectDescriptor )
        {
        }


        public SortedList<string, PropertyDescriptor> Properties
        {
            get { return _properties ?? (_properties = ReadProperties()); }
        }


        private SortedList<string, PropertyDescriptor> ReadSimpleProperties()
        {
            var list = new SortedList<string, PropertyDescriptor>();

            var complex = SchemaType as XmlSchemaComplexType;

            if( complex.ContentModel == null )
            {
                // Get the attributes from the attribute collection
                foreach( XmlSchemaAttribute a in complex.Attributes )
                    list.Add( a.Name, new SimplePropertyDescriptor( a, ProjectDescriptor ) );
            }
            else
            {
                // Get the attributes from content model
                foreach( XmlSchemaAttribute a in (complex.ContentModel.Content as XmlSchemaComplexContentExtension).Attributes )
                    list.Add( a.Name, new SimplePropertyDescriptor( a, ProjectDescriptor ) );
            }
            return list;
        }


        private SortedList<string, PropertyDescriptor> ReadProperties()
        {
            var result = ReadSimpleProperties();

            foreach( var p in ReadComplexProperties() )
                result.Add( p.Key, p.Value );

            return result;
        }


        private SortedList<string, PropertyDescriptor> ReadComplexProperties()
        {
            var list = new SortedList<string, PropertyDescriptor>();

            var complex = (XmlSchemaComplexType)SchemaType;
            XmlSchemaParticle particle = null;

            if( complex.ContentModel == null )
                particle = complex.ContentTypeParticle;
            else
            {
                var extension = (XmlSchemaComplexContentExtension)complex.ContentModel.Content;
                if( extension.Particle != null )
                    particle = extension.Particle;
            }

            if( particle == null )
                return list;

            if( particle is XmlSchemaSequence )
            {
                var group = (XmlSchemaGroupBase)particle;

                foreach( XmlSchemaElement se in group.Items )
                    list.Add( se.Name, new ComplexPropertyDescriptor( se, false, ProjectDescriptor ) );
            }
            else if( particle is XmlSchemaChoice )
            {
                var group = (XmlSchemaGroupBase)particle;

                foreach( XmlSchemaElement se in group.Items )
                    list.Add( se.Name, new ComplexPropertyDescriptor( se, true, ProjectDescriptor ) );
            }

            return list;
        }
    }
}
