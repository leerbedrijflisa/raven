using System.Collections.Generic;
using Lisa.Raven.Protocol;

namespace Lisa.Raven.Parser
{
	public class ParsedHtml
	{
		public IEnumerable<ParseError> Errors { get; set; }
		public IEnumerable<StreamToken> TokenStream { get; set; }
	}
}
