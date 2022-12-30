#region

using System;
using System.IO;
using Scotec.XMLDatabase.BOGenerator.Descriptors;

#endregion


namespace Scotec.XMLDatabase.BOGenerator
{
    internal class BOAssemblyInfoFileGenerator : BOGenerator
    {
        public static void Run( ProjectDescriptor descriptor, GenerateParameters parameters )
        {
            Console.WriteLine( string.Format( "Generating assembly info files for namespace '{0}'...",
                                              descriptor.Name ) );

            CreateAssemblyInfoFile( descriptor, parameters, "Abstractions", true );
            CreateAssemblyInfoFile( descriptor, parameters, "", false );
        }


        private static void CreateAssemblyInfoFile( ProjectDescriptor descriptor, GenerateParameters parameters, string extension, bool isInterface )
        {
            //const string templateName = "AssemblyInfo";

            //var targetFile = Path.Combine( parameters.OutputDirectory, descriptor.ProjectFolder + "." + extension, "Properties", "AssemblyInfo.cs" );

            //Generate( templateName, null, targetFile, descriptor, isInterface );
        }
    }
}
