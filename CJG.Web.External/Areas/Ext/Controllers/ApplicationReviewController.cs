using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Web.Mvc;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Ext.Models.Applications;
using CJG.Web.External.Areas.Int.Models.GrantStreams;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;

namespace CJG.Web.External.Areas.Ext.Controllers
{
    /// <summary>
    /// ApplicationReviewController class, provides a controller endpoints for managing external user grant applications.
    /// </summary>
    [RouteArea("Ext")]
	[ExternalFilter]
	public class ApplicationReviewController : BaseController
	{
		private readonly ISiteMinderService _siteMinderService;
		private readonly IUserService _userService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IGrantStreamService _grantStreamService;
		private readonly IOrganizationService _organizationService;
		private readonly INaIndustryClassificationSystemService _naIndustryClassificationSystemService;
		private readonly IGrantProgramService _grantProgramService;
		private readonly IEligibleExpenseTypeService _eligibleExpenseTypeService;

		/// <summary>
		/// Creates a new instance of a ApplicationReviewController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantApplicationService"></param>
		/// <param name="grantStreamService"></param>
		/// <param name="organizationService"></param>
		/// <param name="naIndustryClassificationSystemService"></param>
		/// <param name="grantProgramService"></param>
		/// <param name="eligibleExpenseTypeService"></param>
		public ApplicationReviewController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			IGrantStreamService grantStreamService,
			IOrganizationService organizationService,
			INaIndustryClassificationSystemService naIndustryClassificationSystemService,
			IGrantProgramService grantProgramService,
			IEligibleExpenseTypeService eligibleExpenseTypeService) : base(controllerService.Logger)
		{
			_userService = controllerService.UserService;
			_siteMinderService = controllerService.SiteMinderService;
			_grantApplicationService = grantApplicationService;
			_grantStreamService = grantStreamService;
			_organizationService = organizationService;
			_naIndustryClassificationSystemService = naIndustryClassificationSystemService;
			_grantProgramService = grantProgramService;
			_eligibleExpenseTypeService = eligibleExpenseTypeService;
		}

		/// <summary>
		/// Display the Grant Application Review View.
		/// This View is used when Reviewing an application before submitting.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Review/View/{grantApplicationId:int}")]
		public ActionResult ApplicationReviewView(int grantApplicationId)
		{
			var grantApplication = _grantApplicationService.Get(grantApplicationId);
			if (grantApplication.OrganizationAddress == null)
			{
				_userService.UpdateUserFromBCeIDAccount();
				var currentUser = _userService.GetUser(_siteMinderService.CurrentUserGuid);
				grantApplication.CopyOrganization(currentUser.Organization, _naIndustryClassificationSystemService.GetNaIndustryClassificationSystemParentByLevel);

				// If it's still null, then redirect.
				if (grantApplication.OrganizationAddress == null)
					return RedirectToAction(nameof(OrganizationProfileController.OrganizationProfileView), nameof(OrganizationProfileController).Replace("Controller", ""));
			}

			ViewBag.GrantApplicationId = grantApplicationId;
			return ReviewValidation(grantApplicationId);
		}

		/// <summary>
		/// Get the data for the application Review.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Review/{grantApplicationId:int}")]
		public JsonResult GetApplicationReview(int grantApplicationId)
		{
			var model = new ApplicationReviewViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				int? userId = grantApplication.BusinessContactRoles.FirstOrDefault()?.UserId;
				model = new ApplicationReviewViewModel(grantApplication, userId == null ? null : _userService.GetUser((int)userId));

				PopulateStreamQuestionsAndAnswers(grantApplication, model);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		private void PopulateStreamQuestionsAndAnswers(GrantApplication grantApplication, ApplicationReviewViewModel model)
		{
			var streamEligibilityQuestions = _grantStreamService.GetGrantStreamQuestions(grantApplication.GrantOpening.GrantStreamId)
				.Where(l => l.IsActive)
				.Select(n => new GrantStreamQuestionViewModel(n)).ToList();

			var answers = _grantStreamService.GetGrantStreamAnswers(grantApplication.Id).ToList();

			foreach (var questionModel in streamEligibilityQuestions)
			{
				var answer = answers.FirstOrDefault(a => a.GrantStreamEligibilityQuestionId == questionModel.Id);

				if (answer == null)
					continue;

				questionModel.EligibilityAnswer = answer.EligibilityAnswer;
				questionModel.RationaleAnswer = answer.RationaleAnswer;
			}

			model.StreamEligibilityQuestions = streamEligibilityQuestions;
		}

		/// <summary>
		/// Return a View to Review the application grant program information.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Review/Program/View/{grantApplicationId:int}")]
		public ActionResult ApplicationReviewGrantProgramView(int grantApplicationId)
		{
			ViewBag.GrantApplicationId = grantApplicationId;
			return ReviewValidation(grantApplicationId);
		}

		/// <summary>
		/// Get the data for the application Review grant program.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Review/Program/{grantApplicationId:int}")]
		public JsonResult GetApplicationReviewGrantProgram(int grantApplicationId)
		{
			var model = new ApplicationGrantProgramReviewViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new ApplicationGrantProgramReviewViewModel(grantApplication);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Return a View to Review the application skills training information.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="eligibleExpenseTypeId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Review/Skills/Training/View/{grantApplicationId:int}/{eligibleExpenseTypeId:int}")]
		public ActionResult ApplicationReviewSkillsTrainingView(int grantApplicationId, int eligibleExpenseTypeId)
		{
			ViewBag.GrantApplicationId = grantApplicationId;
			ViewBag.EligibleExpenseTypeId = eligibleExpenseTypeId;

			var grantApplication = _grantApplicationService.Get(grantApplicationId);

			if (!User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.SubmitApplication))
				throw new NotAuthorizedException($"User does not have permission to submit application '{grantApplicationId}'.");

			var eligibleExpenseType = _eligibleExpenseTypeService.Get(eligibleExpenseTypeId);

			if (!grantApplication.TrainingCost.EligibleCosts.Any(ec => ec.EligibleExpenseTypeId == eligibleExpenseType.Id))
				throw new NoContentException("The skills training component specified does not exist.");


			return ReviewValidation(grantApplicationId);
		}

		/// <summary>
		/// Get the data for the application Review for the skills training.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="eligibleExpenseTypeId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Review/Skills/Training/{grantApplicationId:int}/{eligibleExpenseTypeId:int}")]
		public JsonResult GetApplicationReviewSkillsTraining(int grantApplicationId, int eligibleExpenseTypeId)
		{
			var model = new ApplicationSkillsReviewViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				var eligibleExpenseType = _eligibleExpenseTypeService.Get(eligibleExpenseTypeId);
				model = new ApplicationSkillsReviewViewModel(grantApplication, eligibleExpenseType);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns a View to Review the application for the employment services and supports.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Review/ESS/View/{grantApplicationId:int}")]
		public ActionResult ApplicationReviewESSView(int grantApplicationId)
		{
			ViewBag.GrantApplicationId = grantApplicationId;

			var grantApplication = _grantApplicationService.Get(grantApplicationId);

			if (!User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.SubmitApplication))
				throw new NotAuthorizedException($"User does not have permission to submit application '{grantApplicationId}'.");

			return ReviewValidation(grantApplicationId);
		}

		/// <summary>
		/// Get the data for the application Review for the employment services and supports.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Review/ESS/{grantApplicationId:int}")]
		public JsonResult GetApplicationReviewESS(int grantApplicationId)
		{
			var model = new ApplicationESSReviewViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new ApplicationESSReviewViewModel(grantApplication);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns a View for the application Review of the training costs.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Review/Training/Cost/View/{grantApplicationId:int}")]
		public ActionResult ApplicationReviewTrainingCostView(int grantApplicationId)
		{
			ViewBag.GrantApplicationId = grantApplicationId;

			var grantApplication = _grantApplicationService.Get(grantApplicationId);

			if (!User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.SubmitApplication))
				throw new NotAuthorizedException($"User does not have permission to submit application '{grantApplicationId}'.");

			return ReviewValidation(grantApplicationId);
		}

		/// <summary>
		/// Get the data for the application Review of the training costs.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Review/Training/Cost/{grantApplicationId:int}")]
		public JsonResult GetApplicationReviewTrainingCost(int grantApplicationId)
		{
			var model = new ApplicationTrainingCostReviewViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new ApplicationTrainingCostReviewViewModel(grantApplication);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns a View for the application submit process when a Delivery Partner is required.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet, Route("Application/Review/Delivery/Partner/View/{grantApplicationId:int}")]
		public ActionResult ApplicationReviewDeliveryPartnerView(int grantApplicationId)
		{
			var grantApplication = _grantApplicationService.Get(grantApplicationId);

			if (!grantApplication.GrantOpening.GrantStream.IncludeDeliveryPartner)
			{
				this.SetAlert("The application Delivery page is not available in current application.", AlertType.Warning, true);
				return RedirectToAction("Index", "Home");
			}

			ViewBag.GrantApplicationId = grantApplicationId;
			return ReviewValidation(grantApplicationId);
		}

		/// <summary>
		/// Get the data for the application submit process when a Delivery Partner is required.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet, Route("Application/Review/Delivery/Partner/{grantApplicationId:int}")]
		public JsonResult GetApplicationDeliveryPartner(int grantApplicationId)
		{
			var model = new ApplicationDeliveryViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new ApplicationDeliveryViewModel(grantApplication);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Update the application Delivery Partner information in the datasource.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[Route("Application/Review/Delivery/Partner")]
		public JsonResult UpdateApplicationDeliveryPartner(ApplicationDeliveryViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.Id);

				var useDeliveryPartner = model.UsedDeliveryPartner ?? false;

				if (!useDeliveryPartner)
				{
					model.SelectedDeliveryPartnerServices = new List<int>();
					model.DeliveryPartnerId = null;
				}
				else if (!model.DeliveryPartnerId.HasValue)
				{
					ModelState.AddModelError("DeliveryPartnerId", "Please select a Delivery Partner from the list");
				}
				else if (!model.SelectedDeliveryPartnerServices.Any())
				{
					ModelState.AddModelError("SelectedDeliveryPartnerServices", "Please select Delivery Partner services from the list");
				}

				if (ModelState.IsValid)
				{
					grantApplication.RowVersion = Convert.FromBase64String(model.RowVersion);
					_grantApplicationService.UpdateDeliveryPartner(grantApplication, model.DeliveryPartnerId, model.SelectedDeliveryPartnerServices);
					_grantApplicationService.Update(grantApplication);

					model = new ApplicationDeliveryViewModel(grantApplication);
				}
				else
				{
					HandleModelStateValidation(model);
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		/// <summary>
		/// Return a View to Review and submit the application applicant declaration.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet, Route("Application/Review/Applicant/Declaration/View/{grantApplicationId:int}")]
		public ActionResult ApplicationReviewApplicantDeclarationView(int grantApplicationId)
		{
			var grantApplication = _grantApplicationService.Get(grantApplicationId);
			var currentUser = _userService.GetUser(_siteMinderService.CurrentUserGuid);

			if (!User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.SubmitApplication))
				throw new NotAuthorizedException($"User does not have permission to submit application '{grantApplicationId}'.");

			if (grantApplication.IsSubmittable() && grantApplication.GrantOpening.GrantStream.IncludeDeliveryPartner && !grantApplication.UsedDeliveryPartner.HasValue)
			{
				this.SetAlert("You must confirm Delivery Partner selection first", AlertType.Warning, true);
				return RedirectToAction(nameof(ApplicationReviewDeliveryPartnerView), new { grantApplicationId });
			}

			TempData["strDeclarationIaccept"] = currentUser.Organization.LegalName;
			TempData["strDeclarationEmailAddr"] = currentUser.EmailAddress;

			ViewBag.GrantApplicationId = grantApplicationId;
			return ReviewValidation(grantApplicationId);
		}

		/// <summary>
		/// Get the data for the application applicant declaration during the Review and submit process.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet, Route("Application/Review/Applicant/Declaration/{grantApplicationId:int}")]
		public JsonResult GetApplicantDeclaration(int grantApplicationId)
		{
			var model = new ApplicantDeclarationViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				var currentUser = _userService.GetUser(_siteMinderService.CurrentUserGuid);
				model = new ApplicantDeclarationViewModel(grantApplication, currentUser, _grantProgramService);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Submit the application and update the datasource.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Application/Review/Submit")]
		public JsonResult SubmitApplication(ApplicantDeclarationViewModel model)
		{
			try
			{
				if (!model.DeclarationConfirmed)
				{
					ModelState.AddModelError("DeclarationConfirmed", "You must accept the declaration to be able to submit your application.");
				}

				if (ModelState.IsValid)
				{
					var grantApplication = _grantApplicationService.Get(model.Id);

					grantApplication.RowVersion = Convert.FromBase64String(model.RowVersion);

					if (!grantApplication.GrantOpening.State.In(GrantOpeningStates.Open, GrantOpeningStates.OpenForSubmit))
					{
						this.SetAlert("The application cannot be submitted until the grant stream is open.", AlertType.Warning, true);
						model.RedirectURL = "/Ext/Home/Index";
					}

					if (grantApplication.GrantOpening.GrantStream.IncludeDeliveryPartner && !grantApplication.UsedDeliveryPartner.HasValue)
					{
						this.SetAlert("You must confirm Delivery Partner selection", AlertType.Warning, true);
						model.RedirectURL = "/Ext/application/Review/Delivery/Partner/View/" + model.Id;
					}

					_grantApplicationService.Submit(grantApplication);

					var currentUser = _userService.GetUser(_siteMinderService.CurrentUserGuid);
					var requireProfileUpdate = _organizationService.ProfileSubjectToVerification(currentUser.Organization);
					if (requireProfileUpdate)
					{
						// Since the Organization Profile uses the Referrer and the user cannot come back to this page once submitted, we have to supply a return url.
						TempData["ProfileBackUrl"] = Url.Action("Index", "Home");
						this.SetAlert("Your application has been submitted. It has been over 12 months since your Organization Profile was updated. Please verify that your profile is correct, and hit 'Save Organization Profile'.", AlertType.Success, true);
						model.RedirectURL = Url.Action("OrganizationProfileView", "OrganizationProfile");
					}
					else
					{
						this.SetAlert("Your application has been submitted.", AlertType.Success, true);
						model.RedirectURL = "/Ext/Home/Index";
					}
				}
				else
				{
					HandleModelStateValidation(model);
				}
			}
			catch(CommunicationException ex)
			{
				AddAngularError(model, "Summary", "Unable to connect to BCeID webservices.  Please contact support.");
				_logger.Error(ex);
			}
			catch (WebException ex)
			{
				AddAngularError(model, "Summary", "Unable to connect to BCeID webservices.  Please contact support.");
				_logger.Error(ex);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		/// <summary>
		/// Generates a View for the Review and submit process.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		private ActionResult ReviewValidation(int grantApplicationId)
		{
			var grantApplication = _grantApplicationService.Get(grantApplicationId);

			_userService.UpdateUserFromBCeIDAccount();
			var currentUser = _userService.GetUser(_siteMinderService.CurrentUserGuid);

			if (grantApplication.GrantOpening.State == GrantOpeningStates.Open || grantApplication.GrantOpening.State == GrantOpeningStates.OpenForSubmit) { /* continue */ }
			else
			{
				if (grantApplication.GrantOpening.State == GrantOpeningStates.Closed)
				{
					grantApplication.ApplicationStateExternal = ApplicationStateExternal.Incomplete;
					_grantApplicationService.Update(grantApplication);
				}
				this.SetAlert("The application cannot be submitted until the grant stream is open.", AlertType.Warning, true);
				return RedirectToAction("Index", "Home");
			}

			if (!grantApplication.IsSubmittable())
			{
				this.SetAlert("The application Review page is not available when in current state.", AlertType.Warning, true);
				return RedirectToAction("Index", "Home");
			}

			var grantStream = _grantStreamService.Get(grantApplication.GrantOpening.GrantStreamId);
			if (grantStream == null || grantStream.IsActive != true)
			{
				this.SetAlert("The GrantStream is inactive.", AlertType.Warning, true);
				return RedirectToAction("Index", "Home");
			}

			// Check if an OrganizationProfileAdmin has been created
			if (_organizationService.GetOrganizationProfileAdminUserId(currentUser.Organization.Id) == 0)
			{
				// An organization profile must be created before submitting an application
				this.SetAlert("An Organization profile must be completed before you can submit a grant application.", AlertType.Warning, true);
				return RedirectToAction("Index", "Home");
			}

			// Check if an Organization NAICS code is updated to 2017
			if (!_organizationService.IsOrganizationNaicsStatusUpdated(currentUser.Organization.Id)) {
				this.SetAlert("Your organization’s Canada North American Industry Classification System (NAICS) codes are currently out of date. You, or someone from your Organization need to update the NAICS codes on your Organization Profile before submitting an application.", AlertType.Warning, true);
				return RedirectToAction("Index", "Home");
			}

			// Check if an Organization is required to update their uploaded business license documents
			if (_organizationService.RequiresBusinessLicenseDocuments(currentUser.Organization.Id)) {
				this.SetAlert("Your organization’s Business Information Documents are currently out of date. You, or someone from your Organization will need to update these on your Organization Profile before submitting an application.", AlertType.Warning, true);
				return RedirectToAction("Index", "Home");
			}

			// copy the applicant and organization details into the grant application
			grantApplication.CopyApplicant(currentUser);

			grantApplication.CopyOrganization(currentUser.Organization, _naIndustryClassificationSystemService.GetNaIndustryClassificationSystemParentByLevel);

			// need to save the updates to the applicant and organization
			_grantApplicationService.Update(grantApplication);
			return View();
		}
	}
}
