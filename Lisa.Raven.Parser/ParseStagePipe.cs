using System;
using System.Collections.Generic;

namespace Lisa.Raven.Parser
{
	public class ParseStagePipe<TInData, TOutData, TDictionaryKey, TMetaData>
		: IPipe<IEnumerable<TInData>, IEnumerable<TOutData>>
		where TMetaData : new()
	{
		public ParseStagePipe()
		{
			Handlers = new Dictionary<TDictionaryKey, Func<DataWalker<TInData>, TMetaData, TOutData>>();
		}

		public Func<TInData, TDictionaryKey> SelectLookupKey { get; set; }
		public Dictionary<TDictionaryKey, Func<DataWalker<TInData>, TMetaData, TOutData>> Handlers { get; set; }
		public Func<DataWalker<TInData>, TMetaData, TOutData> DefaultHandler { get; set; }

		public IEnumerable<TOutData> Process(IEnumerable<TInData> inData)
		{
			// Verify function call requirements
			if (inData == null)
			{
				throw new ArgumentNullException("inData");
			}

			// Set up the metadata class
			var data = new TMetaData();

			// Set up the helper class to walk over our data
			var walker = new DataWalker<TInData>(inData);
			walker.Moved += (s, e) => InputMove(this, new ParseStagePipeEventArgs<TInData, TMetaData>(walker, data));

			// Create our output stream
			var outData = new List<TOutData>();

			// Actually perform the lexing
			walker.Next();
			while (!walker.AtEnd)
			{
				Func<DataWalker<TInData>, TMetaData, TOutData> handler;
				var lexeme = Handlers.TryGetValue(SelectLookupKey(walker.Current), out handler)
					? handler(walker, data)
					: DefaultHandler(walker, data);

				outData.Add(lexeme);
			}

			return outData;
		}

		public event EventHandler<ParseStagePipeEventArgs<TInData, TMetaData>> InputMove = (s, e) => { };
	}
}