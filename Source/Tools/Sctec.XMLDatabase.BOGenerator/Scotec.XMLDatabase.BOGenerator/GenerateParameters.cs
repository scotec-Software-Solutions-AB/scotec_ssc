#region

using System;
using System.IO;
using Scotec.XMLDatabase.BOGenerator.Descriptors;

#endregion


namespace Scotec.XMLDatabase.BOGenerator
{
    /// <summary>
    ///   Summary description for GenerateParameters.
    /// </summary>
    public class GenerateParameters
    {
        private string _binDirectory = string.Empty;
        private string _outputDirectory = string.Empty;
        private string _refDirectory = string.Empty;
        private string _schemaFile = string.Empty;
        private string _targetFramework = string.Empty;
        private string _templateDirectory = string.Empty;


        public GenerateParameters()
        {
            //Projects = new Dictionary<XmlSchema, GenerateInfo>();
            Initialize();
        }


        //public IDictionary<XmlSchema, GenerateInfo> Projects { get; set; }

        public string OutputDirectory
        {
            get { return _outputDirectory; }
        }

        public string TemplateDirectory
        {
            get { return _templateDirectory; }
        }

        public string BinDirectory
        {
            get { return _binDirectory; }
        }

        public string RefDirectory
        {
            get { return _refDirectory; }
        }

        public string SchemaFile
        {
            get { return _schemaFile; }
        }

        public string TargetFramework
        {
            get { return _targetFramework; }
        }

        internal DomainDescriptor DomainDescriptor { get; set; }


        //public void SetProjectInfo( XmlSchema schema)
        //{
        //    Projects.Add( schema );
        //}


        private void Initialize()
        {
            var arguments = Environment.GetCommandLineArgs();

            var size = arguments.Length;
            for( var i = 0; i < size; i++ )
            {
                switch( arguments[i].ToLower() )
                {
                    case "-s":
                    case "/s":
                        _schemaFile = arguments[++i].Trim( new[] { '\"' } );
                        _schemaFile.Replace( '/', '\\' );
                        break;
                    case "-t":
                    case "/t":
                        _templateDirectory = arguments[++i].Trim( new[] { '\"' } );
                        _templateDirectory.Replace( '/', '\\' );
                        break;
                    case "-o":
                    case "/o":
                        _outputDirectory = arguments[++i].Trim( new[] { '\"' } );
                        _outputDirectory.Replace( '/', '\\' );
                        break;
                    case "-b":
                    case "/b":
                        _binDirectory = arguments[++i].Trim( new[] { '\"' } );
                        _binDirectory.Replace( '/', '\\' );
                        break;
                    case "-r":
                    case "/r":
                        _refDirectory = arguments[++i].Trim( new[] { '\"' } );
                        _refDirectory.Replace( '/', '\\' );
                        break;
                    case "-v":
                    case "/v":
                        _targetFramework = arguments[++i];
                        break;
                }
            }

            var generatorBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var currentDirectory = Directory.GetCurrentDirectory();

            if( _templateDirectory.Length == 0 )
                _templateDirectory = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, @"Templates" );

            if( !Path.IsPathRooted( _templateDirectory ) )
                _templateDirectory = new Uri( Path.Combine( generatorBaseDirectory, _templateDirectory ) ).LocalPath;
            Console.WriteLine( string.Format( "Template folder: {0}", _templateDirectory ) );

            if( !Path.IsPathRooted( _schemaFile ) )
                _schemaFile = new Uri( Path.Combine( currentDirectory, _schemaFile ) ).LocalPath;
            Console.WriteLine( string.Format( "Schema file: {0}", _schemaFile ) );

            if( !Path.IsPathRooted( _outputDirectory ) )
                _outputDirectory = new Uri( Path.Combine( currentDirectory, _outputDirectory ) ).LocalPath;
            Console.WriteLine( string.Format( "Output folder: {0}", _outputDirectory ) );

            if( !Path.IsPathRooted( _binDirectory ) )
                _binDirectory = new Uri( Path.Combine( currentDirectory, _binDirectory ) ).LocalPath;
            Console.WriteLine( string.Format( "Bin folder: {0}", _binDirectory ) );

            if( !Path.IsPathRooted( _refDirectory ) )
                _refDirectory = new Uri( Path.Combine( currentDirectory, _refDirectory ) ).LocalPath;
            Console.WriteLine( string.Format( "Reference folder: {0}", _refDirectory ) );

            if( string.IsNullOrEmpty( _targetFramework ) )
                _targetFramework = "4.6";

            Console.WriteLine();
        }
    }
}
