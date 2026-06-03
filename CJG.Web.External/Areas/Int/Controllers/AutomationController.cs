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
				var unassessedServiceSetting = GetCurrentStateForKey(SettingServiceKeys.ReturnUnassessedSettingKey);
				var eiServiceSetting = GetCurrentStateForKey(SettingServiceKeys.EiEligibilityCheckServiceSettingKey);

				model = new AutomationSettingsViewModel
				{
					ReturnedToUnassessedServiceState = unassessedServiceSetting,
					EiEligibilityCheckServiceState = eiServiceSetting
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
					UpdateSettingByKey(SettingServiceKeys.ReturnUnassessedSettingKey, model.ReturnedToUnassessedServiceState);
					UpdateSettingByKey(SettingServiceKeys.EiEligibilityCheckServiceSettingKey, model.EiEligibilityCheckServiceState);
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

		private bool GetCurrentStateForKey(string settingKey)
		{
			var jobSetting = _settingService.Get(settingKey);
			var currentState = jobSetting?.GetValue<bool>() ?? false;

			return currentState;
		}

		private void UpdateSettingByKey(string settingKey, bool setValueTo)
		{
			var setting = _settingService.Get(settingKey)
			              ?? new Setting(settingKey, setValueTo);

			setting.SetValue(setValueTo);

			_settingService.AddOrUpdate(setting);
		}
	}
}
