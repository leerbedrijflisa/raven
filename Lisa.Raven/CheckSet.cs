using System.Collections.Generic;

namespace Lisa.Raven
{
	public class CheckSet
	{
		public string Code { get; set; }
		public string Name { get; set; }

		public bool Locked { get; set; }

		public IEnumerable<Check> Checks { get; set; }
	}
}