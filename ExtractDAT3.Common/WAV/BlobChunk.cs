using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavUtilsLib
{
	public class BlobChunk : Chunk
	{
		private uint id;
		public override uint ID
		{
			get { return id; }
		}

		public BlobChunk(uint ID,BinaryReader Reader) :base(Reader)
		{
			this.id = ID;
		}
		protected override void OnReadBody(BinaryReader Reader)
		{
			Reader.BaseStream.Seek(this.ContentSize, SeekOrigin.Current);
		}
		public override void OnDumpBodyToConsole(int Padding)
		{
			Utils.WritePadding(Padding); Console.WriteLine("<BLOB>");
		}
	}
}
