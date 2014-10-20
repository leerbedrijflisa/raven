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
            int htmlPosition = _elementPosition;

            errors.AddRange(CountCheck(html.Tree, "head"));
            int headPosition = _elementPosition;

            errors.AddRange(CountCheck(html.Tree, "body"));
            int bodyPosition = _elementPosition;

            /*

            if(htmlPosition > headPosition)
            {
                errors.Add(new ValidationError(ErrorCategory.Malformed, "The Head is positioned outside the HTML"));
            }

            if(htmlPosition > bodyPosition)
            {
                errors.Add(new ValidationError(ErrorCategory.Malformed, "The Body is positioned outside the HTML"));
            }

            if(bodyPosition > headPosition)
            {
                errors.Add(new ValidationError(ErrorCategory.Malformed, "You cannot position the Body inside the Head"));
            }
            else if(headPosition > bodyPosition)
            {
                errors.Add(new ValidationError(ErrorCategory.Malformed, "You cannot position the Head inside the Body"));
            }
             */

            return errors;
        }

        [HttpPost]
        private IEnumerable<ValidationError> DoctypeCheck([FromUri] string v, [FromBody] ParsedHtml html)
        {
            var errors = new List<ValidationError>();

            var amount = CountDoctypeRecursive(html.Tree, "");

            if (amount > 1)
            {
                errors.Add(new ValidationError(ErrorCategory.Malformed, "Only 1 Doctype in document allowed.", _line, _column));
            }
            else if (amount < 1)
            {
                errors.Add(new ValidationError(ErrorCategory.CodeStyle, "No doctype found.", _line, _column));
            }

            if (amount > 1 || amount == 1)
            {
                if (_previousElement == "DocumentRoot")
                {
                    errors.Add(new ValidationError(ErrorCategory.Malformed, "Doctype must be on the first line of your HTML document.", _line, _column));
                }
            }

            string Doctype1 = "<!DOCTYPE html>";
            string Doctype2 = "<!DOCTYPE HTML PUBLIC " + "\"-//W3C//DTD HTML 4.01//EN\"" + " " + "\"http://www.w3.org/TR/html4/strict.dtd\"" + ">";
            string Doctype3 = "<!DOCTYPE HTML PUBLIC " + "\"-//W3C//DTD HTML 4.01 Transitional//EN\"" + " " + "\"http://www.w3.org/TR/html4/loose.dtd\"" + ">";
            string Doctype4 = "<!DOCTYPE HTML PUBLIC " + "\"-//W3C//DTD HTML 4.01 Frameset//EN\"" + " " + "\"http://www.w3.org/TR/html4/frameset.dtd\"" + ">";
            string Doctype5 = "<!DOCTYPE html PUBLIC " + "\"-//W3C//DTD XHTML 1.0 Strict//EN\"" + " " + "\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\"" + ">";
            string Doctype6 = "<!DOCTYPE html PUBLIC " + "\"-//W3C//DTD XHTML 1.0 Transitional//EN\"" + " " + "\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"" + ">";
            string Doctype7 = "<!DOCTYPE html PUBLIC " + "\"-//W3C//DTD XHTML 1.0 Frameset//EN\"" + " " + "\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd\"" + ">";
            string Doctype8 = "<!DOCTYPE html PUBLIC " + "\"-//W3C//DTD XHTML 1.1//EN\"" + " " + "\"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\"" + ">";

            if (html.Tree.Value == Doctype1 || html.Tree.Value == Doctype2 || html.Tree.Value == Doctype3 || html.Tree.Value == Doctype4 || html.Tree.Value == Doctype5 || html.Tree.Value == Doctype6
                || html.Tree.Value == Doctype7 || html.Tree.Value == Doctype8)
            {
                // OK (DO NOT USE != in the statement above!)
            }
            else
            {
                errors.Add(new ValidationError(ErrorCategory.Malformed, "Your Doctype does not meet interational standards and will not work on many browsers!", _line, _column));
            }

            return errors;
        }

       

        private IEnumerable<ValidationError> CountCheck(SyntaxNode tree, string tag)
        {
            var errors = new List<ValidationError>();
            var amount = CountRecursive(tree, tag, 0, "");

            if (amount > 1)
            {
                errors.Add(new ValidationError(ErrorCategory.Malformed, string.Format("Only 1 {0} tag in document allowed.", tag), _line, _column));
            }
            else if (amount < 1)
            {
                errors.Add(new ValidationError(ErrorCategory.Malformed, string.Format("No {0} tag in document found.", tag), _line, _column));
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

        private int CountDoctypeRecursive(SyntaxNode node, string previous)
        {
            int amount = 0;

            if (node.Type == SyntaxNodeType.Doctype)
            {
                amount++;
                _column = node.Column;
                _line = node.Line;
                _previousElement = previous;
            }

            previous = node.Value;

            foreach (var child in node.Children)
            {
                amount += CountDoctypeRecursive(child, previous);
            }

            return amount;
        }

        private int CountRecursive(SyntaxNode node, string ElementName, int treeLocation, string previous)
        {
            int amount = 0;

            if(node.Type == SyntaxNodeType.Element && node.Value == ElementName)
            {
                amount++;
                _column = node.Column;
                _line = node.Line;
                _elementPosition = treeLocation;
                _previousElement = previous;
            }

            previous = node.Value;

            foreach (var child in node.Children)
            {
                amount += CountRecursive(child, ElementName, treeLocation, previous);
            }

            return amount;
        }

        private int _column;
        private int _line;
        private int _elementPosition;
        private string _previousElement;
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