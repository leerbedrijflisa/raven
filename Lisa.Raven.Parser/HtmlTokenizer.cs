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

					case LexemeType.Text:
						// These tokens in this context are text anyways
					case LexemeType.TagEnd:
						token = ParseText();
						break;

					default:
						throw new NotImplementedException();
				}

				tokens.Add(token);
			}

			return tokens;
		}

		private Token ParseText()
		{
			var token = new Token
			{
				Type = TokenType.Text,
				Value = _currentToken.Source
			};

			NextToken();

			return token;
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

			token.Value = _currentToken.Source.ToLower();

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
			var token = new Token();

			NextToken();
			
			// TODO: Handle more gracefully, this might happen
			if (_currentToken.Type != LexemeType.Text)
				throw new Exception();

			// If the name of the tag starts with a "!", it's a doctype
			var value = _currentToken.Source.ToLower();
			if (!value.StartsWith("!"))
			{
				token.Type = TokenType.OpenTag;
				token.Value = value;
			}
			else
			{
				token.Type = TokenType.Doctype;
				token.Value = value.Substring(1);
			}

			NextToken();

			if (_currentToken.Type == LexemeType.TagEnd)
			{
				NextToken();
			}
			else if (_currentToken.Type == LexemeType.SelfCloseTagEnd)
			{
				// TODO: Handle more gracefully, this might happen
				if (token.Type != TokenType.Doctype)
					token.Type = TokenType.SelfClosingTag;
				else
					throw new Exception();

				NextToken();
			}
			else if (_currentToken.Type == LexemeType.Text)
			{
				//ParseAttributes(token);
			}
			else
			{
				throw new NotImplementedException();
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