using System;
using System.Collections.Generic;
using System.Linq;

namespace Lisa.Raven.Parser.Html.Parser
{
	public class Parser
	{
		private Token _currentToken;
		private bool _endOfSource;
		private IEnumerable<Token> _source;
		private IEnumerator<Token> _sourceEnumerator;

		public SyntaxNode Process(IEnumerable<Token> tokens)
		{
			tokens = tokens.ToArray();

			_source = tokens;
			_sourceEnumerator = _source.GetEnumerator();
			NextToken();

			var tree = ParseContent();
			tree.Type = SyntaxNodeType.DocumentRoot;

			return tree;
		}

		private SyntaxNode ParseElement(SyntaxNode node)
		{
			node.Type = SyntaxNodeType.Element;
			node.Value = _currentToken.Value;

			var openTagNode = ParseOpenTagNode(new SyntaxNode
			{
				Line = _currentToken.Line,
				Column = _currentToken.Column
			});
			node.Children.Add(openTagNode);

			var contentNode = ParseContent();
			node.Children.Add(contentNode);

			if (_currentToken.Value == node.Value)
			{
				var closeTagNode = ParseCloseTagNode(new SyntaxNode
				{
					Line = _currentToken.Line,
					Column = _currentToken.Column
				});
				node.Children.Add(closeTagNode);
			}

			return node;
		}

		private SyntaxNode ParseSelfClosingElement(SyntaxNode node)
		{
			node.Type = SyntaxNodeType.Element;
			node.Value = _currentToken.Value;

			var openTagNode = ParseOpenTagNode(new SyntaxNode
			{
				Line = _currentToken.Line,
				Column = _currentToken.Column
			});
			node.Children.Add(openTagNode);

			// Previously a content node was added here, this is no longer the case.

			return node;
		}

		private SyntaxNode ParseContent()
		{
			var node = new SyntaxNode
			{
				Type = SyntaxNodeType.Content,
				Line = _currentToken.Line,
				Column = _currentToken.Column
			};

			while (!_endOfSource)
			{
				var child = new SyntaxNode
				{
					Line = _currentToken.Line,
					Column = _currentToken.Column
				};

				switch (_currentToken.Type)
				{
					case TokenType.OpenTag:
						child = ParseElement(child);
						break;

					case TokenType.CloseTag:
						return node;
						// Does not need a NextToken() as the close tag is not yet handled here

					case TokenType.SelfClosingTag:
						child = ParseSelfClosingElement(child);
						break;

					case TokenType.Doctype:
						child = ParseDoctype(child);
						break;

					case TokenType.Text:
						child = ParseText(child);
						break;

					default:
						throw new NotImplementedException();
				}

				node.Children.Add(child);
			}

			return node;
		}

		private SyntaxNode ParseDoctype(SyntaxNode node)
		{
			node = ParseOpenTagNode(node);
			node.Type = SyntaxNodeType.Doctype;

			return node;
		}

		private SyntaxNode ParseText(SyntaxNode node)
		{
			node.Type = SyntaxNodeType.Text;
			node.Value = "";

			// Merge all sequential text tokens into this node
			while (!_endOfSource && _currentToken.Type == TokenType.Text)
			{
				node.Value += _currentToken.Value;

				NextToken();
			}

			return node;
		}

		private SyntaxNode ParseOpenTagNode(SyntaxNode node)
		{
			node.Type = SyntaxNodeType.OpenTag;
			node.Value = _currentToken.Value;

			foreach (var attribute in _currentToken.Data.Where(d => d.Type == TokenDataType.Attribute))
			{
				var attributeNode = new SyntaxNode
				{
					Type = SyntaxNodeType.Attribute,
					Line = _currentToken.Line,
					Column = _currentToken.Column
				};

				// It will always have a name, so add it
				attributeNode.Children.Add(new SyntaxNode
				{
					Type = SyntaxNodeType.AttributeName,
					Value = attribute.Name
				});

				// If it has a value, add that as well
				if (attribute.Value != null)
				{
					attributeNode.Children.Add(new SyntaxNode
					{
						Type = SyntaxNodeType.AttributeValue,
						Value = attribute.Value
					});
				}

				// Add our new attribute node to the open tag
				node.Children.Add(attributeNode);
			}

			NextToken();

			return node;
		}

		private SyntaxNode ParseCloseTagNode(SyntaxNode node)
		{
			node.Type = SyntaxNodeType.CloseTag;
			node.Value = _currentToken.Value;

			NextToken();

			return node;
		}

		private void NextToken()
		{
			if (!_sourceEnumerator.MoveNext())
			{
				_endOfSource = true;
			}
			else
			{
				_currentToken = _sourceEnumerator.Current;
			}
		}
	}
}