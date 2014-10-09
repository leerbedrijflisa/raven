namespace Lisa.Raven.Parser.Html.Lexer
{
	public static class TagLexing
	{
		public static Lexeme LexTagStart(DataWalker<char> walker)
		{
			var lexeme = new Lexeme
			{
				//Line = walker.CurrentLine,
				//Column = walker.CurrentColumn
			};

			walker.Next();

			if (walker.Current == '/')
			{
				walker.Next();
				lexeme.Type = LexemeType.CloseTagStart;
				lexeme.Source = "</";
			}
			else
			{
				lexeme.Type = LexemeType.OpenTagStart;
				lexeme.Source = "<";
			}

			return lexeme;
		}
	}
}