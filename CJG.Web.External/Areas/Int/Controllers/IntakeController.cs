using System.Linq;
using System.Web.Mvc;
using CJG.Application.Services;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Int.Models;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;

namespace CJG.Web.External.Areas.Int.Controllers
{
    /// <summary>
    /// IntakeController class, provides endpoints to manage Streams and GrantOpenings.
    /// </summary>
    [AuthorizeAction(Privilege.IA4)]
	public class IntakeController : BaseController
	{
		private readonly IntakeManagementBuilder _intakeManagementBuilder;

		/// <summary>
		/// Creates a new instance of a IntakeController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantOpeningService"></param>
		/// <param name="grantProgramService"></param>
		/// <param name="grantStreamService"></param>
		/// <param name="trainingPeriodService"></param>
		/// <param name="financeInformationService"></param>
		public IntakeController(
			IControllerService controllerService,
			IGrantOpeningService grantOpeningService,
			IGrantProgramService grantProgramService,
			IGrantStreamService grantStreamService,
			ITrainingPeriodService trainingPeriodService,
			IFinanceInformationService financeInformationService
		) : base(controllerService.Logger)
		{
			_intakeManagementBuilder = new IntakeManagementBuilder(controllerService.StaticDataService, grantProgramService, grantStreamService, grantOpeningService, trainingPeriodService, financeInformationService);
		}

		/// <summary>
		/// Gets intake management related data.
		/// </summary>
		/// <param name="fiscalYearId"></param>
		/// <param name="grantStreamId"></param>
		/// <returns>ActionResult</returns>
		[HttpGet]
		public ActionResult IntakeManagementDashboard(int? fiscalYearId, int? grantStreamId, int? budgetTypeId)
		{
			var intakeManagementModel = _intakeManagementBuilder.Build(fiscalYearId, grantStreamId, budgetTypeId);

			ValidateIntakeModel(intakeManagementModel);

			return View(intakeManagementModel);
		}

		/// <summary>
		/// Checks the intake model input for invalid values.
		/// </summary>
		/// <param name="model"></param>
		private void ValidateIntakeModel(IntakeManagementViewModel model)
		{
			if (model.TrainingPeriods.Count == 0)
				this.SetAlert("Cannot find current training period.", AlertType.Warning, false);

			if (!model.GrantPrograms.Any())
				this.SetAlert("Cannot find any active Grant Programs.", AlertType.Warning, false);

			if (!model.GrantStreams.Any())
				this.SetAlert("There are currently no active Grant Streams for the chosen Grant Program.", AlertType.Warning, false);
		}

		[HttpPost]
		public ActionResult SaveIntakeRates(IntakeManagementViewModel model)
		{
			if (ModelState.IsValid)
				_intakeManagementBuilder.SaveRates(model);

			return RedirectToAction("IntakeManagementDashboard", new { fiscalYearId = model.FiscalYearId, grantStreamId = model.GrantStreamId, budgetTypeId = model.BudgetTypeId });
		}
	}
}