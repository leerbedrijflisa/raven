using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lisa.Raven.Protocol.Parser
{
	public class ParseResponse
	{
		public Version Version { get; set; }
		public StreamToken[] Stream { get; set; }
	}
}
