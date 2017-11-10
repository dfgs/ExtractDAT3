using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WavUtilsLib
{
	public class WavChunk:ContainerChunk
	{
		public override uint ID
		{
			get {return 0x46464952;	}
		}

		
		
		public WavChunk(BinaryReader Reader) :base(Reader)
		{

		}

		

		


		

		
	}
}
