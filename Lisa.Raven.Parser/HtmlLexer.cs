using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Lisa.Raven.Parser
{
	public class HtmlLexer
	{
		private char _currentCharacter = '\0';
		private bool _endOfSource;
		private string _source;
		private int _sourceIndex = -1;

		public IEnumerable<Lexeme> Lex(string html)
		{
			var tokens = new List<Lexeme>();

			_source = html;
			NextCharacter();

			while (!_endOfSource)
			{
				Lexeme lexeme;

				switch (_currentCharacter)
				{
					case '<':
						lexeme = TokenizeTagStart();
						break;

					case '/':
						lexeme = TokenizeSlash();
						break;

					case '>':
						lexeme = new Lexeme
						{
							Source = _currentCharacter.ToString(CultureInfo.InvariantCulture),
							Type = LexemeType.TagEnd
						};
						NextCharacter();

						break;

					default:
						lexeme = TokenizeText();
						break;
				}

				tokens.Add(lexeme);
			}

			return tokens;
		}

		private Lexeme TokenizeText()
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
				else if (_currentCharacter != '<' && _currentCharacter != '>')
				{
					source.Append(_currentCharacter);
					NextCharacter();
				}
				else
				{
					break;
				}
			}

			return new Lexeme
			{
				Source = source.ToString(),
				Type = LexemeType.Text
			};
		}

		private Lexeme TokenizeSlash()
		{
			if (PeekCharacter() == '>')
			{
				NextCharacter();
				NextCharacter();
				return new Lexeme
				{
					Source = "/>",
					Type = LexemeType.SelfCloseTagEnd
				};
			}
			return TokenizeText();
		}

		private Lexeme TokenizeTagStart()
		{
			NextCharacter();

			if (_currentCharacter == '/')
			{
				NextCharacter();
				return new Lexeme
				{
					Source = "</",
					Type = LexemeType.CloseTagStart
				};
			}
			return new Lexeme
			{
				Source = "<",
				Type = LexemeType.OpenTagStart
			};
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
			}
			else
			{
				_endOfSource = true;
			}
		}
	}
}