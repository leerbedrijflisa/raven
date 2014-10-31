using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Lisa.Raven.Checkers.DefaultCheckers.Controllers
{
	public class CheckController : ApiController
	{
       
		[HttpPost]
        public IEnumerable<ValidationError> BaseCheck([FromUri] string v, [FromBody] ParsedHtml html)
        {
            var errors = new List<ValidationError>();

            if(html.Tree.Children.Any(n => n.Type == SyntaxNodeType.Element && n.Value != "html"))
            {
                errors.Add(new ValidationError(ErrorCategory.CodeStyle, "Document root can only contain HTML element."));
            }

            //Calls function RunTagCheck, tells what element is going to be checked, blacklist what sort of elements that are not allowed in the element checked.

            errors.AddRange(RunTagCheck(html.Tree, "html", new[] { "html" }));
            errors.AddRange(RunTagCheck(html.Tree, "head", new[] { "html", "head", "body" }));
            errors.AddRange(RunTagCheck(html.Tree, "body", new[] { "html", "head", "body" }));

            // Make sure that in HTML, HEAD and BODY are in the right order
            errors.AddRange(RunOrderCheck(html.Tree));

            return errors;
        }

        [HttpPost]
        public IEnumerable<ValidationError> DoctypeCheck([FromUri] string v, [FromBody] ParsedHtml html)
        {
            var errors = new List<ValidationError>();

            var nodes = GetNodesRecursive(html.Tree, n => n.Type == SyntaxNodeType.Doctype);
            var amount = nodes.Count();
            var doctype = nodes.FirstOrDefault();
            
            // Make sure we have at least one doctype
            if (doctype == null)
            {
                errors.Add(new ValidationError(ErrorCategory.CodeStyle, "No doctype found."));

                // We can't do anything else if we don't have at least one doctype
                return errors;
            }

            // Make sure we have the amount of doctypes we should have
            if (amount > 1)
            {
                foreach (var node in nodes.Skip(1))
                {
                    errors.Add(new ValidationError(ErrorCategory.CodeStyle, "Only 1 Doctype in document allowed.", node.Line, node.Column));
                }
            }

            // Make sure the very first thing in the document is a doctype
            var firstNode = html.Tree.Children.FirstOrDefault();
            if (firstNode == null || firstNode.Type != SyntaxNodeType.Doctype)
            {
                errors.Add(new ValidationError(ErrorCategory.CodeStyle, "Doctype must be the first element of your documents.", firstNode.Line, firstNode.Column));
            }

            var allowedDoctypes = new List<string>();
            allowedDoctypes.Add("<!DOCTYPE html>");
            allowedDoctypes.Add("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\">");
            allowedDoctypes.Add("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\" \"http://www.w3.org/TR/html4/loose.dtd\">");
            allowedDoctypes.Add("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Frameset//EN\" \"http://www.w3.org/TR/html4/frameset.dtd\">");
            allowedDoctypes.Add("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">");
            allowedDoctypes.Add("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            allowedDoctypes.Add("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Frameset//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd\">");
            allowedDoctypes.Add("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">");

            // TODO: node.Lexeme is not yet implemented
            if (!allowedDoctypes.Any(d => d.Equals(doctype.Source, StringComparison.InvariantCultureIgnoreCase)))
            {
                errors.Add(new ValidationError(ErrorCategory.Malformed, "Your Doctype does not meet interational standards and will not work on many browsers!", doctype.Line, doctype.Column));
            }

            return errors;
        }

        private IEnumerable<ValidationError> RunTagCheck(SyntaxNode tree, string tag, IEnumerable<string> blacklist)
        {
            var errors = new List<ValidationError>();
            var nodes = GetNodesRecursive(tree, n => n.Type == SyntaxNodeType.Element && n.Value == tag);
            var amount = nodes.Count();

            // Make sure we have the correct amount of this element
            if (amount > 1)
            {
                foreach (var node in nodes.Skip(1))
                {
                    errors.Add(new ValidationError(ErrorCategory.CodeStyle, string.Format("Only 1 {0} tag in document allowed.", tag), node.Line, node.Column));
                }
            }
            else if (amount < 1)
            {
                errors.Add(new ValidationError(ErrorCategory.CodeStyle, string.Format("No {0} tag in document found.", tag)));

                // Return immediately because the rest of our checks aren't going to be valid with no tags
                return errors;
            }
            
            // Make sure we don't have any blacklisted tags in our elements
            errors.AddRange(nodes.SelectMany(n => RunNestingCheck(n, blacklist)));

            return errors;
        }

        private IEnumerable<ValidationError> RunNestingCheck(SyntaxNode tree, IEnumerable<string> tags)
        {
            var errors = new List<ValidationError>();

            foreach(var tag in tags)
            {
                var nodes = GetNodesRecursive(tree, n => (n.Type == SyntaxNodeType.Element && n.Value == tag));

                if(nodes.Any())
                {
                    errors.Add(new ValidationError(ErrorCategory.CodeStyle, string.Format("{0} element can't contain {1} element.", tree.Value, tag)));
                }
            }

            return errors;
        }


        private IEnumerable<ValidationError> RunOrderCheck(SyntaxNode node)
        {
            var errors = new List<ValidationError>();
            var htmlElement = GetNodesRecursive(node, n => n.Type == SyntaxNodeType.Element && n.Value == "html").FirstOrDefault();

            // If we don't have one we can't run this check
            if (htmlElement == null)
            {
                return errors;
            }

            // Find the head and html elements
            var children = htmlElement.Children.First(n => n.Type == SyntaxNodeType.Content).Children.ToList();
            var head = children.FirstOrDefault(n => n.Type == SyntaxNodeType.Element && n.Value == "head");
            var body = children.FirstOrDefault(n => n.Type == SyntaxNodeType.Element && n.Value == "body");

            // If we don't have both of those, we can't run the check
            if (head == null || body == null)
            {
                return errors;
            }

            // Find the locations of the elements
            var headLocation = children.IndexOf(head);
            var bodyLocation = children.IndexOf(body);

            // Perform the actual check
            if (headLocation > bodyLocation)
            {
                errors.Add(new ValidationError(ErrorCategory.CodeStyle, "HEAD must be before BODY", head.Line, head.Column));
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
                select new ValidationError(ErrorCategory.CodeStyle, attribute.Value, token.Line, token.Column);

			return errors;
		}

        private IEnumerable<SyntaxNode> GetNodesRecursive(SyntaxNode node, Func<SyntaxNode, bool> filter = null)
        {
            var nodeList = new List<SyntaxNode>();

            // Do the same check for all the children of this node
            foreach (var child in node.Children)
            {
                // If this node is a doctype, add it
                if (filter(child))
                {
                    nodeList.Add(child);
                }

                nodeList.AddRange(GetNodesRecursive(child, filter));
            }

            // Return our resulting list
            return nodeList;
        }
	}
}