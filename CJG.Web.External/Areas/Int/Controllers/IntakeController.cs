using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Int.Models;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Int.Controllers
{
	/// <summary>
	/// IntakeController class, provides endpoints to manage Streams and GrantOpenings.
	/// </summary>
	[AuthorizeAction(Privilege.IA4)]
	public class IntakeController : BaseController
	{
		#region Variables
		private readonly IStaticDataService _staticDataService;
		private readonly IGrantOpeningService _grantOpeningService;
		private readonly IGrantProgramService _grantProgramService;
		private readonly IGrantStreamService _grantStreamService;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a IntakeController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantOpeningService"></param>
		/// <param name="grantProgramService"></param>
		/// <param name="grantStreamService"></param>
		public IntakeController(
			IControllerService controllerService,
			IGrantOpeningService grantOpeningService,
			IGrantProgramService grantProgramService,
			IGrantStreamService grantStreamService) : base(controllerService.Logger)
		{
			_staticDataService = controllerService.StaticDataService;
			_grantOpeningService = grantOpeningService;
			_grantProgramService = grantProgramService;
			_grantStreamService = grantStreamService;
		}
		#endregion

		#region Endpoints
		/// <summary>
		/// Gets intake management related data.
		/// </summary>
		/// <param name="grantProgramId"></param>
		/// <param name="grantStreamId"></param>
		/// <param name="trainingPeriodId"></param>
		/// <returns>ActionResult</returns>
		[HttpGet]
		public ActionResult IntakeManagementDashboard(int? grantProgramId, int? grantStreamId, int? trainingPeriodId)
		{
			var intakeManagementModel = new IntakeManagementBuilder(_staticDataService, _grantProgramService, _grantStreamService, _grantOpeningService)
					.Build(grantProgramId, grantStreamId, trainingPeriodId);

			ValidateIntakeModel(intakeManagementModel);

			return View(intakeManagementModel);
		}

		/// <summary>
		/// Gets intake management related data.  
		/// </summary>
		/// <param name="grantProgramId"></param>
		/// <param name="grantStreamId"></param>
		/// <param name="trainingPeriodId"></param>
		/// <param name="navigate"></param>
		/// <returns>ActionResult</returns>
		[HttpPost]
		public ActionResult IntakeManagementDashboard(int? grantProgramId, int? grantStreamId, int? trainingPeriodId, string navigate)
		{
			Enum.TryParse(navigate, true, out IntakeManagementViewModel.NavigationCommand navigationCommand);

			ModelState.Clear();

			var intakeManagementModel = new IntakeManagementBuilder(_staticDataService, _grantProgramService, _grantStreamService, _grantOpeningService)
				.Build(grantProgramId, grantStreamId, trainingPeriodId, navigationCommand);

			ValidateIntakeModel(intakeManagementModel);

			return View(intakeManagementModel);
		}
		#endregion

		#region Helpers
		/// <summary>
		/// Checks the intake model input for invalid values.
		/// </summary>
		/// <param name="model"></param>
		private void ValidateIntakeModel(IntakeManagementViewModel model)
		{
			if (model.TrainingPeriods.Count == 0)
				this.SetAlert("Cannot find current training period.", AlertType.Warning, false);

			if (model.GrantPrograms.Count() == 0)
				this.SetAlert("Cannot find any active Grant Programs.", AlertType.Warning, false);

			if (model.GrantStreams.Count() == 0)
				this.SetAlert("There are currently no active Grant Streams for the chosen Grant Program.", AlertType.Warning, false);
		}
		#endregion
	}
}