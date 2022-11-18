#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;

#endregion


namespace Scotec.XMLDatabase.BOGenerator.Descriptors
{
    public class DomainDescriptor : Descriptor
    {
        public DomainDescriptor( string schemaFile, GenerateParameters parameters )
        {
            SchemaFiles = new List<string>();

            Parameters = parameters;
            ProjectDescriptors = new Dictionary<string, ProjectDescriptor>();
            SchemaToProjectMap = new Dictionary<string, ProjectDescriptor>();

            LoadSchema( schemaFile );
        }


        public GenerateParameters Parameters { get; private set; }

        public int ErrorCount { get; private set; }

        public IDictionary<string, ProjectDescriptor> ProjectDescriptors { get; private set; }

        public IList<string> Namespaces
        {
            get { return (from p in ProjectDescriptors.Values select p.Namespace).ToList(); }
        }

        private IDictionary<string, ProjectDescriptor> SchemaToProjectMap { get; set; }
        private List<string> SchemaFiles { get; set; }


        public TypeDescriptor FindDescriptor( string typeName )
        {
            return (from p in ProjectDescriptors.Values
                    from d in p.TypeDescriptors
                    where d.Key == typeName
                    select d.Value).First();
        }


        public ProjectDescriptor GetProjectBySchemaFileName( string schemaFileName )
        {
            return SchemaToProjectMap[schemaFileName];
        }


        public IEnumerable<ProjectDescriptor> GetReferencedProjects( ProjectDescriptor project )
        {
            return (from s in GetIncludedSchemas( project.Schema ).Distinct()
                    select GetProjectBySchemaFileName( s )).ToList();
        }


        private static IEnumerable<string> GetIncludedSchemas( XmlSchema schema )
        {
            var result = new List<string>();

            foreach( XmlSchemaExternal included in schema.Includes )
            {
                var includedSchema = included.Schema;
                result.Add( Path.GetFileName( new Uri( includedSchema.SourceUri ).LocalPath ) );

                result.AddRange( GetIncludedSchemas( includedSchema ) );
            }

            return result;
        }


        private void LoadSchema( string schemaFile )
        {
            // First the schema must be loaded and compiled. After it has been compiled,
            // a list all all included schemas well be extracted.
            // Finally a schemaReader will be created for each schema file.
            var oldErrorCount = ErrorCount;

            var schemas = new Dictionary<string, XmlSchema>();

            LoadSchema(schemaFile, schemas);
            //var textReader = new XmlTextReader( schemaFile );
            //var settings = new XmlReaderSettings { ValidationType = ValidationType.Schema };
            //var reader = XmlReader.Create( textReader, settings );
            //var schema = XmlSchema.Read( reader, ValidationCallBack );


            if ( ErrorCount == oldErrorCount )
            {
                var schemaSet = new XmlSchemaSet();
                foreach (var schema in schemas.Values)
                    schemaSet.Add(schema);
                //schema.Compile(new ValidationEventHandler(ValidationCallBack));
                //AddSchemaToSchemaList(schema);

                //schemaSet.Add( schema );
                schemaSet.Compile();
                foreach( XmlSchema s in schemaSet.Schemas() )
                    AddSchemaToSchemaList( s );
            }
        }

        private XmlSchema LoadSchema(string schemaFile, Dictionary<string, XmlSchema> schemas)
        {
            if (schemas.TryGetValue(schemaFile.ToLower(), out var loadedSchema))
                return loadedSchema;

            var textReader = new XmlTextReader(schemaFile);
            var settings = new XmlReaderSettings { ValidationType = ValidationType.Schema };
            var reader = XmlReader.Create(textReader, settings);
            var schema = XmlSchema.Read(reader, ValidationCallBack);

            schemas.Add(schemaFile.ToLower(), schema);

            var schemaPath = Path.GetDirectoryName(schemaFile);
            foreach ( var include in schema.Includes.OfType<XmlSchemaInclude>())
            {
                var includedSchema = LoadSchema(Path.Combine(schemaPath, include.SchemaLocation), schemas);
                if (includedSchema != null)
                    include.Schema = includedSchema;
            }

            return schema;
        }

        private void AddSchemaToSchemaList( XmlSchema schema )
        {
            // Recursively add all schemas.
            if( SchemaFiles.Contains( schema.SourceUri ) )
                return;

            var schemaFileName = Path.GetFileName( new Uri( schema.SourceUri ).LocalPath );

            SchemaFiles.Add( schema.SourceUri );

            var projectDescriptor = new ProjectDescriptor( schema, this, Parameters );
            ProjectDescriptors.Add( projectDescriptor.Name, projectDescriptor );
            SchemaToProjectMap.Add( schemaFileName, projectDescriptor );

            foreach( XmlSchemaExternal se in schema.Includes )
            {
                if( se.Schema != null )
                    AddSchemaToSchemaList( se.Schema );
            }
        }


        private void ValidationCallBack( object sender, ValidationEventArgs e )
        {
            if( e.Severity == XmlSeverityType.Error )
                ++ErrorCount;
            Console.WriteLine( "Schema Validation {0}: {1}", e.Severity == XmlSeverityType.Error ? "Error" : "Warning", e.Message );
        }
    }
}
