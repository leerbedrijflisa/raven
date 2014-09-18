using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Lisa.Raven.Checkers.DefaultCheckers.Controllers
{
    public class CheckController : ApiController
    {
        [HttpPost]
        public IHttpActionResult CheckHtml([FromUri] string v, [FromBody] ParsedHtml html)
        {
	        var errors = new List<ValidationError>();
	        var amount = CountHtmlRecursive(html.Document);

	        if (amount > 1)
		        errors.Add(new ValidationError("Only 1 HTML tag in document allowed."));
	        
            return Ok(errors);
        }

	    private static int CountHtmlRecursive(Token token)
	    {
			var amount = token.Children.Count(IsHtmlElement);
		    foreach (var child in token.Children)
		    {
			    amount += CountHtmlRecursive(child);
		    }
		    return amount;
	    }

	    private static bool IsHtmlElement(Token t)
	    {
		    return t.Type == TokenType.Element && t.Value == "html";
	    }
    }
}