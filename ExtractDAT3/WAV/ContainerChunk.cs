using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavUtilsLib
{
	public abstract class ContainerChunk:Chunk
	{
		private uint format;
		public uint Format 
		{
			get { return format; }
		}
		public string DisplayFormat
		{
			get { return Utils.Decode(format); }
		}
		private List<Chunk> chunks;
		public List<Chunk> Chunks
		{
			get { return chunks; }
		}

		public ContainerChunk(BinaryReader Reader):base(Reader)
		{

		}

		public ChunkType GetChunk<ChunkType>()
		{
			return chunks.OfType<ChunkType>().FirstOrDefault();
		}

		// original methid
		/*protected override void OnReadBody(BinaryReader Reader)
		{
			Chunk chunk;

			format = Reader.ReadUInt32();

			chunks = new List<Chunk>();
			uint size = 4;
			while ((size < ContentSize)  && (Reader.BaseStream.Position<Reader.BaseStream.Length))// NTR BUG
			{
				chunk = Chunk.FromReader(Reader);
				chunks.Add(chunk);
				size += chunk.FullSize;
			}
		}*/

			// manage NTR Bug
		protected override void OnReadBody(BinaryReader Reader)
		{
			Chunk chunk;
			long position;

			format = Reader.ReadUInt32();

			chunks = new List<Chunk>();
			uint size = 4;
			while ((size < ContentSize)  && (Reader.BaseStream.Position<Reader.BaseStream.Length))// NTR BUG
			{
				position = Reader.BaseStream.Position;
				chunk = Chunk.FromReader(Reader);
				if (chunk is BlobChunk)
				{ 
					Reader.BaseStream.Seek(position +1,SeekOrigin.Begin);
					chunk = Chunk.FromReader(Reader);
				}
				chunks.Add(chunk);
				size += chunk.FullSize;
			}
		}


		public override void OnDumpBodyToConsole(int Padding)
		{
			Utils.WritePadding(Padding); Console.WriteLine("Format = " + DisplayFormat);
			Utils.WritePadding(Padding); Console.WriteLine("Chunks size = " + chunks.Sum(item => item.FullSize));
			foreach (Chunk chunk in chunks)
			{
				chunk.DumpToConsole(Padding + 1);
			}


		}

		public override IEnumerable<Chunk> EnumerateChunks()
		{
			yield return this;
			foreach (Chunk chunk in chunks)
			{
				foreach(Chunk child in chunk.EnumerateChunks()) yield return child;
			}
		}

	}
}
