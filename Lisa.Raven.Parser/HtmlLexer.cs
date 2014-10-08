using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Lisa.Raven.Parser
{
	public class HtmlLexer
	{
		private char _currentCharacter = '\0';
		private int _currentColumn = 1;
		private int _currentLine = 1;
		private bool _endOfSource;
		private string _source;
		private int _sourceIndex = -1;

		public IEnumerable<Lexeme> Lex(string html)
		{
			if (html == null)
			{
				throw new ArgumentNullException("html");
			}

			var tokens = new List<Lexeme>();

			_source = html;
			NextCharacter();

			while (!_endOfSource)
			{
				var lexeme = new Lexeme
				{
					Line = _currentLine,
					Column = _currentColumn
				};

				switch (_currentCharacter)
				{
					case '<':
						lexeme = LexTagStart(lexeme);
						break;

					case '/':
						lexeme = LexSlash(lexeme);
						break;

					case '>':
						lexeme = LexTagEnd(lexeme);
						break;

					case '\t':
					case '\n':
					case '\r':
					case ' ':
						lexeme = LexWhitespace(lexeme);
						break;

					case '=':
						lexeme = LexEquals(lexeme);
						break;

					default:
						lexeme = LexText(lexeme);
						break;
				}

				tokens.Add(lexeme);
			}

			return tokens;
		}

		private Lexeme LexText(Lexeme lexeme)
		{
			var source = new StringBuilder();

			while (!_endOfSource)
			{
				if (_currentCharacter == '/')
				{
					if (PeekCharacter() == '>')
					{
						break;
					}

					source.Append(_currentCharacter);
					NextCharacter();
				}
				else if (
					// Not <>
					_currentCharacter != '<' && _currentCharacter != '>' &&
					// Not Whitespace
					_currentCharacter != ' ' && _currentCharacter != '\t' &&
					_currentCharacter != '\r' && _currentCharacter != '\n' &&
					// Not special characters
					_currentCharacter != '=')
				{
					source.Append(_currentCharacter);
					NextCharacter();
				}
				else
				{
					break;
				}
			}

			lexeme.Type = LexemeType.Text;
			lexeme.Source = source.ToString();
			return lexeme;
		}

		private Lexeme LexWhitespace(Lexeme lexeme)
		{
			var source = new StringBuilder();
			
			while (!_endOfSource)
			{
				if (_currentCharacter == '/')
				{
					if (PeekCharacter() == '>')
					{
						break;
					}

					source.Append(_currentCharacter);
					NextCharacter();
				}
				else if (// Only Whitespace
					_currentCharacter == ' ' || _currentCharacter == '\t' ||
					_currentCharacter == '\r' || _currentCharacter == '\n')
				{
					source.Append(_currentCharacter);
					NextCharacter();
				}
				else
				{
					break;
				}
			}

			lexeme.Type = LexemeType.Whitespace;
			lexeme.Source = source.ToString();
			return lexeme;
		}

		private Lexeme LexSlash(Lexeme lexeme)
		{
			if (PeekCharacter() != '>')
			{
				return LexText(lexeme);
			}

			NextCharacter();
			NextCharacter();

			lexeme.Type = LexemeType.SelfCloseTagEnd;
			lexeme.Source = "/>";
			return lexeme;
		}

		private Lexeme LexTagStart(Lexeme lexeme)
		{
			NextCharacter();

			if (_currentCharacter == '/')
			{
				NextCharacter();
				lexeme.Type = LexemeType.CloseTagStart;
				lexeme.Source = "</";
			}
			else
			{
				lexeme.Type = LexemeType.OpenTagStart;
				lexeme.Source = "<";
			}

			return lexeme;
		}

		private Lexeme LexTagEnd(Lexeme lexeme)
		{
			lexeme.Type = LexemeType.TagEnd;
			lexeme.Source = _currentCharacter.ToString(CultureInfo.InvariantCulture);
			NextCharacter();

			return lexeme;
		}

		private Lexeme LexEquals(Lexeme lexeme)
		{
			lexeme.Type = LexemeType.Equals;
			lexeme.Source = _currentCharacter.ToString(CultureInfo.InvariantCulture);
			NextCharacter();

			return lexeme;
		}

		private char PeekCharacter()
		{
			if (_sourceIndex + 1 < _source.Length)
			{
				return _source[_sourceIndex + 1];
			}
			return '\0';
		}

		private void NextCharacter()
		{
			_sourceIndex++;

			if (_sourceIndex < _source.Length)
			{
				_currentCharacter = _source[_sourceIndex];

				if (_currentCharacter == '\n')
				{
					_currentColumn = 0;
					_currentLine++;
				}
				else
				{
					_currentColumn++;
				}
			}
			else
			{
				_endOfSource = true;
			}
		}
	}
}