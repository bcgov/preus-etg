using System;
using System.Web.Mvc;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Models.Shared.Reports;

namespace CJG.Web.External.Areas.Int.Controllers
{
	[RouteArea("Int")]
	public class CompletionReportController : BaseController
	{
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly ICompletionReportService _completionReportService;

		/// <summary>
		/// Creates a new instance of a <typeparamref name="CompletionReportController"/> object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantApplicationService"></param>
		/// <param name="completionReportService"></param>
		public CompletionReportController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			ICompletionReportService completionReportService) : base(controllerService.Logger)
		{
			_grantApplicationService = grantApplicationService;
			_completionReportService = completionReportService;
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
