using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavUtilsLib
{
	public class FormatChunk : Chunk
	{
		public override uint ID
		{
			get	{return 0x20746d66; }
		}

		private ushort audioFormat;
		public ushort AudioFormat
		{
			get { return audioFormat; }
		}

		private ushort numChannels;
		public ushort NumChannels
		{
			get { return numChannels; }
		}

		private uint sampleRate;
		public uint SampleRate
		{
			get { return sampleRate; }
		}

		private uint byteRate;
		public uint ByteRate // SampleRate * NumChannels * BitsPerSample/8
		{
			get { return byteRate; }
		}

		private ushort blockAlign;
		public ushort BlockAlign // NumChannels * BitsPerSample/8
		{
			get { return blockAlign; }
		}

		private ushort bitsPerSample;
		public ushort BitsPerSample // 8 bits = 8, 16 bits = 16, etc
		{
			get { return bitsPerSample; }
		}

		private ushort extraParamSize;
		public ushort ExtraParamSize
		{
			get { return extraParamSize; }
		}

		private byte[] extraParams;
		public byte[] ExtraParams
		{
			get { return extraParams; }
		}

		public FormatChunk(BinaryReader Reader) : base(Reader)
		{
			
		}
		protected override void OnReadBody(BinaryReader Reader)
		{
			audioFormat = Reader.ReadUInt16();
			numChannels = Reader.ReadUInt16();
			sampleRate = Reader.ReadUInt32();
			byteRate = Reader.ReadUInt32();
			blockAlign = Reader.ReadUInt16();
			bitsPerSample = Reader.ReadUInt16();
			if (ContentSize > 16)
			{
				extraParamSize = Reader.ReadUInt16();
				extraParams = new byte[extraParamSize];
				Reader.Read(extraParams, 0, extraParamSize);
			}
			else
			{
				extraParamSize = 0;
				extraParams = null;
			}
		}



		public override void OnDumpBodyToConsole(int Padding)
		{
			Utils.WritePadding(Padding); Console.WriteLine("AudioFormat = " + audioFormat);
			Utils.WritePadding(Padding); Console.WriteLine("NumChannels = " + numChannels);
			Utils.WritePadding(Padding); Console.WriteLine("SampleRate = " + sampleRate);
			Utils.WritePadding(Padding); Console.WriteLine("ByteRate = " + byteRate);
			Utils.WritePadding(Padding); Console.WriteLine("BlockAlign = " + blockAlign);
			Utils.WritePadding(Padding); Console.WriteLine("BitsPerSample = " + bitsPerSample);
			Utils.WritePadding(Padding); Console.WriteLine("ExtraParamSize = " + extraParamSize);

		}


	}
}
