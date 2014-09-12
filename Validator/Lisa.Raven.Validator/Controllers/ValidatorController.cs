using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Lisa.Raven.Parser;
using Lisa.Raven.Protocol;

namespace Lisa.Raven.Validator.Controllers
{
    public class ValidatorController : ApiController
    {
		[HttpPost]
	    public IEnumerable<ValidationError> Validate([FromBody] string html)
		{
			return ValidateInternal(html);
		}

	    [HttpPost]
	    public IEnumerable<ValidationError> ValidateUrl([FromBody] string url)
	    {
		    using (var client = new WebClient())
		    {
			    return ValidateInternal(client.DownloadString(url));
		    }
	    }

	    private IEnumerable<ValidationError> ValidateInternal(string html)
	    {
		    var errors = new List<ValidationError>();

			var parsedHtml = HtmlParser.Parse(html);
			errors.AddRange(parsedHtml.Errors.Select(e => new ValidationError(e.Message)));

		    return errors;
	    }
    }
}
