using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WavUtilsLib;

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
		static void Main(string[] args)
		{
			string path;
			Chunk chunk;
			StreamWriter writer;
			TextChunk commentChunk;
			DataChunk dataChunk;
			string metadata;
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
				try
				{
					chunk = Chunk.FromFile(fileName);
				}
				catch(Exception ex)
				{
					Console.WriteLine($"Invalid wav file {fileName}: {ex.Message}");
					continue;
				}
				//chunk.DumpToConsole(0);
				
				if (Path.GetFileName(fileName).StartsWith("R_") || forceFileInvalid)
				{
					dataChunk = chunk.EnumerateChunks().OfType<DataChunk>().FirstOrDefault();
					if (dataChunk == null)
					{
						Console.WriteLine($"Invalid wav file {fileName}: No data chunk found");
						continue;
					}
					metadata=FindMetadataInAudioChunk(dataChunk);
				}
				else
				{
					commentChunk = chunk.EnumerateChunks().OfType<TextChunk>().FirstOrDefault();
					if (commentChunk==null)
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
					Console.WriteLine($"No metadata in file {fileName}");
					continue;
				}
				

				using (FileStream stream = new FileStream(Path.ChangeExtension(fileName, "DAT3"), FileMode.Create))
				{
					writer = new StreamWriter(stream);
					foreach (string line in metadata.Split('\r', '\n'))
					{
						if (line == "") continue;
						if (line.Contains("FingerPrint"))
						{
							if (!includeHash) continue;
							writer.WriteLine( line.Substring(0,Math.Min(45,line.Length)));
							// clean fingerprint line

						} else writer.WriteLine(line);

					}

					writer.Flush();
				}

				Console.WriteLine($"File {fileName} extracted successfully");


			}

			//Console.ReadLine();
		}


	}
}
