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
            CreateProjectFile( projectDescriptor, parameters, "InterfaceProject", "Contracts" );
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

            if (!GetProjectDataFromExistingProject(targetFile, out projectId, out userFiles))
            {
            }
                Generate( templateName, null, targetFile, projectDescriptor, projectId, userFiles, parameters.TargetFramework );
        }


        private static bool GetProjectDataFromExistingProject( string targetFile, out string projectId, out IList<string> userFiles )
        {
            projectId = Guid.NewGuid().ToString("D");
            userFiles = new List<string>();

            return File.Exists(targetFile);

            //if( !File.Exists( targetFile ) )
            //{
            //    projectId = Guid.NewGuid().ToString( "D" );
            //    userFiles = new List<string>();

            //    return;
            //}

            //var document = new XmlDocument();
            //document.Load( targetFile );

            //// Get id from project file and assign it to the project descriptor.
            //var namespaceManager = new XmlNamespaceManager( document.NameTable );
            //namespaceManager.AddNamespace( "ns", "http://schemas.microsoft.com/developer/msbuild/2003" );

            //projectId = document.SelectSingleNode( "//ns:Project/ns:PropertyGroup/ns:ProjectGuid/child::text()", namespaceManager )
            //        .Value.Trim( '{', '}' );


            //var files = document.SelectNodes( @"//ns:Project/ns:ItemGroup/ns:Compile[starts-with(@Include, 'UserFiles\')]",
            //                                  namespaceManager );

            //userFiles = (from XmlElement userFile in files
            //             select userFile.Attributes["Include"].Value).ToList();
        }
    }
}
