using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using System;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Ext.Controllers
{
	/// <summary>
	/// <typeparamref name="AuthController"/> class, MVC controller for authenticating external users.
	/// </summary>
	public class AuthController : BaseController
	{
		#region Variables
		private readonly IAuthenticationService _authenticationService;
		#endregion

		#region Constructors
		
		/// <summary>
		/// Creates a new instance of a <typeparamref name="AuthController"/> object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="authenticationService"></param>
		/// <param name="signInManager"></param>
		/// <param name="userManager"></param>
		public AuthController(
			IControllerService controllerService,
			IAuthenticationService authenticationService) : base(controllerService.Logger)
		{
			_authenticationService = authenticationService;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Display login page.
		/// </summary>
		/// <returns></returns>
		public ActionResult LogIn()
		{
			return View(new LogInViewModel(_authenticationService.GetLogInOptions(AccountTypes.External)));
		}

		/// <summary>
		/// Log the external user into the application.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult LogIn(LogInViewModel model)
		{
			if (!ModelState.IsValid)
			{
				this.SetAlert("Invalid user name or password.", AlertType.Warning, true);
				return RedirectToAction("Login");
			}
			else
			{
				try
				{
					var userGuid = Guid.Parse(model.SelectedUser);
					var userType = "Business";
					_authenticationService.LogIn(userGuid, userType);

					return RedirectToAction("Index", "Home");
				}
				catch (Exception ex)
				{
					this.SetAlert(ex, true);
					return RedirectToAction(nameof(LogIn));
				}
			}
		}

		/// <summary>
		/// Log the external user out of the application.
		/// </summary>
		/// <returns></returns>
		public ActionResult LogOut()
		{
			return Redirect(_authenticationService.LogOut());
		}
		#endregion
	}
}