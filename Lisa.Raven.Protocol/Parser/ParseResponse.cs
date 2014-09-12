namespace Lisa.Raven.Protocol.Parser
{
	public class ParseResponse
	{
		public ApiVersion Version { get; set; }
		public StreamToken[] Stream { get; set; }
	}
}