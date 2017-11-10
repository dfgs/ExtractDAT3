using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavUtilsLib
{
	public class ListChunk : ContainerChunk
	{
		public override uint ID
		{
			get { return 0x5453494c; }
		}

		
		public ListChunk(BinaryReader Reader):base(Reader)
		{
		}
		

	}
}
