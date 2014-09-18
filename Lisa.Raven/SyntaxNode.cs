using System.Collections.Generic;

namespace Lisa.Raven
{
	public class SyntaxNode
	{
		public SyntaxNode()
		{
			Children = new List<SyntaxNode>();
		}

		public string Value { get; set; }
		public SyntaxNodeType Type { get; set; }
		public ICollection<SyntaxNode> Children { get; set; }
	}
}