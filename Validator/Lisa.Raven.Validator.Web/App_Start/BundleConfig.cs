using System.Web.Optimization;

namespace Lisa.Raven.Validator.Web
{
	public class BundleConfig
	{
		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/site").Include(
					  "~/Scripts/site.js"));

			bundles.Add(new StyleBundle("~/Content/css").Include(
					  "~/Content/site.css"));

#if DEBUG
			BundleTable.EnableOptimizations = false;
#else
			BundleTable.EnableOptimizations = true;
#endif
		}
	}
}
