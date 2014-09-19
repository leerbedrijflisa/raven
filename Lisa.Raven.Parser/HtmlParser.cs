using System;
using System.Collections.Generic;
using System.Linq;

namespace Lisa.Raven.Parser
{
    public class HtmlParser
    {
	    public static ParsedHtml Parse(string html)
	    {
			if(html == null)
				throw new ArgumentNullException("html");

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

        private SyntaxNode ParseElement()
        {
            var node = new SyntaxNode
            {
                Type = SyntaxNodeType.Element,
                Value = _currentToken.Value
            };

            var openTagNode = ParseOpenTagNode();
            node.Children.Add(openTagNode);

            var contentNode = ParseContent();
            node.Children.Add(contentNode);

            if (_currentToken.Value == node.Value)
            {
                var closeTagNode = ParseCloseTagNode();
                node.Children.Add(closeTagNode);
            }

            return node;
        }

        private SyntaxNode ParseSelfClosingElement()
        {
            var node = new SyntaxNode
            {
                Type = SyntaxNodeType.Element,
                Value = _currentToken.Value
            };

            var openTagNode = ParseOpenTagNode();
			node.Children.Add(openTagNode);

			// For convenience we're adding an empty content node
            var contentNode = new SyntaxNode
            {
                Type = SyntaxNodeType.Content
            };
			node.Children.Add(contentNode);

            return node;
        }

        private SyntaxNode ParseContent()
        {
            var node = new SyntaxNode
            {
                Type = SyntaxNodeType.Content
            };

            while (!_endOfSource)
            {
                SyntaxNode child;

                switch (_currentToken.Type)
                {
                    case TokenType.OpenTag:
                        child = ParseElement();
                        break;

                    case TokenType.CloseTag:
                        return node;
						// Does not need a NextToken() as the close tag is not yet handled here

                    case TokenType.SelfClosingTag:
                        child = ParseSelfClosingElement();
                        break;

					case TokenType.Doctype:
		                child = ParseDoctype();
		                break;

					case TokenType.Text:
		                child = ParseText();
		                break;

                    default:
		                throw new NotImplementedException();
                }

                node.Children.Add(child);
            }

            return node;
        }

		private SyntaxNode ParseDoctype()
		{
			var node = ParseOpenTagNode();
			node.Type = SyntaxNodeType.Doctype;

			return node;
		}

	    private SyntaxNode ParseText()
	    {
			var node = new SyntaxNode
			{
				Type = SyntaxNodeType.Text,
				Value = ""
			};

			// Merge all sequential text tokens into this node
		    while (!_endOfSource && _currentToken.Type == TokenType.Text)
		    {
			    node.Value += _currentToken.Value;

				NextToken();
		    }

		    return node;
	    }

	    private SyntaxNode ParseOpenTagNode()
        {
            var node = new SyntaxNode
            {
                Type = SyntaxNodeType.OpenTag,
                Value = _currentToken.Value
            };

            foreach (var attribute in _currentToken.Attributes)
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

        private SyntaxNode ParseCloseTagNode()
        {
            var node = new SyntaxNode
            {
				Type = SyntaxNodeType.CloseTag,
                Value = _currentToken.Value
            };

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

        private IEnumerable<Token> _source;
        private IEnumerator<Token> _sourceEnumerator;
        private Token _currentToken;
        private bool _endOfSource;
    }
}
