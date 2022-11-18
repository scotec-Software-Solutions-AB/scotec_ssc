#region

using System;
using System.IO;
using System.Linq;
using Scotec.XMLDatabase.BOGenerator.Descriptors;

#endregion


namespace Scotec.XMLDatabase.BOGenerator
{
    internal class BOTypeGenerator<TTypeDescriptor> : BOGenerator where TTypeDescriptor : TypeDescriptor
    {
        public static void Run( ProjectDescriptor projectDescriptor, GenerateParameters parameters, EGeneratorType generatorType )
        {
            var descriptors = (from d in projectDescriptor.TypeDescriptors.Values
                               where (d is TTypeDescriptor)
                               select d).Cast<TTypeDescriptor>();

            foreach( var descriptor in descriptors )
                GenerateFile( projectDescriptor, descriptor, parameters, generatorType );
        }


        private static void GenerateFile(
                ProjectDescriptor projectDescriptor, TTypeDescriptor typeDescriptor, GenerateParameters parameters, EGeneratorType generatorType )
        {
            Console.WriteLine( string.Format( "Generating implementation file type '{0}'...", typeDescriptor.FullNameImplementation ) );

            var templateName = string.Format( "BO{0}{1}", typeof( TTypeDescriptor ).Name.Replace( "Descriptor", "" ), generatorType.ToString() );
            string outputDirectory;
            string fileName;

            if( generatorType == EGeneratorType.Interface )
            {
                outputDirectory = string.Format("{0}.Contracts", projectDescriptor.OutputDirectory);
                fileName = string.Format( "{0}.cs", typeDescriptor.TypeName );
            }
            else
            {
                outputDirectory = string.Format("{0}", projectDescriptor.OutputDirectory);
                fileName = string.Format( "{0}.cs", typeDescriptor.TypeNameImplementation );
            }

            var targetFile = Path.Combine(outputDirectory, fileName);

            Generate( templateName, projectDescriptor.TemplateType, targetFile, typeDescriptor, parameters );
        }
    }
}
