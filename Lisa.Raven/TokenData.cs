namespace Lisa.Raven
{
	public class TokenData
	{
		public TokenData()
		{
		}

		public TokenData(TokenDataType type, string name, string value)
		{
			Type = type;
			Name = name;
			Value = value;
		}

		public TokenDataType Type { get; set; }
		public string Name { get; set; }
		public string Value { get; set; }
	}
}