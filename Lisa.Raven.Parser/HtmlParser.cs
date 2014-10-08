using System;
using System.Collections.Generic;
using System.Linq;

namespace Lisa.Raven.Parser
{
	public class HtmlParser
	{
		private Token _currentToken;
		private bool _endOfSource;
		private IEnumerable<Token> _source;
		private IEnumerator<Token> _sourceEnumerator;

		public static ParsedHtml Parse(string html)
		{
			if (html == null)
			{
				throw new ArgumentNullException("html");
			}

			var lexer = new HtmlLexer();
			var lexemes = lexer.Lex(html);

			var tokenizer = new HtmlTokenizer();
			var tokens = tokenizer.Tokenize(lexemes).ToArray();

			var parser = new HtmlParser();
			var tree = parser.Parse(tokens);

			var parsed = new ParsedHtml
			{
				Tokens = tokens,
				Tree = tree
			};
			return parsed;
		}

		public SyntaxNode Parse(IEnumerable<Token> tokens)
		{
			_source = tokens;
			_sourceEnumerator = _source.GetEnumerator();
			NextToken();

			var root = ParseContent();
			root.Type = SyntaxNodeType.DocumentRoot;

			return root;
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

			foreach (var attribute in _currentToken.Data)
			{
				var attributeNode = new SyntaxNode
				{
					Type = SyntaxNodeType.Attribute,
					Children = new List<SyntaxNode>
					{
						new SyntaxNode
						{
							Type = SyntaxNodeType.AttributeName,
							Value = attribute.Name
						},
						new SyntaxNode
						{
							Type = SyntaxNodeType.AttributeValue,
							Value = attribute.Value
						}
					}
				};

				// TODO: Add the attribute node
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