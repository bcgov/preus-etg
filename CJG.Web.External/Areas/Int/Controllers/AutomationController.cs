using System;
using System.Web.Mvc;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Int.Models.Automation;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;

namespace CJG.Web.External.Areas.Int.Controllers
{
    /// <summary>
    /// AutomationController class, provides endpoints to manage automated function settings.
    /// </summary>
    [AuthorizeAction(Privilege.AM4, Privilege.SM)]
	[RouteArea("Int")]
	[RoutePrefix("Admin/Automation")]
	public class AutomationController : BaseController
	{
		private readonly ISettingService _settingService;

		/// <summary>
		/// Creates a new instance of a AutomationController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="settingService"></param>
		public AutomationController(
			IControllerService controllerService,
			ISettingService settingService
		   ) : base(controllerService.Logger)
		{
			_settingService = settingService;
		}

		[HttpGet]
		[Route("View")]
		public ActionResult Index()
		{
			return View();
		}

		[HttpGet, Route("Settings")]
		[ValidateRequestHeader]
		public JsonResult GetSettings()
		{
			var model = new AutomationSettingsViewModel();

			try
			{
				var jobSetting = _settingService.Get(SettingServiceKeys.ReturnUnassessedSettingKey);
				var currentState = jobSetting?.GetValue<bool>() ?? false;

				model = new AutomationSettingsViewModel
				{
					ReturnedToUnassessedServiceState = currentState
				};

				return Json(model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Settings")]
		public JsonResult UpdateSettings(AutomationSettingsViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var setting = _settingService.Get(SettingServiceKeys.ReturnUnassessedSettingKey)
					              ?? new Setting(SettingServiceKeys.ReturnUnassessedSettingKey, model.ReturnedToUnassessedServiceState);
					setting.SetValue(model.ReturnedToUnassessedServiceState);

					_settingService.AddOrUpdate(setting);
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

			model.RedirectURL = "/Int/Admin/Automation/View";
			return Json(model);
		}
	}
}
