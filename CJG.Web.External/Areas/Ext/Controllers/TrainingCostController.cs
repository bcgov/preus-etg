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
using CJG.Web.External.Areas.Ext.Models.TrainingCosts;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using Newtonsoft.Json;

namespace CJG.Web.External.Areas.Ext.Controllers
{
    /// <summary>
    /// TrainingCostController class, provides a way to edit training costs.
    /// </summary>
    [ExternalFilter]
	[RouteArea("Ext")]
	public class TrainingCostController : BaseController
	{
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IGrantStreamService _grantStreamService;
		private readonly IAttachmentService _attachmentService;

		/// <summary>
		/// Creates a new instance of a TrainingCostController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantApplicationService"></param>
		/// <param name="grantStreamService"></param>
		/// <param name="attachmentService"></param>
		public TrainingCostController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			IGrantStreamService grantStreamService,
			IAttachmentService attachmentService) : base(controllerService.Logger)
		{
			_grantApplicationService = grantApplicationService;
			_grantStreamService = grantStreamService;
			_attachmentService = attachmentService;
		}

		/// <summary>
		/// Returns a view to edit the training costs.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Training/Cost/View/{id}")]
		public ActionResult TrainingCostView(int id)
		{
			var grantApplication = _grantApplicationService.Get(id);
			if (!User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditTrainingCosts))
				throw new NotAuthorizedException("User does not have permission to edit training costs.");

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
			var model = new TrainingCostViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(id);
				model = new TrainingCostViewModel(grantApplication, User, _grantStreamService);
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
		/// <param name="component"></param>
		/// <param name="files"></param>
		/// <returns></returns>
		[HttpPut]
		[ValidateRequestHeader]
		[Route("Application/Training/Cost")]
		public JsonResult UpdateTrainingCosts(string component, HttpPostedFileBase[] files)
		{
			var model = new TrainingCostViewModel();
			try
			{
				model = JsonConvert.DeserializeObject<TrainingCostViewModel>(component);

				ModelState.Clear();
				TryUpdateModel(model);

				var anyFilesUploaded = files != null && files.Length > 0;
				var existingFileUploaded = model.TravelExpenseDocument?.Id > 0;

				// NEED Extra condition check here
				var ga = _grantApplicationService.Get(model.GrantApplicationId);
				var currentInvites = ga.ParticipantInvitations.Count;
				var currentTakenInvites = ga.ParticipantInvitations.Count(i => i.ParticipantInvitationStatus != ParticipantInvitationStatus.Empty);

				if (model.EstimatedParticipants < currentInvites)
				{
					if (model.EstimatedParticipants < currentTakenInvites)
						ModelState.AddModelError("EstimatedParticipants", "You cannot decrease number of participants below your current Participant invitations. Please go to the Participant Information section and remove some invitations before reducing this amount.");
				}

				var haveAnyTravelExpenses = model.EligibleCosts.Any(ec => ec.EligibleExpenseType.Caption.StartsWith("Travel -"));
				if (haveAnyTravelExpenses && !existingFileUploaded && !anyFilesUploaded)
					ModelState.AddModelError("TravelExpenseDocument", "You must provide a travel expense document.");

				if (ModelState.IsValid)
				{
					var grantApplication = model.UpdateTrainingCosts(_grantApplicationService, _attachmentService, files);
					model = new TrainingCostViewModel(grantApplication, User, _grantStreamService);
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
	}
}