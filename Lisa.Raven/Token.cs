using System.Collections.Generic;

namespace Lisa.Raven
{
	public class Token
	{
		public Token()
		{
			Attributes = new List<TokenAttribute>();
		}

		public string Name { get; set; }
		public TokenType Type { get; set; }
		public ICollection<TokenAttribute> Attributes { get; set; }
	}
}