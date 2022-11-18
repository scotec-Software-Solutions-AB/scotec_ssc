#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Schema;

#endregion


namespace Scotec.XMLDatabase.BOGenerator
{
    [Flags]
    internal enum ObjectType
    {
        Required = 1,
        Optional = 2,
        Choice = 4
    }

    internal class Utility
    {
        private static readonly Regex s_indentRegex = new Regex( "^", RegexOptions.Compiled | RegexOptions.Multiline );


        internal static SortedList<string, XmlSchemaElement> FindObjects( XmlSchemaType type, ObjectType which )
        {
            var list = new SortedList<string, XmlSchemaElement>();

            var complex = (XmlSchemaComplexType)type;
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

            if( (which & (ObjectType.Optional | ObjectType.Required)) != 0 )
            {
                if( particle is XmlSchemaSequence )
                {
                    var group = (XmlSchemaGroupBase)particle;

                    foreach( XmlSchemaElement se in group.Items )
                    {
                        if( se.MinOccurs == 0 && (which & ObjectType.Optional) != 0
                            || se.MinOccurs > 0 && (which & ObjectType.Required) != 0
                                )
                            list.Add( se.Name, se );
                    }
                }
            }
            if( (which & ObjectType.Choice) != 0 )
            {
                if( particle is XmlSchemaChoice )
                {
                    var group = (XmlSchemaGroupBase)particle;

                    foreach( XmlSchemaElement se in group.Items )
                        list.Add( se.Name, se );
                }
            }

            return list;
        }


        internal static void AnalyzeSchemaElement(
                XmlSchemaElement se, out string typeName, out string name, out string fieldName,
                out bool isRefType )
        {
            typeName = se.SchemaTypeName.Name;
            if( typeName.EndsWith( "RefType" ) )
            {
                typeName = typeName.Substring( 0, typeName.Length - "RefType".Length );
                isRefType = true;
            }
            else
            {
                typeName = typeName.Substring( 0, typeName.Length - "Type".Length );
                isRefType = false;
            }
            var index = typeName.LastIndexOf( '.' );
            typeName = typeName.Substring( 0, index + 1 ) + "I" + typeName.Substring( index + 1 );
            fieldName = se.Name;
            name = se.Name.Substring( 0, 1 ).ToUpper() + se.Name.Substring( 1 );
        }


        internal static void Emit( TextWriter to, string format, int indentationLevel, params object[] parameters )
        {
            using( var writer = new StringWriter() )
            {
                writer.Write( format, parameters );
                var result = writer.ToString();
                var indentation = new String( '\t', indentationLevel );
                result = s_indentRegex.Replace( result, indentation );
                to.Write( result + "\r\n" );
            }
        }
    }
}
