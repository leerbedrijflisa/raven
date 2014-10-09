namespace Lisa.Raven.Parser.Html.Lexer
{
	public class LexerData
	{
		public int CurrentLine { get; set; }
		public int CurrentColumn { get; set; }

		public Lexeme CreateLexeme()
		{
			return new Lexeme
			{
				Line = CurrentLine,
				Column = CurrentColumn
			};
		}
	}
}