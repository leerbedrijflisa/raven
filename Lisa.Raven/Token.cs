using System.Collections.Generic;

namespace Lisa.Raven
{
	public class Token
	{
		public TokenType Type { get; set; }
		public string Value { get; set; }
		public IEnumerable<Token> Children { get; set; }
	}
}