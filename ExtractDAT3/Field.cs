using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractDAT3
{
	public struct Field
	{
		public string Name
		{
			get;
			set;
		}
		public object Value
		{
			get;
			set;
		}

		public Field(string Name,object Value)
		{
			this.Name = Name;this.Value = Value;
		}

		public override string ToString()
		{
			return $"{Name} = {Value}";
		}
	}
}
