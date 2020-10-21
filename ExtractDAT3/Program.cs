using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WavUtilsLib;
using MySql.Data;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using ExtractDAT3.Common.ViewModels;
using LogLib;

namespace ExtractDAT3
{
	class Program
	{

		private static IEnumerable<Field> ExtractFieldsFromDatabase(DateTime Date, int Channel)
		{
			MySqlConnection connection;
			MySqlCommand command;
			MySqlDataReader reader;
			List<Field> fields;
			Field item;

			fields = new List<Field>();

			using (connection = new MySqlConnection("Server=127.0.0.1;Database=recorder;Uid=service;Pwd=53rv1c3;"))
			{
				try
				{
					connection.Open();
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Failed to connect to mysql: {ex.Message}");
					return fields;
				}

				command = new MySqlCommand("select * from cvs where CVSSDT=@date and CVSCHN=@channel", connection);
				command.Parameters.AddWithValue("@date", Date);
				command.Parameters.AddWithValue("@channel", Channel);

				try
				{
					reader = command.ExecuteReader();
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Failed to run mysql command: {ex.Message}");
					return fields;
				}

				if (!reader.HasRows)
				{
					Console.WriteLine($"No ticket was found in database");
					return fields;
				}
				reader.Read();
				for (int t = 0; t < reader.FieldCount; t++)
				{
					try
					{
						item = new Field(reader.GetName(t), reader.GetValue(t));
						fields.Add(item);
					}
					catch
					{
						// skip invalid field
					}
				}
				reader.Close();

				return fields;
			}
		}
		
		
		static void Main(string[] args)
		{
			string path;
			ApplicationViewModel viewModel;
			ILogger logger;

			if ((args.Length<1))
			{
				Console.WriteLine("Usage: ExtractDAT3 <Wav files location> [--includehash] [--forcefileinvalid]");
				return;
			}

			logger = new ConsoleLogger(new DefaultLogFormatter());
			viewModel = new ApplicationViewModel(logger);

			path = args[0];
			viewModel.IncludeHash = args.Select(item=>item.ToLower()).Contains("--includehash"); ;
			viewModel.ForceInvalid = args.Select(item => item.ToLower()).Contains("--forcefileinvalid");

			viewModel.LoadDirectory(path,100);
			viewModel.Analyse();				

			
		}


	}
}
