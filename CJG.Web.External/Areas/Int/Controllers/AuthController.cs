using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Int.Models;
using CJG.Web.External.Helpers;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using CJG.Web.External.Controllers;

namespace CJG.Web.External.Areas.Int.Controllers
{
	/// <summary>
	/// AuthController class, provides endpoints for logging into the internal application.
	/// </summary>
	public class AuthController : BaseController
	{
		#region Variables
		private readonly IAuthenticationService _authenticationService;
		private readonly IAuthenticationManager _authenticationManager;
		private readonly ISiteMinderService _siteMinderService;
		private readonly ApplicationSignInManager _signInManager;
		private readonly ApplicationUserManager _userManager;
		#endregion

		#region Constructors
		public AuthController(
			IControllerService controllerService,
			IAuthenticationService authenticationService, 
			IAuthenticationManager authenticationManager, 
			ApplicationSignInManager signInManager, 
			ApplicationUserManager userManager) : base(controllerService.Logger)
		{
			_siteMinderService = controllerService.SiteMinderService;
			_authenticationService = authenticationService;
			_authenticationManager = authenticationManager;
			_signInManager = signInManager;
			_userManager = userManager;
		}
		#endregion

		#region Methods
		// GET: Auth/LogIn
		public ActionResult LogIn()
		{
			return View(GetLogInModel());
		}

		[HttpPost]
		public async Task<ActionResult> LogIn(LoginViewModel model)
		{
			if (!ModelState.IsValid || model.SelectedUser == null)
			{
				return View(GetLogInModel());
			}
			
			var user = _userManager.Users.FirstOrDefault(u => u.Id == model.SelectedUser);

			if (user == null)
			{
				this.SetAlert("User doesn't exist.", AlertType.Warning);
				return View(GetLogInModel());
			}

			if (user.Active == false)
			{
				this.SetAlert("User is disabled.", AlertType.Warning);
				return View(GetLogInModel());
			}
			
			var guid = Guid.Parse(model.SelectedUser);
			await _signInManager.SignInAsync(user, true, true);
			_authenticationService.LogInInternal(guid);
			return RedirectToAction("Index", "Home");
		}

		private LoginViewModel GetLogInModel()
		{
			var users = _userManager.Users
				.Where(u => u.Active == true)
				.Select(u => new { u.Id, u.InternalUser.FirstName, u.InternalUser.LastName })
				.ToList()
				.Select(u => new KeyValuePair<string, string>(u.Id, $"{u.LastName}, {u.FirstName}"))
				.OrderBy(u => u.Value)
				.ToList();

			users.Insert(0, new KeyValuePair<string, string>("", DropDownListHelper.SelectValueText));

			return new LoginViewModel {UserList = users};
		}

		public ActionResult LogOut()
		{
			// CJG-612: Check that referrer matches. Note, this is handled by STG for frms, but LogOut does not have a form
			//			so we need to check manually here.
			if (!Request.Url.Host.Contains(Request.UrlReferrer.Host))
			{
				return Redirect("~/Error");
			}

			_authenticationManager.SignOut();

			return Redirect(_authenticationService.LogOut());
		}
		#endregion
	}

}