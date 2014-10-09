using System;
using System.Collections.Generic;

namespace Lisa.Raven.Parser.Html.Lexer
{
	public class LexerPipe : IPipe<string, IEnumerable<Lexeme>>
	{
		public IEnumerable<Lexeme> Process(string html)
		{
			// Verify function call requirements
			if (html == null)
			{
				throw new ArgumentNullException("html");
			}

			// Set up the lexing metadata class
			var data = new LexerData();

			// Set up the helper class to walk over our data
			var walker = new DataWalker<char>(html);
			walker.Moved += (s, e) =>
			{
				if (walker.Current == '\n')
				{
					data.CurrentLine++;
					data.CurrentColumn = 1;
				}
				else
				{
					data.CurrentColumn++;
				}
			};

			// Create our token output stream
			var tokens = new List<Lexeme>();

			walker.Next();
			while (!walker.AtEnd)
			{
				Lexeme lexeme;

				switch (walker.Current)
				{
					case '<':
						lexeme = TagLexing.LexTagStart(walker, data);
						break;

					case '>':
						lexeme = TagLexing.LexTagEnd(walker, data);
						break;

					case '/':
						lexeme = TagLexing.LexSlash(walker, data);
						break;

					case '=':
						lexeme = TextLexing.LexEquals(walker, data);
						break;

					case '\t':
					case '\n':
					case '\r':
					case ' ':
						lexeme = TextLexing.LexWhitespace(walker, data);
						break;

					default:
						lexeme = TextLexing.LexText(walker, data);
						break;
				}

				tokens.Add(lexeme);
			}

			return tokens;
		}
	}
}