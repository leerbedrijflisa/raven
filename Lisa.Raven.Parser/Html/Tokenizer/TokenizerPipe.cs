using System;
using System.Collections.Generic;
using System.Text;

namespace Lisa.Raven.Parser.Html.Tokenizer
{
	public class TokenizerPipe : IPipe<IEnumerable<Lexeme>, IEnumerable<Token>>
	{
		private Lexeme _currentLexeme;
		private bool _endOfSource;
		private IEnumerable<Lexeme> _source;
		private IEnumerator<Lexeme> _sourceEnumerator;

		public IEnumerable<Token> Process(IEnumerable<Lexeme> lexemes)
		{
			var tokens = new List<Token>();
			_source = lexemes;
			_sourceEnumerator = _source.GetEnumerator();

			NextLexeme();

			while (!_endOfSource)
			{
				var token = new Token
				{
					Line = _currentLexeme.Line,
					Column = _currentLexeme.Column
				};

				switch (_currentLexeme.Type)
				{
					case LexemeType.OpenTagStart:
						token = TokenizeOpenTag(token);
						break;

					case LexemeType.CloseTagStart:
						token = TokenizeCloseTag(token);
						break;

					case LexemeType.Text:
						// The following tokens in this context are seen as text as well
					case LexemeType.TagEnd:
					case LexemeType.Equals:
					case LexemeType.Quote:
						// TODO: Merge whitespace with rest of text
					case LexemeType.Whitespace:
						token = TokenizeText(token);
						break;

					default:
						throw new NotImplementedException();
				}

				tokens.Add(token);
			}

			return tokens;
		}

		private Token TokenizeText(Token token)
		{
			token.Type = TokenType.Text;
			token.Value = _currentLexeme.Source;

			NextLexeme();

			return token;
		}

		private Token TokenizeCloseTag(Token token)
		{
			token.Type = TokenType.CloseTag;

			NextLexeme();

			if (_endOfSource || _currentLexeme.Type != LexemeType.Text)
			{
				token.Data.Add(new TokenData(TokenDataType.Error, "Error", "Element name missing in close tag."));
				return token;
			}

			token.Value = _currentLexeme.Source.ToLower();

			NextLexeme();

			// If the current token is invalid
			if (_endOfSource || _currentLexeme.Type != LexemeType.TagEnd)
			{
				token.Data.Add(new TokenData(TokenDataType.Error, "Error", "Unclosed close tag."));
				return token;
			}

			NextLexeme();

			return token;
		}

		private Token TokenizeOpenTag(Token token)
		{
			NextLexeme();

			// TODO: Handle more gracefully, this might happen
			if (_currentLexeme.Type != LexemeType.Text)
			{
				throw new Exception();
			}

			// If the name of the tag starts with a "!", it's a doctype
			var name = _currentLexeme.Source.ToLower();
			if (!name.StartsWith("!"))
			{
				token.Type = TokenType.OpenTag;
				token.Value = name;
			}
			else
			{
				token.Type = TokenType.Doctype;
				token.Value = name.Substring(1);
			}

			NextLexeme();

			// Parse attributes until we find a tag end
			while (_currentLexeme.Type != LexemeType.TagEnd && _currentLexeme.Type != LexemeType.SelfCloseTagEnd)
			{
				// Text means start of attribute
				if (_currentLexeme.Type == LexemeType.Text)
				{
					var attribute = new TokenData
					{
						Type = TokenDataType.Attribute,
						Name = _currentLexeme.Source
					};

					NextLexeme();
					SkipWhitespace();

					// If there's an equal sign
					if (_currentLexeme.Type == LexemeType.Equals)
					{
						NextLexeme();
						SkipWhitespace();

						if (_currentLexeme.Type == LexemeType.Text)
						{
							// Unquoted value (can't handle whitespace)
							attribute.Value = _currentLexeme.Source;
							NextLexeme();
						}
						else if (_currentLexeme.Type == LexemeType.Quote)
						{
							// Quoted value (can handle whitespace and is closed by another quote)
							NextLexeme();

							// Continue till we find a close quote
							var value = new StringBuilder();
							while (_currentLexeme.Type != LexemeType.Quote)
							{
								value.Append(_currentLexeme.Source);

								// Continue if we're not at the end of the lexemes
								NextLexeme();
								if (!_endOfSource)
									continue;
								
								// We're at the end and the value is still unclosed, add an error
								token.Data.Add(new TokenData(TokenDataType.Error, "Error",
									"Value for attribute \"" + attribute.Name + "\" is never closed."));
								return token;
							}

							// We found a close quote!
							attribute.Value = value.ToString();
							NextLexeme();
						}
						else
						{
							token.Data.Add(new TokenData(TokenDataType.Error, "Error",
								"Attribute \"" + attribute.Name + "\" has a = character but no value."));
						}
					}

					token.Data.Add(attribute);

					// We already went to the next lexeme
					continue;
				}

				// Anything else is skipped
				NextLexeme();
			}


			// Check if the tag is self closing
			if (_currentLexeme.Type == LexemeType.SelfCloseTagEnd)
			{
				if (token.Type != TokenType.Doctype)
				{
					token.Type = TokenType.SelfClosingTag;
				}
				else
				{
					token.Data.Add(new TokenData(TokenDataType.Error, "Error", "Doctype tag does not need self closing tag end."));
				}
			}

			NextLexeme();

			return token;
		}

		private void SkipWhitespace()
		{
			while (_currentLexeme.Type == LexemeType.Whitespace)
			{
				NextLexeme();
			}
		}

		private void NextLexeme()
		{
			if (!_sourceEnumerator.MoveNext())
			{
				_endOfSource = true;
			}
			else
			{
				_currentLexeme = _sourceEnumerator.Current;
			}
		}
	}
}