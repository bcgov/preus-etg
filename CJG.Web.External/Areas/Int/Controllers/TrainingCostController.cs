using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Int.Models;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Int.Controllers
{
	[RouteArea("Int")]
	[Authorize(Roles = "Assessor, System Administrator, Director, Financial Clerk")]
	public class TrainingCostController : BaseController
	{
		#region Variables
		private readonly IStaticDataService _staticDataService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IGrantAgreementService _grantAgreementService;
		private readonly IUserService _userService;
		private readonly IGrantStreamService _grantStreamService;
		#endregion

		#region Constructors
		public TrainingCostController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			IGrantAgreementService grantAgreementService,
			IGrantStreamService grantStreamService
		   ) : base(controllerService.Logger)
		{
			_userService = controllerService.UserService;
			_staticDataService = controllerService.StaticDataService;
			_grantApplicationService = grantApplicationService;
			_grantAgreementService = grantAgreementService;
			_grantStreamService = grantStreamService;
		}
		#endregion

		#region Endpoints
		/// <summary>
		/// Get the data for the ApplicationOverviewView page.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("application/training/cost/{grantApplicationId}")]
		public JsonResult GetTrainingCost(int grantApplicationId)
		{
			var viewModel = new ProgramCostViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				viewModel = new ProgramCostViewModel(grantApplication);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}
			var jsonResult = Json(viewModel, JsonRequestBehavior.AllowGet);
			jsonResult.MaxJsonLength = int.MaxValue;
			return jsonResult;
		}

		/// <summary>
		/// Get all of the eligible expense types for the specified grant stream.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[PreventSpam]
		[Route("application/training/cost/eligible/expense/types/{grantStreamId}")]
		public JsonResult GetEligibleExpenseTypes(int grantStreamId)
		{
			var model = new List<EligibleExpenseTypeModel>();
			try
			{
				model.AddRange(_grantStreamService.GetAllActiveEligibleExpenseTypes(grantStreamId)
					.Where(eet => eet.AllowMultiple && !eet.AutoInclude)
					.Select(eet => new EligibleExpenseTypeModel(eet)));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Update the training costs in the datasource.
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Application/Training/Cost")]
		public JsonResult UpdateTrainingCosts(ProgramCostViewModel viewModel)
		{
			try
			{
				if (viewModel.TrainingCost.AgreedParticipants == null)
					throw new InvalidOperationException("You must enter the number of participants.");

				var grantApplication = _grantApplicationService.Get(viewModel.TrainingCost.GrantApplicationId);
				var trainingCost = _grantApplicationService.Update(viewModel.TrainingCost);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel, () =>
				{
					if (ex is DbEntityValidationException)
					{
						viewModel.ValidationErrors = Utilities.GetErrors((DbEntityValidationException)ex, viewModel);
						var errorToDistinct = new KeyValuePair<string, string>(nameof(TrainingCost.TotalEstimatedReimbursement), nameof(EligibleCost.EstimatedParticipantCost));

						var foundErrorToKeep = viewModel.ValidationErrors.Count(t => t.Key == errorToDistinct.Key) > 0;
						var foundErrorToRemove = viewModel.ValidationErrors.Count(t => t.Key == errorToDistinct.Value) > 0;

						if (foundErrorToKeep && foundErrorToRemove)
						{
							var errorToRemove = viewModel.ValidationErrors.FirstOrDefault(t => t.Key == errorToDistinct.Value);
							viewModel.ValidationErrors.Remove(errorToRemove);
						}

						var summary = String.Join("<br/>", viewModel.ValidationErrors.Select(ve => ve.Value).Where(e => !String.IsNullOrEmpty(e)).Distinct().ToArray());
						AddGenericError(viewModel, summary);
					}
				});
			}
			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}
		#endregion
	}
}
