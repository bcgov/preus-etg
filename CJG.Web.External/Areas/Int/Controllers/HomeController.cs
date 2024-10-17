using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Web.Mvc;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Int.Models;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;

namespace CJG.Web.External.Areas.Int.Controllers
{
    /// <summary>
    /// HomeController class, provides default home page endpoints.
    /// </summary>
    public class HomeController : BaseController
	{
		private readonly ISiteMinderService _siteMinderService;
		private readonly ApplicationUserManager _userManager;
		private readonly ApplicationSignInManager _signInManager;
		private readonly IAuthenticationManager _authenticationManager;

		public HomeController(
			IControllerService controllerService,
			ApplicationUserManager userManager, 
			ApplicationSignInManager signInManager, 
			IAuthenticationManager authenticationManager) : base(controllerService.Logger)
		{
			_siteMinderService = controllerService.SiteMinderService;
			_userManager = userManager;
			_signInManager = signInManager;
			_authenticationManager = authenticationManager;
		}

		[Authorize]
		public ActionResult Index()
		{
			var model = new IndexModel();
			if (TempData["Message"] != null)
			{
				ViewBag.Message = TempData["Message"].ToString();
				TempData["Message"] = "";
			}
			return View(model);
		}

		[HttpGet]
		public ActionResult Logout()
		{
			// CJG-612: Check that referrer matches. Note, this is handled by STG for forms, but LogOut does not have a form
			//			so we need to check manually here.
			if (!Request.Url.Host.Contains(Request.UrlReferrer.Host))
				return Redirect("~/Error");

			_authenticationManager.SignOut();
			_siteMinderService.LogOut();

			return RedirectToAction("Index", "Home", new { area = ""});
		}

		[ChildActionOnly]
		public ActionResult InternalHeaderPartial()
		{
			var user = _userManager.FindById(User.Identity.GetUserId());
			return PartialView("_InternalHeader", user?.InternalUser);
		}

		[HttpGet]
		[AuthorizeAction(Privilege.IA1)]
		public ActionResult MyFiles()
		{
			return RedirectToRoute(nameof(WorkQueueController.WorkQueueView));
		}
	}
}