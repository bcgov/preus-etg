using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces;
using CJG.Core.Interfaces.Service;
using CJG.Core.Interfaces.Service.Settings;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Areas.Int.Models;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;
using Microsoft.AspNet.Identity;

namespace CJG.Web.External.Areas.Int.Controllers
{
    /// <summary>
    /// <paramtyperef name="ClaimController"/> class, provides endpoints to manage the assessment of claims.
    /// </summary>
    [Authorize(Roles = "Assessor, Director, Financial Clerk, System Administrator")]
	[RouteArea("Int")]
	public class ClaimAssessmentController : BaseController
	{
		private const int MaxLengthOfErrorMessage = 64;
		private readonly IGrantStreamService _grantStreamService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IAttachmentService _attachmentService;
		private readonly IAuthorizationService _authorizationService;
		private readonly IUserManagerAdapter _userManager;
		private readonly ITrainingProviderSettings _trainingProviderSettings;
		private readonly IClaimService _claimService;
		private readonly IClaimEligibleCostService _claimEligibleCostService;
		private readonly IEligibleExpenseTypeService _eligibleExpenseTypeService;

		/// <summary>
		/// Creates a new instance of a ClaimAssessmentController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantStreamService"></param>
		/// <param name="grantApplicationService"></param>
		/// <param name="attachmentService"></param>
		/// <param name="authorizationService"></param>
		/// <param name="userManager"></param>
		/// <param name="trainingProviderSettings"></param>
		/// <param name="claimService"></param>
		/// <param name="claimEligibleCostService"></param>
		/// <param name="eligibleExpenseTypeService"></param>
		public ClaimAssessmentController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			IAttachmentService attachmentService,
			IAuthorizationService authorizationService,
			IUserManagerAdapter userManager,
			ITrainingProviderSettings trainingProviderSettings,
			IClaimService claimService,
			IClaimEligibleCostService claimEligibleCostService,
			IEligibleExpenseTypeService eligibleExpenseTypeService,
			IGrantStreamService grantStreamService
		   ) : base(controllerService.Logger)
		{
			_grantApplicationService = grantApplicationService;
			_attachmentService = attachmentService;
			_userManager = userManager;
			_authorizationService = authorizationService;
			_trainingProviderSettings = trainingProviderSettings;
			_claimService = claimService;
			_claimEligibleCostService = claimEligibleCostService;
			_eligibleExpenseTypeService = eligibleExpenseTypeService;
			_grantStreamService = grantStreamService;
		}

		/// <summary>
		/// Return a view for claim assessment.
		/// </summary>
		/// <param name="claimId"></param>
		/// <param name="claimVersion"></param>
		/// <returns></returns>
		[HttpGet]
		[AuthorizeAction(Privilege.IA1)]
		[Route("Claim/Assessment/View/{claimId}/{claimVersion}")]
		public ActionResult ClaimAssessmentView(int claimId, int claimVersion)
		{
			try
			{
				var claim = _claimService.Get(claimId, claimVersion);
				var grantApplication = claim.GrantApplication;
				ViewBag.ClaimId = claimId;
				ViewBag.ClaimVersion = claimVersion;
				ViewBag.GrantApplicationId = grantApplication.Id;
			}
			catch (NotAuthorizedException e)
			{
				HandleAngularException(e);
				this.SetAlert(e, true);
				return RedirectToAction("Index", "Home");
			}

			return View();
		}

		/// <summary>
		/// Get the claim data for the specified id and version for the claim assessment view.
		/// </summary>
		/// <param name="claimId"></param>
		/// <param name="claimVersion"></param>
		/// <returns></returns>
		[HttpGet]
		[AuthorizeAction(Privilege.IA1)]
		[Route("Claim/Assessment/{claimId}/{claimVersion}")]
		public ActionResult GetClaim(int claimId, int claimVersion)
		{
			var model = new Models.Claims.ClaimAssessmentViewModel();
			try
			{
				var claim = _claimService.Get(claimId, claimVersion);
				model = new Models.Claims.ClaimAssessmentViewModel(claim, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get an array of assessors.
		/// </summary>
		/// <param name="currentAssessorId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Claim/Assessors")]
		public ActionResult GetAssessors()
		{
			var viewModel = new BaseViewModel();
			try
			{
				var assessors = _authorizationService.GetAssessors().Select(u => new Models.Claims.InternalUserViewModel(u)).ToArray();
				return Json(assessors, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}
			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get an array of expense types.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Claim/Expense/Types/{grantApplicationId}")]
		public ActionResult GetExpenseTypes(int grantApplicationId)
		{
			var viewModel = new BaseViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				var expenseTypes = _grantStreamService.GetAllActiveEligibleExpenseTypes(grantApplication.GrantOpening.GrantStreamId)
					.Select(t => new
					{
						t.Id,
						t.Caption,
						t.Description,
						ExpenseType = t.ExpenseTypeId,
						ReimbursementRate = t.Rate,
						t.AllowMultiple,
						ServiceType = t.ServiceCategory?.ServiceTypeId
					});
				return Json(expenseTypes, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}
			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Reassign the assessor on the grant application.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="newAssessorId"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Claim/Reassign")]
		public ActionResult ReassignAssessor(Models.Claims.ClaimAssessmentViewModel model)
		{
			try
			{
				var claim = _claimService.Get(model.Id, model.Version);
				claim.GrantApplication.RowVersion = Convert.FromBase64String(model.GrantApplicationRowVersion);
				_grantApplicationService.AssignAssessor(claim.GrantApplication, model.AssessorId);
				model = new Models.Claims.ClaimAssessmentViewModel(claim, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model);
		}


		/// <summary>
		/// Get the claim detail data for the specified id and version for the claim assessment view.
		/// </summary>
		/// <param name="claimId"></param>
		/// <param name="claimVersion"></param>
		/// <returns></returns>
		[HttpGet]
		[AuthorizeAction(Privilege.IA1)]
		[Route("Claim/Assessment/Details/{claimId}/{claimVersion}")]
		public ActionResult GetClaimDetails(int claimId, int claimVersion)
		{
			var model = new Models.Claims.ClaimAssessmentDetailsViewModel();
			try
			{
				var claim = _claimService.Get(claimId, claimVersion);
				model = new Models.Claims.ClaimAssessmentDetailsViewModel(claim);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Update the claim information in the datasource.
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		[HttpPut]
		[ValidateRequestHeader]
		[AuthorizeAction(Privilege.AM2, Privilege.AM4, Privilege.AM5)]
		[Route("Claim")]
		public ActionResult UpdateClaim(Models.Claims.ClaimAssessmentDetailsViewModel model)
		{
			try
			{
				var claim = _claimService.Get(model.Id, model.Version);
				claim.RowVersion = Convert.FromBase64String(model.RowVersion);

				// Add or update eligible costs.
				model.EligibleCosts.ForEach(ec =>
				{
					if (ec.Id == 0)
					{
						var claimEligibleCost = new ClaimEligibleCost(claim);
						claimEligibleCost.AddedByAssessor = true;

						var eligibleExpenseType = _eligibleExpenseTypeService.Get(ec.EligibleExpenseTypeId);
						claimEligibleCost.EligibleExpenseType = eligibleExpenseType;

						claimEligibleCost.AssessedParticipants = ec.AssessedParticipants;
						claimEligibleCost.AssessedCost = ec.AssessedCost;
						claimEligibleCost.AssessedMaxParticipantCost = ec.AssessedMaxParticipantCost;
						claimEligibleCost.AssessedReimbursementCost = ec.AssessedReimbursementCost;
						claimEligibleCost.AssessedMaxParticipantReimbursementCost = ec.AssessedMaxParticipantReimbursementCost;
						claimEligibleCost.AssessedParticipantEmployerContribution = ec.AssessedParticipantEmployerContribution;

						if (ec.Breakdowns != null)
						{
							ec.Breakdowns.ForEach(b => {
								var expenseBreakdown = new EligibleExpenseBreakdown(); // TODO: Get the entity
								var breakdown = new ClaimBreakdownCost(expenseBreakdown, claimEligibleCost)
								{
									AssessedCost = b.AssessedCost
								};
								claimEligibleCost.Breakdowns.Add(breakdown);
								// TODO: Might need to add to datasource.
							});
						}
						if (ec.ParticipantCosts != null)
						{
							ec.ParticipantCosts.ForEach(pc =>
							{
								var participantCost = new ParticipantCost(claimEligibleCost, pc.Participant.ParticipantFormId)
								{
									AssessedParticipantCost = pc.AssessedParticipantCost,
									AssessedReimbursement = pc.AssessedReimbursement,
									AssessedEmployerContribution = pc.AssessedEmployerContribution,
								};
								claimEligibleCost.ParticipantCosts.Add(participantCost);
								// TODO: Might need to add to datasource.
							});
						}
						claim.EligibleCosts.Add(claimEligibleCost);
						// TODO: Might need to add to datasource.
					}
					else
					{
						var claimEligibleCost = _claimEligibleCostService.Get(ec.Id);

						claimEligibleCost.AssessedParticipants = ec.AssessedParticipants;
						claimEligibleCost.AssessedCost = ec.AssessedCost;
						claimEligibleCost.AssessedMaxParticipantCost = ec.AssessedMaxParticipantCost;
						claimEligibleCost.AssessedReimbursementCost = ec.AssessedReimbursementCost;
						claimEligibleCost.AssessedMaxParticipantReimbursementCost = ec.AssessedMaxParticipantReimbursementCost;
						claimEligibleCost.AssessedParticipantEmployerContribution = ec.AssessedParticipantEmployerContribution;

						// Breakdowns
						if (ec.Breakdowns != null)
						{
							ec.Breakdowns.ForEach(b =>
							{
								if (b.Id == 0)
								{
									var expenseBreakdown = new EligibleExpenseBreakdown(); // TODO: Get the entity
									var breakdown = new ClaimBreakdownCost(expenseBreakdown, claimEligibleCost)
									{
										AssessedCost = b.AssessedCost
									};
									claimEligibleCost.Breakdowns.Add(breakdown);
									// TODO: Might need to add to datasource.
								}
								else
								{
									var breakdown = claimEligibleCost.Breakdowns.FirstOrDefault(bb => bb.Id == b.Id) ?? throw new NoContentException("Breakdown cost does not exist.");
									breakdown.AssessedCost = b.AssessedCost;
								}
							});
						}

						// Delete the breakdown costs that were removed.
						var deleteBreakdowns = claimEligibleCost.Breakdowns.Where(b => !ec.Breakdowns.Any(bb => bb.Id == b.Id)).ToArray();
						deleteBreakdowns.ForEach(b => {
							claimEligibleCost.Breakdowns.Remove(b);
							_claimEligibleCostService.Remove(b);
						});

						// Participant Costs
						if (ec.ParticipantCosts != null)
						{
							ec.ParticipantCosts.ForEach(pc =>
							{
								var participantCost = claimEligibleCost.ParticipantCosts.FirstOrDefault(c => c.Id == pc.Id) ?? throw new NoContentException("Participant cost does not exist.");
								participantCost.AssessedParticipantCost = pc.AssessedParticipantCost;
								participantCost.AssessedReimbursement = pc.AssessedReimbursement;
								participantCost.AssessedEmployerContribution = pc.AssessedEmployerContribution;
							});
						}
					}
				});

				// Delete the eligible costs that were removed.
				var deleteCosts = claim.EligibleCosts.Where(ec => !model.EligibleCosts.Any(c => c.Id == ec.Id)).ToArray();
				deleteCosts.ForEach(ec => {
					claim.EligibleCosts.Remove(ec);
					_claimEligibleCostService.Remove(ec);
				});

				_claimService.Update(claim, User.HasPrivilege(Privilege.AM4));

				model = new Models.Claims.ClaimAssessmentDetailsViewModel(claim);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		/// <summary>
		/// Update the claim notes.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[ValidateRequestHeader]
		[AuthorizeAction(Privilege.AM2, Privilege.AM4, Privilege.AM5)]
		[Route("Claim/Notes")]
		public ActionResult SaveClaimNotes(Models.Claims.ClaimAssessmentViewModel model)
		{
			try
			{
				var claim = _claimService.Get(model.Id, model.Version);
				claim.RowVersion = Convert.FromBase64String(model.RowVersion);

				claim.EligibilityAssessmentNotes = model.EligibilityAssessmentNotes;
				claim.ReimbursementAssessmentNotes = model.ReimbursementAssessmentNotes;
				claim.ClaimAssessmentNotes = model.ClaimAssessmentNotes;

				_claimService.Update(claim);
				model = new Models.Claims.ClaimAssessmentViewModel(claim, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model);
		}

		/// <summary>
		/// TODO: Not sure if this is used anymore.
		/// </summary>
		/// <param name="claimId"></param>
		/// <param name="claimVersion"></param>
		/// <returns></returns>
		[HttpGet]
		[AuthorizeAction(Privilege.IA1)]
		[Route("Application/Claim/Assessment/Details/{claimId}/{claimVersion}")]
		public ActionResult GetClaimAssessmentDetail(int claimId, int claimVersion)
		{
			ViewDataDictionary viewData;
			try
			{
				var claim = _claimService.Get(claimId, claimVersion);
				var grantApplication = claim.GrantApplication;
				var model = new ClaimOldViewModel(claim);

				viewData = new ViewDataDictionary()
				{
					Model = new ClaimAssessmentDetailViewModel(grantApplication, model, _trainingProviderSettings.AllowFileAttachmentExtensions)
					{
						ParticipantForms = grantApplication.ParticipantForms,
						ReadOnly = false,
						HasPriorApprovedClaim = claim.HasPriorApprovedClaim()
					}
				};

				viewData.Add("claimId", claimId);
				viewData.Add("claimVersion", claimVersion);
				viewData.Add("RowVersion", Convert.ToBase64String(claim.RowVersion));
			}
			catch (NotAuthorizedException)
			{
				var currentUser = _userManager.FindById(User.Identity.GetUserId());
				_logger.Info($"GetClaimDetails({claimId}, {claimVersion}), {currentUser?.GetInternalUserFullName()}");
				return new HttpUnauthorizedResult("You are not authorized to perform this action.").HttpStatusCodeResultWithAlert(Response, AlertType.Warning);
			}
			catch (DbEntityValidationException e)
			{
				_logger.Debug(e.GetValidationMessages());
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest, e.GetValidationMessages()).HttpStatusCodeResultWithAlert(Response, AlertType.Warning);
			}
			catch (DbUpdateConcurrencyException e)
			{
				_logger.Debug(e.GetAllMessages());
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest, e.GetAllMessages()).HttpStatusCodeResultWithAlert(Response, AlertType.Warning);
			}
			catch (Exception e)
			{
				_logger.Error(e.GetAllMessages());
				return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, e.GetAllMessages()).HttpStatusCodeResultWithAlert(Response, AlertType.Error);
			}

			return new PartialViewResult() { ViewData = viewData, ViewName = "_ClaimAssessmentDetail" };
		}

		/// <summary>
		/// Get the claim attachments for the specified claim.
		/// </summary>
		/// <param name="claimId"></param>
		/// <param name="claimVersion"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Claim/Attachments/{claimId}/{claimVersion}")]
		public JsonResult GetClaimAttachments(int claimId, int claimVersion)
		{
			var viewModel = new ClaimAttachmentDetailViewModel();

			try
			{
				var claim = _claimService.Get(claimId, claimVersion);
				viewModel = new ClaimAttachmentDetailViewModel(claim);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}
			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Adds specified attachment
		/// </summary>
		/// <param name="claimId"></param>
		/// <param name="claimVersion"></param>
		/// <param name="file"></param>
		/// <param name="attachments"></param>
		/// <returns></returns>
		[HttpPost]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Claim/Attachment")]
		public ActionResult AddAttachment(int claimId, int claimVersion, HttpPostedFileBase file, string attachments)
		{
			var model = new BaseViewModel();
			try
			{
				var attachmentViewModel = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Attachments.UpdateAttachmentViewModel>(attachments);
				var attachment = file.UploadFile(attachmentViewModel.Description, attachmentViewModel.FileName);
				_claimService.AddReceipt(claimId, claimVersion, attachment);
				var claim = _claimService.Get(claimId, claimVersion);
				model = new ClaimAttachmentDetailViewModel(claim);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Updates specified attachment
		/// </summary>
		/// <param name="claimId"></param>
		/// <param name="claimVersion"></param>
		/// <param name="file"></param>
		/// <param name="attachments"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Claim/Attachment")]
		public ActionResult UpdateAttachment(int claimId, int claimVersion, HttpPostedFileBase file, string attachments)
		{
			var model = new BaseViewModel();
			try
			{
				var attachmentViewModel = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Attachments.UpdateAttachmentViewModel>(attachments);
				var attachment = file.UploadFile(attachmentViewModel.Description, attachmentViewModel.FileName);
				var existing = _attachmentService.Get(attachmentViewModel.Id);
				existing.RowVersion = Convert.FromBase64String(attachmentViewModel.RowVersion);
				attachmentViewModel.MapToEntity(existing);
				existing.AttachmentData = attachment.AttachmentData;
				_claimService.AddReceipt(claimId, claimVersion, existing);

				var claim = _claimService.Get(claimId, claimVersion);
				model = new ClaimAttachmentDetailViewModel(claim);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Downloads specified attachment
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="attachmentId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Claim/Attachment/Download/{claimId}/{claimVersion}/{attachmentId}")]
		public ActionResult DownloadAttachment(int claimId, int claimVersion, int attachmentId)
		{
			var model = new BaseViewModel();
			try
			{
				var attachment = _claimService.GetAttachment(claimId, claimVersion, attachmentId);
				return File(attachment.AttachmentData, System.Net.Mime.MediaTypeNames.Application.Octet, $"{attachment.FileName}{attachment.FileExtension}");
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Deletes specified attachment
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="attachmentId"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Claim/Attachment/Delete/{claimId}/{claimVersion}/{attachmentId}")]
		public ActionResult DeleteAttachment(int claimId, int claimVersion, int attachmentId)
		{
			var model = new BaseViewModel();
			try
			{
				var attachment = _claimService.GetAttachment(claimId, claimVersion, attachmentId);
				_claimService.RemoveReceipt(claimId, claimVersion, attachment);
				var claim = _claimService.Get(claimId, claimVersion);
				model = new ClaimAttachmentDetailViewModel(claim);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}
	}
}
