using System.Web;
using System.Web.Http;

namespace Lisa.Raven.Checkers.DefaultCheckers
{
	public class WebApiApplication : HttpApplication
	{
		protected void Application_Start()
		{
			GlobalConfiguration.Configure(WebApiConfig.Register);
		}
	}
}