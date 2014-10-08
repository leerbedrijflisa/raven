using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Lisa.Raven.Checkers.DefaultCheckers.Controllers
{
	public class CheckController : ApiController
	{
		[HttpPost]
		public IEnumerable<ValidationError> Html([FromUri] string v, [FromBody] ParsedHtml html)
		{
			var errors = new List<ValidationError>();
			var amount = CountHtmlRecursive(html.Tree);

			if (amount > 1)
			{
				errors.Add(new ValidationError(ErrorCategory.Malformed, "Only 1 HTML tag in document allowed."));
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

		private static int CountHtmlRecursive(SyntaxNode node)
		{
			var amount = node.Children.Count(IsHtmlElement);
			foreach (var child in node.Children)
			{
				amount += CountHtmlRecursive(child);
			}
			return amount;
		}

		private static bool IsHtmlElement(SyntaxNode node)
		{
			return node.Type == SyntaxNodeType.Element && node.Value == "html";
		}
	}
}