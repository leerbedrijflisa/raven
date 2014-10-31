using System.Collections.Generic;

namespace Lisa.Raven
{
	public class SyntaxNode
	{
		public SyntaxNode()
		{
			Children = new List<SyntaxNode>();
		}

		public SyntaxNodeType Type { get; set; }

		public string Value { get; set; }
		public ICollection<SyntaxNode> Children { get; set; }

		public int Line { get; set; }
		public int Column { get; set; }

		public string Source { get; set; }
	}
}