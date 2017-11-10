using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavUtilsLib
{
	public abstract class Chunk
	{
		
		public abstract uint ID 
		{
			get;
		}

		public string DisplayID
		{
			get { return Utils.Decode(ID); }
		}

		private long offset;
		public long Offset
		{
			get { return offset; }
		}

		private uint contentSize;
		public uint ContentSize 
		{
			get { return contentSize; }
		}

		public uint FullSize
		{
			get { return contentSize +(contentSize&1)+8 ; }
		}

		private Chunk()
		{
			
		}

		public Chunk(BinaryReader Reader)
		{
			offset = Reader.BaseStream.Position-4;
			contentSize = Reader.ReadUInt32();
			OnReadBody(Reader);
			// NTR Bug
			/*if (Reader.BaseStream.Position < Reader.BaseStream.Length)
			{
				if ((Reader.BaseStream.Position & 1) != 0) Reader.ReadByte();
			}//*/
		}
		protected abstract void OnReadBody(BinaryReader Reader);

		

		public static Chunk FromFile(string FileName)
		{
			using (FileStream stream = new FileStream(FileName, FileMode.Open, FileAccess.Read))
			{
				return FromStream(stream);
			}
		}
		public static Chunk FromStream(Stream Stream)
		{
			BinaryReader reader;
			reader = new BinaryReader(Stream);
			return FromReader(reader);
		}
		public static Chunk FromReader(BinaryReader Reader)
		{
			uint id;

			id = Reader.ReadUInt32();
			switch(id)
			{
				case 0x46464952:return new WavChunk(Reader);
				case 0x20746d66:return new FormatChunk(Reader);
				case 0x61746164:return new DataChunk(Reader);
				case 0x74636166:return new FactChunk(Reader);
				case 0x5453494c:return new ListChunk(Reader);
				case 0x20657563:return new CueChunk(Reader);

					
				case 0x4D414E49:return new TextChunk(id, Reader);
				case 0x4A425349:return new TextChunk(id, Reader);
				case 0x54524149:return new TextChunk(id, Reader);
				case 0x544D4349:return new TextChunk(id, Reader);
				case 0x59454B49:return new TextChunk(id, Reader);
				case 0x54465349:return new TextChunk(id, Reader);
				case 0x474E4549:return new TextChunk(id, Reader);
				case 0x48435449:return new TextChunk(id, Reader);
				case 0x44524349:return new TextChunk(id, Reader);
				case 0x524E4547: return new TextChunk(id, Reader);
				case 0x504F4349: return new TextChunk(id, Reader);
				case 0x6C62616C: return new LabelChunk(Reader);

				default: return new BlobChunk(id,Reader);

			}
		}
		
		public void DumpToConsole(int Padding)
		{
			Utils.WritePadding(Padding); Console.WriteLine("ID = " + DisplayID + " (Offset = "+offset+")" );
			Utils.WritePadding(Padding + 1); Console.WriteLine("Content size = " + contentSize);
			Utils.WritePadding(Padding + 1); Console.WriteLine("Full size = " + FullSize);
			OnDumpBodyToConsole(Padding+1);		
		}
	
		public abstract void OnDumpBodyToConsole(int Padding);


		public virtual IEnumerable<Chunk> EnumerateChunks()
		{
			yield return this;
		}
	}
}
