using System;
using System.Web.Http;
using Lisa.Raven.Protocol.Parser;

namespace Lisa.Raven.Parser.Controllers
{
    public class ParserController : ApiController
    {
		[HttpPost]
	    public ParseResponse Parse([FromBody] string html)
	    {
		    return new ParseResponse
		    {
				Version = new Version(1, 0),
				Stream = new []
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
	    }
    }
}
