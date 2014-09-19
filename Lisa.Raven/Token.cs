using System.Collections.Generic;

namespace Lisa.Raven
{
	public class Token
	{
		public Token()
		{
			Attributes = new List<TokenAttribute>();
		}

		public TokenType Type { get; set; }
		public string Value { get; set; }
		public ICollection<TokenAttribute> Attributes { get; set; }

		public int Line { get; set; }
		public int Column { get; set; }
	}
}