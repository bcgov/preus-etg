using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Int.Models;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace CJG.Web.External.Areas.Int.Controllers
{
	/// <summary>
	/// UserController class, provides endpoints to manage internal application users.
	/// </summary>
	[RouteArea("Int")]
	[AuthorizeAction(Privilege.UM1)]
	public class UserController : BaseController
	{
		#region Variables
		private readonly IUserService _userService;
		private readonly ApplicationUserManager _userManager;
		private readonly ApplicationRoleManager _roleManager;
		private readonly IGrantApplicationService _grantApplicationService;
		#endregion

		#region Constructors
		public UserController(
			IControllerService controllerService,
			ApplicationUserManager userManager, 
			ApplicationRoleManager roleManager,
			IGrantApplicationService grantApplicationService) : base(controllerService.Logger)
		{
			_userService = controllerService.UserService;
			_userManager = userManager;
			_roleManager = roleManager;
			_grantApplicationService = grantApplicationService;
		}
		#endregion

		#region Endpoints
		/// <summary>
		/// Returns a view to manage internal user accounts.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Admin/Users/View")]
		public ActionResult UserManagementView()
		{
			return View();
		}

		/// <summary>
		/// Returns a paged result of internal user accounts based on the filter.
		/// </summary>
		/// <param name="userFilter"></param>
		/// <param name="page"></param>
		/// <param name="quantity"></param>
		/// <returns></returns>
		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPost]
		[AuthorizeAction(Privilege.UM1)]
		[Route("Admin/Users")]
		public JsonResult GetUsers(UserFilterViewModel userFilter)
		{
			var model = new UserManagementViewModel();
			try {
				int.TryParse(Request.QueryString["page"], out int pageNumber);
				int.TryParse(Request.QueryString["quantity"], out int quantityNumber);

				var pageList = new PageList<UserViewModel>();
				var query = userFilter.GetFilter();
				var users = _userManager.GetUsers(pageNumber, quantityNumber, query);		
				var roles = _roleManager.Roles.ToList();

				pageList.Page = users.Page;
				pageList.Quantity = users.Quantity;
				pageList.Total = users.Total;
				pageList.Items = users.Items.Select(u => new UserViewModel(u, roles)).ToList();

				model = new UserManagementViewModel(pageList);
			} catch (Exception ex) {
				HandleAngularException(ex, model);
			}

			return Json(model);
		}

		/// <summary>
		/// Returns the Modal view.
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Admin/User/View/{userId}")]
		public PartialViewResult UserView(string userId)
		{
			ViewBag.UserId = userId;
			return PartialView("_UserView");
		}

		/// <summary>
		/// The data for the specified user 'id'.
		/// </summary>
		/// <param name="id"></param>
		[HttpGet]
		[Route("Admin/User/{id}")]
		public JsonResult GetUser(string id)
		{
			var user = new UserViewModel();
			try {
				user = new UserViewModel(_userManager.FindById(id), _roleManager.Roles.ToList());
			} catch (Exception ex) {
				HandleAngularException(ex, user);
			}
			return Json(user, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns an Json result of available application roles.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Admin/Application/Roles")]
		public JsonResult GetApplicationRoles()
		{
			var roles = new UserRoleListViewModel();
			try {
				roles = new UserRoleListViewModel(_roleManager.Roles.ToArray());
			} catch (Exception ex) {
				HandleAngularException(ex, roles);
			}
			return Json(roles, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Adds a new internal user to the application.
		/// </summary>
		/// <param name="userViewModel"></param>
		[HttpPost]
		[PreventSpam, ValidateRequestHeader]
		[Route("Admin/User")]
		public async Task<JsonResult> AddUser(UserViewModel model)
		{
			try {
				if (model.IDIR != null) {
					if (_userManager.FindByName(model.IDIR) != null) 
						ModelState.AddModelError("IDIR", "A user with this IDIR already exists.");

					if (_userService.GetIDIRUser(model.IDIR) == null) 
						ModelState.AddModelError("IDIR", "Invalid IDIR.");
				}

				if (ModelState.IsValid) {
					var internalUser = new InternalUser {
						DateAdded = AppDateTime.UtcNow,
						FirstName = model.FirstName,
						LastName = model.LastName,
						Salutation = model.Salutation,
						Email = model.Email,
						PhoneNumber = model.PhoneNumber,
						IDIR = model.IDIR
					};

					var applicationUser = new ApplicationUser {
						Id = _userService.GetIDIRUser(model.IDIR).BCeIDGuid.ToString(),
						Email = model.Email,
						InternalUser = internalUser,
						UserName = model.IDIR,
						Active = model.Active
					};

					var result = await _userManager.CreateAsync(applicationUser);
					if (result.Succeeded) {
						var roles = _roleManager.Roles.ToArray();
						var roleName = roles.FirstOrDefault(r => r.Id == model.RoleId).Name;
						_userManager.AddToRole(applicationUser.Id, roleName);
					} else {
						throw new Exception("An error has occured and the Internal User could not be saved. Please try again later.");
					}
				} else {
					HandleModelStateValidation(model);
				}
			} catch (Exception ex) {
				HandleAngularException(ex, model);
			}

			return Json(model);
		}

		/// <summary>
		/// Updates Internal user.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam, ValidateRequestHeader]
		[Route("Admin/User/")]
		public JsonResult UpdateUser(UserViewModel model)
		{
			try {
				if (ModelState.IsValid) {
					var user = _userManager.FindById(model.ApplicationUserId);
					if (user != null) {
						var roles = _roleManager.Roles.ToList();
						var currentRole = roles.Find(r => r.Id == user.Roles.FirstOrDefault()?.RoleId).Name;
						var newRole = roles.Find(r => r.Id == model.RoleId).Name;

						if (currentRole != newRole && User.Identity.GetUserId() == model.ApplicationUserId)
							throw new InvalidOperationException("You cannot change your own role.");
						
						user.Email = user.InternalUser.Email = model.Email;
						user.InternalUser.FirstName = model.FirstName;
						user.InternalUser.LastName = model.LastName;
						user.InternalUser.PhoneNumber = model.PhoneNumber;
						user.InternalUser.Salutation = model.Salutation;
						user.Active = model.Active;

						if (currentRole != newRole) {
							_userManager.RemoveFromRole(model.ApplicationUserId, currentRole);
							_userManager.AddToRole(model.ApplicationUserId, newRole);
							model.Role = newRole;
						} else {
							var result = _userManager.Update(user);
							if (!result.Succeeded) 
								throw new InvalidOperationException("An error has occured and the Internal User could not be updated.");
							if (!(user.Active ?? false))
							{
								_grantApplicationService.UnassignAssessor(user.InternalUser.Id);
							}
						}
					}
				} else {
					HandleModelStateValidation(model);
				}
			} catch (Exception ex) {
				HandleAngularException(ex, model);
			}

			return Json(model);
		}
		#endregion
	}
}