using System;
using System.Collections.Generic;
using Lisa.Raven.Parser.Html.Lexer;
using Lisa.Raven.Parser.Html.Parser;
using Lisa.Raven.Parser.Html.Tokenizer;

namespace Lisa.Raven.Parser.Html
{
	public static class HtmlParser
	{
		public static ParsedHtml Parse(string html)
		{
			if (html == null)
			{
				throw new ArgumentNullException("html");
			}

			var parser = Create();
			return parser(html);
		}

		public static Func<string, ParsedHtml> Create()
		{
			// All the different CreateXPipe functions configure a parser stage
			return PipelineBuilder
				.Start(CreateLexerPipe())
				.Chain(new TokenizerPipe())
				.End(new ParserPipe());
		}

		private static IPipe<string, IEnumerable<Lexeme>> CreateLexerPipe()
		{
			var lexer = new ParseStagePipe<char, LexerData>();

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
	}
}