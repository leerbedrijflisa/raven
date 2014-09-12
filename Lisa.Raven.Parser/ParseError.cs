namespace Lisa.Raven.Parser
{
	public class ParseError
	{
		public ParseError(string message)
		{
			Message = message;
		}

		public string Message { get; set; }
	}
}
