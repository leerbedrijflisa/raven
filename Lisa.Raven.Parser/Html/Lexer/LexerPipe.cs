using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Lisa.Raven.Parser.Html.Lexer
{
	public class LexerPipe : IPipe<string, IEnumerable<Lexeme>>
	{
		private int _currentColumn = 1;
		private int _currentLine = 1;
		private DataWalker<char> _walker;
		private LexerData _data;

		public IEnumerable<Lexeme> Process(string html)
		{
			// Verify function call requirements
			if (html == null)
			{
				throw new ArgumentNullException("html");
			}

			// Set up the helper class to walk over our data
			_walker = new DataWalker<char>(html);
			_walker.Moved += OnWalkerMoved;

			// Set up the lexing metadata class
			_data = new LexerData();

			var tokens = new List<Lexeme>();

			_walker.Next();
			while (!_walker.AtEnd)
			{
				var lexeme = new Lexeme
				{
					Line = _currentLine,
					Column = _currentColumn
				};

				switch (_walker.Current)
				{
					case '<':
						lexeme = TagLexing.LexTagStart(_walker, _data);
						break;

					case '/':
						lexeme = TagLexing.LexSlash(_walker, _data);
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
						lexeme = TextLexing.LexText(_walker, _data);
						break;
				}

				tokens.Add(lexeme);
			}

			return tokens;
		}

		private void OnWalkerMoved(object sender, EventArgs e)
		{
			if (_walker.Current == '\n')
			{
				_currentLine++;
				_currentColumn = 1;
			}
			else
			{
				_currentColumn++;
			}
		}

		private Lexeme LexWhitespace(Lexeme lexeme)
		{
			var source = new StringBuilder();

			while (!_walker.AtEnd)
			{
				if ( // Only Whitespace
					_walker.Current == ' ' || _walker.Current == '\t' ||
					_walker.Current == '\r' || _walker.Current == '\n')
				{
					source.Append(_walker.Current);
					_walker.Next();
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

		private Lexeme LexTagEnd(Lexeme lexeme)
		{
			lexeme.Type = LexemeType.TagEnd;
			lexeme.Source = _walker.Current.ToString(CultureInfo.InvariantCulture);
			_walker.Next();

			return lexeme;
		}

		private Lexeme LexEquals(Lexeme lexeme)
		{
			lexeme.Type = LexemeType.Equals;
			lexeme.Source = _walker.Current.ToString(CultureInfo.InvariantCulture);
			_walker.Next();

			return lexeme;
		}
	}
}