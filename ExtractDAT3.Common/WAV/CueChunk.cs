using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavUtilsLib
{
	public class CueChunk : Chunk
	{
		public override uint ID
		{
			get { return 0x20657563; }
		}

		private uint count;
		public uint Count
		{
			get { return count; }
		}
		private CuePoint[] points;
		public CuePoint[] Points
		{
			get { return points; }
		}

		
		public CueChunk(BinaryReader Reader):base(Reader)
		{
		}

		protected override void OnReadBody(BinaryReader Reader)
		{
			
			count = Reader.ReadUInt32();
			points = new CuePoint[count];

			for(int t=0;t<count;t++)
			{
				points[t] = new CuePoint(Reader);
			}
		}

		public override void OnDumpBodyToConsole(int Padding)
		{
			Utils.WritePadding(Padding); Console.WriteLine("Count = " + count);
			foreach (CuePoint chunk in points)
			{
				chunk.DumpToConsole(Padding + 1);
			}
		}
	}
}
