using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CJG.Core.Entities.Helpers;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Int.Models.IntakeQueue;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;

namespace CJG.Web.External.Areas.Int.Controllers
{
    /// <summary>
    /// IntakeController class, provides endpoints to select and search for grant applications.
    /// </summary>
    [RouteArea("Int")]
	[Authorize(Roles = "Assessor, System Administrator, Director, Financial Clerk")]
	public class IntakeQueueController : BaseController
	{
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IAuthorizationService _authorizationService;
		private readonly IFiscalYearService _fiscalYearService;
		private readonly IGrantProgramService _grantProgramService;
		private readonly IGrantStreamService _grantStreamService;

		public IntakeQueueController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			IAuthorizationService authorizationService,
			IFiscalYearService fiscalYearService,
			IGrantProgramService grantProgramService,
			IGrantStreamService grantStreamService
		   ) : base(controllerService.Logger)
		{
			_grantApplicationService = grantApplicationService;
			_authorizationService = authorizationService;
			_fiscalYearService = fiscalYearService;
			_grantProgramService = grantProgramService;
			_grantStreamService = grantStreamService;
		}

		[HttpGet]
		[Route("Intake/Queue/View")]
		public ActionResult IntakeQueueView()
		{
			var model = new IntakeQueueUserModel
			{
				CanReturnUnassessed = User.IsInRole("Director")
			};

			return View(model);
		}

		[HttpGet]
		[Route("Intake/Queue/Assessors")]
		public JsonResult GetAssessors()
		{
			IEnumerable<KeyValuePair<int, string>> results = new KeyValuePair<int, string>[0];
			try
			{
				var assessors = _authorizationService.GetAssessors();
				results = assessors.Select(a => new KeyValuePair<int, string>(a.Id, $"{a.FirstName} {a.LastName}")).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(results, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Route("Intake/Queue/Fiscal/Years")]
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
		[Route("Intake/Queue/Training/Periods/{fiscalYearId:int}/{grantStreamId:int?}")]
		public JsonResult GetTrainingPeriods(int fiscalYearId, int? grantStreamId)
		{
			IEnumerable<KeyValuePair<string, string>> results = new KeyValuePair<string, string>[0];
			try
			{
				results = _fiscalYearService.GetTrainingPeriodLabels(fiscalYearId, grantStreamId);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(results, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Route("Intake/Queue/Grant/Programs")]
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
		[Route("Intake/Queue/PrioritizationExceptions")]
		public JsonResult GetTotalExceptions()
		{
			var totalExceptions = 0;
			try
			{
				totalExceptions = _grantApplicationService.CurrentPrioritizationRegionalExceptions();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(new { RegionalExceptions = totalExceptions }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Route("Intake/Queue/Grant/Streams/{grantProgramId?}")]
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
		[Route("Intake/Queue")]
		public JsonResult GetGrantApplications(Models.IntakeQueue.IntakeQueueFilterViewModel filter)
		{
			var model = new PageList<Models.IntakeQueue.GrantApplicationViewModel>();
			try
			{
				int.TryParse(Request.QueryString["page"], out int pageNumber);
				int.TryParse(Request.QueryString["quantity"], out int quantityNumber);

				var query = filter.GetFilter();

				var applications = _grantApplicationService.GetGrantApplications(pageNumber, quantityNumber, query);

				model.Page = applications.Page;
				model.Quantity = applications.Quantity;
				model.Total = applications.Total;
				model.Items = applications.Items.Select(a => new Models.IntakeQueue.GrantApplicationViewModel(a));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, filter);
				return Json(filter);
			}
			return Json(model);
		}


		/// <summary>
		/// Select for assessment the specified grant application.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="rowVersion"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[AuthorizeAction(Privilege.IA1)]
		[Route("Intake/Queue/Select/For/Assessment/{grantApplicationId}")]
		public JsonResult SelectForAssessment(int grantApplicationId, string rowVersion)
		{
			var model = new Models.IntakeQueue.GrantApplicationViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);

				if (string.IsNullOrWhiteSpace(rowVersion))
					throw new InvalidOperationException($"The parameter '{nameof(rowVersion)}' cannot be null, empty or whitespace.");

				grantApplication.RowVersion = Convert.FromBase64String(rowVersion.Replace(" ", "+"));
				_grantApplicationService.SelectForAssessment(grantApplication);

				model = new Models.IntakeQueue.GrantApplicationViewModel(grantApplication);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model);
		}

		/// <summary>
		/// Begin assessment for the specified grant application and assign the specified assessor to it.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="assessorId"></param>
		/// <param name="rowVersion"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[AuthorizeAction(Privilege.IA1)]
		[Route("Intake/Queue/Begin/Assessment/{grantApplicationId}/{assessorId}")]
		public JsonResult BeginAssessment(int grantApplicationId, int assessorId, string rowVersion)
		{
			var model = new Models.IntakeQueue.GrantApplicationViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);

				if (string.IsNullOrWhiteSpace(rowVersion))
					throw new InvalidOperationException($"The parameter '{nameof(rowVersion)}' cannot be null, empty or whitespace.");

				grantApplication.RowVersion = Convert.FromBase64String(rowVersion.Replace(" ", "+"));
				_grantApplicationService.BeginAssessment(grantApplication, assessorId);

				model = new Models.IntakeQueue.GrantApplicationViewModel(grantApplication);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);

			}
			return Json(model);
		}

		/// <summary>
		/// Return the specified grant application without assigning, or assessing it
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="rowVersion"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[AuthorizeAction(Privilege.IA1)]
		[Route("Intake/Queue/Return/Application/{grantApplicationId}")]
		public JsonResult ReturnApplicationUnassessed(int grantApplicationId, string rowVersion)
		{
			var model = new Models.IntakeQueue.GrantApplicationViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);

				if (string.IsNullOrWhiteSpace(rowVersion))
					throw new InvalidOperationException($"The parameter '{nameof(rowVersion)}' cannot be null, empty or whitespace.");

				grantApplication.RowVersion = Convert.FromBase64String(rowVersion.Replace(" ", "+"));
				_grantApplicationService.ReturnUnassessed(grantApplication);

				model = new Models.IntakeQueue.GrantApplicationViewModel(grantApplication);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);

			}
			return Json(model);
		}
	}
}