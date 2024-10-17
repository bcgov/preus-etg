using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Int.Models.IntakePeriods;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Int.Controllers
{
	[RouteArea("Int")]
	[RoutePrefix("Admin/IntakePeriods")]
	[AuthorizeAction(Privilege.SM, Privilege.AM4)]
	public class IntakePeriodsController : BaseController
	{
		private readonly IStaticDataService _staticDataService;
		private readonly IGrantProgramService _grantProgramService;
		private readonly IGrantStreamService _grantStreamService;
		private readonly ITrainingPeriodService _trainingPeriodService;

		/// <summary>
		/// Creates a new instance of a <paramtyperef name="IntakePeriodsController"/> object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantProgramService"></param>
		/// <param name="grantStreamService"></param>
		/// <param name="trainingPeriodService"></param>
		public IntakePeriodsController(
			IControllerService controllerService,
			IGrantProgramService grantProgramService,
			IGrantStreamService grantStreamService,
			ITrainingPeriodService trainingPeriodService
		) : base(controllerService.Logger)
		{
			_staticDataService = controllerService.StaticDataService;
			_grantProgramService = grantProgramService;
			_grantStreamService = grantStreamService;
			_trainingPeriodService = trainingPeriodService;
		}

		/// <summary>
		/// Return the intake periods management view.
		/// </summary>
		/// <param name="fiscalId"></param>
		/// <param name="grantProgramId"></param>
		/// <param name="grantStreamId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("View/{fiscalId?}/{grantProgramId?}")]
		public ActionResult IntakePeriodsView(int? fiscalId, int? grantProgramId, int? grantStreamId)
		{
			ViewBag.FiscalYearId = fiscalId ?? _staticDataService.GetFiscalYear(0)?.Id;
			ViewBag.GrantProgramId = grantProgramId;
			ViewBag.GrantStreamId = grantStreamId;
			ViewBag.AppDateTime = AppDateTime.UtcNow.ToStringLocalTime();

			return View();
		}

		/// <summary>
		/// Get the intake periods for the specified fiscal year, grant program and stream.
		/// </summary>
		/// <param name="fiscalId"></param>
		/// <param name="grantProgramId"></param>
		/// <param name="grantStreamId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Periods/{fiscalId}/{grantProgramId}/{grantStreamId}")]
		public JsonResult GetIntakePeriods(int fiscalId, int grantProgramId, int grantStreamId)
		{
			var model = new IntakePeriodsListModel();
			try
			{
				if (fiscalId > 0 && grantProgramId > 0 && grantStreamId > 0)
				{
					model.TrainingPeriods = new List<IntakePeriodsTrainingPeriodModel>();

					var trainingPeriods =_trainingPeriodService.GetAllFor(fiscalId, grantProgramId, grantStreamId);
					foreach (var trainingPeriod in trainingPeriods)
						model.TrainingPeriods.Add(trainingPeriod.ToListModel());
				}
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
		[Route("Fiscal/Years")]
		public JsonResult GetFiscalYears()
		{
			var fiscalYears = _staticDataService.GetFiscalYears()
				.Select(t => new
				{
					t.Id,
					t.Caption
				})
				.ToArray();

			return Json(fiscalYears, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get a list of grant programs.
		/// </summary>
		/// <returns></returns>
		[Route("Programs")]
		public JsonResult GetGrantPrograms()
		{
			var programs = _grantProgramService.GetWithActiveStreams()
				.Select(t => new
				{
					t.Id,
					t.Name
				})
				.ToArray();

			return Json(programs, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get a list of grant streams.
		/// </summary>
		/// <param name="grantProgramId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Streams/{grantProgramId?}")]
		public JsonResult GetGrantStreams(int grantProgramId)
		{
			var programs = _grantStreamService
				.GetGrantStreamsForProgram(grantProgramId)
				.Select(t => new
				{
					t.Id,
					t.Name
				})
				.ToArray();

			return Json(programs, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns the Modal view.
		/// </summary>
		/// <param name="grantStreamId"></param>
		/// <param name="fiscalId"></param>
		/// <param name="grantProgramId"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("IntakePeriod/View/{fiscalId}/{grantProgramId}/{grantStreamId}/{id}")]
		public PartialViewResult IntakePeriodView(int fiscalId, int grantProgramId, int grantStreamId, int id)
		{
			return PartialView("_IntakePeriodView");
		}

		/// <summary>
		/// The data for the specified Intake Period.
		/// </summary>
		/// <param name="fiscalId"></param>
		/// <param name="grantProgramId"></param>
		/// <param name="grantStreamId"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("IntakePeriod/{fiscalId}/{grantProgramId}/{grantStreamId}/{id}")]
		public JsonResult GetIntakePeriod(int fiscalId, int grantProgramId, int grantStreamId, int id)
		{
			var fiscalYear = _staticDataService.GetFiscalYear(fiscalId);
			var intakePeriods = _trainingPeriodService.GetAllFor(fiscalId, grantProgramId, grantStreamId).Count();

			var model = new IntakePeriodsTrainingPeriodModel();
			model.GetEmptyModel(fiscalYear, grantProgramId, grantStreamId, intakePeriods);

			try
			{
				var intakePeriod = _trainingPeriodService.Get(id);
				if (intakePeriod != null)
					model.LoadModel(intakePeriod, fiscalYear);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[PreventSpam, ValidateRequestHeader]
		[Route("IntakePeriod/Save")]
		public JsonResult AddIntakePeriod(IntakePeriodsTrainingPeriodModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var grantStream = _grantStreamService.Get(model.GrantStreamId);
					var grantProgram = grantStream.GrantProgram; // Lazy load
					var fiscalYear = _staticDataService.GetFiscalYear(model.FiscalId);

					var intakePeriod = new TrainingPeriod
					{
						// Due to validation being done at the entity level via Angular, we have to send down super early dates rather than nulls
						StartDate = model.StartDate?.Date.ToUtcMorning() ?? new DateTime(1900, 1, 1).Date.ToUtcMorning(),
						EndDate = model.EndDate?.Date.ToUtcMidnight() ?? new DateTime(1900, 1, 1).Date.ToUtcMidnight(),
						IsActive = model.IsActive,
						GrantStream = grantStream,
						FiscalYear = fiscalYear,
						Caption = model.Caption ?? "Set Intake Period Name" // Temp. Figure out expected named, or use posted name?
					};

					intakePeriod = _trainingPeriodService.Add(intakePeriod);

					model.Id = intakePeriod.Id;
					model.FormattedStartDate = intakePeriod.StartDate.ToStringLocalTime();
					model.FormattedEndDate = intakePeriod.EndDate.ToStringLocalTime();
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

			return Json(model);
		}

		[HttpPut]
		[PreventSpam, ValidateRequestHeader]
		[Route("IntakePeriod/Save")]
		public JsonResult UpdateIntakePeriod(IntakePeriodsTrainingPeriodModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var intakePeriod = _trainingPeriodService.Get(model.Id);
					if (intakePeriod != null)
					{
						var fiscal = intakePeriod.FiscalYear; // Lazy load FiscalYear
						var stream = intakePeriod.GrantStream; // Lazy load GrantStream
						var program = intakePeriod.GrantStream.GrantProgram; // Lazy load GrantProgram
						var grantOpenings = intakePeriod.GrantOpenings.Count; // Lazy load GrantOpenings

						// Due to validation being done at the entity level via Angular, we have to send down super early dates rather than nulls
						intakePeriod.StartDate = model.StartDate?.Date.ToUtcMorning() ?? new DateTime(1900, 1, 1).Date.ToUtcMorning();
						intakePeriod.EndDate = model.EndDate?.Date.ToUtcMidnight() ?? new DateTime(1900, 1, 1).Date.ToUtcMidnight();

						_trainingPeriodService.Update(intakePeriod);

						model.FormattedStartDate = intakePeriod.StartDate.ToStringLocalTime();
						model.FormattedEndDate = intakePeriod.EndDate.ToStringLocalTime();
					}
					else
					{
						throw new InvalidOperationException("An Intake Period by that Id could not be found.");
					}
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

			return Json(model);
		}

		[HttpGet]
		[Route("IntakePeriod/CheckInflight/{id}")]
		public PartialViewResult CheckInflight(int id)
		{
			var period = _trainingPeriodService.Get(id);
			var model = period.ToConfirmationDialogModel();

			return PartialView("_IntakePeriodDisableConfirm", model);
		}
		
		[HttpPut]
		[Route("IntakePeriod/ToggleStatus/{id}")]
		public JsonResult ToggleIntakePeriodStatus(IntakePeriodsTrainingPeriodModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var intakePeriod = _trainingPeriodService.Get(model.Id);
					if (intakePeriod == null)
						throw new InvalidOperationException("An Intake Period by that Id could not be found.");

					var fiscal = intakePeriod.FiscalYear; // Lazy load FiscalYear
					var stream = intakePeriod.GrantStream; // Lazy load GrantStream
					var program = intakePeriod.GrantStream.GrantProgram; // Lazy load GrantProgram
					var grantOpenings = intakePeriod.GrantOpenings.Count; // Lazy load GrantOpenings

					_trainingPeriodService.ToggleStatus(intakePeriod);

					model.LoadModel(intakePeriod, intakePeriod.FiscalYear);
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

			return Json(model);
		}
	}
}
