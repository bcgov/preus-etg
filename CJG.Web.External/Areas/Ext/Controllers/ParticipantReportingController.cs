using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Ext.Models.ParticipantReporting;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using System;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Ext.Controllers
{
	[RouteArea("Ext")]
	[ExternalFilter]
	public class ParticipantReportingController : BaseController
	{
		#region Properties
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IClaimService _claimService;
		private readonly IParticipantService _participantService;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a ParticipantReportingController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantApplicationService"></param>
		/// <param name="claimService"></param>
		/// <param name="participantService"></param>
		public ParticipantReportingController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			IClaimService claimService,
			IParticipantService participantService) : base(controllerService.Logger)
		{
			_grantApplicationService = grantApplicationService;
			_claimService = claimService;
			_participantService = participantService;
		}
		#endregion

		#region Endpoints
		/// <summary>
		/// Launched when the 'Report Participants' link on the 'GrantFiles' page is clicked
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Reporting/Participant/View/{grantApplicationId}")]
		public ActionResult ParticipantReportingView(int grantApplicationId)
		{
			var grantApplication = _grantApplicationService.Get(grantApplicationId);

			if (!User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.ViewParticipants))
			{
				this.SetAlert("You are not authorized to manage participants.", AlertType.Error, true);
				return RedirectToAction(nameof(ApplicationViewController.ApplicationDetailsView), nameof(ApplicationViewController).Replace("Controller", ""), new { grantApplicationId });
			}

			ViewBag.GrantApplicationId = grantApplicationId;
			return View();
		}

		/// <summary>
		/// Get the data for the participant reporting view.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Reporting/Participants/{grantApplicationId}")]
		public JsonResult GetParticipantReporting(int grantApplicationId)
		{
			var model = new ReportingViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);

				if (!User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.ViewParticipants))
				{
					throw new NotAuthorizedException("You are not authorized to manage participants.");
				}

				model = new ReportingViewModel(grantApplication, this.HttpContext);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Launched when the 'Set Outcome' modal on the 'ParticipantsReport' page is clicked
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Reporting/Participant/SetOutcome")]
		public ActionResult SetOutcome(ParticipantViewModel model)
		{
			var viewModel = new ReportingViewModel();
			try
			{
				var participant = _participantService.Get(model.Id);
				participant.RowVersion = Convert.FromBase64String(model.RowVersion);
				var grantApplication = participant.GrantApplication;
				_participantService.UpdateExpectedOutcome(participant, model.ExpectedOutcome);

				viewModel = new ReportingViewModel(grantApplication, this.HttpContext);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}

			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Launched when the 'Remove' link on the 'ParticipantsReport' page is clicked
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Reporting/Participant/Delete")]
		public ActionResult RemoveParticipant(ParticipantViewModel model)
		{
			var viewModel = new ReportingViewModel();
			try
			{
				var participant = _participantService.Get(model.Id);
				participant.RowVersion = Convert.FromBase64String(model.RowVersion);
				var grantApplication = participant.GrantApplication;
				_participantService.RemoveParticipant(participant);

				viewModel = new ReportingViewModel(grantApplication, this.HttpContext);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}

			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Toggles whether a participant is included or excluded from the claim currently being reported.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Reporting/Participant/Toggle")]
		public JsonResult ToggleParticipant(IncludeParticipantsViewModel model)
		{
			var viewModel = new ReportingViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(model.GrantApplicationId);
				var claim = grantApplication.GetCurrentClaim();
				claim.RowVersion = Convert.FromBase64String(model.ClaimRowVersion);
				
				foreach (var participantFormId in model.ParticipantFormIds ?? new int[0])
				{
					if (participantFormId <= 0) continue;
					var participantForm = _participantService.Get(participantFormId);

					if (model.Include) _participantService.IncludeParticipant(participantForm);
					else _participantService.ExcludeParticipant(participantForm);
				}

				viewModel = new ReportingViewModel(grantApplication, this.HttpContext);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}
			return Json(viewModel);
		}
		#endregion
	}
}
