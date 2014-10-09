﻿using System;
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

			var parser = CreateParser();
			return parser(html);
		}

		private static Func<string, ParsedHtml> CreateParser()
		{
			return PipelineBuilder
				.Start(CreateLexerPipe())
				.Chain(new TokenizerPipe())
				.End(new ParserPipe());
		}

		private static IPipe<string, IEnumerable<Lexeme>> CreateLexerPipe()
		{
			var lexer = new LexerPipe();

			lexer.Handlers.Add('<', TagLexing.LexTagStart);
			lexer.Handlers.Add('>', TagLexing.LexTagEnd);

			lexer.Handlers.Add('/', TagLexing.LexSlash);

			lexer.Handlers.Add('=', TextLexing.LexEquals);

			lexer.Handlers.Add(' ', TextLexing.LexWhitespace);
			lexer.Handlers.Add('\t', TextLexing.LexWhitespace);
			lexer.Handlers.Add('\n', TextLexing.LexWhitespace);
			lexer.Handlers.Add('\r', TextLexing.LexWhitespace);
			
			lexer.DefaultHandler = TextLexing.LexText;

			return lexer;
		}
	}
}