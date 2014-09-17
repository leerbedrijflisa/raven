using System.Collections.Generic;

namespace Lisa.Raven
{
	public class ParsedHtml
	{
		public IEnumerable<ParseError> Errors { get; set; }
		public IEnumerable<StreamToken> TokenStream { get; set; }
	}
}