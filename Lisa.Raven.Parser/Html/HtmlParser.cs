using System;
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

			var pipeline = PipelineBuilder
				.Start(new LexerPipe())
				.Chain(new TokenizerPipe())
				.End(new ParserPipe());

			return pipeline(html);
		}
	}
}