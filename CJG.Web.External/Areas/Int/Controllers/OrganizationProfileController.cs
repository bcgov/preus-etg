using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Int.Models.Organizations;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Controllers
{
    [AuthorizeAction(Privilege.AM2, Privilege.AM3, Privilege.AM5)]
	[RouteArea("Int")]
	[RoutePrefix("Admin")]
	public class OrganizationProfileController : BaseController
	{
		private readonly IOrganizationService _organizationService;
		private readonly IUserService _userService;

		/// <summary>
		/// Creates a new instance of a <paramtyperef name="OrganizationProfileController"/> object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="organizationService"></param>

		public OrganizationProfileController(
			IControllerService controllerService,
			IOrganizationService organizationService
		   ) : base(controllerService.Logger)
		{
			_organizationService = organizationService;
			_userService = controllerService.UserService;
		}

		/// <summary>
		/// Returns a view that provides a way to search for organizations and set their profile administrators.
		/// </summary>
		/// <returns></returns>
		[Route("Organization/View")]
		public ActionResult OrganizationView()
		{
			return View();
		}

		/// <summary>
		/// Get the data for the organization profile view.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Organization")]
		public JsonResult GetOrganization()
		{
			var model = new OrganizationProfileViewModel();
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Update the organization with the specified profile administrators.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam, ValidateRequestHeader]
		[Route("Organization")]
		public ActionResult UpdateOrganization(OrganizationProfileViewModel model)
		{
			try
			{
				var selectedUser = _userService.GetUser(model.SelectedUserId);
				var originalUser = _userService.GetUsersForOrganization(selectedUser.OrganizationId).FirstOrDefault(o => o.IsOrganizationProfileAdministrator);
				if (originalUser?.Id != model.SelectedUserId)
				{
					selectedUser.RowVersion = Convert.FromBase64String(model.RowVersion);
					selectedUser.IsOrganizationProfileAdministrator = true;
					_userService.Update(selectedUser);
					if (originalUser != null)
					{
						originalUser.IsOrganizationProfileAdministrator = false;
						_userService.Update(originalUser);
					}
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Search for organizations with the legal name that matches the specified search criteria.
		/// </summary>
		/// <param name="search"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Organization/Organizations")]
		public JsonResult GetOrganizations(string search)
		{
			try
			{
				var organizations = _organizationService.Search(search);
				var result = organizations.Select(o => new KeyValuePair<int, string>(o.Id, o.LegalName)).ToArray();
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				var model = new BaseViewModel();
				HandleAngularException(ex, model);
				return Json(model, JsonRequestBehavior.AllowGet);
			}
		}

		/// <summary>
		/// Get all the users for the specified organization Id.
		/// </summary>
		/// <param name="organizationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Organization/Users/{organizationId}")]
		public JsonResult GetUsers(int organizationId)
		{
			try
			{
				return Json(new OrganizationProfileUserListViewModel(_userService, _organizationService, organizationId), JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				var model = new BaseViewModel();
				HandleAngularException(ex, model);
				return Json(model, JsonRequestBehavior.AllowGet);
			}
		}
	}
}
