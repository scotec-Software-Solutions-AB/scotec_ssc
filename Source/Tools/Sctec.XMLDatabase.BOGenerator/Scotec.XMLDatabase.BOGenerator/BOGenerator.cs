#region

using System.IO;
using System.Reflection;
using Scotec.T4Generator;

#endregion

namespace Scotec.XMLDatabase.BOGenerator
{
    internal class BOGenerator
    {
        private static IGenerator s_generator;


        private static IGenerator Generator
        {
            get
            {
                if (s_generator == null)
                {
                    s_generator = new T4Generator.Generator();
                    s_generator.Settings.ReferencePaths.Add(
                        new FileInfo(Assembly.GetEntryAssembly().Location).Directory.FullName);
                }

                return s_generator;
            }
        }


        protected static string CreateDirectory(string targetPath, string @namespace, string folder)
        {
            var path = Path.Combine(targetPath, @namespace);
            path = Path.Combine(path, folder);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }


        protected static void Generate(string templateName, string templateType, string output, params object[] parameters)
        {
            Generator.GenerateToFile(GetTemplateFileName(templateName, templateType), output, parameters);
        }


        private static string GetTemplateFileName(string templateName, string templateType)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if( !string.IsNullOrEmpty( templateType ) )
            {
                var templateFile = Path.Combine(Path.Combine(path, "Templates"), templateName + "." + templateType + ".tt");
                if( File.Exists( templateFile ) )
                    return templateFile;
            }

            return Path.Combine(Path.Combine(path, "Templates"), templateName + ".tt");
        }
    }
}