using System.Collections.Generic;

namespace Lisa.Raven
{
	public class ValidateRequestData
	{
		public string Html { get; set; }
		public IEnumerable<Check> Checks { get; set; }
	}
}