using CJG.Core.Interfaces.Service;
using CJG.Web.External.Helpers;
using Microsoft.Owin.Security;
using System.Security.Principal;
using System.Web.Mvc;

namespace CJG.Web.External.Controllers
{
	public class ErrorController : BaseController
	{
		#region Variables
		private readonly ISiteMinderService _siteMinderService;
		private readonly IAuthenticationManager _authenticationManager;
		#endregion

		#region Constructors
		public ErrorController(
			IControllerService controllerService,
			IAuthenticationManager authenticationManager) : base(controllerService.Logger)
		{
			_siteMinderService = controllerService.SiteMinderService;
			_authenticationManager = authenticationManager;
		}
		#endregion

		#region Endpoints
		// GET: Error
		public ActionResult Index()
		{
			Response.StatusCode = 500;
			Response.TrySkipIisCustomErrors = true;
			return View("Error");
		}

		public ActionResult InternalServerError()
		{
			ViewBag.StatusCode = 500;
			ViewBag.ErrorMessage = "An unexpected application error occured.";
			ViewBag.Title = "Application Error";
			Response.StatusCode = 500;
			Response.TrySkipIisCustomErrors = true;
			return View("Error");
		}

		public ActionResult BadRequest()
		{
			ViewBag.StatusCode = 400;
			ViewBag.ErrorMessage = "The content you requested may not exist, or you have entered an invalid URL.";
			ViewBag.Title = "Bad Request";
			Response.StatusCode = 400;
			Response.TrySkipIisCustomErrors = true;
			return View("Error");
		}

		public ActionResult Forbidden()
		{
			ViewBag.StatusCode = 403;
			ViewBag.ErrorMessage = "You are not authorized to view this resource";
			ViewBag.Title = "Forbidden";
			Response.StatusCode = 403;
			return View("Error");
		}

		public ViewResult NotFound()
		{
			ViewBag.Title = "Page Not Found";
			Response.StatusCode = 404;
			return View("NotFound");
		}

		public ViewResult Unauthorized()
		{
			ViewBag.ErrorMessage = "You are not authorized to view this resource";
			ViewBag.Title = "Unauthorized";
			Response.StatusCode = 401;
			return View("Error");
		}

		public ViewResult Timeout()
		{
			_authenticationManager.SignOut();
			_siteMinderService.LogOut();
			this.HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
			ViewBag.Title = "Session Timeout";
			return View("Timeout");
		}
		#endregion
	}
}