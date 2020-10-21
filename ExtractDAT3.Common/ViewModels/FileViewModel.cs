using LogLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WavUtilsLib;

namespace ExtractDAT3.Common.ViewModels
{
	public class FileViewModel:ViewModel
	{
		public override int ComponentID => 2;

		public string ImageSource
		{
			get;
			set;
		}

		public string Path
		{
			get;
			set;
		}



		public string Metadata
		{
			get;
			set;
		}


		public Statuses WavChunkStatus
		{
			get;
			set;
		}
		public Statuses DataChunkStatus
		{
			get;
			set;
		}
		public Statuses MetadataStatus
		{
			get;
			set;
		}
		public Statuses DAT3Status
		{
			get;
			set;
		}

		public string Message
		{
			get;
			set;
		}

		public FileViewModel(ILogger Logger):base(Logger)
		{
		}

		public void Initialize(string Path)
		{
			LogEnter();
			this.Path = Path;
		}

		private  string FindMetadataInAudioChunk(DataChunk Chunk)
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
						reader = new StreamReader(stream, Encoding.ASCII);
						return reader.ReadToEnd();
					}
					stream.Seek(position + 1, SeekOrigin.Begin);
				}
			}

			return null;

		}

		public void BeginAnalyse()
		{
			LogEnter();

			Log(LogLevels.Information,"Cleaning statuses");
			Metadata = null;
			WavChunkStatus = Statuses.Unknow;
			DataChunkStatus = Statuses.Unknow;
			MetadataStatus = Statuses.Unknow;
			DAT3Status = Statuses.Unknow;
			Message = null;
		}


		public void Analyse(bool ForceFileInvalid)
		{
			Chunk chunk;
			DataChunk dataChunk;
			TextChunk commentChunk;

			string path;


			Log(LogLevels.Information, $"Loading chunk from file {Path}");
			try
			{
				path = Path;
				chunk = Chunk.FromFile(path);
				WavChunkStatus = Statuses.Valid;
			}
			catch (Exception ex)
			{
				Message = ex.Message;
				Log(ex);
				WavChunkStatus = Statuses.Invalid;
				return;
			}


			if (System.IO.Path.GetFileName(Path).StartsWith("R_") || ForceFileInvalid)
			{
				Log(LogLevels.Information, $"Trying to extract data chunk from recovered file");

				dataChunk = chunk.EnumerateChunks().OfType<DataChunk>().FirstOrDefault();
				if (dataChunk == null)
				{
					Message = "No data chunk found";
					Log(LogLevels.Warning, $"Invalid wav file {Path}: No data chunk found");
					DataChunkStatus = Statuses.Invalid;
					MetadataStatus = Statuses.Invalid;
					return;
				}
				else
				{
					DataChunkStatus = Statuses.Valid;
					Metadata = FindMetadataInAudioChunk(dataChunk);
				}
			}
			else
			{
				Log(LogLevels.Information, $"Trying to extract comment chunk from valid file");

				commentChunk = chunk.EnumerateChunks().OfType<TextChunk>().FirstOrDefault();
				if (commentChunk == null)
				{
					Message = "No data chunk found";
					DataChunkStatus = Statuses.Invalid;
					MetadataStatus = Statuses.Invalid;
					return;
				}
				else
				{
					DataChunkStatus = Statuses.Valid;
					Metadata = commentChunk.Value;
				}
			}

			if (Metadata == null)
			{
				Message = "No metadata found";
				Log( LogLevels.Warning, $"No metadata in file {Path}");
				MetadataStatus = Statuses.Invalid;
			}
			else MetadataStatus = Statuses.Valid;
			



		}

		public void EndAnalyse(bool IncludeHash)
		{
			StreamWriter writer;
			//DateTime date = DateTime.MinValue;
			//int channel = -1;
			string fileName;

			LogEnter();

			if (Metadata == null)
			{
				Log(LogLevels.Information, $"No metadata for file {Path}, skipping...");
				DAT3Status = Statuses.Invalid;
				return;
			}

			fileName = System.IO.Path.ChangeExtension(Path, "DAT3");
			Log(LogLevels.Information, $"Trying to write DAT3 file");
			try
			{
				using (FileStream stream = new FileStream(fileName, FileMode.Create))
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
						/*if (line.Contains("StartTime"))
						{
							try
							{
								date = DateTime.Parse(line.Split('=')[1]);
							}
							catch (Exception ex)
							{
								Console.WriteLine($"Invalid date format: {ex.Message}");
							}
						}*/
						/*if (line.Contains("Channel"))
						{
							try
							{
								channel = Int32.Parse(line.Split('=')[1]);
							}
							catch (Exception ex)
							{
								Console.WriteLine($"Invalid channel format: {ex.Message}");
							}
						}*/
						writer.WriteLine(line);
					}

					/*if ((date != DateTime.MinValue) && (channel != -1))
					{
						fields = ExtractFieldsFromDatabase(date, channel);
						Console.WriteLine(string.Join(",", fields));
					}//*/

					writer.Flush();
					DAT3Status = Statuses.Valid;
				}
			}
			catch(Exception ex)
			{
				Log(ex);
				Message = ex.Message;
				DAT3Status = Statuses.Invalid;
			}
		}





	}
}
