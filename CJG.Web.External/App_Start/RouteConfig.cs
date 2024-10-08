﻿using System.Web.Mvc;
using System.Web.Routing;

namespace CJG.Web.External
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.IgnoreRoute("{file}.html");

			routes.MapMvcAttributeRoutes();

			AreaRegistration.RegisterAllAreas();

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
				namespaces: new string[] { "CJG.Web.External.Controllers" }
			);
		}
	}
}
