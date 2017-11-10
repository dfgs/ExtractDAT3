using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavUtilsLib
{
	public class LabelChunk : Chunk
	{
		
		public override uint ID
		{
			get { return 0x6C62616C;}
		}

		private uint index;
		public uint Index
		{
			get { return index; }
		}

		private string description;
		public string Description
		{
			get { return description; }
		}

		public LabelChunk(BinaryReader Reader) : base(Reader)
		{
		}

		protected override void OnReadBody(BinaryReader Reader)
		{
			byte[] buffer;

			index = Reader.ReadUInt32();

			buffer = new byte[this.ContentSize-4];
			Reader.Read(buffer, 0, (int)this.ContentSize-4);
			description = Encoding.ASCII.GetString(buffer);

		}

		public override void OnDumpBodyToConsole(int Padding)
		{
			Utils.WritePadding(Padding); Console.WriteLine("Index = " + index);
			Utils.WritePadding(Padding); Console.WriteLine("Description = " + description);
		}


	}
}
