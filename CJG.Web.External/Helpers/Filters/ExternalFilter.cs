using System.Security.Claims;
using System.Web.Mvc;
using NLog;
using System.Web.Routing;
using CJG.Application.Services;
using CJG.Core.Entities;

namespace CJG.Web.External.Helpers.Filters
{
	/// <summary>
	/// <typeparamref name="ExternalFilter"/> class, provides a way to decorate External controllers that will block access by Identity authenticated users from the internal side.
	/// </summary>
	public class ExternalFilter : ActionFilterAttribute
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		public ExternalFilter()
		{
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (filterContext.HttpContext.User != null
				&& filterContext.HttpContext.User.Identity.IsAuthenticated
				&& ((ClaimsIdentity)filterContext.HttpContext.User.Identity).FindFirst(AppClaimTypes.AccountType)?.Value != AccountTypes.External.ToString())
			{
				filterContext.Controller.TempData["Message"] = "You must log out of the internal application before logging into the external.";
				filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { area = "Int", controller = "Home", action = "Index" }));
			}

			base.OnActionExecuting(filterContext);
		}
	}
}