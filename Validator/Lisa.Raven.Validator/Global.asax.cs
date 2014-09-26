using System.Web;
using System.Web.Http;

namespace Lisa.Raven.Validator
{
	public class WebApiApplication : HttpApplication
	{
		protected void Application_Start()
		{
			GlobalConfiguration.Configure(WebApiConfig.Register);
		}
	}
}