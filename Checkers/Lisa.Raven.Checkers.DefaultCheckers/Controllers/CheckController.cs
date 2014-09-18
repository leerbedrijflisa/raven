using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Lisa.Raven.Checkers.DefaultCheckers.Controllers
{
    public class CheckController : ApiController
    {
        [HttpPost]

        public static string checkValue { get; set; }

        public IHttpActionResult CheckHtml([FromUri] string v, [FromBody] ParsedHtml html)
        {
            checkValue = "html";
	        var errors = new List<ValidationError>();
	        var amount = CountRecursive(html.Document);

	        if (amount > 1)
		        errors.Add(new ValidationError("Only 1 HTML tag in document allowed."));
            else if (amount < 1)
                errors.Add(new ValidationError("Document does not contain a HTML tag."));
	        
            return Ok(errors);
        }

        public IHttpActionResult CheckDoctype([FromUri] string v, [FromBody] ParsedHtml html)
        {
            checkValue = "doctype";
            var errors = new List<ValidationError>();
            var amount = CountDoctypes(html.Document);

            if (amount > 1)
                errors.Add(new ValidationError("Only 1 doctype in document allowed."));
            else if (amount < 1)
                errors.Add(new ValidationError("Document does not contain a doctype."));

            return Ok(errors);
        }

        private static int CountDoctypes(Token token)
        {
            int counter = 0;

            if (token.Type == TokenType.Doctype)
            {
                counter++;
            }

            foreach (var child in token.Children)
            {
                counter += CountDoctypes(child);
            }

            return counter;
        }

        private static int CountHtmlElements(Token token)
        {
            int counter = 0;

            if (token.Type == TokenType.Element && token.Value == "html")
            {
                counter++;
            }

            foreach (var child in token.Children)
            {
                counter += CountDoctypes(child);
            }

            return counter;
        }

	    private static int CountRecursive(Token token)
	    {
			var amount = token.Children.Count(IsElement);
		    foreach (var child in token.Children)
		    {
			    amount += CountRecursive(child);
		    }
		    return amount;
	    }

	    private static bool IsElement(Token t)
	    {
		    return t.Type == TokenType.Element && t.Value == checkValue;
	    }
    }
}