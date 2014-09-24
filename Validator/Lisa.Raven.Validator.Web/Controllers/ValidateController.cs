﻿using System.Web.Mvc;

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
	}
}