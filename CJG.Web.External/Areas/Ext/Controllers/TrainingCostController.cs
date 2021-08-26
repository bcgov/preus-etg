using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Ext.Controllers
{
	/// <summary>
	/// TrainingCostController class, provides a way to edit training costs.
	/// </summary>
	[ExternalFilter]
	[RouteArea("Ext")]
	public class TrainingCostController : BaseController
	{
		#region Variables
		private readonly IEligibleExpenseTypeService _eligibleExpenseTypeService;
		private readonly ITrainingProgramService _trainingProgramService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IGrantStreamService _grantStreamService;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a TrainingCostController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantApplicationService"></param>
		/// <param name="grantStreamService"></param>
		public TrainingCostController(
			IControllerService controllerService,
			IEligibleExpenseTypeService eligibleExpenseTypeService,
			ITrainingProgramService trainingProgramService,
			IGrantApplicationService grantApplicationService,
			IGrantStreamService grantStreamService) : base(controllerService.Logger)
		{
			_eligibleExpenseTypeService = eligibleExpenseTypeService;
			_trainingProgramService = trainingProgramService;
			_grantApplicationService = grantApplicationService;
			_grantStreamService = grantStreamService;
		}
		#endregion

		#region Endpoints
		/// <summary>
		/// Returns a view to edit the training costs.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Training/Cost/View/{id}")]
		public ActionResult TrainingCostView(int id)
		{
			var grantAppliation = _grantApplicationService.Get(id);
			if (!User.CanPerformAction(grantAppliation, ApplicationWorkflowTrigger.EditTrainingCosts))
				throw new NotAuthorizedException($"User does not have permission to edit training costs.");

			ViewBag.GrantApplicationId = id;
			return View();
		}

		/// <summary>
		/// Get the data for the training cost view.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Training/Cost/{id}")]
		public JsonResult GetTrainingCosts(int id)
		{
			var model = new Models.TrainingCosts.TrainingCostViewModel();
			try
			{
				var grantAppliation = _grantApplicationService.Get(id);
				model = new Models.TrainingCosts.TrainingCostViewModel(grantAppliation, User, _grantStreamService);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Update the training costs in the datasource.
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		[HttpPut]
		[ValidateRequestHeader]
		[Route("Application/Training/Cost")]
		public JsonResult UpdateTrainingCosts(Models.TrainingCosts.TrainingCostViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var grantApplication = model.UpdateTrainingCosts(_grantApplicationService);
					model = new Models.TrainingCosts.TrainingCostViewModel(grantApplication, User, _grantStreamService);
				}
				else
				{
					HandleModelStateValidation(model);
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model, () =>
				{
					if (ex is DbEntityValidationException)
					{
						model.ValidationErrors = Utilities.GetErrors((DbEntityValidationException)ex, model);
						var errorToDistinct = new KeyValuePair<string, string>(nameof(TrainingCost.TotalEstimatedReimbursement), nameof(EligibleCost.EstimatedParticipantCost));

						var foundErrorToKeep = model.ValidationErrors.Count(t => t.Key == errorToDistinct.Key) > 0;
						var foundErrorToRemove = model.ValidationErrors.Count(t => t.Key == errorToDistinct.Value) > 0;

						if (foundErrorToKeep && foundErrorToRemove)
						{
							var errorToRemove = model.ValidationErrors.FirstOrDefault(t => t.Key == errorToDistinct.Value);
							model.ValidationErrors.Remove(errorToRemove);
						}

						var summary = String.Join("<br/>", model.ValidationErrors.Select(ve => ve.Value).Where(e => !String.IsNullOrEmpty(e)).Distinct().ToArray());
						AddGenericError(model, summary);
					}
				});
			}
			return Json(model);
		}

		/// <summary>
		/// Get all of the eligible expense types for the specified grant application.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Training/Eligible/Expense/Types/{id}")]
		public JsonResult GetEligibleExpenseTypes(int id)
		{
			var model = new List<EligibleExpenseTypeModel>();
			try
			{
				var application = _grantApplicationService.Get(id);
				var streamId = application.GrantOpening.GrantStreamId;

				model.AddRange(_grantStreamService.GetAllActiveEligibleExpenseTypes(streamId)
					.Where(eet => eet.AllowMultiple && !eet.AutoInclude)
					.Select(eet => new EligibleExpenseTypeModel(eet)));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}
		#endregion
	}
}