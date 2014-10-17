using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Lisa.Raven.Checkers.DefaultCheckers.Controllers
{
	public class CheckController : ApiController
	{
       
		[HttpPost]
        private IEnumerable<ValidationError> BaseCheck([FromUri] string v, [FromBody] ParsedHtml html)
        {
            var errors = new List<ValidationError>();

            errors.AddRange(CountCheck(html.Tree, "html"));
            errors.AddRange(CountCheck(html.Tree, "head"));
            errors.AddRange(CountCheck(html.Tree, "body"));


            return errors;
        }

        [HttpPost]
        private IEnumerable<ValidationError> DoctypeCheck([FromUri] string v, [FromBody] ParsedHtml html)
        {
            var errors = new List<ValidationError>();

            
            
            return errors;

        }

        private IEnumerable<ValidationError> CountCheck(SyntaxNode tree, string tag)
        {
            var errors = new List<ValidationError>();
            var amount = CountRecursive(tree, tag);

            if (amount > 1)
            {
                errors.Add(new ValidationError(ErrorCategory.CodeStyle, string.Format("Only 1 {0} tag in document allowed.", tag, _line, _column)));
            }
            else if (amount < 1)
            {
                errors.Add(new ValidationError(ErrorCategory.CodeStyle, string.Format("Only 1 {0} tag in document allowed.", tag), _line, _column));
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

        private int CountRecursive(SyntaxNode node, string ElementName)
        {
            int amount = 0;

            if(node.Type == SyntaxNodeType.Element && node.Value == ElementName)
            {
                amount++;
                _column = node.Column;
                _line = node.Line;
                
            }

            foreach (var child in node.Children)
            {
                amount += CountRecursive(child, ElementName);
            }

            return amount;
        }

        private int _column;
        private int _line;
	}

    /*
    internal class ReturnStuff
    {
        internal int Result { get; set; }
        internal int Column { get; set; }
        internal int Line { get; set; }
    }
     */
}