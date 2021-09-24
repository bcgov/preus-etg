using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Int.Models.ClaimDashboard;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Int.Controllers
{
	/// <summary>
	/// <paramtyperef name="ClaimController"/> class, provides endpoints to manage the assessment of claims.
	/// </summary>
	[Authorize(Roles = "Assessor, Director, Financial Clerk, System Administrator")]
	[RouteArea("Int")]
	public class ClaimIntakeController : BaseController
	{
		#region Variables
		private readonly IStaticDataService _staticDataService;
		private readonly IGrantProgramService _grantProgramService;
		private readonly IGrantStreamService _grantStreamService;
		private readonly ITrainingPeriodService _trainingPeriodService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IGrantOpeningService _grantOpeningService;
		private readonly IUserManagerAdapter _userManager;
		private readonly IClaimService _claimService;
		private readonly IReportRateService _reportRateService;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a ClaimController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantProgramService"></param>
		/// <param name="grantStreamService"></param>
		/// <param name="grantApplicationService"></param>
		/// <param name="grantOpeningService"></param>
		/// <param name="userManager"></param>
		/// <param name="claimService"></param>
		/// <param name="reportRateService"></param>
		public ClaimIntakeController(
			IControllerService controllerService,
			IGrantProgramService grantProgramService,
			IGrantStreamService grantStreamService,
			IGrantApplicationService grantApplicationService,
			IGrantOpeningService grantOpeningService,
			IUserManagerAdapter userManager,
			IClaimService claimService,
			ITrainingPeriodService trainingPeriodService,
			IReportRateService reportRateService
		   ) : base(controllerService.Logger)
		{
			_staticDataService = controllerService.StaticDataService;
			_grantProgramService = grantProgramService;
			_grantStreamService = grantStreamService;
			_grantApplicationService = grantApplicationService;
			_grantOpeningService = grantOpeningService;
			_userManager = userManager;
			_claimService = claimService;
			_trainingPeriodService = trainingPeriodService;
			_reportRateService = reportRateService;
		}
		#endregion

		#region Endpoints
		/// <summary>
		/// A claim intake management dashboard view.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[AuthorizeAction(Privilege.IA1)]
		[Route("Admin/Claim/View")]
		public ActionResult ClaimManagementDashboardView()
		{
			return View();
		}

		/// <summary>
		/// Get the data for the claim intake management dashboard view.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Admin/Claim")]
		public JsonResult GetClaimManagementDashboard()
		{
			var model = new ClaimDashboardViewModel();

			try
			{
				model = new ClaimDashboardViewModelBuilder(_staticDataService, _grantOpeningService, _grantProgramService, _grantStreamService, _reportRateService, User).Build(model).Calculate();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get a list of fiscal years.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Admin/Claim/Fiscal/Years")]
		public JsonResult GetFiscalYears()
		{
			IEnumerable<KeyValuePair<int, string>> results = new KeyValuePair<int, string>[0];
			try
			{
				var fiscalYears = _staticDataService.GetFiscalYears();
				results = fiscalYears.Select(fy => new KeyValuePair<int, string>(fy.Id, fy.Caption)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(results, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get a list of grant programs.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Admin/Claim/Grant/Programs/{fiscalYearId?}")]
		public JsonResult GetGrantPrograms(int? fiscalYearId)
		{
			IEnumerable<KeyValuePair<int, string>> results = new KeyValuePair<int, string>[0];
			try
			{
				var grantPrograms = _grantProgramService.GetForFiscalYear(fiscalYearId);
				results = grantPrograms.Select(p => new KeyValuePair<int, string>(p.Id, p.Name)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(results, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get a list of grant streams.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Admin/Claim/Grant/Streams/{fiscalYearId?}/{grantProgramId?}")]
		public JsonResult GetGrantStreams(int? fiscalYearId, int? grantProgramId)
		{
			IEnumerable<KeyValuePair<int, string>> results = new KeyValuePair<int, string>[0];
			try
			{
				var grantStreams = _grantStreamService.GetGrantStreams(fiscalYearId, grantProgramId);
				results = grantStreams.Select(s => new KeyValuePair<int, string>(s.Id, s.Name)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(results, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Update the claim management dashboard rates.
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		[HttpPost]
		[AuthorizeAction(Privilege.IA1)]
		[Route("Admin/Claim/Refresh")]
		public JsonResult RefreshClaimManagementDashboard(ClaimDashboardViewModel viewModel)
		{
			var model = new ClaimDashboardViewModel();

			try
			{
				viewModel.UnclaimedSlippageRate =
					viewModel.UnclaimedCancellationRate =
						viewModel.ClaimedSlippageRate = 0;

				model = new ClaimDashboardViewModelBuilder(_staticDataService, _grantOpeningService, _grantProgramService, _grantStreamService, _reportRateService, User).Build(viewModel).Calculate();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// The claim management dashboard data table.
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		[HttpPost]
		[AuthorizeAction(Privilege.IA1)]
		[Route("Admin/Claim/Calculate")]
		public JsonResult CalculateClaimManagementTable(ClaimDashboardViewModel viewModel)
		{
			var model = new ClaimDashboardViewModel();

			try
			{
				model = new ClaimDashboardViewModelBuilder(_staticDataService, _grantOpeningService, _grantProgramService, _grantStreamService, _reportRateService, User).Calculate(viewModel);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Update the claim management dashboard rates.
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		[HttpPost]
		[PreventSpam]
		[ValidateRequestHeader]
		[AuthorizeAction(Privilege.IA1)]
		[Route("Admin/Claim")]
		public JsonResult SaveClaimManagementDashboard(ClaimDashboardViewModel viewModel)
		{
			var model = new ClaimDashboardViewModel();

			try
			{
				if (!User.HasPrivilege(Privilege.AM3))
				{
					throw new NotAuthorizedException();
				}

				if (!viewModel.SelectedFiscalYearId.HasValue)
				{
					throw new InvalidOperationException("Fiscal Year is required field");
				}

				if (!viewModel.SelectedGrantStreamId.HasValue)
				{
					throw new InvalidOperationException("Grant Stream is required field");
				}

				var stream = _grantStreamService.Get(viewModel.SelectedGrantStreamId.Value);
				var reportRate = _reportRateService.Get(
					viewModel.SelectedFiscalYearId ?? 0,
					viewModel.SelectedGrantProgramId ?? 0,
					viewModel.SelectedGrantStreamId ?? 0);

				if (reportRate == null)
				{
					reportRate = new ReportRate()
					{
						FiscalYearId = viewModel.SelectedFiscalYearId ?? 0,
						GrantProgramId = viewModel.SelectedGrantProgramId ?? 0,
						GrantStreamId = (viewModel.SelectedGrantStreamId ?? 0) <= 0 ? 0 : viewModel.SelectedGrantStreamId.Value
					};
					_reportRateService.Add(reportRate);
				}

				reportRate.AgreementCancellationRate = viewModel.UnclaimedCancellationRate;
				reportRate.AgreementSlippageRate = viewModel.UnclaimedSlippageRate;
				reportRate.ClaimSlippageRate = viewModel.ClaimedSlippageRate;
				_reportRateService.CommitTransaction();

				model = new ClaimDashboardViewModelBuilder(_staticDataService, _grantOpeningService, _grantProgramService, _grantStreamService, _reportRateService, User).Build(viewModel).Calculate();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Update the overpayment amounts.
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		[HttpPost]
		[PreventSpam]
		[ValidateRequestHeader]
		[AuthorizeAction(Privilege.IA1)]
		[Route("Admin/Claim/Overpayments")]
		public JsonResult SaveOverpayments(ClaimDashboardViewModel viewModel)
		{
			var model = new ClaimDashboardViewModel();
			try
			{
				if (!(User.HasPrivilege(Privilege.AM3) || User.HasPrivilege(Privilege.AM5)))
				{
					throw new NotAuthorizedException();
				}
				// Update the OverpaymentAmount
				foreach (var column in viewModel.DataColumns)
				{
					if (column.Id != 0 && column.GrantOpeningExists)
					{
						var trainingPeriod = _trainingPeriodService.Get(column.Id);
						if (trainingPeriod == null)
						{
							throw new InvalidOperationException("Training period for overpayments is not on file.");
						}
						// Getting "Non-static method requires a target." when updating. Below retrieves the linked data so it works.
						var getData1 = trainingPeriod.GrantStream;
						var getData2 = getData1.GrantStreamEligibilityQuestions;
						var getData3 = getData1.GrantOpenings;
						var getData4 = getData1.GrantProgram;
						var getData5 = trainingPeriod.GrantOpenings.Count();
						var getData6 = trainingPeriod.FiscalYear.Id;
						trainingPeriod.OverpaymentAmount = column.TotalLine6_Overpayments;
						_trainingPeriodService.Update(trainingPeriod);
					}
				}
				model = new ClaimDashboardViewModelBuilder(_staticDataService, _grantOpeningService, _grantProgramService, _grantStreamService, _reportRateService, User).Build(viewModel).Calculate();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}
		#endregion
	}
}
