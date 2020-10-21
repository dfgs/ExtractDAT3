using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WavUtilsLib
{
	public static class Utils
	{

		public static void WritePadding(int Padding)
		{
			for (int t = 0; t < Padding; t++)
			{
				Console.Write("\t");
			}
		}


		public static string Decode(uint Code)
		{
			return Encoding.ASCII.GetString(BitConverter.GetBytes(Code));
		}
		public static string Decode(ushort Code)
		{
			return Encoding.ASCII.GetString(BitConverter.GetBytes(Code));
		}



	}
}
