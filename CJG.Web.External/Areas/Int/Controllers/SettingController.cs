using System;
using System.Web.Mvc;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Helpers;
using CJG.Web.External.Controllers;
using System.Net;
using CJG.Core.Interfaces;
using CJG.Web.External.Helpers.Filters;
using CJG.Core.Entities;
using CJG.Application.Services;

namespace CJG.Web.External.Areas.Int.Controllers
{
	/// <summary>
	/// <typeparamref name="SettingController" /> class, provides endpoints for managing the application settings.
	/// </summary>
	[RouteArea("Int")]
	public class SettingController : BaseController
	{
		#region Variables
		private readonly ISiteMinderService _siteMinderService;
		private readonly IUserService _userService;
		private readonly ISettingService _settingService;
		#endregion

		#region Constructors

		/// <summary>
		///     Creates a new instance of a <typeparamref name="SettingController" /> object.
		/// </summary>
		/// <param name="logService"></param>
		/// <param name="settingService"></param>
		public SettingController(
			IControllerService controllerService,
			ISettingService settingService) : base(controllerService.Logger)
		{
			_siteMinderService = controllerService.SiteMinderService;
			_userService = controllerService.UserService;
			_settingService = settingService;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Display the debug screen menu with all settings.
		/// </summary>
		/// <returns></returns>
		[AuthorizeAction(Privilege.SM)]
		[Route("Setting")]
		public ActionResult Index()
		{
			var model = new Models.Settings.AppSettingsViewModel(_settingService);
			return View(model);
		}

		/// <summary>
		/// Save the specified setting in the datasource.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[AuthorizeAction(Privilege.SM)]
		[Route("Setting")]
		public ActionResult Update(Models.Settings.AppSettingsViewModel model)
		{
			try
			{
				var setting = _settingService.Get(model.Key);

				if (setting != null)
				{
					setting.SetValue(model.Value);
					_settingService.Update(setting);
					return new HttpStatusCodeResult(HttpStatusCode.OK);
				}
				else
				{
					return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
				}
			}
			catch (Exception ex)
			{
				return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.GetAllMessages());
			}
		}

		/// <summary>
		/// Get the settings for the current user.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Setting/User/{key}")]
		public JsonResult GetUserSetting(string key)
		{
			var model = new Models.Settings.UserSettingViewModel();
			try
			{
				var setting = _settingService.Get($"{key}-{User.GetUserId()}");
				if (setting == null)
				{
					return null;
				}
				model = new Models.Settings.UserSettingViewModel(setting);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Add or update the user setting.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[ValidateRequestHeader]
		[Route("Setting/User")]
		public JsonResult AddOrUpdateUserSetting(Models.Settings.UserSettingViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var user = _userService.GetInternalUser(User.GetUserId().Value);
					var setting = new Setting(user, $"{model.Key}-{user.Id}", model.Value);

					_settingService.AddOrUpdate(setting);

					model = new Models.Settings.UserSettingViewModel(setting);
				}
				else
				{
					HandleModelStateValidation(model);
				}
			}
			catch(Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}
		#endregion
	}
}