﻿namespace Lisa.Raven.Protocol
{
	public class ValidationError
	{
		public ValidationError(string message)
		{
			Message = message;
		}

		public string Message { get; set; }
	}
}