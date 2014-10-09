using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Lisa.Raven.Parser.Html.Lexer
{
	public class LexerPipe : IPipe<string, IEnumerable<Lexeme>>
	{
		private DataWalker<char> _walker;

		private int _currentColumn = 1;
		private int _currentLine = 1;

		public IEnumerable<Lexeme> Process(string html)
		{
			if (html == null)
			{
				throw new ArgumentNullException("html");
			}

			_walker = new DataWalker<char>(html);
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
						lexeme = TagLexing.LexTagStart(_walker);
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

			while (!_walker.AtEnd)
			{
				if (
					// Not <>
					_walker.Current != '<' && _walker.Current != '>' &&
					// Not Whitespace
					_walker.Current != ' ' && _walker.Current != '\t' &&
					_walker.Current != '\r' && _walker.Current != '\n' &&
					// Not special characters
					_walker.Current != '=' && _walker.Current != '/')
				{
					source.Append(_walker.Current);
					_walker.Next();
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
			
			while (!_walker.AtEnd)
			{
				if (// Only Whitespace
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

		private Lexeme LexSlash(Lexeme lexeme)
		{
			_walker.Next();

			// If the character after the slash isn't a >
			if (_walker.Current != '>')
			{
				// Then it's just a bit of text
				return LexText(lexeme);
			}

			// If it is a >, then we're looking at a closing tag
			lexeme.Type = LexemeType.SelfCloseTagEnd;
			lexeme.Source = "/>";

			_walker.Next();
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