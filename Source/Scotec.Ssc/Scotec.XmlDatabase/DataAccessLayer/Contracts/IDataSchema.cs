#region

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

#endregion


namespace Scotec.XMLDatabase
{
    public interface IDataSchema
    {
        Uri SchemaLocation { get; }


        /// <summary>
        ///   Loads a schema file.
        /// </summary>
        /// <param name = "schema"></param>
        void LoadSchema( Uri schema );


        /// <summary>
        ///   Called from XML documents to resolve namespaces.
        ///   Sample: 
        ///   XmlDocument xmlDocument = new XmlDocument();
        ///   XmlNamespaceManager nsm = new XmlNamespaceManager(xmlDocument.NameTable);
        ///   DataSchema.GetNamespaces(nsm);
        /// </summary>
        /// <param name = "namespaceManager"></param>
        void GetNamespaces( XmlNamespaceManager namespaceManager );


        /// <summary>
        ///   Returns the schema type for an element specified in typeName.
        /// </summary>
        /// <param name = "typeName"></param>
        /// <returns></returns>
        XmlSchemaType GetSchemaType( XmlQualifiedName typeName );


        IDictionary<string, string> GetSchemaInstructions( XmlSchema schema );


        XmlSchemaObjectCollection GetSchemaTypeAttributes( XmlQualifiedName typeName );
    }
}
