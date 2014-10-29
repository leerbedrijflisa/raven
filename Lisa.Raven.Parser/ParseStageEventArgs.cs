namespace Lisa.Raven.Parser
{
	public class ParseStageEventArgs<TInData, TData>
	{
		public ParseStageEventArgs(DataWalker<TInData> walker, TData data)
		{
			Walker = walker;
			Data = data;
		}

		public DataWalker<TInData> Walker { get; set; }
		public TData Data { get; set; }
	}
}