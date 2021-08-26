using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Int.Models;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using System;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Int.Controllers
{
	[RouteArea("Int")]
	public class WorkflowController : BaseController
	{
		#region Variables
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IClaimService _claimService;
		#endregion

		#region Constructors
		public WorkflowController(IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			IClaimService claimService
		   ) : base(controllerService.Logger)
		{
			_grantApplicationService = grantApplicationService;
			_claimService = claimService;
		}
		#endregion

		#region Endpoints
		#region Application Workflow
		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut, Route("Workflow/Select/For/Assessment", Name = "SelectForAssessment")]
		public JsonResult SelectForAssessment(WorkflowViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.ApplicationWorkflowViewModel.Id);
				grantApplication.RowVersion = Convert.FromBase64String(model.ApplicationWorkflowViewModel.RowVersion);
				_grantApplicationService.SelectForAssessment(grantApplication);
				model = new WorkflowViewModel(grantApplication, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut, Route("Workflow/Remove/From/Assessment", Name = "RemoveFromAssessment")]
		public JsonResult RemoveFromAssessment(WorkflowViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.ApplicationWorkflowViewModel.Id);
				grantApplication.RowVersion = Convert.FromBase64String(model.ApplicationWorkflowViewModel.RowVersion);
				_grantApplicationService.RemoveFromAssessment(grantApplication);
				model = new WorkflowViewModel(grantApplication, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut, Route("Workflow/Begin/Assessment", Name = "BeginAssessment")]
		public JsonResult BeginAssessment(WorkflowViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.ApplicationWorkflowViewModel.Id);
				grantApplication.RowVersion = Convert.FromBase64String(model.ApplicationWorkflowViewModel.RowVersion);
				if (!model.ApplicationWorkflowViewModel.AssessorId.HasValue) throw new InvalidOperationException("An assessor must be assigned before begining assessment.");
				_grantApplicationService.BeginAssessment(grantApplication, model.ApplicationWorkflowViewModel.AssessorId.Value);
				model = new WorkflowViewModel(grantApplication, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut, Route("Workflow/Return/To/Assessment", Name = "ReturnToAssessment")]
		public JsonResult ReturnToAssessment(WorkflowViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.ApplicationWorkflowViewModel.Id);
				grantApplication.RowVersion = Convert.FromBase64String(model.ApplicationWorkflowViewModel.RowVersion);
				_grantApplicationService.ReturnToAssessment(grantApplication, model.ApplicationWorkflowViewModel.ReasonToReassess);
				model = new WorkflowViewModel(grantApplication, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut, Route("Workflow/Recommend/For/Denial", Name = "RecommendForDenial")]
		public JsonResult RecommendForDenial(WorkflowViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.ApplicationWorkflowViewModel.Id);
				grantApplication.RowVersion = Convert.FromBase64String(model.ApplicationWorkflowViewModel.RowVersion);
				_grantApplicationService.RecommendForDenial(grantApplication, model.ApplicationWorkflowViewModel.ReasonToDeny);
				model = new WorkflowViewModel(grantApplication, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut, Route("Workflow/Deny/Application", Name = "DenyApplication")]
		public JsonResult DenyApplication(WorkflowViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.ApplicationWorkflowViewModel.Id);
				grantApplication.RowVersion = Convert.FromBase64String(model.ApplicationWorkflowViewModel.RowVersion);
				_grantApplicationService.DenyApplication(grantApplication, grantApplication.GetDeniedReason());
				model = new WorkflowViewModel(grantApplication, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut, Route("Workflow/Recommend/For/Approval", Name = "RecommendForApproval")]
		public JsonResult RecommendForApproval(WorkflowViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.ApplicationWorkflowViewModel.Id);
				grantApplication.RowVersion = Convert.FromBase64String(model.ApplicationWorkflowViewModel.RowVersion);
				_grantApplicationService.RecommendForApproval(grantApplication);
				model = new WorkflowViewModel(grantApplication, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut, Route("Workflow/Issue/Offer", Name = "IssueOffer")]
		public JsonResult IssueOffer(WorkflowViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.ApplicationWorkflowViewModel.Id);
				grantApplication.RowVersion = Convert.FromBase64String(model.ApplicationWorkflowViewModel.RowVersion);
				_grantApplicationService.IssueOffer(grantApplication);
				model = new WorkflowViewModel(grantApplication, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut, Route("Workflow/Withdraw/Offer", Name = "WithdrawOffer")]
		public JsonResult WithdrawOffer(WorkflowViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.ApplicationWorkflowViewModel.Id);
				grantApplication.RowVersion = Convert.FromBase64String(model.ApplicationWorkflowViewModel.RowVersion);
				_grantApplicationService.WithdrawOffer(grantApplication, model.ApplicationWorkflowViewModel.ReasonToWithdraw);
				model = new WorkflowViewModel(grantApplication, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut, Route("Workflow/Recommend/Change/For/Denial", Name = "RecommendChangeForDenial")]
		public JsonResult RecommendChangeForDenial(WorkflowViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.ApplicationWorkflowViewModel.Id);
				grantApplication.RowVersion = Convert.FromBase64String(model.ApplicationWorkflowViewModel.RowVersion);
				_grantApplicationService.RecommendChangeForDenial(grantApplication, model.ApplicationWorkflowViewModel.ReasonToDenyChangeRequest);
				model = new WorkflowViewModel(grantApplication, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut, Route("Workflow/Recommend/Change/For/Approval", Name = "RecommendChangeForApproval")]
		public JsonResult RecommendChangeForApproval(WorkflowViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.ApplicationWorkflowViewModel.Id);
				grantApplication.RowVersion = Convert.FromBase64String(model.ApplicationWorkflowViewModel.RowVersion);
				_grantApplicationService.RecommendChangeForApproval(grantApplication);
				model = new WorkflowViewModel(grantApplication, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut, Route("Workflow/Return/Change/To/Assessment", Name = "ReturnChangeToAssessment")]
		public JsonResult ReturnChangeToAssessment(WorkflowViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.ApplicationWorkflowViewModel.Id);
				grantApplication.RowVersion = Convert.FromBase64String(model.ApplicationWorkflowViewModel.RowVersion);
				_grantApplicationService.ReturnChangeToAssessment(grantApplication);
				model = new WorkflowViewModel(grantApplication, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut, Route("Workflow/Deny/Change/Request", Name = "DenyChangeRequest")]
		public JsonResult DenyChangeRequest(WorkflowViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.ApplicationWorkflowViewModel.Id);
				grantApplication.RowVersion = Convert.FromBase64String(model.ApplicationWorkflowViewModel.RowVersion);
				_grantApplicationService.DenyChangeRequest(grantApplication, grantApplication.GetDeniedReason());
				model = new WorkflowViewModel(grantApplication, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut, Route("Workflow/Approve/Change/Request", Name = "ApproveChangeRequest")]
		public JsonResult ApproveChangeRequest(WorkflowViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.ApplicationWorkflowViewModel.Id);
				grantApplication.RowVersion = Convert.FromBase64String(model.ApplicationWorkflowViewModel.RowVersion);
				_grantApplicationService.ApproveChangeRequest(grantApplication);
				model = new WorkflowViewModel(grantApplication, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut, Route("Workflow/Cancel/Agreement", Name = "CancelAgreementMinistry")]
		public JsonResult CancelAgreement(WorkflowViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.ApplicationWorkflowViewModel.Id);
				grantApplication.RowVersion = Convert.FromBase64String(model.ApplicationWorkflowViewModel.RowVersion);
				_grantApplicationService.CancelApplicationAgreement(grantApplication, model.ApplicationWorkflowViewModel.ReasonToCancel);
				model = new WorkflowViewModel(grantApplication, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut, Route("Workflow/Initiate/Amendment", Name = "AmendClaim")]
		public JsonResult InitiateAmendment(WorkflowViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.ApplicationWorkflowViewModel.Id);
				grantApplication.RowVersion = Convert.FromBase64String(model.ApplicationWorkflowViewModel.RowVersion);
				var claim = grantApplication.GetCurrentClaim();
				_claimService.InitiateClaimAmendment(claim);
				model = new WorkflowViewModel(grantApplication, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut, Route("Workflow/Close/Claim/Reporting", Name = "CloseClaimReporting")]
		public JsonResult CloseClaimReporting(WorkflowViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.ApplicationWorkflowViewModel.Id);
				grantApplication.RowVersion = Convert.FromBase64String(model.ApplicationWorkflowViewModel.RowVersion);
				_claimService.CloseClaimReporting(grantApplication);
				model = new WorkflowViewModel(grantApplication, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut, Route("Workflow/Enable/Claim/Reporting", Name = "EnableClaimReporting")]
		public JsonResult EnableClaimReporting(WorkflowViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.ApplicationWorkflowViewModel.Id);
				grantApplication.RowVersion = Convert.FromBase64String(model.ApplicationWorkflowViewModel.RowVersion);
				_claimService.EnableClaimReporting(grantApplication);
				model = new WorkflowViewModel(grantApplication, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut, Route("Workflow/Enable/Completion/Reporting", Name = "EnableCompletionReporting")]
		public JsonResult EnableCompletionReporting(WorkflowViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.ApplicationWorkflowViewModel.Id);
				grantApplication.RowVersion = Convert.FromBase64String(model.ApplicationWorkflowViewModel.RowVersion);
				_grantApplicationService.EnableCompletionReporting(grantApplication);
				model = new WorkflowViewModel(grantApplication, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut, Route("Workflow/Close/Grant/File", Name = "Close")]
		public JsonResult CloseGrantFile(WorkflowViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.ApplicationWorkflowViewModel.Id);
				grantApplication.RowVersion = Convert.FromBase64String(model.ApplicationWorkflowViewModel.RowVersion);
				_grantApplicationService.CloseGrantFile(grantApplication);
				model = new WorkflowViewModel(grantApplication, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}
		#endregion

		#region Claim Workflow
		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut, Route("Workflow/Select/Claim/For/Assessment", Name = "SelectClaimForAssessment")]
		public JsonResult SelectClaimForAssessment(WorkflowViewModel model)
		{
			try
			{
				var claim = _claimService.Get(model.ClaimWorkflowViewModel.Id, model.ClaimWorkflowViewModel.ClaimVersion);
				claim.RowVersion = Convert.FromBase64String(model.ClaimWorkflowViewModel.RowVersion);
				_claimService.SelectClaimForAssessment(claim);
				model = new WorkflowViewModel(claim, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut, Route("Workflow/Remove/Claim/From/Assessment", Name = "RemoveClaimFromAssessment")]
		public JsonResult RemoveClaimFromAssessment(WorkflowViewModel model)
		{
			try
			{
				var claim = _claimService.Get(model.ClaimWorkflowViewModel.Id, model.ClaimWorkflowViewModel.ClaimVersion);
				claim.RowVersion = Convert.FromBase64String(model.ClaimWorkflowViewModel.RowVersion);
				_claimService.RemoveClaimFromAssessment(claim);
				model = new WorkflowViewModel(claim, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut, Route("Workflow/Return/Claim/To/Applicant", Name = "ReturnClaimToApplicant")]
		public JsonResult ReturnClaimToApplicant(WorkflowViewModel model)
		{
			try
			{
				var claim = _claimService.Get(model.ClaimWorkflowViewModel.Id, model.ClaimWorkflowViewModel.ClaimVersion);
				claim.RowVersion = Convert.FromBase64String(model.ClaimWorkflowViewModel.RowVersion);
				_claimService.ReturnClaimToApplicant(claim);
				model = new WorkflowViewModel(claim, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut, Route("Workflow/Assess/Reimbursement", Name = "AssessReimbursement")]
		public JsonResult AssessReimbursement(WorkflowViewModel model)
		{
			try
			{
				var claim = _claimService.Get(model.ClaimWorkflowViewModel.Id, model.ClaimWorkflowViewModel.ClaimVersion);
				claim.RowVersion = Convert.FromBase64String(model.ClaimWorkflowViewModel.RowVersion);
				_claimService.AssessClaimReimbursement(claim);
				model = new WorkflowViewModel(claim, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut, Route("Workflow/Deny/Claim", Name = "DenyClaim")]
		public JsonResult DenyClaim(WorkflowViewModel model)
		{
			try
			{
				var claim = _claimService.Get(model.ClaimWorkflowViewModel.Id, model.ClaimWorkflowViewModel.ClaimVersion);
				claim.RowVersion = Convert.FromBase64String(model.ClaimWorkflowViewModel.RowVersion);
				_claimService.DenyClaim(claim);
				model = new WorkflowViewModel(claim, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut, Route("Workflow/Approve/Claim", Name = "ApproveClaim")]
		public JsonResult ApproveClaim(WorkflowViewModel model)
		{
			try
			{
				var claim = _claimService.Get(model.ClaimWorkflowViewModel.Id, model.ClaimWorkflowViewModel.ClaimVersion);
				claim.RowVersion = Convert.FromBase64String(model.ClaimWorkflowViewModel.RowVersion);
				_claimService.ApproveClaim(claim);
				model = new WorkflowViewModel(claim, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut, Route("Workflow/Assess/Eligibility", Name = "AssessEligibility")]
		public JsonResult AssessEligibility(WorkflowViewModel model)
		{
			try
			{
				var claim = _claimService.Get(model.ClaimWorkflowViewModel.Id, model.ClaimWorkflowViewModel.ClaimVersion);
				claim.RowVersion = Convert.FromBase64String(model.ClaimWorkflowViewModel.RowVersion);
				_claimService.AssessClaimEligibility(claim);
				model = new WorkflowViewModel(claim, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}
		#endregion
		#endregion
	}
}
