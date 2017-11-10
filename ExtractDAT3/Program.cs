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
		static void Main(string[] args)
		{
			string path;
			Chunk chunk;
			StreamWriter writer;
			TextChunk[] commentChunks;

			if (args.Length==0)
			{
				Console.WriteLine("Usage: ExtractDAT3 <Wav files location>");
				return;
			}

			path = args[0];

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
				
				
				commentChunks = chunk.EnumerateChunks().OfType<TextChunk>().ToArray();

				if (commentChunks.Length == 0)
				{
					Console.WriteLine($"No metadata in file {fileName}");
					continue;
				}
				else
				{
					
					using (FileStream stream = new FileStream(Path.ChangeExtension(fileName, "DAT3"), FileMode.Create))
					{
						writer = new StreamWriter(stream);
						foreach (TextChunk commentChunk in commentChunks)
						{
							foreach (string line in commentChunk.Value.Split('\r', '\n'))
							{
								if ((line == "") || (line.Contains("FingerPrint"))) continue;
								writer.WriteLine(line);
							}
						}
						writer.Flush();
					}

					Console.WriteLine($"File {fileName} extracted successfully");


				}


			}


		}
	}
}
