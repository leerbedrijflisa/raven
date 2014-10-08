using System.Collections.Generic;

namespace Lisa.Raven
{
	public class Token
	{
		public Token()
		{
			Data = new List<TokenData>();
		}

		public TokenType Type { get; set; }
		public string Value { get; set; }
		public ICollection<TokenData> Data { get; set; }

		public int Line { get; set; }
		public int Column { get; set; }
	}
}