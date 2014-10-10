namespace Lisa.Raven.Parser
{
	public class ParseStagePipeEventArgs<TInData, TData>
	{
		public ParseStagePipeEventArgs(DataWalker<TInData> walker, TData data)
		{
			Walker = walker;
			Data = data;
		}

		public DataWalker<TInData> Walker { get; set; }
		public TData Data { get; set; }
	}
}