using System.Collections.Generic;

namespace Lisa.Raven
{
	public class ParsedHtml
	{
		public IEnumerable<ParseError> Errors { get; set; }
		public Token Document { get; set; }
	}
}