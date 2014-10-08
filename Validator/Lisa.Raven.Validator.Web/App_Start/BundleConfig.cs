using System.Web.Optimization;

namespace Lisa.Raven.Validator.Web
{
	public class BundleConfig
	{
		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			// jQuery
			bundles.Add(new ScriptBundle("~/bundles/jquery",
				"https://code.jquery.com/jquery-2.1.1.min.js").Include(
					"~/Scripts/jquery/jquery-{version}.js"));

			// Modernizr
			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
				"~/Scripts/modernizr/modernizr-*"));

			// Foundation 5
			bundles.Add(new ScriptBundle("~/bundles/foundation").Include(
				"~/Scripts/foundation/fastclick.js",
				"~/Scripts/foundation/jquery.cookie.js",
				"~/Scripts/foundation/foundation.js",
				"~/Scripts/foundation/foundation.*",
				"~/Scripts/foundation/app.js"));

			// Angular.JS
			bundles.Add(new ScriptBundle("~/bundles/angular",
				"https://ajax.googleapis.com/ajax/libs/angularjs/1.2.25/angular.min.js").Include(
					"~/Scripts/angular/angular.min.js"));

			// Custom site scripts and styling
			bundles.Add(new ScriptBundle("~/bundles/site").Include(
				"~/Scripts/app/controllers/*.js",
				"~/Scripts/app/app.js",
				"~/Scripts/global.js"));
			bundles.Add(new StyleBundle("~/Content/css").Include(
				"~/Content/app.css"));

			// Enable optimization only in DEBUG target
#if DEBUG
			BundleTable.EnableOptimizations = false;
#else
			BundleTable.EnableOptimizations = true;
#endif
		}
	}
}