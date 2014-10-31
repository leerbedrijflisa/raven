using System.Net;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Lisa.Raven.Validator.Web.Controllers
{
	[RoutePrefix("validate")]
	public class ValidateController : Controller
	{
		[Route]
		[Route("~/", Name = "default")]
		public ActionResult Index()
		{
			return View();
		}
		
		[Route("set/{code}")]
		public ActionResult Set(string code)
		{
			var client = new WebClient();
			client.Headers["Content-Type"] = "application/json";

			// Get the set from the MetaData server
			var setJson = client.DownloadString("http://localhost:14512/api/v1/sets/get/" + code);
			var set = JsonConvert.DeserializeObject<CheckSet>(setJson);

			return View(set);
		}
	}
}