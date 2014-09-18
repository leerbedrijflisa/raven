using System.Collections.Generic;
using System.Linq;

namespace Lisa.Raven.Parser
{
    public class HtmlParser
    {
	    public static ParsedHtml Parse(string html)
	    {
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
                Value = _currentToken.Name
            };

            var openTagNode = ParseOpenTagNode();
            node.Children.Add(openTagNode);

            var contentNode = ParseContent();
            node.Children.Add(contentNode);

            if (_currentToken.Name == node.Value)
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
                Value = _currentToken.Name
            };

            var openTagNode = ParseOpenTagNode();
			node.Children.Add(openTagNode);

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

                    case TokenType.SelfClosingTag:
                        child = ParseSelfClosingElement();
                        break;

                    default:
                        child = new SyntaxNode
                        {
                            Type = SyntaxNodeType.Error
                        };

                        NextToken();
                        break;
                }

                node.Children.Add(child);
            }

            return node;
        }

        private SyntaxNode ParseOpenTagNode()
        {
            var node = new SyntaxNode
            {
                Type = SyntaxNodeType.OpenTag,
                Value = _currentToken.Name
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
                Value = _currentToken.Name
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
