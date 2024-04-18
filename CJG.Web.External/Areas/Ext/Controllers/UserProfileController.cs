using CJG.Application.Services;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.BCeID.WebService;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Ext.Controllers
{
	/// <summary>
	/// UserProfileController class, MVC Controller provides endpoints to manage the external User.
	/// </summary>
	[ExternalFilter]
	[RouteArea("Ext")]
	public class UserProfileController : BaseController
	{
		#region Variables
		private readonly ISiteMinderService _siteMinderService;
		private readonly IUserService _userService;
		private readonly IOrganizationService _organizationService;
		private readonly IStaticDataService _staticDataService;
		private readonly IAuthenticationService _authenticationService;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a UserProfileController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="organizationService"></param>
		/// <param name="authenticationService"></param>
		public UserProfileController(
			IControllerService controllerService,
			IOrganizationService organizationService,
			IAuthenticationService authenticationService) : base(controllerService.Logger)
		{
			_userService = controllerService.UserService;
			_siteMinderService = controllerService.SiteMinderService;
			_staticDataService = controllerService.StaticDataService;
			_organizationService = organizationService;
			_authenticationService = authenticationService;
		}
		#endregion

		#region Endpoints
		#region User Profile Endpoints
		/// <summary>
		/// Display the User Profile creation form View.
		/// </summary>
		/// <returns></returns>
		[Route("User/Profile/Create/View")]
		public ActionResult CreateUserProfileView()
		{
			var currentUser = _userService.GetUser(_siteMinderService.CurrentUserGuid);
			ViewBag.UserId = currentUser?.Id ?? 0;
			return View();
		}

		/// <summary>
		/// Get the data for the user profile page.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("User/Profile/{id}")]
		public JsonResult GetUserProfile(int id)
		{
			var viewModel = new UserProfileViewModel();
			try
			{
				var currentUser = _userService.GetUser(_siteMinderService.CurrentUserGuid) ?? _userService.GetBCeIDUser(_siteMinderService.CurrentUserGuid) ?? throw new NotAuthorizedException("BCeID user is not valid.");
				if (_userService.SyncOrganizationFromBCeIDAccount(currentUser))
					_authenticationService.Refresh(currentUser);

				viewModel.UserId = id;
				viewModel.UserProfileDetails = _userService.GetUserProfileDetails(id);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}
			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Create a new user profile in the datasource.
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		[HttpPost]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("User/Profile")]
		public JsonResult CreateUserProfile(UserProfileViewModel viewModel)
		{
			try
			{
				viewModel.UserProfileDetails.Provinces = _staticDataService.GetProvinces().OrderBy(x => x.Name).Select(x => new KeyValuePair<string, string>(x.Id, x.Name)).ToList();

				if (viewModel.UserProfileDetails.MailingAddressSameAsBusiness)
				{
					ModelState.Remove("UserProfileDetails.MailingAddressLine1");
					ModelState.Remove("UserProfileDetails.MailingCity");
					ModelState.Remove("UserProfileDetails.MailingPostalCode");
					ModelState.Remove("UserProfileDetails.MailingRegionId");
					ModelState.Remove("UserProfileDetails.MailingCountryId");
				}

				if (ModelState.IsValid)
				{
					_userService.CreateUserProfile(_siteMinderService.CurrentUserGuid, viewModel.UserProfileDetails);
				}
				else
				{
					HandleModelStateValidation(viewModel);
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}

			return Json(viewModel);
		}

		/// <summary>
		/// Display the User Profile creation form View.
		/// </summary>
		/// <returns></returns>
		[Route("User/Profile/Update/View")]
		public ActionResult UpdateUserProfileView()
		{
			var currentUser = _userService.GetUser(_siteMinderService.CurrentUserGuid);
			ViewBag.UserId = currentUser?.Id ?? 0;
			ViewBag.BackURL = "/Ext/Home";
			return View();
		}

		/// <summary>
		/// Update the user profile in the datasource.
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("User/Profile")]
		public JsonResult UpdateUserProfile(UserProfileViewModel viewModel)
		{
			try
			{
				viewModel.UserProfileDetails.Provinces = _staticDataService.GetProvinces().OrderBy(x => x.Name).Select(x => new KeyValuePair<string, string>(x.Id, x.Name)).ToList();

				if (viewModel.UserProfileDetails.MailingAddressSameAsBusiness)
				{
					ModelState.Remove("UserProfileDetails.MailingAddressLine1");
					ModelState.Remove("UserProfileDetails.MailingCity");
					ModelState.Remove("UserProfileDetails.MailingPostalCode");
					ModelState.Remove("UserProfileDetails.MailingRegionId");
					ModelState.Remove("UserProfileDetails.MailingCountryId");
				}

				if (ModelState.IsValid)
				{
					_userService.UpdateUserProfile(viewModel.UserId, viewModel.UserProfileDetails);
				}
				else
				{
					viewModel.ValidationErrors = GetClientErrors();
					AddGenericError(viewModel, ModelState.GetErrorMessages("<br />"));
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}

			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// View user profile confirmation page.
		/// </summary>
		/// <returns></returns>
		[Route("User/Profile/Confirm/View")]
		public ActionResult ConfirmDetailsView()
		{
			var bceidUser = _userService.GetBCeIDUser(_siteMinderService.CurrentUserGuid);

			if (bceidUser == null)
				throw new BCeIDException("Invalid BCeID for user, unable to confirm this account.");

			return View();
		}

		/// <summary>
		/// Get the data for the user profile confirmation page.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("User/Profile/Confirm")]
		public JsonResult GetConfirmDetails()
		{
			var viewModel = new UserProfileViewModel();
			try
			{
				viewModel.UserProfileConfirmation = _userService.GetConfirmationDetails(_siteMinderService.CurrentUserGuid);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}
			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#endregion
	}
}
