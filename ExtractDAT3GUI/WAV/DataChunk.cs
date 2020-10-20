using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WavUtilsLib
{
	public class DataChunk : Chunk
	{
		private byte[] data;
		public byte[] Data
		{
			get { return data; }
		}

		private string fingerPrint;
		public string FingerPrint
		{
			get { return fingerPrint; }
		}

		public override uint ID
		{
			get { return 0x61746164; }
		}

		public DataChunk(BinaryReader Reader) : base(Reader)
		{
		}
		protected override void OnReadBody(BinaryReader Reader)
		{
			byte[] hash;


			data = new byte[this.ContentSize];
			Reader.Read(data, 0, (int)this.ContentSize);

			using (var md5 = MD5.Create())
			{
				hash = md5.ComputeHash(data);
			}
			fingerPrint = BitConverter.ToString(hash).Replace("-", "").ToLower();
		}

		public override void OnDumpBodyToConsole(int Padding)
		{
			Utils.WritePadding(Padding); Console.WriteLine("<DATA>");
		}

		
	}
}
