using System;
using System.Linq;
using Lisa.Raven.Parser.Html.Lexer;
using Lisa.Raven.Parser.Html.Tokenizer;

namespace Lisa.Raven.Parser.Html
{
	public static class HtmlParser
	{
		public static Func<string, ParsedHtml> Create()
		{
			// Set up the different parser components
			var lexer = CreateLexer();
			var tokenizer = CreateTokenizer();
			var parser = new Parser.Parser();

			// The actual resulting parser function
			return s =>
			{
				var lexemes = lexer.Process(s);
				var tokens = tokenizer.Process(lexemes).ToArray();
				var tree = parser.Process(tokens);

				var parsedHtml = new ParsedHtml
				{
					Tokens = tokens,
					Tree = tree
				};

				return parsedHtml;
			};
		}

		private static ParseStage<char, Lexeme, char, LexerData> CreateLexer()
		{
			var lexer = new ParseStage<char, Lexeme, char, LexerData>();

			// Set up the transform function the stage needs to look up stuff
			lexer.SelectLookupKey = c => c;

			// Set up the different handlers for different characters
			lexer.Handlers.Add('<', TagLexing.LexTagStart);
			lexer.Handlers.Add('>', TagLexing.LexTagEnd);
			lexer.Handlers.Add('/', TagLexing.LexSlash);

			lexer.Handlers.Add('=', TextLexing.LexEquals);
			lexer.Handlers.Add('"', TextLexing.LexQuote);
			lexer.Handlers.Add('\'', TextLexing.LexQuote);

			lexer.Handlers.Add(' ', TextLexing.LexWhitespace);
			lexer.Handlers.Add('\t', TextLexing.LexWhitespace);
			lexer.Handlers.Add('\n', TextLexing.LexWhitespace);
			lexer.Handlers.Add('\r', TextLexing.LexWhitespace);

			lexer.DefaultHandler = TextLexing.LexText;

			// Set up custom on-move functionality
			lexer.InputMove += (s, e) =>
			{
				if (e.Walker.Current == '\n')
				{
					e.Data.CurrentLine++;
					e.Data.CurrentColumn = 1;
				}
				else
				{
					e.Data.CurrentColumn++;
				}
			};

			return lexer;
		}

		private static ParseStage<Lexeme, Token, LexemeType, object> CreateTokenizer()
		{
			var tokenizer = new ParseStage<Lexeme, Token, LexemeType, object>();

			// Set up the transform function the stage needs to look up stuff
			tokenizer.SelectLookupKey = l => l.Type;

			// Set up the different handlers for different token types
			tokenizer.Handlers.Add(LexemeType.OpenTagStart, TagTokenizing.TokenizeOpenTag);
			tokenizer.Handlers.Add(LexemeType.CloseTagStart, TagTokenizing.TokenizeCloseTag);

			// TODO: Merge all of these together in this stage
			tokenizer.Handlers.Add(LexemeType.Text, TextTokenizing.TokenizeText);
			// The following tokens in this context are seen as text as well
			tokenizer.Handlers.Add(LexemeType.TagEnd, TextTokenizing.TokenizeText);
			tokenizer.Handlers.Add(LexemeType.Equals, TextTokenizing.TokenizeText);
			tokenizer.Handlers.Add(LexemeType.Quote, TextTokenizing.TokenizeText);
			tokenizer.Handlers.Add(LexemeType.Whitespace, TextTokenizing.TokenizeText);

			tokenizer.DefaultHandler = (w, d) => { throw new NotImplementedException(); };

			return tokenizer;
		}
	}
}