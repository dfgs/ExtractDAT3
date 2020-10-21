using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavUtilsLib
{
	public class TextChunk : Chunk
	{
		private uint id;
		public override uint ID
		{
			get { return id; }
		}

		private string value;
		public string Value
		{
			get { return value; }
		}

		public TextChunk(uint ID,BinaryReader Reader) : base(Reader)
		{
			this.id = ID;
		}

		protected override void OnReadBody(BinaryReader Reader)
		{
			byte[] buffer;

			buffer = new byte[this.ContentSize];
			Reader.Read(buffer, 0, (int)this.ContentSize);
			value = Encoding.ASCII.GetString(buffer);

		}

		public override void OnDumpBodyToConsole(int Padding)
		{
			Utils.WritePadding(Padding); Console.WriteLine("Value = " + value);
		}

		
	}
}
