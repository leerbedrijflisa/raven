namespace Lisa.Raven
{
	public class ValidationError
	{
		public ValidationError()
		{
		}

		public ValidationError(string message)
		{
			Message = message;
			Line = -1;
			Column = -1;
		}

		public ValidationError(string message, int line, int column)
		{
			Message = message;
			Line = line;
			Column = column;
		}

		public string Message { get; set; }

		public int Line { get; set; }
		public int Column { get; set; }
	}
}