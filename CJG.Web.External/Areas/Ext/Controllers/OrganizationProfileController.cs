using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Ext.Models.OrganizationProfile;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Ext.Controllers
{
	/// <summary>
	/// <typeparamref name="OrganizationProfileController"/> class, MVC Controller provides endpoints for managing external organization information.
	/// </summary>
	[ExternalFilter]
	[RouteArea("Ext")]
	public class OrganizationProfileController : BaseController
	{
		#region Variables
		private readonly IAuthenticationService _authenticationService;
		private readonly ISiteMinderService _siteMinderService;
		private readonly IUserService _userService;
		private readonly IStaticDataService _staticDataService;
		private readonly IOrganizationService _organizationService;
		private readonly INaIndustryClassificationSystemService _naIndustryClassificationSystemService;
		#endregion

		#region Constructors
		public OrganizationProfileController(
			IControllerService controllerService,
			IAuthenticationService authenticationService,
			IOrganizationService organizationService,
			INaIndustryClassificationSystemService naIndustryClassificationSystem) : base(controllerService.Logger)
		{
			_authenticationService = authenticationService;
			_userService = controllerService.UserService;
			_siteMinderService = controllerService.SiteMinderService;
			_staticDataService = controllerService.StaticDataService;
			_organizationService = organizationService;
			_naIndustryClassificationSystemService = naIndustryClassificationSystem;
		}
		#endregion

		#region Endpoints
		/// <summary>
		/// Return a view for organization.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Organization/Profile/View")]
		public ActionResult OrganizationProfileView()
		{
			var backUrl = Request.UrlReferrer?.ToString();
			if (TempData["ProfileBackUrl"] != null)
				backUrl = (string)TempData["ProfileBackUrl"];

			TempData["BackURL"] = backUrl;
			return View();
		}

		/// <summary>
		/// Get the data for the organization view.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Organization/Profile")]
		public JsonResult GetOrganization()
		{
			var model = new OrganizationProfileViewNewModel();

			try
			{
				var currentUser = _userService.GetUser(_siteMinderService.CurrentUserGuid);
				var createOrganizationProfile = currentUser.Organization == null || _organizationService.GetOrganizationProfileAdminUserId(currentUser.Organization.Id) == 0;

				if (_userService.SyncOrganizationFromBCeIDAccount(currentUser))
					_authenticationService.Refresh(currentUser);

				model = new OrganizationProfileViewNewModel(currentUser.Organization, _naIndustryClassificationSystemService)
				{
					BackURL = TempData["BackURL"]?.ToString() ?? "/Ext/Home/",
					CreateOrganizationProfile = createOrganizationProfile,
					IsOrganizationProfileAdministrator = currentUser.IsOrganizationProfileAdministrator || model.CreateOrganizationProfile
				};
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Update and save the specified organization.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Organization/Profile")]
		public JsonResult CreateOrganization(OrganizationProfileViewNewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					model.UpdateOrganization(_userService, _siteMinderService, _organizationService);
				}
				else
				{
					HandleModelStateValidation(model);
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Update and save the specified organization.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Organization/Profile")]
		public JsonResult UpdateOrganization(OrganizationProfileViewNewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var currentUser = _userService.GetUser(_siteMinderService.CurrentUserGuid);

					model.UpdateOrganization(_userService, _siteMinderService, _organizationService);
				}
				else
				{
					HandleModelStateValidation(model);
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get the organization types.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Organization/Types")]
		public JsonResult GetOrganizationTypes()
		{
			var model = new BaseViewModel();

			try
			{
				var result = _staticDataService.GetOrganizationTypes().Select(t => new KeyValuePair<int, string>(t.Id, t.Caption)).ToArray();
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get the Legal Structures data.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Organization/Legal/Structures")]
		public JsonResult GetLegalStructures()
		{
			try
			{
				var model = _staticDataService.GetLegalStructures().Select(x => new KeyValuePair<int, string>(x.Id, x.Caption)).ToArray();
				return Json(model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				var model = new BaseViewModel();
				HandleAngularException(ex, model);
				return Json(model, JsonRequestBehavior.AllowGet);
			}
		}

		/// <summary>
		/// Get the Provinces data.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Organization/Provinces")]
		public JsonResult GetProvinces()
		{
			try
			{
				var model = _staticDataService.GetProvinces().Select(x => new KeyValuePair<string, string>(x.Id, x.Name)).ToArray();
				return Json(model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				var model = new BaseViewModel();
				HandleAngularException(ex, model);
				return Json(model, JsonRequestBehavior.AllowGet);
			}
		}

		/// <summary>
		/// Get the NAICS data.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Organization/NAICS/{level}/{parentId?}")]
		public JsonResult GetNAICS(int level, int? parentId)
		{
			try
			{
				var model = _naIndustryClassificationSystemService.GetNaIndustryClassificationSystemChildren(parentId ?? 0, level).Select(n => new KeyValuePair<int, string>(n.Id, $"{n.Code} | {n.Description}")).ToArray();
				return Json(model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				var model = new BaseViewModel();
				HandleAngularException(ex, model);
				return Json(model, JsonRequestBehavior.AllowGet);
			}
		}
		#endregion
	}
}
