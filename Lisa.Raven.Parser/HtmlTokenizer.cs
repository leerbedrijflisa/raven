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

			NextLexeme();

			while (!_endOfSource)
			{
				var token = new Token
				{
					Line = _currentToken.Line,
					Column = _currentToken.Column
				};

				switch (_currentToken.Type)
				{
					case LexemeType.OpenTagStart:
						token = ParseOpenTag(token);
						break;

					case LexemeType.CloseTagStart:
						token = ParseCloseTag(token);
						break;

					case LexemeType.Text:
						// These tokens in this context are text anyways
					case LexemeType.TagEnd:
						token = ParseText(token);
						break;

					default:
						throw new NotImplementedException();
				}

				tokens.Add(token);
			}

			return tokens;
		}

		private Token ParseText(Token token)
		{
			token.Type = TokenType.Text;
			token.Value = _currentToken.Source;

			NextLexeme();

			return token;
		}

		private Token ParseCloseTag(Token token)
		{
			token.Type = TokenType.CloseTag;

			NextLexeme();

			if (_currentToken.Type != LexemeType.Text)
			{
				throw new Exception();
			}

			token.Value = _currentToken.Source.ToLower();

			NextLexeme();

			if (_currentToken.Type != LexemeType.TagEnd)
			{
				throw new Exception();
			}

			NextLexeme();

			return token;
		}

		private Token ParseOpenTag(Token token)
		{
			NextLexeme();

			// TODO: Handle more gracefully, this might happen
			if (_currentToken.Type != LexemeType.Text)
			{
				throw new Exception();
			}

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

			NextLexeme();

			if (_currentToken.Type == LexemeType.TagEnd)
			{
				NextLexeme();
			}
			else if (_currentToken.Type == LexemeType.SelfCloseTagEnd)
			{
				// TODO: Handle more gracefully, this might happen
				if (token.Type != TokenType.Doctype)
				{
					token.Type = TokenType.SelfClosingTag;
				}
				else
				{
					throw new Exception();
				}

				NextLexeme();
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

		private void NextLexeme()
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