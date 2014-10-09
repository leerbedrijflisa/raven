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

		public bool Next()
		{
			if (!_enumerator.MoveNext())
			{
				AtEnd = true;
				return false;
			}
			Current = _enumerator.Current;
			return true;
		}
	}
}