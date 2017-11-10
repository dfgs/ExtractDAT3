using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavUtilsLib
{
	public class FactChunk : Chunk
	{

		public override uint ID
		{
			get { return 0x74636166; }
		}
		private uint sampleLength;
		public uint SampleLength
		{
			get { return sampleLength; }
		}

		public FactChunk(BinaryReader Reader) :base(Reader)
		{

		}
		protected override void OnReadBody(BinaryReader Reader)
		{
			sampleLength = Reader.ReadUInt32();
		}

		public override void OnDumpBodyToConsole(int Padding)
		{
			Utils.WritePadding(Padding); Console.WriteLine("SampleLength = " + sampleLength);
		}


	}
}
