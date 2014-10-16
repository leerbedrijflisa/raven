namespace Lisa.Raven.Parser.Html.Tokenizer
{
	public static class TextTokenizing
	{
		public static Token TokenizeText(DataWalker<Lexeme> walker, object data)
		{
			var token = new Token
			{
				Line = walker.Current.Line,
				Column = walker.Current.Column,
				Type = TokenType.Text,
				Value = walker.Current.Source
			};

			walker.Next();
			return token;
		}
	}
}