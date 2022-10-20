#region

using System.Collections.Generic;
using System.Xml.Schema;

#endregion


namespace Scotec.XMLDatabase.ReaderWriter.Xml
{
    internal static class XmlElementDescriptionProvider
    {
        private static readonly IDictionary<XmlSchemaType, XmlSchemaObjectCollection> s_elementDescriptions
                = new Dictionary<XmlSchemaType, XmlSchemaObjectCollection>();

        private static readonly IDictionary<XmlSchemaType, XmlDataObjectType> s_objectTypes
                = new Dictionary<XmlSchemaType, XmlDataObjectType>();


        public static void GetElementDescription(
                XmlSchemaType schemaType, out XmlSchemaObjectCollection elementDescription,
                out XmlDataObjectType objectType )
        {
            if( !s_elementDescriptions.TryGetValue( schemaType, out elementDescription ) )
            {
                CreateDescription( schemaType, out elementDescription, out objectType );
                if( elementDescription != null )
                {
                    s_elementDescriptions.Add( schemaType, elementDescription );
                    s_objectTypes.Add( schemaType, objectType );
                }
            }
            else
                objectType = s_objectTypes[schemaType];
        }


        private static void CreateDescription(
                XmlSchemaType schemaType, out XmlSchemaObjectCollection elementDescriptions,
                out XmlDataObjectType objectType )
        {
            elementDescriptions = null;
            objectType = XmlDataObjectType.Empty;

            if( schemaType is XmlSchemaComplexType )
            {
                var complex = (XmlSchemaComplexType)schemaType;
                var particle = complex.ContentTypeParticle;
                // Get the type of particle
                switch( particle.GetType().Name )
                {
                    case "EmptyParticle":
                        objectType = XmlDataObjectType.Empty;
                        break;
                    case "XmlSchemaSequence":
                        objectType = XmlDataObjectType.Sequence;
                        break;
                    case "XmlSchemaChoice":
                        objectType = XmlDataObjectType.Choice;
                        break;
                    default:
                        objectType = XmlDataObjectType.Empty;
                        break;
                }

                if( objectType == XmlDataObjectType.Sequence || objectType == XmlDataObjectType.Choice )
                    elementDescriptions = ((XmlSchemaGroupBase)complex.ContentTypeParticle).Items;
                else // Empty particle
                    elementDescriptions = new XmlSchemaObjectCollection(); // Just set an empty list.
            }
            else
            {
                // Simple type
            }
        }
    }
}
