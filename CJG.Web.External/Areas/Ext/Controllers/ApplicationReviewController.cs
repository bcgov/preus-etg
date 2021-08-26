using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Ext.Controllers
{
	/// <summary>
	/// ApplicationReviewController class, provides a controller endpoints for managing external user grant applications.
	/// </summary>
	[RouteArea("Ext")]
	[ExternalFilter]
	public class ApplicationReviewController : BaseController
	{
		#region Variables
		private readonly ISiteMinderService _siteMinderService;
		private readonly IUserService _userService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IGrantStreamService _grantStreamService;
		private readonly IOrganizationService _organizationService;
		private readonly INaIndustryClassificationSystemService _naIndustryClassificationSystemService;
		private readonly IGrantProgramService _grantProgramService;
		private readonly IEligibleExpenseTypeService _eligibleExpenseTypeService;
		#endregion

		#region Constructors
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
			IGrantOpeningManageScheduledService grantOpeningManageScheduledService,
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
		#endregion

		#region Endpoints
		/// <summary>
		/// Display the Grant Application Review View.
		/// This View is used when Reviewing an application before submiting.
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
			var model = new Models.Applications.ApplicationReviewViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				int? UserId = null;
				UserId = grantApplication.BusinessContactRoles.FirstOrDefault()?.UserId;
				model = new Models.Applications.ApplicationReviewViewModel(grantApplication, UserId == null ? null : _userService.GetUser((int)UserId));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
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
			var model = new Models.Applications.ApplicationGrantProgramReviewViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new Models.Applications.ApplicationGrantProgramReviewViewModel(grantApplication);
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
				throw new NoContentException($"The skills training component specified does not exist.");


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
			var model = new Models.Applications.ApplicationSkillsReviewViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				var eligibleExpenseType = _eligibleExpenseTypeService.Get(eligibleExpenseTypeId);
				model = new Models.Applications.ApplicationSkillsReviewViewModel(grantApplication, eligibleExpenseType);
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
			var model = new Models.Applications.ApplicationESSReviewViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new Models.Applications.ApplicationESSReviewViewModel(grantApplication);
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
			var model = new Models.Applications.ApplicationTrainingCostReviewViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new Models.Applications.ApplicationTrainingCostReviewViewModel(grantApplication);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns a View for the application submital process when a Delivery Partner is required.
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
		/// Get the data for the application submital process when a Delivery Partner is required.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet, Route("Application/Review/Delivery/Partner/{grantApplicationId:int}")]
		public JsonResult GetApplicationDeliveryPartner(int grantApplicationId)
		{
			var model = new Models.Applications.ApplicationDeliveryViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new Models.Applications.ApplicationDeliveryViewModel(grantApplication);
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
		public JsonResult UpdateApplicationDeliveryPartner(Models.Applications.ApplicationDeliveryViewModel model)
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
				else if (model.SelectedDeliveryPartnerServices.Count() == 0)
				{
					ModelState.AddModelError("SelectedDeliveryPartnerServices", "Please select Delivery Partner services from the list");
				}

				if (ModelState.IsValid)
				{
					grantApplication.RowVersion = Convert.FromBase64String(model.RowVersion);
					_grantApplicationService.UpdateDeliveryPartner(grantApplication, model.DeliveryPartnerId, model.SelectedDeliveryPartnerServices);
					_grantApplicationService.Update(grantApplication);

					model = new Models.Applications.ApplicationDeliveryViewModel(grantApplication);
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
		/// Return a View to Review and submit the application applicant declartion.
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

			if (grantApplication.IsSubmitable() && grantApplication.GrantOpening.GrantStream.IncludeDeliveryPartner && !grantApplication.UsedDeliveryPartner.HasValue)
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
			var model = new Models.Applications.ApplicantDeclarationViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				var currentUser = _userService.GetUser(_siteMinderService.CurrentUserGuid);
				model = new Models.Applications.ApplicantDeclarationViewModel(grantApplication, currentUser, _grantProgramService);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region Workflow Endpoints
		/// <summary>
		/// Submit the application and update the datasource.
		/// </summary>
		/// <param name="ViewModel"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Application/Review/Submit")]
		public JsonResult SubmitApplication(Models.Applications.ApplicantDeclarationViewModel model)
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
						model.RedirectURL = "/Ext/application/Review/Delivery/Partner/View/" + model.Id.ToString();
					}

					_grantApplicationService.Submit(grantApplication);

					this.SetAlert("Your application has been submitted.", AlertType.Success, true);
					model.RedirectURL = "/Ext/Home/Index";
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
		#endregion

		#region Helpers
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

			if (!grantApplication.IsSubmitable())
			{
				this.SetAlert("The application Review page is not available when in current state.", AlertType.Warning, true);
				return RedirectToAction("Index", "Home");
			}
			else
			{
				var grantStream = _grantStreamService.Get(grantApplication.GrantOpening.GrantStreamId);
				if (grantStream == null || grantStream.IsActive != true)
				{
					this.SetAlert("The GrantStream is inactive.", AlertType.Warning, true);
					return RedirectToAction("Index", "Home");
				}
			}

			//Check if an OrganizationProfileAdmin has been created
			if (_organizationService.GetOrganizationProfileAdminUserId(currentUser.Organization.Id) == 0)
			{
				// An organization profile must be created before submitting an application
				this.SetAlert("An Organization profile must be completed before you can submit a grant application.", AlertType.Warning, true);
				return RedirectToAction("Index", "Home");
			}

			// copy the applicant and organization details into the grant application
			grantApplication.CopyApplicant(currentUser);

			grantApplication.CopyOrganization(currentUser.Organization, _naIndustryClassificationSystemService.GetNaIndustryClassificationSystemParentByLevel);

			// need to save the updates to the applicant and organization
			_grantApplicationService.Update(grantApplication);
			return View();
		}
		#endregion
	}
}
