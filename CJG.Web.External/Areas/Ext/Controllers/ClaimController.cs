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
using CJG.Core.Interfaces.Service.Settings;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Areas.Ext.Models.Attachments;
using CJG.Web.External.Areas.Ext.Models.Claims;
using CJG.Web.External.Areas.Ext.Models.ParticipantReporting;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Ext.Controllers
{
    /// <summary>
    /// <paramtyperef name="ClaimController"/> class, provides endpoints to manage claims.
    /// </summary>
    [RouteArea("Ext")]
	public class ClaimController : BaseController
	{
		#region Variables
		private readonly ISiteMinderService _siteMinderService;
		private readonly IUserService _userService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly ITrainingProgramService _trainingProgramService;
		private readonly ITrainingProviderSettings _trainingProviderSettings;
		private readonly IAttachmentService _attachmentService;
		private readonly IClaimService _claimService;
		private readonly ISettingService _settingService;
		private readonly IClaimEligibleCostService _claimEligibleCostService;
		private readonly IParticipantService _participantService;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <paramtyperef name="ClaimController"/> object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantApplicationService"></param>
		/// <param name="trainingProgramService"></param>
		/// <param name="trainingProviderSettings"></param>
		/// <param name="attachmentService"></param>
		/// <param name="claimService"></param>
		public ClaimController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			ITrainingProgramService trainingProgramService,
			ITrainingProviderSettings trainingProviderSettings,
			IAttachmentService attachmentService,
			IClaimService claimService,
			IClaimEligibleCostService claimEligibleCostService,
			ISettingService settingService,
			IParticipantService participantService) : base(controllerService.Logger)
		{
			_userService = controllerService.UserService;
			_siteMinderService = controllerService.SiteMinderService;
			_trainingProgramService = trainingProgramService;
			_grantApplicationService = grantApplicationService;
			_attachmentService = attachmentService;
			_claimService = claimService;
			_trainingProviderSettings = trainingProviderSettings;
			_settingService = settingService;
			_claimEligibleCostService = claimEligibleCostService;
			_participantService = participantService;
		}
		#endregion

		#region Endpoints
		#region Claim Reporting View
		/// <summary>
		/// Return a view to report a claim.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[RedirectOn(typeof(DbEntityValidationException), nameof(ReportingController.GrantFileView), "Reporting")]
		[Route("Claim/Report/View/{grantApplicationId}")]
		public ActionResult ClaimReportView(int grantApplicationId)
		{
			if (_settingService.Get("EnableClaimsOn")?.GetValue<DateTime>() > AppDateTime.Now)
			{
				return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", ""));
			}

			var grantApplication = _grantApplicationService.Get(grantApplicationId);

			if (!User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.EditClaim) && !User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.CreateClaim))
			{
				this.SetAlert("The Claim is not in a valid state for management.", AlertType.Error, true);
				return RedirectToAction(nameof(ReportingController.GrantFileView), "Reporting", new { grantApplicationId = grantApplication.Id });
			}

			var claim = grantApplication.GetCurrentClaim();

			// If no claim exists, create one.
			// If the claim type is multiple and there isn't one currently being worked on, create a new one.
			if (claim == null || (grantApplication.GetClaimType() == ClaimTypes.MultipleClaimsWithoutAmendments && !(claim?.ClaimState.In(ClaimState.Incomplete, ClaimState.Complete) ?? true)))
			{
				claim = _claimService.AddNewClaim(grantApplication);
			}
			else if (claim.ClaimState == ClaimState.Incomplete)
			{
				// Need to update state if it's ready to be submitted.
				if ((claim.ClaimTypeId == ClaimTypes.SingleAmendableClaim && claim.ParticipantsWithEligibleCosts() == grantApplication.ParticipantForms.Count(pf => !pf.IsExcludedFromClaim))
					|| (claim.ClaimTypeId == ClaimTypes.MultipleClaimsWithoutAmendments && claim.TotalClaimReimbursement != 0))
				{
					claim.ClaimState = ClaimState.Complete;
					_claimService.Update(claim);
				}
			}

			ViewBag.GrantApplicationId = grantApplicationId;
			ViewBag.ClaimId = claim.Id;
			ViewBag.ClaimVersion = claim.ClaimVersion;
			ViewBag.Reporting = true;
			ViewBag.ReviewAndSubmit = false;
			ViewBag.GrantProgramCode = grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramCode;

			return View(SidebarViewModelFactory.Create(grantApplication, ControllerContext));
		}

		/// <summary>
		/// Get the data for the claim report view.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Claim/Report/{grantApplicationId}")]
		public JsonResult GetClaimReport(int grantApplicationId)
		{
			var model = new ClaimReportViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				var claim = grantApplication.GetCurrentClaim();
				model = new ClaimReportViewModel(claim, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			var jsonResult = Json(model, JsonRequestBehavior.AllowGet);
			jsonResult.MaxJsonLength = int.MaxValue;
			return jsonResult;
		}

		/// <summary>
		/// Get the data for the claim report view.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Claim/Warnings/{grantApplicationId}")]
		public JsonResult GetClaimWarnings(int grantApplicationId)
		{
			var model = new ParticipantWarningsModel
			{
				ParticipantWarnings = new List<ParticipantWarningModel>()
			};
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model.ParticipantWarnings = GetParticipantWarnings(grantApplication, _participantService);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			var jsonResult = Json(model, JsonRequestBehavior.AllowGet);
			jsonResult.MaxJsonLength = int.MaxValue;
			return jsonResult;
		}

		private List<ParticipantWarningModel> GetParticipantWarnings(GrantApplication grantApplication, IParticipantService participantService)
		{
			var participantSinList = grantApplication.ParticipantForms
				.Select(pf => new { ParticipantFormId = pf.Id, pf.SIN })
				.ToList();

			var warnings = new List<ParticipantWarningModel>();

			var maxReimbursementAmount = grantApplication.MaxReimbursementAmt;
			var grantApplicationFiscal = grantApplication.GrantOpening.TrainingPeriod.FiscalYearId;

			var applicationClaimStatuses = new List<ApplicationStateInternal>
			{
				ApplicationStateInternal.Closed,
				ApplicationStateInternal.CompletionReporting
			};

			foreach (var participant in grantApplication.ParticipantForms)
			{
				var otherParticipantForms = participantService.GetParticipantFormsBySIN(participant.SIN);

				var participantPayments = 0M;

				foreach (var form in otherParticipantForms.Where(opf => opf.GrantApplicationId != grantApplication.Id)
					.Where(opf => opf.GrantApplication.GrantOpening.TrainingPeriod.FiscalYearId == grantApplicationFiscal)
					.Where(opf => applicationClaimStatuses.Contains(opf.GrantApplication.ApplicationStateInternal)))
				{
					var totalPastCosts = form.ParticipantCosts.Sum(c => c.AssessedReimbursement);
					participantPayments += totalPastCosts;
				}

				warnings.Add(new ParticipantWarningModel
				{
					MappedParticipantFormId = participantSinList.FirstOrDefault(p => p.SIN == participant.SIN)?.ParticipantFormId ?? 0,
					ParticipantName = $"{participant.FirstName} {participant.LastName}",
					CurrentClaims = participantPayments,
					FiscalYearLimit = maxReimbursementAmount
				});
			}

			return warnings;
		}

		/// <summary>
		/// Get the claim attachments view data.
		/// </summary>
		/// <param name="claimId"></param>
		/// <param name="claimVersion"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Claim/Attachments/{claimId}/{claimVersion}")]
		public JsonResult GetClaimAttachments(int claimId, int claimVersion)
		{
			var model = new ClaimAttachmentsViewModel();

			try
			{
				var claim = _claimService.Get(claimId, claimVersion);
				model = new ClaimAttachmentsViewModel(claim);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get the specified attachment.
		/// </summary>
		/// <param name="attachmentId"></param>
		/// <param name="claimId"></param>
		/// <param name="claimVersion"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Claim/Attachment/{attachmentId}/{claimId}/{claimVersion}")]
		public JsonResult GetAttachment(int attachmentId, int claimId, int claimVersion)
		{
			var model = new ClaimAttachmentViewModel();
			try
			{
				var claim = _claimService.Get(claimId, claimVersion);
				var attachment = attachmentId > 0 ? new AttachmentModel(_attachmentService.Get(attachmentId), attachmentId) : new AttachmentModel();
				model = new ClaimAttachmentViewModel(claim, attachment);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Update the attachments (delete/update/create) for the specified grant application.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="files"></param>
		/// <param name="attachments"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Claim/Attachments/{claimId}/{claimVersion}")]
		public JsonResult UpdateAttachments(int claimId, int claimVersion, HttpPostedFileBase[] files, string attachments)
		{
			var model = new ClaimAttachmentsViewModel();
			try
			{
				var claim = _claimService.Get(claimId, claimVersion);

				// Deserialize model.  This is required because it isn't easy to deserialize an array when including files in a multipart data form.
				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<UpdateAttachmentViewModel>>(attachments);

				foreach (var attachment in data)
				{
					if (attachment.Delete) // Delete
					{
						var existing = _attachmentService.Get(attachment.Id);
						existing.RowVersion = Convert.FromBase64String(attachment.RowVersion);
						_claimService.RemoveReceipt(claimId, claimVersion, existing);
					}
					else if (attachment.Index.HasValue == false) // Update data only
					{
						var existing = _attachmentService.Get(attachment.Id);
						existing.RowVersion = Convert.FromBase64String(attachment.RowVersion);
						attachment.MapToEntity(existing);
						_attachmentService.Update(existing, true);
					}
					else if (files.Length > attachment.Index.Value && files[attachment.Index.Value] != null && attachment.Id == 0) // Add
					{
						var file = files[attachment.Index.Value].UploadFile(attachment.Description, attachment.FileName);
						_claimService.AddReceipt(claimId, claimVersion, file);
					}
					else if (files.Length > attachment.Index.Value && files[attachment.Index.Value] != null && attachment.Id != 0) // Update with file
					{
						var file = files[attachment.Index.Value].UploadFile(attachment.Description, attachment.FileName);
						var existing = _attachmentService.Get(attachment.Id);
						existing.RowVersion = Convert.FromBase64String(attachment.RowVersion);

						attachment.MapToEntity(existing);
						existing.AttachmentData = file.AttachmentData;
						_attachmentService.Update(existing, true);
					}
				}

				model = new ClaimAttachmentsViewModel(claim);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}
		
		/// <summary>
		/// Download the specified attachment in the claim.
		/// </summary>
		/// <param name="claimId"></param>
		/// <param name="claimVersion"></param>
		/// <param name="attachmentId"></param>
		/// <returns></returns>
		[HttpGet]
		[PreventSpam]
		[Route("Claim/Attachment/Download/{claimId}/{claimVersion}/{attachmentId}")]
		public ActionResult DownloadAttachment(int claimId, int claimVersion, int attachmentId)
		{
			if (claimId != 0 && claimVersion != 0 && attachmentId != 0)
			{
				var attachment = _claimService.GetAttachment(claimId, claimVersion, attachmentId);

				if (attachment != null)
				{
					return File(attachment.AttachmentData, System.Net.Mime.MediaTypeNames.Application.Octet,
						$"{attachment.FileName}{attachment.FileExtension}");
				}
			}

			this.SetAlert("This attachment has been removed or changed and cannot be accessed. Please return and select your claim again to view the current attachments.", AlertType.Warning, true);
			return Redirect(Request.UrlReferrer.ToString());
		}

		/// <summary>
		/// Update the claim with the list of eligible claim costs.
		/// </summary>
		/// <param name="eligibleCosts"></param>
		/// <param name="participantsPaidForExpenses"></param>
		/// <param name="participantsHaveBeenReimbursed"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Claim/Cost")]
		public JsonResult UpdateClaimEligibleCosts(List<ClaimEligibleCostModel> eligibleCosts, bool? participantsPaidForExpenses, bool? participantsHaveBeenReimbursed)
		{
			var model = new ClaimReportViewModel();
			try
			{
				var claimEligibleCost = _claimEligibleCostService.Get(eligibleCosts.FirstOrDefault().Id);

				_claimEligibleCostService.Update(eligibleCosts, participantsPaidForExpenses, participantsHaveBeenReimbursed);

				model = new ClaimReportViewModel(claimEligibleCost.Claim, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		/// <summary>
		/// Update the claim with the list of eligible claim costs.
		/// </summary>
		/// <param name="claimModel"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Claim/Attendance")]
		public JsonResult UpdateClaimAttendance(ClaimModel claimModel)
		{
			var model = new ClaimReportViewModel();
			try
			{
				var claimEligibleCost = _claimEligibleCostService.Get(claimModel.EligibleCosts.FirstOrDefault().Id);

				//save the participant attendance info, update the Attended property
				var participantsAttended = claimModel.Participants.ToDictionary(d => d.Id, d => d.Attended);

				_participantService.ReportAttendance(participantsAttended);

				//reset all claim amounts and costs
				_claimEligibleCostService.ResetClaimAmounts(claimEligibleCost.Claim);				

				model = new ClaimReportViewModel(claimEligibleCost.Claim, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}
		#endregion

		#region Claim Review View
		/// <summary>
		/// this is called when the "Review and Submit" button is pressed
		/// </summary>
		/// <param name="claimId"></param>
		/// <param name="claimVersion"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Claim/Reporting/Review/View/{claimId}/{claimVersion}")]
		public ActionResult ClaimReviewView(int claimId, int claimVersion)
		{
			var preventSubmit = false;

			if (_settingService.Get("EnableClaimsOn")?.GetValue<DateTime>() > AppDateTime.Now)
			{
				return RedirectToAction("Index", "Home");
			}

			var claim = _claimService.Get(claimId, claimVersion);
			if (!User.CanPerformAction(claim.GrantApplication, ApplicationWorkflowTrigger.ViewClaim))
			{
				this.SetAlert("The claim you requested to review does not exist or you are not allowed to view it.", AlertType.Warning, true);
				return RedirectToAction(nameof(ReportingController.GrantFileView), nameof(ReportingController).Replace("Controller", ""), new { grantApplicationId = claim.GrantApplicationId });
			}

			if (claim.HasNumberOfParticipantsChanged())
			{
				claim.ClaimState = ClaimState.Incomplete;
				_claimService.Update(claim);
				this.SetAlert("Claim values have been recalculated for the changed number of participants reported.", AlertType.Warning, true);
				return RedirectToAction(nameof(ReportingController.GrantFileView), nameof(ReportingController).Replace("Controller", ""), new { grantApplicationId = claim.GrantApplicationId });
			}

			if (!User.CanPerformAction(claim.GrantApplication, ApplicationWorkflowTrigger.SubmitClaim))
			{
				this.SetAlert("The claim you requested to review does not exist or you are not allowed to view it.", AlertType.Warning, true);
				return RedirectToAction(nameof(ReportingController.GrantFileView), nameof(ReportingController).Replace("Controller", ""), new { grantApplicationId = claim.GrantApplicationId });
			}

			claim.ClaimState = ClaimState.Unassessed;
			claim.DateSubmitted = AppDateTime.UtcNow;
			var results = _claimService.Validate(claim);
			if (results.Any())
			{
				preventSubmit = true;
				this.SetAlert(results);
			}

			ViewBag.ReviewAndSubmit = true;
			ViewBag.Reporting = false;
			ViewBag.GrantApplicationId = claim.GrantApplicationId;
			ViewBag.PreventSubmit = preventSubmit;
			ViewBag.ClaimId = claimId;
			ViewBag.ClaimVersion = claimVersion;
			ViewBag.ShowWDADescription = claim.GrantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.WDAService;
			return View();
		}

		/// <summary>
		/// Get the data for the claim review view.
		/// </summary>
		/// <param name="claimId"></param>
		/// <param name="claimVersion"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Claim/Reporting/Review/{claimId}/{claimVersion}")]
		public JsonResult GetClaimReview(int claimId, int claimVersion)
		{
			var viewModel = new ClaimReviewViewModel();
			try
			{
				var claim = _claimService.Get(claimId, claimVersion);
				if (!User.CanPerformAction(claim.GrantApplication, ApplicationWorkflowTrigger.SubmitClaim))
				{
					this.SetAlert("The claim you requested to review does not exist or you are not allowed to view it.", AlertType.Warning, true);
					viewModel.RedirectURL = "/Ext/Reporting/Grant/File/View/" + viewModel.Id;
				}
				viewModel = new ClaimReviewViewModel(claim);
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
		/// Submit the claim.
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Claim/Submit")]
		public JsonResult SubmitClaim(ClaimReviewViewModel viewModel)
		{
			try
			{
				var claim = _claimService.Get(viewModel.Claim.Id, viewModel.Claim.Version);
				claim.IsFinalClaim = viewModel.Claim.IsFinalClaim;
				_claimService.SubmitClaim(claim);

				this.SetAlert("Claim submitted successfully", AlertType.Success, true);
				viewModel.RedirectURL = "/Ext/Reporting/Grant/File/View/" + viewModel.Id;
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}
			var jsonResult = Json(viewModel, JsonRequestBehavior.AllowGet);
			jsonResult.MaxJsonLength = int.MaxValue;
			return jsonResult;
		}
		#endregion

		#region Claim Details View
		/// <summary>
		/// Return the submitted claim view.
		/// </summary>
		/// <param name="claimId"></param>
		/// <param name="claimVersion"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Claim/Details/View/{claimId}/{claimVersion}")]
		public ActionResult DetailsView(int claimId, int claimVersion)
		{
			if (_settingService.Get("EnableClaimsOn")?.GetValue<DateTime>() > AppDateTime.Now)
			{
				return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", ""));
			}

			var claim = _claimService.Get(claimId, claimVersion);

			// Must be in one of the accepted states to view.
			if (claim.GrantApplication.ApplicationStateExternal != ApplicationStateExternal.ClaimSubmitted)
			{
				this.SetAlert("The claim view page is not available when in the current state.", AlertType.Warning, true);
				return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", ""));
			}

			ViewBag.GrantApplicationId = claim.GrantApplicationId;
			ViewBag.ClaimId = claim.Id;
			ViewBag.ClaimVersion = claim.ClaimVersion;
			ViewBag.Reporting = false;
			ViewBag.ReviewAndSubmit = false;
			ViewBag.GrantProgramCode = claim.GrantApplication.GrantOpening.GrantStream.GrantProgram.ProgramCode;
			return View(SidebarViewModelFactory.Create(claim.GrantApplication, ControllerContext));
		}

		/// <summary>
		/// Get the data for the claim view.
		/// </summary>
		/// <param name="claimId"></param>
		/// <param name="claimVersion"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Claim/Details/{claimId}/{claimVersion}")]
		public JsonResult GetDetailsView(int claimId, int claimVersion)
		{
			var model = new ClaimReviewViewModel();
			try
			{
				var claim = _claimService.Get(claimId, claimVersion);
				model = new ClaimReviewViewModel(claim);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			var jsonResult = Json(model, JsonRequestBehavior.AllowGet);
			jsonResult.MaxJsonLength = int.MaxValue;
			return jsonResult;
		}

		/// <summary>
		/// Modal popup view to withdraw the claim.
		/// </summary>
		/// <param name="claimId"></param>
		/// <param name="claimVersion"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Claim/Withdraw/View/{claimId}/{claimVersion}")]
		public ActionResult WithdrawClaimView(int claimId, int claimVersion)
		{
			var claim = _claimService.Get(claimId, claimVersion);

			ViewBag.GrantApplicationId = claim.GrantApplicationId;
			ViewBag.ClaimId = claimId;
			ViewBag.ClaimVersion = claimVersion;
			ViewBag.RowVersion = Convert.ToBase64String(claim.RowVersion);
			return PartialView("_WithdrawClaimView");
		}

		/// <summary>
		/// Withdraw the claim.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Claim/Withdraw")]
		public JsonResult WithdrawClaim(WithdrawClaimViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var claim = _claimService.Get(model.ClaimId, model.ClaimVersion);
					claim.RowVersion = Convert.FromBase64String(model.RowVersion);

					_claimService.WithdrawClaim(claim, model.WithdrawReason);

					model.RedirectURL = $"/Ext/Reporting/Grant/File/View/{claim.GrantApplicationId}";
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model);
		}
		#endregion

		#region Claim Assessment View
		/// <summary>
		/// Return the claim assessment view.
		/// </summary>
		/// <param name="claimId"></param>
		/// <param name="claimVersion"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Claim/Assessment/View/{claimId}/{claimVersion}")]
		public ActionResult AssessmentView(int claimId, int claimVersion)
		{
			var claim = _claimService.Get(claimId, claimVersion);

			ViewBag.GrantApplicationId = claim.GrantApplicationId;
			ViewBag.ClaimId = claimId;
			ViewBag.ClaimVersion = claimVersion;
			ViewBag.ShowWDADescription = claim.GrantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.WDAService;
			return View();
		}

		/// <summary>
		/// Get the claim assessment view data.
		/// </summary>
		/// <param name="claimId"></param>
		/// <param name="claimVersion"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Claim/Assessment/{claimId}/{claimVersion}")]
		public JsonResult GetAssessment(int claimId, int claimVersion)
		{
			var model = new ClaimAssessedViewModel();
			try
			{
				var claim = _claimService.Get(claimId, claimVersion);
				model = new ClaimAssessedViewModel(claim, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			var jsonResult = Json(model, JsonRequestBehavior.AllowGet);
			jsonResult.MaxJsonLength = int.MaxValue;
			return jsonResult;
		}
		#endregion
		#endregion
	}

    public class ParticipantWarningsModel : BaseViewModel
    {
	    public List<ParticipantWarningModel> ParticipantWarnings { get; set; }
    }
}
