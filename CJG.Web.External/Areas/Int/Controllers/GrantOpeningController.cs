using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Int.Models.GrantOpenings;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Int.Controllers
{
	[RouteArea("Int")]
	[RoutePrefix("Admin/Grant")]
	[AuthorizeAction(Privilege.GM1, Privilege.SM)]
	public class GrantOpeningController : BaseController
	{
		#region Variables
		private readonly IStaticDataService _staticDataService;
		private readonly IGrantProgramService _grantProgramService;
		private readonly IGrantOpeningService _grantOpeningService;
		private readonly IGrantStreamService _grantStreamService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly ITrainingPeriodService _trainingPeriodService;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance of a <paramtyperef name="GrantOpeningController"/> object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantProgramService"></param>
		/// <param name="grantOpeningService"></param>
		/// <param name="grantStreamService"></param>
		/// <param name="grantApplicationService"></param>
		/// <param name="trainingPeriodService"></param>
		public GrantOpeningController(
			IControllerService controllerService,
			IGrantProgramService grantProgramService,
			IGrantOpeningService grantOpeningService,
			IGrantStreamService grantStreamService,
			IGrantApplicationService grantApplicationService,
			ITrainingPeriodService trainingPeriodService
		   ) : base(controllerService.Logger)
		{
			_staticDataService = controllerService.StaticDataService;
			_grantProgramService = grantProgramService;
			_grantOpeningService = grantOpeningService;
			_grantStreamService = grantStreamService;
			_grantApplicationService = grantApplicationService;
			_trainingPeriodService = trainingPeriodService;
		}
		#endregion

		#region Endpoints
		/// <summary>
		/// Return the grant opening management dashboard view.
		/// </summary>
		/// <param name="fiscalId"></param>
		/// <param name="grantProgramId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Opening/View/Fiscal/{fiscalId?}/{grantProgramId?}")]
		public ActionResult GrantOpeningView(int? fiscalId, int? grantProgramId)
		{
			ViewBag.FiscalYearId = fiscalId ?? _staticDataService.GetFiscalYear(0)?.Id;
			ViewBag.GrantProgramId = grantProgramId;
			ViewBag.AppDateTime = AppDateTime.UtcNow.ToStringLocalTime();
			return View();
		}

		/// <summary>
		/// Get the grant openings for the specified fiscal year and grant program.
		/// </summary>
		/// <param name="fiscalId"></param>
		/// <param name="grantProgramId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Opening/Fiscal/{fiscalId}/{grantProgramId}")]
		public JsonResult GetGrantOpenings(int fiscalId, int grantProgramId)
		{
			var viewModel = new FiscalGrantOpeningViewModel();
			try
			{
				if (fiscalId > 0 && grantProgramId > 0)
					viewModel = new FiscalGrantOpeningViewModel(fiscalId, grantProgramId, _grantOpeningService, _grantStreamService, _trainingPeriodService);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}

			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get the grant opening details for the selected grant opening.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Opening/View")]
		public ActionResult GetGrantOpeningModalView()
		{
			return PartialView("_GrantOpeningView");
		}

		/// <summary>
		/// Get a default grant opening when creating a new grant opening.
		/// THIS IS A HORRIBLE IMPLEMENTATION
		/// </summary>
		/// <param name="trainingPeriodId"></param>
		/// <param name="grantStreamId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Opening/{trainingPeriodId:int}/{grantStreamId:int}")]
		public JsonResult GetInitialGrantOpening(int trainingPeriodId, int grantStreamId)
		{
			var model = new GrantOpeningViewModel();
			try
			{
				// Just setup a grantOpening, but do not save it, the save will occur when user saves dates / financials
				model.Id = 0;
				if (_grantOpeningService.CheckGrantOpeningByFiscalAndStream(trainingPeriodId, grantStreamId))
				{
					AddGenericError(model, "There are already a opening grant under grant stream for the funding period.");
				}
				else
				{
					var grantStream = _grantStreamService.Get(grantStreamId);
					var trainingPeriod = _trainingPeriodService.Get(trainingPeriodId) ?? throw new NoContentException();
					var grantOpening = new GrantOpening(grantStream, trainingPeriod, 0);
					if (grantOpening.GrantStream.DateFirstUsed == null)
						grantOpening.GrantStream.DateFirstUsed = grantOpening.TrainingPeriod.StartDate;

					model = new GrantOpeningViewModel(grantOpening, _grantOpeningService, _grantApplicationService, User);
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get the data for the specified grant opening.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Opening/{id:int}")]
		public JsonResult GetGrantOpening(int id)
		{
			var model = new GrantOpeningViewModel();
			try
			{
				var grantOpening = _grantOpeningService.Get(id);
				model = new GrantOpeningViewModel(grantOpening, _grantOpeningService, _grantApplicationService, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Add the specified grant opening to the datasource.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Opening")]
		public JsonResult AddGrantOpening(GrantOpeningViewModel model)
		{
			try
			{
				if (_grantOpeningService.CheckGrantOpeningByFiscalAndStream(model.TrainingPeriodId, model.GrantStreamId))
					throw new InvalidOperationException("There are already a opening grant under grant stream for the funding period.");

				var grantOpening = AddOrUpdate(model);
				model = new GrantOpeningViewModel(grantOpening, _grantOpeningService, _grantApplicationService, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		/// <summary>
		/// Update the specified grant opening in the datasource.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Opening")]
		public JsonResult UpdateGrantOpening(GrantOpeningViewModel model)
		{
			try
			{
				var grantOpening = AddOrUpdate(model);
				model = new GrantOpeningViewModel(grantOpening, _grantOpeningService, _grantApplicationService, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		/// <summary>
		/// Delete the specified grant opening in the datasource.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="rowVersion"></param>
		/// <returns></returns>
		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Opening/Delete")]
		public JsonResult DeleteGrantOpening(GrantOpeningViewModel model)
		{
			try
			{
				var grantOpening = _grantOpeningService.Get(model.Id);
				grantOpening.RowVersion = Convert.FromBase64String(model.RowVersion);
				_grantOpeningService.Delete(grantOpening);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		/// <summary>
		/// Schedule the specified grant opening.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="modelExisting"></param>
		/// <returns></returns>
		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Opening/Schedule/{id}")]
		public JsonResult Schedule(int id, GrantOpeningViewModel model)
		{
			try
			{
				var grantOpening = _grantOpeningService.Get(id);
				grantOpening.RowVersion = Convert.FromBase64String(model.RowVersion);
				_grantOpeningService.Schedule(grantOpening);
				model = new GrantOpeningViewModel(grantOpening, _grantOpeningService, _grantApplicationService, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		/// <summary>
		/// Unschedule the specified grant opening.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="modelExisting"></param>
		/// <returns></returns>
		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Opening/Unschedule/{id}")]
		public JsonResult Unschedule(int id, GrantOpeningViewModel model)
		{
			try
			{
				var grantOpening = _grantOpeningService.Get(id);
				grantOpening.RowVersion = Convert.FromBase64String(model.RowVersion);
				_grantOpeningService.Unschedule(grantOpening);
				model = new GrantOpeningViewModel(grantOpening, _grantOpeningService, _grantApplicationService, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		/// <summary>
		/// Close the specified grant opening.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="modelExisting"></param>
		/// <returns></returns>
		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Opening/Close/{id}")]
		public JsonResult Close(int id, GrantOpeningViewModel model)
		{
			try
			{
				var grantOpening = _grantOpeningService.Get(id);
				grantOpening.RowVersion = Convert.FromBase64String(model.RowVersion);
				_grantOpeningService.Close(grantOpening);
				model = new GrantOpeningViewModel(grantOpening, _grantOpeningService, _grantApplicationService, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		/// <summary>
		/// Reopen the specified grant opening.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="modelExisting"></param>
		/// <returns></returns>
		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Opening/Reopen/{id}")]
		public JsonResult Reopen(int id, GrantOpeningViewModel model)
		{
			try
			{
				var grantOpening = _grantOpeningService.Get(id);
				grantOpening.RowVersion = Convert.FromBase64String(model.RowVersion);
				_grantOpeningService.Reopen(grantOpening);
				model = new GrantOpeningViewModel(grantOpening, _grantOpeningService, _grantApplicationService, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		/// <summary>
		/// Open the specified grant opening for submit only.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="modelExisting"></param>
		/// <returns></returns>
		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Opening/OpenForSubmit/{id}")]
		public JsonResult OpenForSubmit(int id, GrantOpeningViewModel model)
		{
			try
			{
				var grantOpening = _grantOpeningService.Get(id);
				grantOpening.RowVersion = Convert.FromBase64String(model.RowVersion);
				_grantOpeningService.OpenForSubmit(grantOpening);
				model = new GrantOpeningViewModel(grantOpening, _grantOpeningService, _grantApplicationService, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		/// <summary>
		/// Return all unfunded applications for the specified grant opening.
		/// </summary>
		/// <param name="grantOpeningId"></param>
		/// <returns></returns>
		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Opening/ReturnUnfundedApplications/{grantOpeningId}")]
		public JsonResult ReturnUnfundedApplications(int grantOpeningId)
		{
			var model = new GrantOpeningViewModel();
			try
			{
				var grantOpening = _grantOpeningService.Get(grantOpeningId);
				_grantApplicationService.ReturnUnfundedApplications(grantOpeningId);
				model = new GrantOpeningViewModel(grantOpening, _grantOpeningService, _grantApplicationService, User);
			}
			catch(Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}
		#endregion

		#region Dropdowns
		/// <summary>
		/// Get a list of fiscal years.
		/// </summary>
		/// <returns></returns>
		[Route("Opening/Fiscal/Years")]
		public JsonResult GetFiscalYears()
		{
			var fiscalYears = _staticDataService.GetFiscalYears().Select(t => new
			{
				t.Id,
				t.Caption
			}).ToArray();
			return Json(fiscalYears, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get a list of grant programs.
		/// </summary>
		/// <param name="year"></param>
		/// <returns></returns>
		[Route("Opening/Programs")]
		public JsonResult GetGrantPrograms()
		{
			var programs = _grantProgramService.GetWithActiveStreams().Select(t => new
			{
				t.Id,
				t.Name
			}).ToArray();
			return Json(programs, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region Support Methods
		private GrantOpening AddOrUpdate(GrantOpeningViewModel model)
		{
			GrantOpening grantOpening;
			var trainingPeriod = _trainingPeriodService.Get(model.TrainingPeriodId);
			var grantStream = _grantStreamService.Get(model.GrantStreamId);

			if (model.Id == 0)
			{
				// Create the Opening
				grantOpening = new GrantOpening(grantStream, trainingPeriod, model.BudgetAllocationAmt);
			}
			else
			{
				// Get the Opening
				grantOpening = _grantOpeningService.Get(model.Id);
			}

			// Assign the dates entered
			grantOpening.PublishDate = DateTime.SpecifyKind(model.PublishDate, DateTimeKind.Local).ToLocalMorning().ToUniversalTime();
			grantOpening.OpeningDate = DateTime.SpecifyKind(model.OpeningDate, DateTimeKind.Local).ToLocalMorning().ToUniversalTime();
			// need to set this to be at the end of the specified day in local time, then converted to UTC
			grantOpening.ClosingDate = DateTime.SpecifyKind(model.ClosingDate, DateTimeKind.Local).ToLocalMidnight().ToUniversalTime();

			// Assign the amounts entered
			grantOpening.BudgetAllocationAmt = model.BudgetAllocationAmt;
			grantOpening.PlanDeniedRate = model.PlanDeniedRate;
			grantOpening.PlanWithdrawnRate = model.PlanWithdrawnRate;
			grantOpening.PlanReductionRate = model.PlanReductionRate;
			grantOpening.PlanSlippageRate = model.PlanSlippageRate;
			grantOpening.PlanCancellationRate = model.PlanCancellationRate;

			// Set date of first use
			if (grantStream.DateFirstUsed == null)
				grantStream.DateFirstUsed = trainingPeriod.StartDate;

			// Calculate the TargetAmt
			grantOpening.IntakeTargetAmt = grantOpening.CalculateIntakeTarget();

			if (model.Id == 0)
			{
				// Insert the GrantOpening
				_grantOpeningService.Add(grantOpening);
			}
			else
			{
				// Update the GrantOpening
				_grantOpeningService.Update(grantOpening);
			}
			return grantOpening;
		}
		#endregion
	}
}