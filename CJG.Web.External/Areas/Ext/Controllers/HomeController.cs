using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
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
		private readonly IOrganizationService _organizationService;
		private readonly IProgramDescriptionService _programDescriptionService;
		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance of a HomeController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantApplicationService"></param>
		/// <param name="grantProgramService"></param>
		/// <param name="grantOpeningService"></param>
		/// <param name="organizationService"></param>
		/// <param name="programDescriptionService"></param>
		public HomeController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			IGrantProgramService grantProgramService,
			IGrantOpeningService grantOpeningService,
			IOrganizationService organizationService,
			IProgramDescriptionService programDescriptionService) : base(controllerService.Logger)
		{
			_userService = controllerService.UserService;
			_siteMinderService = controllerService.SiteMinderService;
			_grantApplicationService = grantApplicationService;
			_grantProgramService = grantProgramService;
			_grantOpeningService = grantOpeningService;
			_organizationService = organizationService;
			_programDescriptionService = programDescriptionService;
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

			if (!_userService.UpdateUserFromBCeIDAccount(currentUser))
			{
				_logger.Debug($"An existing user must complete the account profile - '{_siteMinderService.CurrentUserName}':{_siteMinderService.CurrentUserGuid}");
				return RedirectToAction(nameof(OrganizationProfileController.OrganizationProfileView), nameof(OrganizationProfileController).Replace("Controller", ""));
			}
			//Check if an Organization NAICS code is updated to 2017
			if (!_organizationService.IsOrganizationNaicsStatusUpdated(currentUser.Organization.Id))
			{
				if (currentUser.IsOrganizationProfileAdministrator && _organizationService.NotSubmittedGrantApplications(currentUser.Organization.Id) > 0)
				{
					//Clear NAICS
					_organizationService.ClearOrganizationOldNaicsCode(currentUser.Organization.Id);
				}

				this.SetAlerts("Your organization’s Canada North American Industry Classification System (NAICS) codes are currently out of date. " +
					"The Profile Administrator (individual responsible for your Organization Profile) " +
					"will need to update the NAICS codes on your Organization Profile before submitting an application.", AlertType.Warning);
			}

			if (_organizationService.RequiresBusinessLicenseDocuments(currentUser.Organization.Id))
			{
				_logger.Info($"The Organization is missing up-to-date Business License Documents - {_siteMinderService.CurrentUserGuid}");
				this.SetAlerts("Your organization’s Business Information Documents (e.g. business licence) are currently out of date.", AlertType.Warning);
			}

			if (_organizationService.NotSubmittedGrantApplicationsForUser(currentUser.Organization.Id, currentUser.BCeIDGuid) > 0)
			{
				//Clear NAICS and revert status in case application is complete and not submitted
				_grantApplicationService.RevertApplicationStatus(currentUser.BCeIDGuid, currentUser.Organization.Id);
				_programDescriptionService.ClearApplicationNaics(currentUser.BCeIDGuid, currentUser.Organization.Id);
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
				var result = _grantProgramService.GetImplementedGrantPrograms().Select(t => new
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
