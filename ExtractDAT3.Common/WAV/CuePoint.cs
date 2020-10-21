using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavUtilsLib
{
	public struct CuePoint 
	{
		private uint name;
		public uint Name
		{
			get { return name; }
		}
		

		private uint position;
		public uint Position
		{
			get { return position; }
		}
		private uint chunkID;
		public uint ChunkID
		{
			get { return chunkID; }
		}

		public string ChunkName
		{
			get { return Utils.Decode(chunkID); }
		}

		private uint chunkStart;
		public uint ChunkStart
		{
			get { return chunkStart; }
		}
		private uint blockStart;
		public uint BlockStart
		{
			get { return blockStart; }
		}
		private uint sampleOffset;
		public uint SampleOffset
		{
			get { return sampleOffset; }
		}

		public CuePoint(BinaryReader Reader)
		{
			name = Reader.ReadUInt32() ;
			position = Reader.ReadUInt32();
			chunkID = Reader.ReadUInt32();
			chunkStart = Reader.ReadUInt32();
			blockStart = Reader.ReadUInt32();
			sampleOffset = Reader.ReadUInt32();
		}

		
		public void DumpToConsole(int Padding)
		{
			Utils.WritePadding(Padding); Console.WriteLine("Name = " + name);
			Utils.WritePadding(Padding); Console.WriteLine("Position = " + position);
			Utils.WritePadding(Padding); Console.WriteLine("Chunk ID = " + ChunkName);
			Utils.WritePadding(Padding); Console.WriteLine("Chunk start = " + chunkStart);
			Utils.WritePadding(Padding); Console.WriteLine("Block start = " + blockStart);
			Utils.WritePadding(Padding); Console.WriteLine("Sample offset = " + sampleOffset);

		}
	}
}
