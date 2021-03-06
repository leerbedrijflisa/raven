﻿using System.Web.Http;

namespace Lisa.Raven.Validator
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services
			config.EnableCors();

			// Web API routes
			config.MapHttpAttributeRoutes();
		}
	}
}