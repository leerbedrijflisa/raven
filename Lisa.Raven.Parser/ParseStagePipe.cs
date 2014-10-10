using System;
using System.Collections.Generic;

namespace Lisa.Raven.Parser
{
	public class ParseStagePipe<TInData, TData> : IPipe<IEnumerable<TInData>, IEnumerable<Lexeme>> where TData : new()
	{
		public ParseStagePipe()
		{
			Handlers = new Dictionary<TInData, Func<DataWalker<TInData>, TData, Lexeme>>();
		}

		public Dictionary<TInData, Func<DataWalker<TInData>, TData, Lexeme>> Handlers { get; set; }
		public Func<DataWalker<TInData>, TData, Lexeme> DefaultHandler { get; set; }

		public IEnumerable<Lexeme> Process(IEnumerable<TInData> inData)
		{
			// Verify function call requirements
			if (inData == null)
			{
				throw new ArgumentNullException("inData");
			}

			// Set up the lexing metadata class
			var data = new TData();

			// Set up the helper class to walk over our data
			var walker = new DataWalker<TInData>(inData);
			walker.Moved += (s, e) => InputMove(this, new ParseStagePipeEventArgs<TInData, TData>(walker, data));

			// Create our token output stream
			var outData = new List<Lexeme>();

			// Actually perform the lexing
			walker.Next();
			while (!walker.AtEnd)
			{
				Func<DataWalker<TInData>, TData, Lexeme> handler;
				var lexeme = Handlers.TryGetValue(walker.Current, out handler)
					? handler(walker, data)
					: DefaultHandler(walker, data);

				outData.Add(lexeme);
			}

			return outData;
		}

		public event EventHandler<ParseStagePipeEventArgs<TInData, TData>> InputMove = (s, e) => { };
	}
}