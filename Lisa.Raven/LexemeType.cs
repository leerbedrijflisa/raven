namespace Lisa.Raven
{
	public enum LexemeType
	{
		OpenTagStart,
		CloseTagStart,
		
		TagEnd,
		SelfCloseTagEnd,

		Text,
		Whitespace,

		Equals,
		Quote
	}
}