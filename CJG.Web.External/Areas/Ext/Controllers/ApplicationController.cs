using CJG.Core.Entities;
using CJG.Core.Interfaces;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Areas.Ext.Models.Applications;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Ext.Controllers
{
	/// <summary>
	/// <typeparamref name="ApplicationController"/> class, provides a controller endpoints for managing external user grant applications.
	/// </summary>
	[RouteArea("Ext")]
	[ExternalFilter]
	public class ApplicationController : BaseController
	{
		#region Variables
		private readonly IStaticDataService _staticDataService;
		private readonly ISiteMinderService _siteMinderService;
		private readonly IUserService _userService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IAttachmentService _attachmentService;
		private readonly IGrantOpeningService _grantOpeningService;
		private readonly IGrantStreamService _grantStreamService;
		private readonly IOrganizationService _organizationService;
		private readonly IApplicationAddressService _applicationAddressService;
		private readonly IGrantOpeningManageScheduledService _grantOpeningManageScheduledService;
		private readonly INaIndustryClassificationSystemService _naIndustryClassificationSystemService;
		private readonly IFiscalYearService _fiscalYearService;
		private readonly IGrantProgramService _grantProgramService;
		private readonly IEligibleExpenseTypeService _eligibleExpenseTypeService;
		private readonly ICompletionReportService _completionReportService;
		private readonly ISettingService _settingService;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="ApplicationController"/> object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantApplicationService"></param>
		/// <param name="attachmentService"></param>
		/// <param name="grantOpeningService"></param>
		/// <param name="grantStreamService"></param>
		/// <param name="organizationService"></param>
		/// <param name="applicationAddressService"></param>
		/// <param name="grantOpeningManageScheduledService"></param>
		/// <param name="naIndustryClassificationSystemService"></param>
		/// <param name="grantProgramService"></param>
		/// <param name="eligibleExpenseTypeService"></param>
		/// <param name="fiscalYearService"></param>
		/// <param name="settingService"></param>
		public ApplicationController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			IAttachmentService attachmentService,
			IGrantOpeningService grantOpeningService,
			IGrantStreamService grantStreamService,
			IOrganizationService organizationService,
			IApplicationAddressService applicationAddressService,
			IGrantOpeningManageScheduledService grantOpeningManageScheduledService,
			INaIndustryClassificationSystemService naIndustryClassificationSystemService,
			IGrantProgramService grantProgramService,
			IEligibleExpenseTypeService eligibleExpenseTypeService,
			ICompletionReportService completionReportService,
			IFiscalYearService fiscalYearService,
			ISettingService settingService) : base(controllerService.Logger)
		{
			_userService = controllerService.UserService;
			_staticDataService = controllerService.StaticDataService;
			_siteMinderService = controllerService.SiteMinderService;
			_grantApplicationService = grantApplicationService;
			_attachmentService = attachmentService;
			_grantOpeningService = grantOpeningService;
			_grantStreamService = grantStreamService;
			_organizationService = organizationService;
			_applicationAddressService = applicationAddressService;
			_grantOpeningManageScheduledService = grantOpeningManageScheduledService;
			_naIndustryClassificationSystemService = naIndustryClassificationSystemService;
			_fiscalYearService = fiscalYearService;
			_grantProgramService = grantProgramService;
			_eligibleExpenseTypeService = eligibleExpenseTypeService;
			_completionReportService = completionReportService;
			_settingService = settingService;
		}
		#endregion

		#region Endpoints

		#region Grant Selection
		/// <summary>
		/// Create a new Grant Application View / Edit an exist Grant Application View.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="grantProgramId"></param>
		/// <returns></returns>
		[Route("Application/Grant/Selection/View/{grantApplicationId}/{grantProgramId?}")]
		public ActionResult GrantSelectionView(int grantApplicationId, int grantProgramId = 0)
		{
			if (grantApplicationId == 0)
			{
				var currentUser = _userService.GetUser(_siteMinderService.CurrentUserGuid);
				if (currentUser.PhysicalAddress == null)
				{
					this.SetAlert("Business Address is required to start new application", AlertType.Warning, true);
					return RedirectToAction(nameof(UserProfileController.UpdateUserProfileView), nameof(UserProfileController).Replace("Controller", ""));
				}
			}
			else
			{
				_grantApplicationService.Get(grantApplicationId);
			}

			ViewBag.GrantApplicationId = grantApplicationId;
			ViewBag.GrantProgramId = grantProgramId;
			return View();
		}

		/// <summary>
		/// Get the data for the grant selection view page.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="grantProgramId"></param>
		/// <returns></returns>
		[Route("Application/Grant/Selection/{grantApplicationId}/{grantProgramId?}")]
		public JsonResult GetGrantSelection(int grantApplicationId, int grantProgramId = 0)
		{
			var model = new ApplicationStartViewModel();
			try
			{
				var fiscalYear = _fiscalYearService.GetFiscalYear(AppDateTime.UtcNow);
				_grantOpeningManageScheduledService.ManageStateTransitions(fiscalYear.Id);

				if (grantApplicationId == 0)
				{
					model = new ApplicationStartViewModel(grantProgramId, _grantOpeningService, _grantProgramService, _staticDataService);
				}
				else
				{
					var grantApplication = _grantApplicationService.Get(grantApplicationId);
					model = new ApplicationStartViewModel(grantApplication, _grantOpeningService, _grantProgramService, _staticDataService);
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get the stream eligibility requirements data.
		/// </summary>
		/// <param name="grantOpeningId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Grant/Stream/Eligibility/Requirements/{grantOpeningId}")]
		public JsonResult GetStreamEligibilityRequirements(int grantOpeningId)
		{
			var model = new GrantStreamEligibilityViewModel();
			try
			{
				var grantOpening = _grantOpeningService.Get(grantOpeningId);
				model = new GrantStreamEligibilityViewModel(grantOpening.GrantStream);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Add the specified grant application.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Application")]
		public JsonResult AddGrantApplication(ApplicationStartViewModel model)
		{
			try
			{
				var grantOpening = _grantOpeningService.Get(model.GrantOpeningId.GetValueOrDefault());

				if (grantOpening.GrantStream.EligibilityRequired() && (!model.EligibilityConfirmed.HasValue || !model.EligibilityConfirmed.Value))
				{
					ModelState.AddModelError("EligibilityQuestion", "The stream eligibility requirements must be met for your application to be submitted and assessed.");
				}

				if (ModelState.IsValid)
				{
					// Setup
					User currentUser = _userService.GetUser(_siteMinderService.CurrentUserGuid);

					var grantApplication = new GrantApplication();
					grantApplication.CopyApplicant(currentUser);
					grantApplication.AddApplicationAdministrator(currentUser);
					grantApplication.ApplicationType = _grantApplicationService.GetDefaultApplicationType();
					grantApplication.CompletionReportId = _completionReportService.GetCurrentCompletionReport()?.Id ?? 0;
					grantApplication.GrantOpeningId = model.GrantOpeningId.GetValueOrDefault();
					grantApplication.OrganizationId = currentUser.OrganizationId;
					grantApplication.OrganizationBCeID = currentUser.Organization.BCeIDGuid;
					grantApplication.EligibilityConfirmed = (model.EligibilityConfirmed == null ? false : (bool)model.EligibilityConfirmed);
					grantApplication.InvitationKey = Guid.NewGuid();

					grantApplication.MaxReimbursementAmt = grantOpening.GrantStream.MaxReimbursementAmt;
					grantApplication.ReimbursementRate = grantOpening.GrantStream.ReimbursementRate;
					grantApplication.TrainingCost = grantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.EmployerGrant ? new TrainingCost(grantApplication, 0) : new TrainingCost(grantApplication, 1);

					// set start/end dates to user selected dates
					var earliest = grantApplication.DateSubmitted ?? grantApplication.DateAdded;
					grantApplication.StartDate = model.DeliveryStartDate.HasValue ? new DateTime(model.DeliveryStartYear, model.DeliveryStartMonth, model.DeliveryStartDay, 0, 0, 0, DateTimeKind.Local).ToUtcMorning() : earliest;
					grantApplication.EndDate = model.DeliveryEndDate.HasValue ? new DateTime(model.DeliveryEndYear, model.DeliveryEndMonth, model.DeliveryEndDay, 0, 0, 0, DateTimeKind.Local).ToUtcMidnight() : earliest.AddMonths(1);

					_grantApplicationService.Add(grantApplication);
					grantApplication.EligibilityConfirmed();
					model = new ApplicationStartViewModel(grantApplication, _grantOpeningService, _grantProgramService, _staticDataService)
					{
						RedirectURL = Url.Action(nameof(ApplicationOverviewView), new { grantApplicationId = grantApplication.Id })
					};
				}
				else
				{
					HandleModelStateValidation(model);
				}
			}
			catch (Exception e)
			{
				HandleAngularException(e, model);
			}

			return Json(model);
		}

		/// <summary>
		/// Display the Grant Application Funding Selection View.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Application")]
		public JsonResult UpdateGrantApplication(ApplicationStartViewModel model)
		{
			try
			{
				// retrieve the original entry
				var grantApplication = _grantApplicationService.Get(model.Id);

				// record UTC time only
				grantApplication.StartDate = new DateTime(model.DeliveryStartYear, model.DeliveryStartMonth, model.DeliveryStartDay, 0, 0, 0, DateTimeKind.Local).ToUtcMorning();
				grantApplication.EndDate = new DateTime(model.DeliveryEndYear, model.DeliveryEndMonth, model.DeliveryEndDay, 0, 0, 0, DateTimeKind.Local).ToUtcMidnight();

				// If the training program dates fall outside of the delivery dates, make the training program dates equal to the delivery dates.
				if (grantApplication.ApplicationStateInternal == ApplicationStateInternal.Draft)
				{
					grantApplication.TrainingPrograms.Where(tp => tp.StartDate < grantApplication.StartDate || tp.StartDate > grantApplication.EndDate).ForEach(x => x.StartDate = grantApplication.StartDate);
					grantApplication.TrainingPrograms.Where(tp => tp.EndDate > grantApplication.EndDate || tp.EndDate < grantApplication.StartDate).ForEach(x => x.EndDate = grantApplication.EndDate);
				}

				// update the original entry
				_grantApplicationService.ConvertAndValidate(model, grantApplication, ModelState);

				if (grantApplication.GrantOpening.State == GrantOpeningStates.Closed)
					ModelState.AddModelError("", "Your grant selection is no longer available.  You must make a new grant selection for this application.");

				if (!grantApplication.EligibilityConfirmed())
					ModelState.AddModelError("EligibilityQuestion", "The stream eligibility requirements must be met for your application to be submitted and assessed.");

				if (ModelState.IsValid)
				{
					var originalStartDate = (DateTime)_grantApplicationService.OriginalValue(grantApplication, ga => ga.StartDate);
					var originalEndDate = (DateTime)_grantApplicationService.OriginalValue(grantApplication, ga => ga.EndDate);
					var originalGrantOpeningId = (int)_grantApplicationService.OriginalValue(grantApplication, ga => ga.GrantOpeningId);
					var originalEligibilityConfirmed = (bool)_grantApplicationService.OriginalValue(grantApplication, ga => ga.EligibilityConfirmed);

					if (originalStartDate != grantApplication.StartDate || originalEndDate != grantApplication.EndDate || originalGrantOpeningId != grantApplication.GrantOpeningId || originalEligibilityConfirmed != grantApplication.EligibilityConfirmed)
					{
						_grantApplicationService.ChangeGrantOpening(grantApplication);

						grantApplication.ParticipantForms.ForEach(x => x.ProgramStartDate = grantApplication.StartDate);

						// mark TrainingProgram state as Incomplte if dates are out of range
						grantApplication.TrainingPrograms.Where(tp => !tp.HasValidDates()).ForEach(x => x.TrainingProgramState = TrainingProgramStates.Incomplete);

						var original_reimbursement = grantApplication.ReimbursementRate;
						grantApplication.ReimbursementRate = grantApplication.GrantOpening.GrantStream.ReimbursementRate;
						grantApplication.MaxReimbursementAmt = grantApplication.GrantOpening.GrantStream.MaxReimbursementAmt;

						// if the reimbursement rate has changed, then mark the state of the training cost as incomplete and store the new rate
						if (Math.Abs(original_reimbursement - grantApplication.GrantOpening.GrantStream.ReimbursementRate) > TypeExtensions.FloatTollerance)
						{
							if (grantApplication.TrainingCost.TrainingCostState == TrainingCostStates.Complete || grantApplication.TrainingCost.EligibleCosts.Any() || grantApplication.TrainingCost.TotalEstimatedCost > 0)
							{
								grantApplication.TrainingCost.TrainingCostState = TrainingCostStates.Incomplete;
							}

							// also need to re-calculate the eligible costs
							grantApplication.RecalculateEstimatedCosts();
						}

						grantApplication.MarkWithdrawnAndReturnedApplicationAsIncomplete();

						_grantApplicationService.Update(grantApplication);
					}
					else if (grantApplication.ApplicationStateInternal == ApplicationStateInternal.ApplicationWithdrawn)
					{
						grantApplication.MarkWithdrawnAndReturnedApplicationAsIncomplete();
						_grantApplicationService.Update(grantApplication);
					}

					model = new ApplicationStartViewModel(grantApplication, _grantOpeningService, _grantProgramService, _staticDataService)
					{
						RedirectURL = Url.Action(nameof(ApplicationOverviewView), new { grantApplicationId = grantApplication.Id })
					};

					this.SetAlert("Grant Selection details are complete.", AlertType.Success, true);
				}
				else
				{
					HandleModelStateValidation(model);
				}
			}
			catch (Exception e)
			{
				HandleAngularException(e, model);
			}

			return Json(model);
		}
		#endregion

		#region Application Overview
		/// <summary>
		/// Display the Grant Application overview View.
		/// This view is used when editing a grant application.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[Route("Application/Overview/View/{grantApplicationId}")]
		public ActionResult ApplicationOverviewView(int grantApplicationId)
		{
			ViewBag.GrantApplicationId = grantApplicationId;

			var grantApplication = _grantApplicationService.Get(grantApplicationId);

			if (!grantApplication.ApplicationStateExternal.In(
					ApplicationStateExternal.NotStarted,
					ApplicationStateExternal.Incomplete,
					ApplicationStateExternal.Complete,
					ApplicationStateExternal.ApplicationWithdrawn,
					ApplicationStateExternal.NotAccepted))
			{
				this.SetAlert("The application overview page is not available when in current state.", AlertType.Warning, true);
				return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", ""));
			}

			if (grantApplication.TrainingPrograms.Any(x => x.TrainingProgramState == TrainingProgramStates.Incomplete && !x.HasValidDates()))
				this.SetAlert("Skills training dates do not fall within your delivery period and will need to be rescheduled. Make sure all your skills training dates are accurate to your plan.", AlertType.Warning);

			if (grantApplication.IsSubmitable())
			{
				if (grantApplication.ApplicationStateExternal == ApplicationStateExternal.Incomplete)
				{
					grantApplication.ApplicationStateExternal = ApplicationStateExternal.Complete;
					_grantApplicationService.Update(grantApplication);
				}
			}
			else if (grantApplication.ApplicationStateExternal == ApplicationStateExternal.Complete)
			{
				grantApplication.ApplicationStateExternal = ApplicationStateExternal.Incomplete;
				_grantApplicationService.Update(grantApplication);
			}

			return View(SidebarViewModelFactory.Create(grantApplication, ControllerContext));
		}

		/// <summary>
		/// Get the data for the ApplicationOverviewView page.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Overview/{grantApplicationId}")]
		public JsonResult GetApplicationOverview(int grantApplicationId)
		{
			var model = new ApplicationOverviewViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new ApplicationOverviewViewModel(grantApplication, _settingService);
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
		/// Delete the specified grant application.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="rowVersion"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Application/Delete/{grantApplicationId}")]
		public JsonResult DeleteApplication(int grantApplicationId, string rowVersion)
		{
			var model = new BaseApplicationViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);

				if (!grantApplication.ApplicationStateExternal.In(ApplicationStateExternal.Incomplete, ApplicationStateExternal.Complete)) throw new InvalidOperationException("Unable to delete application.");

				grantApplication.RowVersion = Convert.FromBase64String(rowVersion.Replace(" ", "+"));
				_grantApplicationService.Delete(grantApplication);
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
