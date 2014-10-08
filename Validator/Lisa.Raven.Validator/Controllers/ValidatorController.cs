using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Lisa.Raven.Parser;
using Newtonsoft.Json;

namespace Lisa.Raven.Validator.Controllers
{
	[RoutePrefix("api/v1/validator")]
	[EnableCors("*", "*", "*")]
	public class ValidatorController : ApiController
	{
		private string[][] _setArray =
		{
			new []
			{
				"http://localhost:2746/api/check/checkhtml",
				"http://localhost:2746/api/check/404test"
			}
		};

		[Route("testparse")]
		[HttpGet]
		public ParsedHtml TestParse()
		{
			const string html = "<!DOCTYPE html>\n" +
			                    "<Html><bOdY>\n" +
			                    "<P>Hello > <strong>World</stroNg>!</p><P>Hello again!<br/></p>\n" +
			                    "</boDy></HTml>\n";
			return HtmlParser.Parse(html);
		}

		[Route("validate")]
		[HttpPost]
		public IHttpActionResult Validate([FromBody] ValidateRequestData data)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			return Ok(ValidateInternal(data.CheckUrls, data.Html));
		}

		private IEnumerable<ValidationError> ValidateInternal(IEnumerable<string> checkUrls, string html)
		{
			if (string.IsNullOrEmpty(html))
			{
				return new[] {new ValidationError(ErrorCategory.Meta, "Cannot validate an empty document!")};
			}

			var errors = new List<ValidationError>();

			// Parse the received HTML
			var parsedHtml = HtmlParser.Parse(html);
			var jsonedHtml = JsonConvert.SerializeObject(parsedHtml);

			// Send it to the check URLs
			using (var client = new WebClient())
			{
				errors.AddRange(checkUrls.Select(u => AppendApiVersion(u, "1.0"))
					.SelectMany(url => TryRunCheck(client, url, jsonedHtml)));
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

		private static IEnumerable<ValidationError> TryRunCheck(WebClient client, string url, string jsonedHtml)
		{
			try
			{
				client.Headers["Content-Type"] = "application/json";

				// If needed, this can be made async
				var errors = client.UploadString(url, "POST", jsonedHtml);
				return JsonConvert.DeserializeObject<IEnumerable<ValidationError>>(errors);
			}
			catch (WebException e)
			{
				return new[] {new ValidationError(ErrorCategory.Meta, "Could not connect to check \"" + url + "\". " + e.Message)};
			}
		}
	}
}