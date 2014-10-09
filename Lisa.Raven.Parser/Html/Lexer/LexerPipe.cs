using System;
using System.Collections.Generic;

namespace Lisa.Raven.Parser.Html.Lexer
{
	using HandlerFunc = Func<DataWalker<char>, LexerData, Lexeme>;

	public class LexerPipe : IPipe<string, IEnumerable<Lexeme>>
	{
		public LexerPipe()
		{
			Handlers = new Dictionary<char, HandlerFunc>();
		}

		public Dictionary<char, HandlerFunc> Handlers { get; set; }
		public HandlerFunc DefaultHandler { get; set; }

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
			walker.Moved += (s, e) => IncrementData(data, walker.Current);

			// Create our token output stream
			var tokens = new List<Lexeme>();

			// Actually perform the lexing
			walker.Next();
			while (!walker.AtEnd)
			{
				HandlerFunc handler;
				var lexeme = Handlers.TryGetValue(walker.Current, out handler)
					? handler(walker, data)
					: DefaultHandler(walker, data);

				tokens.Add(lexeme);
			}

			return tokens;
		}

		private static void IncrementData(LexerData data, char c)
		{
			if (c == '\n')
			{
				data.CurrentLine++;
				data.CurrentColumn = 1;
			}
			else
			{
				data.CurrentColumn++;
			}
		}
	}
}