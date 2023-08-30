using System;
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
using CJG.Web.External.Models.Shared;

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
					IndustryAssignedScore = thresholds.IndustryAssignedScore,

					HighOpportunityOccupationThreshold = thresholds.HighOpportunityOccupationThreshold,
					HighOpportunityOccupationAssignedScore = thresholds.HighOpportunityOccupationAssignedScore,

					RegionalThreshold = thresholds.RegionalThreshold,
					RegionalThresholdAssignedScore = thresholds.RegionalThresholdAssignedScore,

					EmployeeCountThreshold = thresholds.EmployeeCountThreshold,
					EmployeeCountAssignedScore = thresholds.EmployeeCountAssignedScore,

					FirstTimeApplicantAssignedScore = thresholds.FirstTimeApplicantAssignedScore,
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

				var postalCodeCounts = _prioritizationService
					.GetRegionPostalCodeCounts()
					.ToList();

				var regionModels = regions.Select(r => new ScoreViewModel
				{
					Name = r.Name,
					Score = r.RegionalScore,
					IsPriority = r.RegionalScore >= thresholds.RegionalThreshold,
					PostalCodeCount = postalCodeCounts.FirstOrDefault(c => c.Item1 == r.Id)?.Item2 ?? 0
				}).ToList();

                var result = new
                {
                    RecordsFiltered = regionModels.Count,
                    RecordsTotal = regionModels.Count,
                    Data = regionModels
				};
                return Json(result, JsonRequestBehavior.AllowGet);
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

				var industryModels = industries.Select(r => new ScoreViewModel
				{
					Name = r.Name,
					Code = r.NaicsCode,
					Score = r.IndustryScore,
					IsPriority = r.IndustryScore <= thresholds.IndustryThreshold
				}).ToList();

				var result = new
				{
					RecordsFiltered = industryModels.Count,
					RecordsTotal = industryModels.Count,
					Data = industryModels
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		[HttpGet, Route("HOO")]
		[ValidateRequestHeader]
		public JsonResult GetHighOpportunityOccupations()
		{
			var model = new PrioritizationScoresViewModel();
			try
			{
				var thresholds = _prioritizationService.GetThresholds();
				var industries = _prioritizationService.GetPrioritizationHighOpportunityOccupationScores();

				var hooModels = industries.Select(r => new ScoreViewModel
				{
					Name = r.Name,
					Code = r.NOCCode,
					Score = r.HighOpportunityOccupationScore,
					IsPriority = r.HighOpportunityOccupationScore <= thresholds.HighOpportunityOccupationThreshold
				}).ToList();

				var result = new
				{
					RecordsFiltered = hooModels.Count,
					RecordsTotal = hooModels.Count,
					Data = hooModels
				};
				return Json(result, JsonRequestBehavior.AllowGet);
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
					thresholds.HighOpportunityOccupationThreshold = model.HighOpportunityOccupationThreshold;
					thresholds.RegionalThreshold = model.RegionalThreshold;
					thresholds.EmployeeCountThreshold = model.EmployeeCountThreshold;

					thresholds.IndustryAssignedScore = model.IndustryAssignedScore;
					thresholds.HighOpportunityOccupationAssignedScore = model.HighOpportunityOccupationAssignedScore;
					thresholds.RegionalThresholdAssignedScore = model.RegionalThresholdAssignedScore;
					thresholds.EmployeeCountAssignedScore = model.EmployeeCountAssignedScore;
					thresholds.FirstTimeApplicantAssignedScore = model.FirstTimeApplicantAssignedScore;

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

		/// <summary>
		/// Forces the Intake Queue to recalculate all Prioritization Scores
		/// </summary>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Recalculate")]
		public JsonResult RecalculatePriorities()
		{
			var model = new BaseViewModel();
			try
			{
				if (ModelState.IsValid)
				{
					_prioritizationService.RecalculatePriorityScores();
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
		[Route("UpdateIndustries")]
		public JsonResult UpdateIndustries(HttpPostedFileBase file)
		{
			var model = new PrioritizationUpdateScoresFileViewModel();
			int maxUploadSize = int.Parse(ConfigurationManager.AppSettings["MaxUploadSizeInBytes"]);
			string[] permittedAttachmentTypes = ConfigurationManager.AppSettings["PrioritizationPermittedAttachmentTypes"].Split('|');
			try
			{
				_prioritizationService.UpdateIndustryScores(file.Validate(maxUploadSize, permittedAttachmentTypes).InputStream);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[HttpPost]
		[ValidateRequestHeader]
		[Route("UpdateHOO")]
		public JsonResult UpdateHighOpportunityOccupations(HttpPostedFileBase file)
		{
			var model = new PrioritizationUpdateScoresFileViewModel();
			int maxUploadSize = int.Parse(ConfigurationManager.AppSettings["MaxUploadSizeInBytes"]);
			string[] permittedAttachmentTypes = ConfigurationManager.AppSettings["PrioritizationPermittedAttachmentTypes"].Split('|');
			try
			{
				_prioritizationService.UpdateHighOpportunityOccupationScores(file.Validate(maxUploadSize, permittedAttachmentTypes).InputStream);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[HttpPost]
		[ValidateRequestHeader]
		[Route("UpdateRegions")]
		public JsonResult UpdateRegions(HttpPostedFileBase file)
		{
			var model = new PrioritizationUpdateScoresFileViewModel();
			int maxUploadSize = int.Parse(ConfigurationManager.AppSettings["MaxUploadSizeInBytes"]);
			string[] permittedAttachmentTypes = ConfigurationManager.AppSettings["PrioritizationPermittedAttachmentTypes"].Split('|');
			try
			{
				_prioritizationService.UpdateRegionScores(file.Validate(maxUploadSize, permittedAttachmentTypes).InputStream);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}
	}
}
