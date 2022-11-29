#region

using System;
using System.Diagnostics;
using System.IO;
using Scotec.XMLDatabase.BOGenerator.Descriptors;

#endregion


namespace Scotec.XMLDatabase.BOGenerator
{
    internal enum EGeneratorType
    {
        Interface,
        Implementation
    }


    internal class Generator
    {
        public static void Main()
        {
            var test = new Generator();
            test.Run();
        }


        public void Run()
        {
            //Debugger.Launch();
            var parameters = new GenerateParameters();

            try
            {
                var domainDescriptor = new DomainDescriptor( parameters.SchemaFile, parameters );
                parameters.DomainDescriptor = domainDescriptor;
            }
            catch( IOException ex )
            {
                Console.WriteLine( "Schema file \"" + parameters.SchemaFile + "\" could not be opened:" );
                Console.WriteLine( ex.Message );
            }

            foreach( var project in parameters.DomainDescriptor.ProjectDescriptors )
            {
                // Generate Project files
                BOProjectFileGenerator.Run( project.Value, parameters );

                // Generate assembly info
                BOAssemblyInfoFileGenerator.Run( project.Value, parameters );

                // Generate interfaces
                BOTypeGenerator<ObjectTypeDescriptor>.Run( project.Value, parameters, EGeneratorType.Interface );
                BOTypeGenerator<SimpleTypeDescriptor>.Run( project.Value, parameters, EGeneratorType.Interface );
                BOTypeGenerator<RefTypeDescriptor>.Run( project.Value, parameters, EGeneratorType.Interface );
                BOTypeGenerator<ListTypeDescriptor>.Run( project.Value, parameters, EGeneratorType.Interface );
                BOTypeGenerator<RefListTypeDescriptor>.Run( project.Value, parameters, EGeneratorType.Interface );

                // Generate implementation
                BOTypeGenerator<ObjectTypeDescriptor>.Run( project.Value, parameters, EGeneratorType.Implementation );
                BOTypeGenerator<SimpleTypeDescriptor>.Run( project.Value, parameters, EGeneratorType.Implementation );
                BOTypeGenerator<RefTypeDescriptor>.Run( project.Value, parameters, EGeneratorType.Implementation );
                BOTypeGenerator<ListTypeDescriptor>.Run( project.Value, parameters, EGeneratorType.Implementation );
                BOTypeGenerator<RefListTypeDescriptor>.Run( project.Value, parameters, EGeneratorType.Implementation );
            }
        }
    }
}
