using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Lisa.Raven.Validator.MetaData.Controllers
{
	[RoutePrefix("api/v1/sets")]
	[EnableCors("*", "*", "*")]
	public class SetsController : ApiController
	{
		private static readonly List<CheckSet> Sets = new List<CheckSet>
		{
			new CheckSet
			{
				Name = "Default - Base Checks",
				Code = "0",
				Locked = false,
				Checks = new[]
				{
					new Check
					{
						Url = "http://localhost:2746/api/check/basecheck",
						Locked = false
					},
					new Check
					{
						Url = "http://localhost:2746/api/check/doctypecheck",
						Locked = false
					}
				}
			},
			new CheckSet
			{
				Name = "Default - Paser Errors",
				Code = "1",
				Locked = true,
				Checks = new[]
				{
					new Check
					{
						Url = "http://localhost:2746/api/check/tokenerrors",
						Locked = true
					}
				}
			}
		};
		private static int _nextCode = 2;

		[Route("get/{code}")]
		[HttpGet]
		public IHttpActionResult Get([FromUri] string code)
		{
			var set = Sets.FirstOrDefault(s => s.Code == code);

			if (set == null)
			{
				return NotFound();
			}

			return Ok(set);
		}

		[Route("create")]
		[HttpPost]
		public CheckSet Create([FromBody] CheckSet set)
		{
			set.Code = _nextCode.ToString(CultureInfo.InvariantCulture);
			_nextCode++;

			Sets.Add(set);

			return set;
		}
	}
}