namespace Lisa.Raven
{
	public class TokenAttribute
	{
		public TokenAttribute()
		{
		}

		public TokenAttribute(string name, string value)
		{
			Name = name;
			Value = value;
		}

		public string Name { get; set; }
		public string Value { get; set; }
	}
}