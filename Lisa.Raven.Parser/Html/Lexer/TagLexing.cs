namespace Lisa.Raven.Parser.Html.Lexer
{
	public static class TagLexing
	{
		public static Lexeme LexTagStart(DataWalker<char> walker, LexerData data)
		{
			var lexeme = data.CreateLexeme();

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

		public static Lexeme LexSlash(DataWalker<char> walker, LexerData data)
		{
			var lexeme = data.CreateLexeme();

			walker.Next();

			// If the character after the slash isn't a >
			if (walker.Current != '>')
			{
				// Then it's just a bit of text
				return TextLexing.LexText(walker, data);
			}

			// If it is a >, then we're looking at a closing tag
			lexeme.Type = LexemeType.SelfCloseTagEnd;
			lexeme.Source = "/>";

			walker.Next();
			return lexeme;
		}
	}
}