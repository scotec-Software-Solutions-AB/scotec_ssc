#region

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;

#endregion


namespace Scotec.XMLDatabase.BOGenerator.Descriptors
{
    public class ProjectDescriptor : Descriptor
    {
        public ProjectDescriptor( XmlSchema schema, DomainDescriptor domainDescriptor, GenerateParameters parameters )
        {
            Schema = schema;
            DomainDescriptor = domainDescriptor;
            TypeDescriptors = new Dictionary<string, TypeDescriptor>();
            Parameters = parameters;

            LoadAttributes();

            LoadTypeDescriptors();

            OutputDirectory = Path.Combine( Parameters.OutputDirectory, ProjectFolder );
            ReferencePath = MakeRelativePath( OutputDirectory, parameters.RefDirectory );
        }


        public GenerateParameters Parameters { get; private set; }


        public IDictionary<string, TypeDescriptor> TypeDescriptors { get; private set; }


        public DomainDescriptor DomainDescriptor { get; private set; }

        public IEnumerable<ProjectDescriptor> ReferencedProjects
        {
            get
            {
                //return (from o in TypeDescriptors.Values.OfType<ObjectTypeDescriptor>()
                //        from p in o.Properties
                //        where p.Value.ReturnType.Namespace != Namespace
                //        select p.Value.ReturnType.ProjectDescriptor.Name).Distinct();

                return (from p in DomainDescriptor.GetReferencedProjects( this )
                        select p).ToList();
            }
        }


        public string Name { get; private set; }

        public string Namespace { get; private set; }

        public string Version { get; private set; }

        public string ProjectFolder { get; set; }

        public bool Ignore { get; private set; }

        public string TemplateType { get; private set; }

        public string ReferencePath { get; set; }

        public string OutputDirectory { get; private set; }

        public XmlSchema Schema { get; set; }


        private void LoadTypeDescriptors()
        {
            foreach( var type in Schema.Items )
            {
                var schemaType = type as XmlSchemaType;

                if( schemaType == null )
                    continue;

                if( schemaType is XmlSchemaSimpleType )
                {
                    var simpleTypeDescriptor = "Units" != TemplateType ? new SimpleTypeDescriptor( schemaType, this ) : new SimpleUnitTypeDescriptor(schemaType, this);
                    TypeDescriptors.Add( simpleTypeDescriptor.FullName, simpleTypeDescriptor );
                }
                else if( schemaType is XmlSchemaComplexType )
                {
                    var name = (schemaType).Name;

                    TypeDescriptor complexTypeDescriptor;

                    if( name.EndsWith( "RefListType" ) )
                        complexTypeDescriptor = new RefListTypeDescriptor( schemaType, this );
                    else if( name.EndsWith( "ListType" ) )
                        complexTypeDescriptor = new ListTypeDescriptor( schemaType, this );
                    else if( name.EndsWith( "RefType" ) )
                        complexTypeDescriptor = new RefTypeDescriptor( schemaType, this );
                    else if ("Units" == TemplateType)
                        complexTypeDescriptor = new ObjectUnitTypeDescriptor(schemaType, this);
                    else if ( name.EndsWith( "Type" ) )
                        complexTypeDescriptor = new ObjectTypeDescriptor( schemaType, this );
                    else
                        throw new Exception( "Wrong object type." );

                    TypeDescriptors.Add( complexTypeDescriptor.FullName, complexTypeDescriptor );
                }
            }
        }


        private void LoadAttributes()
        {
            var document = new XmlDocument();
            document.Load( Schema.SourceUri );
            bool hasProcessingInstruction = false;

            foreach( XmlNode node in document.ChildNodes )
            {
                if( node is XmlProcessingInstruction && node.Name == "XMLDatabase" )
                {
                    hasProcessingInstruction = true;
                    // Just put the content into a temporary node.
                    // This makes it easier to extract the attributes.
                    XmlNode tempNode = node.OwnerDocument.CreateElement( "temp" );
                    tempNode.InnerXml = "<attribute " + node.InnerText + "/>";

                    foreach( XmlAttribute attrib in tempNode.FirstChild.Attributes )
                    {
                        switch( attrib.Name )
                        {
                            case "Name":
                                Name = attrib.Value;
                                break;
                            case "Namespace":
                                Namespace = attrib.Value;
                                break;
                            case "Version":
                                Version = attrib.Value;
                                break;
                            case "ProjectFolder":
                                ProjectFolder = attrib.Value;
                                break;
                            case "Ignore":
                                bool ignore;
                                bool.TryParse( attrib.Value, out ignore );
                                Ignore = ignore;
                                break;
                            case "TemplateType":
                                TemplateType = attrib.Value;
                                break;
                        }
                    }

                    if( string.IsNullOrEmpty( Namespace ) )
                        Namespace = Name;

                    if (string.IsNullOrEmpty(ProjectFolder))
                        ProjectFolder = Name;

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine();
                    Console.WriteLine("Project attributes:");
                    Console.WriteLine("------------------------------------------------------");
                    Console.WriteLine(string.Format("Name: {0}", Name));
                    Console.WriteLine(string.Format("Namespace: {0}", Namespace));
                    Console.WriteLine(string.Format("Project folder {0}:", ProjectFolder));
                    Console.WriteLine(string.Format("Version {0}:", Version));
                    Console.WriteLine(string.Format("Ignore: {0}", Ignore));
                    Console.ResetColor();

                    break;
                }
            }

            if(!hasProcessingInstruction)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: No preprocessing instruction with name 'XMLDatabase' found in schema file.");
                Console.ResetColor();
            }
        }


        private static string MakeRelativePath( string pathSource, string pathTarget )
        {
            var uriSource = new Uri( pathSource );
            var uriTarget = new Uri( pathTarget );

            var relativeUri = uriSource.MakeRelativeUri( uriTarget );

            return Path.Combine( "..", relativeUri.OriginalString ).Replace( '/', Path.DirectorySeparatorChar );
        }
    }
}
