using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Lisa.Raven.Checkers.DefaultCheckers.Controllers
{
	public class CheckController : ApiController
	{
        /*
		[HttpPost]
        private IEnumerable<ValidationError> BaseCheck([FromUri] string v, [FromBody] ParsedHtml html)
        {
            var errors = new List<ValidationError>();

            var amountHtml = CountRecursive(html.Tree, "html");
           


           

            var amountHead = CountRecursive(html.Tree, "head");
           

            var amountBody = CountRecursive(html.Tree, "body");
          

           
        }

        private ValidationError ErrorList(int amount, List<ValidationError> errors, string ElementName)
        {
            if (amount > 1)
            {
                errors.Add(new ValidationError(ErrorCategory.Malformed, string.Format("Only 1 {0} tag in document allowed.", ElementName)));
            }
            else if (amount < 1)
            {
                errors.Add(new ValidationError(ErrorCategory.Malformed, string.Format("No {0} tag was found.", ElementName)));
            }
            else if (amount == 0)
            {

            }

            //return errors;
        }*/

        [HttpPost]
        public IEnumerable<ValidationError> Html([FromUri] string v, [FromBody] ParsedHtml html)
        {
            var errors = new List<ValidationError>();
            var amount = CountRecursive(html.Tree, "html");

            if (amount > 1)
            {
                errors.Add(new ValidationError(ErrorCategory.CodeStyle, "Only 1 HTML tag in document allowed."));
            }
            else if (amount < 1)
            {
				errors.Add(new ValidationError(ErrorCategory.CodeStyle, "No HTML tag found."));
            }

            return errors;
        }

		[HttpPost]
        public IEnumerable<ValidationError> Head([FromUri] string v, [FromBody] ParsedHtml html)
        {
            var errors = new List<ValidationError>();
            var amount = CountRecursive(html.Tree, "head");

            if (amount > 1)
            {
				errors.Add(new ValidationError(ErrorCategory.CodeStyle, "Only 1 HEAD tag in document allowed."));
            }
            else if (amount < 1)
            {
				errors.Add(new ValidationError(ErrorCategory.CodeStyle, "No HEAD tag found."));
            }

            return errors;
        }

		[HttpPost]
		public IEnumerable<ValidationError> TokenErrors([FromUri] string v, [FromBody] ParsedHtml html)
		{
			var errors =
				from token in html.Tokens
				from attribute in token.Data
				where attribute.Name == "Error"
				select new ValidationError(ErrorCategory.Malformed, attribute.Value, token.Line, token.Column);

			return errors;
		}

        private static int CountRecursive(SyntaxNode node, string ElementName)
        {
            int amount = 0;

            if(node.Type == SyntaxNodeType.Element && node.Value == ElementName)
            {
                amount++; 
            }

            foreach (var child in node.Children)
            {
                amount += CountRecursive(child, ElementName);
            }
            return amount;
        }
	}
}