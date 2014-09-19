namespace Lisa.Raven
{
	public class Lexeme
	{
		public LexemeType Type { get; set; }
		public string Source { get; set; }

		public int Line { get; set; }
		public int Column { get; set; }
	}
}