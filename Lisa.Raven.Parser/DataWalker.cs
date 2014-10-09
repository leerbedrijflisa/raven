using System;
using System.Collections.Generic;

namespace Lisa.Raven.Parser
{
	public class /*Luke*/ DataWalker<TData>
	{
		private readonly IEnumerator<TData> _enumerator;

		public DataWalker(IEnumerable<TData> data)
		{
			_enumerator = data.GetEnumerator();
		}

		public bool AtEnd { get; private set; }
		public TData Current { get; private set; }

		public event EventHandler Moved = (s, e) => { };

		public bool Next()
		{
			if (!_enumerator.MoveNext())
			{
				AtEnd = true;
				return false;
			}

			Moved(this, EventArgs.Empty);
			Current = _enumerator.Current;
			return true;
		}
	}
}