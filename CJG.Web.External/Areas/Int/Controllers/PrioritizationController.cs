using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CJG.Application.Services;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Int.Models.Prioritization;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;

namespace CJG.Web.External.Areas.Int.Controllers
{
    /// <summary>
    /// PrioritizationController class, provides endpoints to manage prioritization information.
    /// </summary>
    [AuthorizeAction(Privilege.AM4, Privilege.SM)]
	[RouteArea("Int")]
	[RoutePrefix("Admin/Prioritization")]
	public class PrioritizationController : BaseController
	{
		private readonly IPrioritizationService _prioritizationService;

		/// <summary>
		/// Creates a new instance of a PrioritizationController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="prioritizationService"></param>
		public PrioritizationController(
			IControllerService controllerService,
			IPrioritizationService prioritizationService
		   ) : base(controllerService.Logger)
		{
			_prioritizationService = prioritizationService;
		}

		[HttpGet]
		[Route("Thresholds/View")]
		public ActionResult PrioritizationView()
		{
			return View("Thresholds");
		}

		[HttpGet, Route("Thresholds")]
		[ValidateRequestHeader]
		public JsonResult GetThresholds()
		{
			var model = new PrioritizationThresholdsViewModel();
			try
			{
				var thresholds = _prioritizationService.GetThresholds();
				model = new PrioritizationThresholdsViewModel
				{
					IndustryThreshold = thresholds.IndustryThreshold,
					RegionalThreshold = thresholds.RegionalThreshold,
					EmployeeCountThreshold = thresholds.EmployeeCountThreshold
				};
				return Json(model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		[HttpGet, Route("Regions")]
		[ValidateRequestHeader]
		public JsonResult GetRegions()
		{
			var model = new PrioritizationScoresViewModel();
			try
			{
				var thresholds = _prioritizationService.GetThresholds();
				var regions = _prioritizationService.GetPrioritizationRegions();

				model = new PrioritizationScoresViewModel
				{
					Regions = regions.Select(r => new ScoreViewModel
					{
						Name = r.Name,
						Score = r.RegionalScore,
						IsPriority = r.RegionalScore >= thresholds.RegionalThreshold
					})
				};
				return Json(model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		[HttpGet, Route("Industries")]
		[ValidateRequestHeader]
		public JsonResult GetIndustries()
		{
			var model = new PrioritizationScoresViewModel();
			try
			{
				var thresholds = _prioritizationService.GetThresholds();
				var industries = _prioritizationService.GetPrioritizationIndustryScores();

				model = new PrioritizationScoresViewModel
				{
					Industries = industries.Select(r => new ScoreViewModel
					{
						Name = r.Name,
						Code = r.NaicsCode,
						Score = r.IndustryScore,
						IsPriority = r.IndustryScore <= thresholds.IndustryThreshold
					}),

				};
				return Json(model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Updates the Prioritization thresholds in the datasource.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Thresholds")]
		public JsonResult UpdateThresholds(PrioritizationThresholdsViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var thresholds = _prioritizationService.GetThresholds();
					thresholds.IndustryThreshold = model.IndustryThreshold;
					thresholds.RegionalThreshold = model.RegionalThreshold;
					thresholds.EmployeeCountThreshold = model.EmployeeCountThreshold;

					_prioritizationService.UpdateThresholds(thresholds);
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

			model.RedirectURL = "/Int/Admin/Prioritization/Thresholds/View";
			return Json(model);
		}

		[HttpPost]
		[ValidateRequestHeader]
		[Route("Industries")]
		public JsonResult UpdateIndustries(HttpPostedFileBase file)
		{
			var model = new PrioritizationUpdateScoresFileViewModel();
			int maxUploadSize = int.Parse(ConfigurationManager.AppSettings["MaxUploadSizeInBytes"]);
			string[] permittedAttachmentTypes = ConfigurationManager.AppSettings["ReconciliationPermittedAttachmentTypes"].Split('|');
			try
			{
				var report = _prioritizationService.UpdateIndustryScores(file.Validate(maxUploadSize, permittedAttachmentTypes).InputStream);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[HttpPost]
		[ValidateRequestHeader]
		[Route("Regions")]
		public JsonResult UpdateRegions(HttpPostedFileBase file)
		{
			var model = new PrioritizationUpdateScoresFileViewModel();
			int maxUploadSize = int.Parse(ConfigurationManager.AppSettings["MaxUploadSizeInBytes"]);
			string[] permittedAttachmentTypes = ConfigurationManager.AppSettings["ReconciliationPermittedAttachmentTypes"].Split('|');
			try
			{
				var report = _prioritizationService.UpdateRegionScores(file.Validate(maxUploadSize, permittedAttachmentTypes).InputStream);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}
	}
}
