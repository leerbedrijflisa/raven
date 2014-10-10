namespace Lisa.Raven.Parser
{
	public interface IPipe<in TIn, out TOut>
	{
		TOut Process(TIn value);
	}
}