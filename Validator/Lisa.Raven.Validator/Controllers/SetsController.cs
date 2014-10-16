using System.Web.Http;
using System.Web.Http.Cors;

namespace Lisa.Raven.Validator.Controllers
{
	[RoutePrefix("api/v1/sets")]
	[EnableCors("*", "*", "*")]
	public class SetsController : ApiController
	{
		private readonly Check[][] _testSets =
		{
			new[]
			{
				new Check {Url = "http://localhost:2746/api/check/html"},
				new Check {Url = "http://localhost:2746/api/check/head"}
			},
			new[]
			{
				new Check {Url = "http://localhost:2746/api/check/html"},
				new Check {Url = "http://localhost:2746/api/check/404"}
			}
		};

		[Route("{id}")]
		[HttpGet]
		public IHttpActionResult Get([FromUri] int id)
		{
			if (id >= _testSets.Length)
			{
				return NotFound();
			}

			return Ok(_testSets[id]);
		}
	}
}