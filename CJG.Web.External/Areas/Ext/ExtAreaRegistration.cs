using System.Web.Mvc;

namespace CJG.Web.External.Areas.Ext
{
	public class ExtAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "Ext";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
				name: "CreateUserProfile",
				url: "Ext/ext/user/profile/create",
				defaults: new { controller = "UserProfile", action = "CreateUserProfileView", isExternal = true }
			);  
			context.MapRoute(
				name: "GrantApplication",
				url: "Ext/{controller}/{action}/{grantApplicationId}",
				defaults: new { controller = "Application", action = "ApplicationReview", isExternal = true }
			);

			context.MapRoute(
				name: "WithdrawClaim",
				url: "Ext/Claim/Withdraw",
				defaults: new { controller = "Claim", action = "Withdraw", isExternal = true }
			);

			context.MapRoute(
				"Ext_default",
				"Ext/{controller}/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional, isExternal = true }
			);
			context.MapRoute(
				"DefaultExtRoute",
				"Ext/{*.}",
				 new { controller = "Home", action = "Index", isExternal = true },
				 namespaces: new[] { "CJG.Web.External.Areas.Ext.Controllers" }
			);
			context.MapRoute(
				"RootRoute",
				"",
				 new { controller = "Home", action = "Index" },
				 namespaces: new[] { "CJG.Web.External.Controllers" }
			).DataTokens["area"] = "";
			context.MapRoute(
				name: "ContactUs",
				url: "ContactUs",
				defaults: new { controller = "ContactUs", action = "Index" },
				namespaces: new[] { "CJG.Web.External.Areas.Ext.Controllers" }
			);
			// catch errors
			context.MapRoute(
				"RootDefaultRoute",
				"{controller}/{action}/{id}",
				 new { action = "Index", id = UrlParameter.Optional },
				 namespaces: new[] { "CJG.Web.External.Controllers" }
			).DataTokens["area"] = "";
			context.MapRoute(
				"DefaultCatchAllRoute",
				"{*.}",
				 new { controller = "Error", action = "Index" },
				 namespaces: new[] { "CJG.Web.External.Controllers" }
			).DataTokens["area"] = "";
		}
	}
}