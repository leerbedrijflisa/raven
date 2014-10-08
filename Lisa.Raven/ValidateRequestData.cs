using System.Collections.Generic;

namespace Lisa.Raven
{
	public class ValidateRequestData
	{
		public string Html { get; set; }
		public IEnumerable<string> CheckUrls { get; set; }
		public IEnumerable<CheckSet> CheckSets { get; set; }
	}
}