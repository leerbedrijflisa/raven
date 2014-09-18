using System.Diagnostics;
using System.Text;

namespace Lisa.Raven.Parser
{
    public static class HtmlParser
    {
	    public static ParsedHtml Parse(string html)
	    {
		    var parsed = new ParsedHtml
		    {
				Errors = new [] {new ParseError("Test parse error!")},
				Document = new Token
				{
					Type = TokenType.Document,
					Children = new []
					{
						new Token
						{
							Type = TokenType.Doctype,
							Value = "html"
						},
						new Token
						{
							Type = TokenType.Element,
							Value = "html",
							Children = new []
							{
								// <html>
								new Token { Type = TokenType.OpenTag },
								new Token
								{
									Type = TokenType.ElementContent,
									Children = new []
									{
										new Token
										{
											Type = TokenType.Element,
											Value = "body",
											Children = new []
											{
												// <body>
												new Token { Type = TokenType.OpenTag },
												new Token
												{
													Type = TokenType.ElementContent,
													Children = new []
													{
														new Token
														{
															Type = TokenType.Element,
															Value = "p",
															Children = new []
															{
																// <p>
																new Token { Type = TokenType.OpenTag },
																// Hello World!
																new Token
																{
																	Type = TokenType.ElementContent,
																	Children = new []
																	{
																		 new Token
																		 {
																			 Type = TokenType.Text,
																			 Value = "Hello"
																		 }
																	}
																},
																// </p>
																new Token { Type = TokenType.CloseTag }
															}
														}
													}
												},
												// </body>
												new Token { Type = TokenType.CloseTag }
											}
										},
									}
								},
								// </html>
								new Token { Type = TokenType.CloseTag }
							}
						}
					}
				}
		    };

		    return parsed;
	    }
    }
}
