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

namespace ExtractDAT3
{
	class Program
	{
		private static string FindMetadataInAudioChunk(DataChunk Chunk)
		{
			byte[] pattern = Encoding.ASCII.GetBytes("[FileType]");
			byte[] buffer;
			int length;
			long position;
			StreamReader reader;

			length = pattern.Length;
			buffer = new byte[length];
			using (MemoryStream stream = new MemoryStream(Chunk.Data))
			{
				while (stream.Position < stream.Length - length)
				{
					position = stream.Position;
					stream.Read(buffer, 0, length);
					if (buffer.SequenceEqual(pattern))
					{
						stream.Seek(position, SeekOrigin.Begin);
						reader = new StreamReader(stream,Encoding.ASCII);
						return reader.ReadToEnd();
					}
					stream.Seek(position+1, SeekOrigin.Begin);
				}
			}

			return null;

		}

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
		
		private static void WriteDAT3(string FileName,string Metadata,bool IncludeHash)
		{
			StreamWriter writer;
			DateTime date=DateTime.MinValue;
			int channel=-1;
			IEnumerable<Field> fields;

			using (FileStream stream = new FileStream(FileName, FileMode.Create))
			{
				writer = new StreamWriter(stream);
				
				foreach (string line in Metadata.Split('\r', '\n'))
				{
					if (line == "") continue;
					
					if (line.Contains("FingerPrint"))
					{
						if (!IncludeHash) continue;
						writer.WriteLine(line.Substring(0, Math.Min(45, line.Length)));
						// clean fingerprint line
						continue;
					}
					if (line.Contains("StartTime"))
					{
						try
						{
							date = DateTime.Parse(line.Split('=')[1]);
						}
						catch(Exception ex)
						{
							Console.WriteLine($"Invalid date format: {ex.Message}");
						}
					}
					if (line.Contains("Channel"))
					{
						try
						{
							channel = Int32.Parse(line.Split('=')[1]);
						}
						catch (Exception ex)
						{
							Console.WriteLine($"Invalid channel format: {ex.Message}");
						}
					}
					writer.WriteLine(line);
				}

				if ((date != DateTime.MinValue) && (channel != -1))
				{
					fields=ExtractFieldsFromDatabase(date, channel);
					Console.WriteLine(string.Join(",", fields));
				}//*/

				writer.Flush();
			}
		}
		private static void ProcessWavFile(string FileName,bool ForceFileInvalid,bool IncludeHash)
		{
			Chunk chunk;
			TextChunk commentChunk;
			DataChunk dataChunk;
			string metadata;

			try
			{
				chunk = Chunk.FromFile(FileName);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Invalid wav file {FileName}: {ex.Message}");
				return;
			}
			//chunk.DumpToConsole(0);

			if (Path.GetFileName(FileName).StartsWith("R_") || ForceFileInvalid)
			{
				dataChunk = chunk.EnumerateChunks().OfType<DataChunk>().FirstOrDefault();
				if (dataChunk == null)
				{
					Console.WriteLine($"Invalid wav file {FileName}: No data chunk found");
					return;
				}
				metadata = FindMetadataInAudioChunk(dataChunk);
			}
			else
			{
				commentChunk = chunk.EnumerateChunks().OfType<TextChunk>().FirstOrDefault();
				if (commentChunk == null)
				{
					metadata = null;
				}
				else
				{
					metadata = commentChunk.Value;
				}
			}

			if (metadata == null)
			{
				Console.WriteLine($"No metadata in file {FileName}");
				return;
			}

			WriteDAT3(Path.ChangeExtension(FileName, "DAT3"), metadata,IncludeHash);
			

			Console.WriteLine($"File {FileName} extracted successfully");
		}
		
		static void Main(string[] args)
		{
			string path;
			bool includeHash;
			bool forceFileInvalid;

			if ((args.Length<1))
			{
				Console.WriteLine("Usage: ExtractDAT3 <Wav files location> [--includehash] [--forcefileinvalid]");
				return;
			}
			path = args[0];

			includeHash=args.Select(item=>item.ToLower()).Contains("--includehash"); ;
			forceFileInvalid = args.Select(item => item.ToLower()).Contains("--forcefileinvalid");

			foreach(string fileName in Directory.EnumerateFiles(path, "*.wav",SearchOption.AllDirectories))
			{
				ProcessWavFile(fileName,forceFileInvalid,includeHash);
			}

			Console.ReadLine();
			
		}


	}
}
