using CJG.Application.Services;
using CJG.Core.Entities.Helpers;
using CJG.Core.Interfaces;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Int.Controllers
{
	[RouteArea("Int")]
	[Authorize(Roles = "Director, System Administrator")]
	public class DirectorDashboardController : BaseController
	{
		private readonly IUserService _userService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IGrantOpeningService _grantOpeningService;
		private readonly IFiscalYearService _fiscalYearService;
		private readonly IGrantProgramService _grantProgramService;
		private readonly IGrantStreamService _grantStreamService;
		private readonly IAuthorizationService _authorizationService;

		public DirectorDashboardController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			IGrantOpeningService grantOpeningService,
			IFiscalYearService fiscalYearService,
			IGrantProgramService grantProgramService,
			IGrantStreamService grantStreamService,
			IAuthorizationService authorizationService
		   ) : base(controllerService.Logger)
		{
			_userService = controllerService.UserService;
			_grantApplicationService = grantApplicationService;
			_grantOpeningService = grantOpeningService;
			_fiscalYearService = fiscalYearService;
			_grantProgramService = grantProgramService;
			_grantStreamService = grantStreamService;
			_authorizationService = authorizationService;
		}

		#region Endpoints
		[HttpGet]
		[Route("Application/Batch/Approval/View")]
		public ActionResult ApplicationBatchApprovalView()
		{
			ViewBag.FiscalYearId = _fiscalYearService.GetCurrentFiscalYear()?.Id;
			return View();
		}

		[HttpGet]
		[Route("Application/Batch/Approval/Assessors")]
		public JsonResult GetAssessors()
		{
			IEnumerable<KeyValuePair<int, string>> results = new KeyValuePair<int, string>[0];
			try
			{
				var assessors = _authorizationService.GetAssessors();
				results = assessors.Select(a => new KeyValuePair<int, string>(a.Id, $"{a.FirstName} {a.LastName}")).ToArray();
			}
			catch(Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(results, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Route("Application/Batch/Approval/Fiscal/Years")]
		public JsonResult GetFiscalYears()
		{
			IEnumerable<KeyValuePair<int, string>> results = new KeyValuePair<int, string>[0];
			try
			{
				var fiscalYears = _fiscalYearService.GetFiscalYears();
				results = fiscalYears.Select(fy => new KeyValuePair<int, string>(fy.Id, fy.Caption)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(results, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Route("Application/Batch/Approval/Training/Periods/{fiscalYearId:int}/{grantStreamId:int?}")]
		public JsonResult GetTrainingPeriods(int fiscalYearId, int? grantStreamId)
		{
			IEnumerable<KeyValuePair<string, string>> results = new KeyValuePair<string, string>[0];
			var currentTrainingPeriod = string.Empty;

			try
			{
				results = _fiscalYearService.GetTrainingPeriodLabels(fiscalYearId, grantStreamId);
				var currentPeriod = _fiscalYearService.GetCurrentTrainingPeriodFor(fiscalYearId, grantStreamId);
				if (currentPeriod != null)
					currentTrainingPeriod = currentPeriod.Caption;

			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(new { TrainingPeriods = results, CurrentPeriod = currentTrainingPeriod ?? string.Empty }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Route("Application/Batch/Approval/Grant/Programs")]
		public JsonResult GetGrantPrograms()
		{
			IEnumerable<KeyValuePair<int, string>> results = new KeyValuePair<int, string>[0];
			try
			{
				var grantPrograms = _grantProgramService.GetAll();
				results = grantPrograms.Select(p => new KeyValuePair<int, string>(p.Id, p.Name)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(results, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Route("Application/Batch/Approval/Grant/Streams/{grantProgramId?}")]
		public JsonResult GetGrantStreams(int? grantProgramId)
		{
			IEnumerable<KeyValuePair<int, string>> results = new KeyValuePair<int, string>[0];
			try
			{
				var grantStreams = grantProgramId.HasValue ? _grantStreamService.GetGrantStreamsForProgram(grantProgramId.Value) : _grantStreamService.GetAll();
				results = grantStreams.Select(s => new KeyValuePair<int, string>(s.Id, s.Name)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(results, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[ValidateRequestHeader]
		[Route("Application/Batch/Approval")]
		public JsonResult GetGrantApplications(Models.BatchApprovals.BatchApprovalFilterViewModel filter)
		{
			PageList<Models.BatchApprovals.GrantApplicationViewModel> results = new PageList<Models.BatchApprovals.GrantApplicationViewModel>();
			try
			{
				int.TryParse(Request.QueryString["page"], out int pageNumber);
				int.TryParse(Request.QueryString["quantity"], out int quantityNumber);

				var query = filter.GetFilter();
				var applications = _grantApplicationService.GetGrantApplications(pageNumber, quantityNumber, query);

				results.Page = applications.Page;
				results.Quantity = applications.Quantity;
				results.Total = applications.Total;
				results.Items = applications.Items.Select(a => new Models.BatchApprovals.GrantApplicationViewModel(a));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, filter);
				return Json(filter);
			}
			return Json(results);
		}

		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Application/Batch/Approval/Issue/Offers")]
		public JsonResult IssueOffers(Models.BatchApprovals.BatchApprovalViewModel model)
		{
			var result = new List<Models.BatchApprovals.IssueOfferViewModel>();

			// Must make a massive query for all the applications in the filter.
			if (model.SelectAll)
			{
				var query = model.GetFilter();
				var applications = _grantApplicationService.GetGrantApplications(1, model.Total, query);

				model.GrantApplications = applications.Items.Select(ga => new Models.BatchApprovals.IssueOfferViewModel(ga)).ToList();
			}

			foreach (var application in model.GrantApplications)
			{
				try
				{
					var grantApplication = _grantApplicationService.Get(application.Id);
					grantApplication.RowVersion = Convert.FromBase64String(application.RowVersion);
					_grantApplicationService.IssueOffer(grantApplication);
				}
				catch (Exception ex)
				{
					String errorMsg = null;
					if (ex is DbUpdateConcurrencyException)
					{
						errorMsg = "Someone has updated the record. You must refresh the page.";
					}
					else if (ex is NoContentException ||
						ex is InvalidOperationException ||
						ex is DbUpdateException ||
						ex is DbEntityValidationException)
					{
						if (ex is DbEntityValidationException && model != null)
						{
							if (model.ValidationErrors.Count > 0)
								errorMsg = string.Join("<br/>", model.ValidationErrors.Select(ve => ve.Value).Where(e => !String.IsNullOrEmpty(e)).Distinct().ToArray());
						}
						else if (ex is InvalidOperationException)
						{
							errorMsg = ex.GetAllMessages();
						}
						else if (ex is NoContentException)
						{
							errorMsg = "The content requested does not exist.";
						}
						else
						{
							_logger.Error(ex);
							errorMsg = "An unexpected error occurred, please try again later or contact support.";
						}
					}
					else if (ex is NotAuthorizedException)
					{
						_logger.Warn(ex);
						errorMsg = "You are not authorized to perform this action.";
					}
					else
					{
						_logger.Error(ex);
						errorMsg = "An unexpected error occurred, please try again later or contact support.";
					}

					application.ErrorMessage = errorMsg;
				}

				result.Add(application);
			}
			model.GrantApplications = result;
			return Json(model, JsonRequestBehavior.AllowGet);
		}
		#endregion
	}
}