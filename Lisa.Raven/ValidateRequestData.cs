﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lisa.Raven
{
	// Example: { "Html": "<blah></blah>", "CheckUrls": ["http://localhost:1262/api/ex/ample"] }
	public class ValidateRequestData
	{
		public string Html { get; set; }
		public IEnumerable<string> CheckUrls { get; set; }
	}
}
