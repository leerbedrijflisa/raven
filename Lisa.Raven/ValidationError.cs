namespace Lisa.Raven
{
	public class ValidationError
	{
		public ValidationError()
		{
		}

		public ValidationError(ErrorCategory category, string message)
		{
			Category = category;
			Message = message;
			Line = -1;
			Column = -1;
		}

		public ValidationError(ErrorCategory category, string message, int line, int column)
		{
			Category = category;
			Message = message;
			Line = line;
			Column = column;
		}

		public ErrorCategory Category { get; set; }
		public string Message { get; set; }

		public int Line { get; set; }
		public int Column { get; set; }
	}
}