using System.Web.Mvc;

namespace CJG.Web.External.Areas.Int
{
	public class IntAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "Int";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
				"Int_default",
				"Int/{controller}/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional }
			);

			context.MapRoute(
				"DefaultIntRoute",
				"Int/{*.}",
				 new { controller = "Home", action = "Index", isExternal = false },
				 namespaces: new[] { "CJG.Web.External.Areas.Int.Controllers" }
			).DataTokens["area"] = "Int";

			context.MapRoute(
				"IntRootRoute",
				"",
				 new { controller = "Home", action = "Index" },
				 namespaces: new[] { "CJG.Web.External.Controllers" }
			).DataTokens["area"] = "";
		}
	}
}