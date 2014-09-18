using System.Collections.Generic;

namespace Lisa.Raven
{
	public class ParsedHtml
	{
		public IEnumerable<Token> Tokens { get; set; }
		public SyntaxNode Tree { get; set; }
	}
}