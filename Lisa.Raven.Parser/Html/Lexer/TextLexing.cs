using System.Text;

namespace Lisa.Raven.Parser.Html.Lexer
{
	public static class TextLexing
	{
		public static Lexeme LexText(DataWalker<char> walker, LexerData data)
		{
			var lexeme = data.CreateLexeme();
			var source = new StringBuilder();

			while (!walker.AtEnd)
			{
				if (
					// Not <>
					walker.Current != '<' && walker.Current != '>' &&
					// Not Whitespace
					walker.Current != ' ' && walker.Current != '\t' &&
					walker.Current != '\r' && walker.Current != '\n' &&
					// Not special characters
					walker.Current != '=' && walker.Current != '/')
				{
					source.Append(walker.Current);
					walker.Next();
				}
				else
				{
					break;
				}
			}

			lexeme.Type = LexemeType.Text;
			lexeme.Source = source.ToString();
			return lexeme;
		}
	}
}