using System.Text;

namespace Lisa.Raven.Parser.Html.Lexer
{
	public static class TextLexing
	{
		public static Lexeme LexText(DataWalker<char> walker, LexerData data)
		{
			var lexeme = data.CreateLexeme();
			var source = new StringBuilder();

			while (!walker.AtEnd && (
				// Not <>
				walker.Current != '<' && walker.Current != '>' &&
				// Not Whitespace
				walker.Current != ' ' && walker.Current != '\t' &&
				walker.Current != '\r' && walker.Current != '\n' &&
				// Not special characters
				walker.Current != '=' && walker.Current != '/'))
			{
				source.Append(walker.Current);
				walker.Next();
			}

			lexeme.Type = LexemeType.Text;
			lexeme.Source = source.ToString();
			return lexeme;
		}

		public static Lexeme LexWhitespace(DataWalker<char> walker, LexerData data)
		{
			var lexeme = data.CreateLexeme();
			var source = new StringBuilder();

			while (!walker.AtEnd && (
				// Only Whitespace
				walker.Current == ' ' || walker.Current == '\t' ||
				walker.Current == '\r' || walker.Current == '\n'))
			{
				source.Append(walker.Current);
				walker.Next();
			}

			lexeme.Type = LexemeType.Whitespace;
			lexeme.Source = source.ToString();
			return lexeme;
		}

		public static Lexeme LexEquals(DataWalker<char> walker, LexerData data)
		{
			var lexeme = data.CreateLexeme();

			lexeme.Type = LexemeType.Equals;
			lexeme.Source = "=";
			walker.Next();

			return lexeme;
		}
	}
}