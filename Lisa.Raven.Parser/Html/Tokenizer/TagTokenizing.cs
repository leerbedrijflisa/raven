using System;
using System.Text;

namespace Lisa.Raven.Parser.Html.Tokenizer
{
	public static class TagTokenizing
	{
		public static Token TokenizeOpenTag(DataWalker<Lexeme> walker, object data)
		{
			var token = new Token
			{
				Line = walker.Current.Line,
				Column = walker.Current.Column
			};

			walker.Next();

			// TODO: Handle more gracefully, this might happen
			if (walker.Current.Type != LexemeType.Text)
			{
				throw new Exception();
			}

			// If the name of the tag starts with a "!", it's a doctype
			var name = walker.Current.Source.ToLower();
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

			walker.Next();

			// Parse attributes until we find a tag end
			while (walker.Current.Type != LexemeType.TagEnd && walker.Current.Type != LexemeType.SelfCloseTagEnd)
			{
				// Text means start of attribute
				if (walker.Current.Type == LexemeType.Text)
				{
					var attribute = new TokenData
					{
						Type = TokenDataType.Attribute,
						Name = walker.Current.Source
					};

					walker.Next();
					SkipWhitespace(walker);

					// If there's an equal sign
					if (walker.Current.Type == LexemeType.Equals)
					{
						walker.Next();
						SkipWhitespace(walker);

						if (walker.Current.Type == LexemeType.Text)
						{
							// Unquoted value (can't handle whitespace)
							attribute.Value = walker.Current.Source;
							walker.Next();
						}
						else if (walker.Current.Type == LexemeType.Quote)
						{
							// Quoted value (can handle whitespace and is closed by another quote)
							walker.Next();

							// Continue till we find a close quote
							var value = new StringBuilder();
							while (walker.Current.Type != LexemeType.Quote)
							{
								value.Append(walker.Current.Source);

								// Continue if we're not at the end of the lexemes
								walker.Next();
								if (!walker.AtEnd)
									continue;

								// We're at the end and the value is still unclosed, add an error
								token.Data.Add(new TokenData(TokenDataType.Error, "Error",
									"Value for attribute \"" + attribute.Name + "\" is never closed."));
								return token;
							}

							// We found a close quote!
							attribute.Value = value.ToString();
							walker.Next();
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
				walker.Next();
			}


			// Check if the tag is self closing
			if (walker.Current.Type == LexemeType.SelfCloseTagEnd)
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

			walker.Next();

			return token;
		}

		public static Token TokenizeCloseTag(DataWalker<Lexeme> walker, object data)
		{
			var token = new Token
			{
				Line = walker.Current.Line,
				Column = walker.Current.Column,
				Type = TokenType.CloseTag
			};
			
			walker.Next();

			if (walker.AtEnd || walker.Current.Type != LexemeType.Text)
			{
				token.Data.Add(new TokenData(TokenDataType.Error, "Error", "Element name missing in close tag."));
				return token;
			}

			token.Value = walker.Current.Source.ToLower();

			walker.Next();

			// If the current token is invalid
			if (walker.AtEnd || walker.Current.Type != LexemeType.TagEnd)
			{
				token.Data.Add(new TokenData(TokenDataType.Error, "Error", "Unclosed close tag."));
				return token;
			}

			walker.Next();

			return token;
		}

		private static void SkipWhitespace(DataWalker<Lexeme> walker)
		{
			while (walker.Current.Type == LexemeType.Whitespace)
			{
				walker.Next();
			}
		}
	}
}