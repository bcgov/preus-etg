using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Int.Models;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;

namespace CJG.Web.External.Areas.Int.Controllers
{
    [RouteArea("Int")]
	[Authorize(Roles = "Assessor, System Administrator, Director, Financial Clerk")]
	public class TrainingCostController : BaseController
	{
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IGrantStreamService _grantStreamService;
		private readonly IAttachmentService _attachmentService;

		public TrainingCostController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			IGrantAgreementService grantAgreementService,
			IGrantStreamService grantStreamService,
			IAttachmentService attachmentService
		   ) : base(controllerService.Logger)
		{
			_grantApplicationService = grantApplicationService;
			_grantStreamService = grantStreamService;
			_attachmentService = attachmentService;
		}

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
		/// <param name="component"></param>
		/// <param name="files"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Application/Training/Cost")]
		public JsonResult UpdateTrainingCosts(string component, HttpPostedFileBase[] files)
		{
			var model = new ProgramCostViewModel();
			try
			{
				model = Newtonsoft.Json.JsonConvert.DeserializeObject<ProgramCostViewModel>(component);
				TryValidateModel(model);

				if (ModelState.IsValid)
				{

					if (model.TrainingCost.AgreedParticipants == null)
						throw new InvalidOperationException("You must enter the number of participants.");

					var grantApplication = _grantApplicationService.Get(model.TrainingCost.GrantApplicationId);
					var trainingCost = _grantApplicationService.Update(model.TrainingCost);

					if (files != null && files.Any())
					{
						if (model.TravelExpenseDocument != null && files.Any())
						{
							var attachment = files.First().UploadFile(model.TravelExpenseDocument.Description, model.TravelExpenseDocument.FileName);
							attachment.Id = model.TravelExpenseDocument.Id;
							if (model.TravelExpenseDocument.Id == 0)
							{
								trainingCost.TravelExpenseDocument = attachment;
								_attachmentService.Add(attachment, commit: true);
							}
							else
							{
								attachment.RowVersion = Convert.FromBase64String(model.TravelExpenseDocument.RowVersion);
								_attachmentService.Update(attachment, commit: true);
							}
						}
					}
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
			return Json(model, JsonRequestBehavior.AllowGet);
		}
	}
}
