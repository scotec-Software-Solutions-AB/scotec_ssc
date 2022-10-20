#region

using System.Collections.Generic;
using System.Xml.Schema;

#endregion


namespace Scotec.XMLDatabase.ReaderWriter.Xml
{
    internal static class XmlAttributeDescriptionProvider
    {
        private static readonly IDictionary<XmlSchemaType, XmlSchemaObjectCollection> s_attributeDescriptions
                = new Dictionary<XmlSchemaType, XmlSchemaObjectCollection>();


        public static XmlSchemaObjectCollection GetAttributeDescription( XmlSchemaType schemaType )
        {
            XmlSchemaObjectCollection description;

            if( !s_attributeDescriptions.TryGetValue( schemaType, out description ) )
            {
                description = CreateDescription( schemaType );
                if( description != null )
                    s_attributeDescriptions.Add( schemaType, description );
            }

            return description;
        }


        private static XmlSchemaObjectCollection CreateDescription( XmlSchemaType schemaType )
        {
            XmlSchemaObjectCollection attributeDescriptions = null;

            if( schemaType is XmlSchemaComplexType )
            {
                var complex = (XmlSchemaComplexType)schemaType;

                attributeDescriptions = complex.Attributes;

                if( complex.ContentModel == null ) // Not a derived class
                    complex = null;

                // Get attributes from base classes.
                while( complex != null )
                {
                    XmlSchemaObjectCollection collection = complex.ContentModel != null
                                                                   ? ((XmlSchemaComplexContentExtension)complex.ContentModel.Content).Attributes
                                                                   : complex.Attributes;

                    // If the complex is a derived type, it contains a content model.
                    // Get thew attributes from the content model and from the base classes.
                    foreach( XmlSchemaAttribute a in collection )
                        attributeDescriptions.Add( a );
                    complex = complex.BaseXmlSchemaType as XmlSchemaComplexType;
                }
            }

            return attributeDescriptions;
        }
    }
}
