using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Lisa.Raven.Parser;
using Lisa.Raven.Protocol;

namespace Lisa.Raven.Validator.Controllers
{
    public class ValidatorController : ApiController
    {
		[HttpPost]
	    public IEnumerable<ValidationError> Validate([FromBody] ValidateRequestData data)
		{
			return ValidateInternal(data.CheckUrls, data.Html);
		}

	    private IEnumerable<ValidationError> ValidateInternal(IEnumerable<string> checkUrls, string html)
	    {
		    var errors = new List<ValidationError>();

			// Parse the received HTML
			var parsedHtml = HtmlParser.Parse(html);
			errors.AddRange(parsedHtml.Errors.Select(e => new ValidationError(e.Message)));

		    return errors;
	    }
    }
}
