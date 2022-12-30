#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Scotec.XMLDatabase.BOGenerator.Descriptors;

#endregion


namespace Scotec.XMLDatabase.BOGenerator
{
    internal class BOProjectFileGenerator : BOGenerator
    {
        public static void Run( ProjectDescriptor projectDescriptor, GenerateParameters parameters )
        {
            CreateProjectFile( projectDescriptor, parameters, "InterfaceProject", "Abstractions");
            CreateProjectFile( projectDescriptor, parameters, "ImplementationProject", "" );
        }


        private static void CreateProjectFile( ProjectDescriptor projectDescriptor, GenerateParameters parameters, string templateName, string extension )
        {
            Console.WriteLine( string.Format( "Generating Visual Studio project files for namespace '{0}'...",
                                              projectDescriptor.Name ) );

            var targetPath = projectDescriptor.OutputDirectory;
            if(!string.IsNullOrEmpty(extension))
               targetPath += "." + extension;

            if(!string.IsNullOrEmpty(extension))
                extension = "." + extension;
            var targetFile = Path.Combine( targetPath, string.Format( "{0}{1}.csproj", projectDescriptor.Name, extension ) );


            string projectId;
            IList<string> userFiles;

            Generate( templateName, null, targetFile, projectDescriptor, parameters.TargetFramework );
        }
    }
}
