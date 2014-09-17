using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using Lisa.Raven.Parser;
using Newtonsoft.Json;

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

			// Send it to the check URLs
		    using (var client = new WebClient())
		    {
				client.Headers["Content-Type"] = "application/json";
			    errors.AddRange(checkUrls.Select(u => AppendApiVersion(u, "1.0"))
				    .SelectMany(url => TryRunCheck(client, url, parsedHtml)));
		    }

		    return errors;
	    }

	    private static string AppendApiVersion(string url, string version)
	    {
			var uriBuilder = new UriBuilder(url);
			var query = HttpUtility.ParseQueryString(uriBuilder.Query);
			query["v"] = version;
			uriBuilder.Query = query.ToString();
			return uriBuilder.ToString();
	    }

	    private static IEnumerable<ValidationError> TryRunCheck(WebClient client, string url, ParsedHtml html)
		{
		    try
			{
				// If needed, this can be made async
			    var errors = client.UploadString(url, "POST", JsonConvert.SerializeObject(html));
			    return JsonConvert.DeserializeObject<IEnumerable<ValidationError>>(errors);
		    }
		    catch (WebException)
		    {
			    return new[] {new ValidationError("Couldn't connect to " + url + ".")};
		    }
	    }
    }
}
