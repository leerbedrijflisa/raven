using System.Collections.Generic;

namespace Lisa.Raven.Protocol
{
	public class ValidateRequestData
	{
		public string Html { get; set; }
		public IEnumerable<string> CheckUrls { get; set; }
	}
}
