using System;
using System.Collections.Generic;

namespace Lisa.Raven.Parser
{
	public class HtmlTokenizer
	{
		private Lexeme _currentToken;
		private bool _endOfSource;
		private IEnumerable<Lexeme> _source;
		private IEnumerator<Lexeme> _sourceEnumerator;

		public IEnumerable<Token> Tokenize(IEnumerable<Lexeme> lexemes)
		{
			var tokens = new List<Token>();
			_source = lexemes;
			_sourceEnumerator = _source.GetEnumerator();

			NextToken();

			while (!_endOfSource)
			{
				Token token;

				switch (_currentToken.Type)
				{
					case LexemeType.OpenTagStart:
						token = ParseOpenTag();
						break;

					case LexemeType.CloseTagStart:
						token = ParseCloseTag();
						break;

					default:
						throw new NotImplementedException();
				}

				tokens.Add(token);
			}

			return tokens;
		}

		private Token ParseCloseTag()
		{
			var token = new Token
			{
				Type = TokenType.CloseTag
			};

			NextToken();

			if (_currentToken.Type != LexemeType.Text)
			{
				throw new Exception();
			}

			token.Name = _currentToken.Source;

			NextToken();

			if (_currentToken.Type != LexemeType.TagEnd)
			{
				throw new Exception();
			}

			NextToken();

			return token;
		}

		private Token ParseOpenTag()
		{
			var token = new Token
			{
				Type = TokenType.OpenTag
			};

			NextToken();

			if (_currentToken.Type != LexemeType.Text)
			{
				throw new Exception();
			}

			token.Name = _currentToken.Source;

			NextToken();

			if (_currentToken.Type == LexemeType.TagEnd)
			{
				NextToken();
			}
			else if (_currentToken.Type == LexemeType.SelfCloseTagEnd)
			{
				token.Type = TokenType.SelfClosingTag;
				NextToken();
			}
			else if (_currentToken.Type == LexemeType.Text)
			{
				//ParseAttributes(token);
			}
			else
			{
				throw new Exception();
			}

			return token;
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