﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Lisa.Raven.Parser.Html;
using Newtonsoft.Json;

namespace Lisa.Raven.Validator.Controllers
{
	[RoutePrefix("api/v1/validator")]
	[EnableCors("*", "*", "*")]
	public class ValidatorController : ApiController
	{
		private readonly Func<string, ParsedHtml> _parser = HtmlParser.Create();

		[Route("testparse")]
		[HttpGet]
		public ParsedHtml TestParse()
		{
			const string html = "<!DOCTYPE html>\n" +
			                    "<Html><bOdY class=\"hello world\" test>\n" +
			                    "<P>Hello >= <strong class=test>World</stroNg>!</p><P>Hello again!<br/></p>\n" +
			                    "</boDy></HTml>\n";
			return _parser(html);
		}

		[Route("validate")]
		[HttpPost]
		public IHttpActionResult Validate([FromBody] ValidateRequestData data)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			return Ok(ValidateInternal(data.Checks, data.Html));
		}

		private IEnumerable<ValidationError> ValidateInternal(IEnumerable<Check> checks, string html)
		{
			if (string.IsNullOrEmpty(html))
			{
				return new[] {new ValidationError(ErrorCategory.Meta, "Cannot validate an empty document!")};
			}

			var errors = new List<ValidationError>();

			// Parse the received HTML
			var parsedHtml = _parser(html);
			var jsonedHtml = JsonConvert.SerializeObject(parsedHtml);

			// Send it to the check URLs
			using (var client = new WebClient())
			{
				errors.AddRange(checks.Select(u => AppendApiVersion(u.Url, "1.0"))
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
				var errorsJson = client.UploadString(url, "POST", jsonedHtml);
				var errors = JsonConvert.DeserializeObject<IEnumerable<ValidationError>>(errorsJson).ToArray();

				// Add the url to the errors
				foreach (var error in errors)
				{
					error.Url = url;
				}

				return errors;
			}
			catch (WebException e)
			{
				return new[] {new ValidationError(ErrorCategory.Meta, "Could not connect to check \"" + url + "\". " + e.Message)};
			}
		}
	}
}