using Lisa.Raven.Protocol;

namespace Lisa.Raven.Parser
{
    public static class HtmlParser
    {
	    public static ParsedHtml Parse(string html)
	    {
		    var parsed = new ParsedHtml
		    {
				Errors = new [] {new ParseError("Test parse error!")},
			    TokenStream = new[]
			    {
				    new StreamToken
				    {
					    Type = "Open Tag",
					    Lexeme = "<html>"
				    },
				    new StreamToken
				    {
					    Type = "Open Tag",
					    Lexeme = "<body>"
				    },
				    new StreamToken
				    {
					    Type = "Open Tag",
					    Lexeme = "<p>"
				    },
				    new StreamToken
				    {
					    Type = "Text",
					    Lexeme = "Hello World!"
				    },
				    new StreamToken
				    {
					    Type = "Close Tag",
					    Lexeme = "</p>"
				    },
				    new StreamToken
				    {
					    Type = "Close Tag",
					    Lexeme = "</body>"
				    },
				    new StreamToken
				    {
					    Type = "Close Tag",
					    Lexeme = "</html>"
				    }
			    }
		    };

		    return parsed;
	    }
    }
}
