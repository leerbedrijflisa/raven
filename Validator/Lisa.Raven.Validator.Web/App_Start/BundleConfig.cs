using System.Web.Optimization;

namespace Lisa.Raven.Validator.Web
{
	public class BundleConfig
	{
		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			// Angular.JS
			bundles.Add(new ScriptBundle(
				"~/bundles/angular", "https://ajax.googleapis.com/ajax/libs/angularjs/1.2.25/angular.min.js").Include(
					"~/Scripts/angular.min.js"));

			// Custom site scritps and styling
			bundles.Add(new ScriptBundle("~/bundles/site").Include(
				"~/Scripts/app.js"));
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