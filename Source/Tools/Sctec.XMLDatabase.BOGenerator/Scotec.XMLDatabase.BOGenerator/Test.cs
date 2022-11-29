using System;
using System.Collections;
using System.IO;
using System.Xml.Schema;

namespace XMLObjectGenerator
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	/// 
	internal class Test
	{
		public const string s = "aaa";

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			var test = new Test();
			test.Run();
		}

		public void Run()
		{
			var loader = new SchemaLoader();
			var parameters = new GenerateParameters();

			try
			{
				loader.LoadSchema(parameters.SchemaFile);
				if (loader.ErrorCount > 0)
				{
					Console.WriteLine("Input Schema is invalid. Aborting.");
					return;
				}
			}
			catch (IOException ex)
			{
				Console.WriteLine("Schema file \"" + parameters.SchemaFile + "\" could not be opened:");
				Console.WriteLine(ex.Message);
			}

			// Fill the project info.
			foreach (DictionaryEntry de in loader.Schemas)
			{
				var sg = de.Value as SchemaGenerator;
				sg.Initialize(parameters);
			}

			// Run the generartors.
			foreach (DictionaryEntry de in loader.Schemas)
			{
				var sg = de.Value as SchemaGenerator;
				sg.Run(parameters);
			}
		}

		private void ValidationCallBack(object sender, ValidationEventArgs e)
		{
			Console.WriteLine("Validation Error: {0}", e.Message);
		}
	}
}