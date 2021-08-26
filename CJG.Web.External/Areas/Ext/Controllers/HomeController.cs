using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Ext.Controllers
{
	/// <summary>
	/// HomeController class, MVC controller provides the home page interaction for the external site.
	/// </summary>
	[ExternalFilter]
	[RouteArea("Ext")]
	public class HomeController : BaseController
	{
		#region Variables
		private readonly ISiteMinderService _siteMinderService;
		private readonly IUserService _userService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IGrantProgramService _grantProgramService;
		private readonly IGrantOpeningService _grantOpeningService;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a HomeController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantApplicationService"></param>
		/// <param name="grantProgramService"></param>
		/// <param name="grantOpeningService"></param>
		public HomeController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			IGrantProgramService grantProgramService,
			IGrantOpeningService grantOpeningService) : base(controllerService.Logger)
		{
			_userService = controllerService.UserService;
			_siteMinderService = controllerService.SiteMinderService;
			_grantApplicationService = grantApplicationService;
			_grantProgramService = grantProgramService;
			_grantOpeningService = grantOpeningService;
		}
		#endregion

		#region Endpoints
		/// <summary>
		/// The default page for the external user.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public ActionResult Index()
		{
			var currentUser = _userService.GetUser(_siteMinderService.CurrentUserGuid);

			if (currentUser == null)
			{
				_logger.Debug($"New BCeID user must first create an account profile - '{_siteMinderService.CurrentUserName}':{_siteMinderService.CurrentUserGuid}");
				return RedirectToAction(nameof(UserProfileController.ConfirmDetailsView), nameof(UserProfileController).Replace("Controller", ""));
			}

			_userService.UpdateUserFromBCeIDAccount(currentUser);

			if (_userService.MustSelectGrantProgramPreferences(currentUser.Id))
			{
				return RedirectToAction(nameof(UserProfileController.GrantProgramPreferencesView), nameof(UserProfileController).Replace("Controller", ""));
			}

			return View();
		}
		
		/// <summary>
		/// Returns the data for the default index page.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="quantity"></param>
		/// <returns></returns>
		[HttpGet, Route("Home/Grant/Applications/{page}/{quantity}")]
		public JsonResult GetGrantApplications(int page, int quantity)
		{
			var model = new BaseViewModel();
			try
			{
				var currentUser = _userService.GetUser(_siteMinderService.CurrentUserGuid);
				var grantApplications = _grantApplicationService.GetGrantApplications(currentUser, page, quantity, x => (x.DateUpdated ?? x.DateAdded));
				var result = new
				{
					RecordsFiltered = grantApplications.Items.Count(),
					RecordsTotal = grantApplications.Total,
					Data = grantApplications.Items.Select(o => new GrantApplicationListDetailsViewModel(o)).ToArray()
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns the grant programs.
		/// </summary>
		/// <returns></returns>
		[HttpGet, Route("Home/Grant/Programs")]
		public JsonResult GetGrantPrograms()
		{
			var model = new BaseViewModel();
			try
			{
				var result = _grantProgramService.GetUserProgramPreferences().Select(t => new
				{
					t.Id,
					t.Name,
					Message = t.ShowMessage ? t.Message : "",
					t.EligibilityDescription,
					GrantOpenings = _grantOpeningService.GetGrantOpenings(t)
					   .Where(o => o.State == GrantOpeningStates.Published || o.State == GrantOpeningStates.Open)
					   .ToList()
					   .Select(x => new
					   {
						   StartDate = x.TrainingPeriod.StartDate.ToStringLocalTime(),
						   EndDate = x.TrainingPeriod.EndDate.ToStringLocalTime()
					   }).Distinct().OrderBy(x => x.StartDate).ToList()
				}).OrderBy(x => x.Name).ToArray();
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}
