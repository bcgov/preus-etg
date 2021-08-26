using CJG.Web.External.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NLog;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Helpers;
using CJG.Core.Interfaces.Service.Settings;
using CJG.Web.External.Areas.Int.Models;
using CJG.Core.Entities;
using CJG.Web.External.Helpers.Filters;
using CJG.Infrastructure.Identity;
using CJG.Application.Services;
using CJG.Web.External.Models.Shared.Reports;

namespace CJG.Web.External.Areas.Int.Controllers
{
	[RouteArea("Int")]
	public class CompletionReportController : BaseController
	{
		#region Variables
		private readonly ITrainingProgramService _trainingProgramService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IAttachmentService _attachmentService;
		private readonly IClaimService _claimService;
		private readonly IParticipantService _participantService;
		private readonly ITrainingProviderSettings _trainingProviderSettings;
		private readonly ISettingService _settingService;
		private readonly ICompletionReportService _completionReportService;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="ReportingController"/> object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="trainingProgramService"></param>
		/// <param name="grantApplicationService"></param>
		/// <param name="attachmentService"></param>
		/// <param name="claimService"></param>
		/// <param name="participantService"></param>
		/// <param name="trainingProviderSettings"></param>
		/// <param name="settingService"></param>
		public CompletionReportController(
			IControllerService controllerService,
			ITrainingProgramService trainingProgramService,
			IGrantApplicationService grantApplicationService,
			IAttachmentService attachmentService,
			IClaimService claimService,
			IParticipantService participantService,
			ITrainingProviderSettings trainingProviderSettings,
			ISettingService settingService,
			ICompletionReportService completionReportService) : base(controllerService.Logger)
		{
			_trainingProgramService = trainingProgramService;
			_grantApplicationService = grantApplicationService;
			_attachmentService = attachmentService;
			_claimService = claimService;
			_participantService = participantService;
			_trainingProviderSettings = trainingProviderSettings;
			_settingService = settingService;
			_completionReportService = completionReportService;
		}
		#endregion

		// GET: Int/CompletionReport
		public ActionResult Index()
		{
			return View();
		}

		/// <summary>
		/// View the completion report submitted by the applicant.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Completion/Report/View/{grantApplicationId}")]
		public ActionResult CompletionReportView(int grantApplicationId)
		{
			ViewBag.GrantApplicationId = grantApplicationId;
			return View();
		}

		/// <summary>
		/// Get the data for the completion report view.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Completion/Report/{grantApplicationId}")]
		public JsonResult GetCompletionReport(int grantApplicationId)
		{
			var model = new CompletionReportBaseViewModel();

			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);

				model = new CompletionReportBaseViewModel(grantApplication, _completionReportService);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get the completion report ESS data.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Completion/Report/ESS/{grantApplicationId}")]
		public JsonResult GetEmploymentServicesAndSupports(int grantApplicationId)
		{
			var model = new CompletionReportDynamicCheckboxViewModel();

			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new CompletionReportDynamicCheckboxViewModel(grantApplication);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get the completion report data for the application details view.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Completion/Report/Summary/{grantApplicationId}")]
		public JsonResult GetCompletionReportSummary(int grantApplicationId)
		{
			var grantApplication = _grantApplicationService.Get(grantApplicationId);
			var completionReportStatus = _completionReportService.GetCompletionReportStatus(grantApplication.Id);

			return Json(new { Status = completionReportStatus, RowVersion = Convert.ToBase64String(grantApplication.RowVersion) }, JsonRequestBehavior.AllowGet);
		}
	}
}
