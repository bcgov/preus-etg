using CJG.Core.Entities;
using CJG.Core.Interfaces;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Areas.Ext.Models.Applications;
using CJG.Web.External.Areas.Int.Models.GrantStreams;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using System;
using System.Collections.Generic;
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
		private readonly IGrantOpeningService _grantOpeningService;
		private readonly IGrantStreamService _grantStreamService;
		private readonly IGrantOpeningManageScheduledService _grantOpeningManageScheduledService;
		private readonly IFiscalYearService _fiscalYearService;
		private readonly IGrantProgramService _grantProgramService;
		private readonly ISettingService _settingService;
		private readonly IOrganizationService _organizationService;
		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance of a <typeparamref name="ApplicationController"/> object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantApplicationService"></param>
		/// <param name="grantOpeningService"></param>
		/// <param name="grantStreamService"></param>
		/// <param name="grantOpeningManageScheduledService"></param>
		/// <param name="grantProgramService"></param>
		/// <param name="fiscalYearService"></param>
		/// <param name="settingService"></param>
		/// <param name="organizationService"></param>
		public ApplicationController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			IGrantOpeningService grantOpeningService,
			IGrantStreamService grantStreamService,
			IGrantOpeningManageScheduledService grantOpeningManageScheduledService,
			IGrantProgramService grantProgramService,
			IFiscalYearService fiscalYearService,
			ISettingService settingService,
			IOrganizationService organizationService) : base(controllerService.Logger)
		{
			_userService = controllerService.UserService;
			_staticDataService = controllerService.StaticDataService;
			_siteMinderService = controllerService.SiteMinderService;
			_grantApplicationService = grantApplicationService;
			_grantOpeningService = grantOpeningService;
			_grantStreamService = grantStreamService;
			_grantOpeningManageScheduledService = grantOpeningManageScheduledService;
			_fiscalYearService = fiscalYearService;
			_grantProgramService = grantProgramService;
			_settingService = settingService;
			_organizationService = organizationService;
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
		[Route("Application/Grant/Selection/View/{grantApplicationId}/{grantProgramId?}/{seedGrantApplicationId?}")]
		public ActionResult GrantSelectionView(int grantApplicationId, int grantProgramId = 0, int seedGrantApplicationId = 0)
		{
			if (grantApplicationId == 0)
			{
				var currentUser = _userService.GetUser(_siteMinderService.CurrentUserGuid);
				if (currentUser.PhysicalAddress == null)
				{
					this.SetAlert("B.C. Business Address is required to start new application", AlertType.Warning, true);
					return RedirectToAction(nameof(UserProfileController.UpdateUserProfileView), nameof(UserProfileController).Replace("Controller", ""));
				}
			}
			else
			{
				_grantApplicationService.Get(grantApplicationId);
			}

			ViewBag.GrantApplicationId = grantApplicationId;
			ViewBag.GrantProgramId = grantProgramId;
			ViewBag.SeedGrantApplicationId = seedGrantApplicationId;
			return View();
		}

		/// <summary>
		/// Get the data for the grant selection view page.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="grantProgramId"></param>
		/// <returns></returns>
		[Route("Application/Grant/Selection/{grantApplicationId}/{grantProgramId?}/{seedGrantApplicationId?}")]
		public JsonResult GetGrantSelection(int grantApplicationId, int grantProgramId = 0, int seedGrantApplicationId = 0)
		{
			var model = new ApplicationStartViewModel();
			try
			{
				var fiscalYear = _fiscalYearService.GetFiscalYear(AppDateTime.UtcNow);
				_grantOpeningManageScheduledService.ManageStateTransitions(fiscalYear.Id);

				if (grantApplicationId == 0)
				{
					model = new ApplicationStartViewModel(grantProgramId, seedGrantApplicationId, _grantOpeningService, _grantProgramService, _staticDataService, _grantStreamService);
				}
				else
				{
					var grantApplication = _grantApplicationService.Get(grantApplicationId);
					model = new ApplicationStartViewModel(grantApplication, _grantOpeningService, _grantProgramService, _staticDataService, _grantStreamService);
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
		/// <param name="grantApplicationId">Optional. If provided, will return answers to Eligibility Questions on application.</param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Grant/Stream/Eligibility/Requirements/{grantOpeningId}/{grantApplicationId?}")]
		public JsonResult GetStreamEligibilityRequirements(int grantOpeningId, int? grantApplicationId = 0)
		{
			var model = new GrantStreamEligibilityViewModel();

			try
			{
				var grantOpening = _grantOpeningService.Get(grantOpeningId);
				model = new GrantStreamEligibilityViewModel(grantOpening.GrantStream, _grantStreamService);

				if (grantApplicationId.HasValue && grantApplicationId.Value > 0)
				{
					var grantApplication = _grantApplicationService.Get(grantApplicationId.Value);
					if (grantApplication != null)
					{
						var answers = _grantStreamService.GetGrantStreamAnswers(grantApplication.Id).ToList();

						foreach (var questionModel in model.StreamEligibilityQuestions)
						{
							var answer = answers.FirstOrDefault(a => a.GrantStreamEligibilityQuestionId == questionModel.Id);

							if (answer == null)
								continue;

							questionModel.EligibilityAnswer = answer.EligibilityAnswer;
							questionModel.RationaleAnswer = answer.RationaleAnswer;
						}
					}
				}
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
				var dbModel = new GrantStreamEligibilityViewModel(grantOpening.GrantStream, _grantStreamService);

				// Handle the eligibility questions. Use the database, valid versions of the questions to test if the client data is correct.
				// passedEligibilityQuestions replaces previous code which set GrantApplication.EligibilityConfirmed to false if
				// there were no eligibility questions, and set it to true if there were (since if there are questions that must be
				// true to add the grant application with the extant eligibility question).
				int numQuestions = 0;
				int numFoundQuestions = 0;
				bool passedEligibilityQuestions = true;
				if (dbModel.StreamEligibilityQuestions.Count() != 0)
				{
					foreach (var question in dbModel.StreamEligibilityQuestions)
					{
						numQuestions++;
						foreach (var clientQuestion in model.GrantStream.StreamEligibilityQuestions)
						{
							if (question.Id == clientQuestion.Id)
							{
								numFoundQuestions++;
								if (clientQuestion.EligibilityAnswer == null || question.EligibilityPositiveAnswerRequired && clientQuestion.EligibilityAnswer != true)
								{
									ModelState.AddModelError("EligibilityQuestion" + question.Id, "The stream eligibility requirements must be met for your application to be submitted and assessed.");
									passedEligibilityQuestions = false;
								}

								if (question.EligibilityRationaleAnswerAllowed
								    && clientQuestion.EligibilityAnswer.HasValue
								    && clientQuestion.EligibilityAnswer.Value
								    && string.IsNullOrWhiteSpace(clientQuestion.RationaleAnswer))
								{
									ModelState.AddModelError("RationaleAnswer" + question.Id, "You must provide a reason when selecting 'Yes' for this question.");
									passedEligibilityQuestions = false;
								}
							}
						}
					}
				}
				if (numFoundQuestions != numQuestions)
				{
					ModelState.AddModelError("EligibilityQuestion", "The stream eligibility requirements must be met for your application to be submitted and assessed.");
					passedEligibilityQuestions = false;
				}

				if (model.ProgramType == ProgramTypes.WDAService && !model.HasRequestedAdditionalFunding.HasValue)
					ModelState.AddModelError("AdditionalFundingQuestion", "You must select whether you have previously received or are requesting additional funding.");

				if (!model.HasRequestedAdditionalFunding ?? true)
					ModelState.Remove("DescriptionOfFundingRequested");

				ModelState.Remove("AlternatePhoneViewModel.PhoneNumber");
				ModelState.Remove("AlternatePhoneViewModel.Phone");

				if (ModelState.IsValid)
				{
					// Setup
					User currentUser = _userService.GetUser(_siteMinderService.CurrentUserGuid);

					var grantApplication = new GrantApplication();
					grantApplication.CopyApplicant(currentUser);
					grantApplication.AddApplicationAdministrator(currentUser);
					grantApplication.ApplicationType = _grantApplicationService.GetDefaultApplicationType();

					// Base the completion report that will be processed, on whether the grant program has Account Code for CWRG.
					if (grantOpening.GrantStream.GrantProgram.AccountCodeId == Constants.GrantProgramNameCWRGIdKey)
						grantApplication.CompletionReportId = Constants.CompletionReportCWRG;
					else
						grantApplication.CompletionReportId = Constants.CompletionReportETG;
					grantApplication.GrantOpeningId = model.GrantOpeningId.GetValueOrDefault();
					grantApplication.OrganizationId = currentUser.OrganizationId;
					grantApplication.OrganizationBCeID = currentUser.Organization.BCeIDGuid;
					grantApplication.EligibilityConfirmed = passedEligibilityQuestions;
					grantApplication.InvitationKey = Guid.NewGuid();
					grantApplication.IsAlternateContact = model.IsAlternateContact;

					if (grantApplication.IsAlternateContact == true)
					{
						grantApplication.AlternateEmail = model.AlternateEmail;
						grantApplication.AlternateFirstName = model.AlternateFirstName;
						grantApplication.AlternateJobTitle = model.AlternateJobTitle;
						grantApplication.AlternateLastName = model.AlternateLastName;
						grantApplication.AlternatePhoneExtension = model.AlternatePhoneExtension;
						grantApplication.AlternatePhoneNumber = model.AlternatePhone;
						grantApplication.AlternateSalutation = model.AlternateSalutation;
					}

					grantApplication.InsuranceConfirmed = null;     // InsuranceConfirmed is no longer usable, values stay in GrantApp and are copied to Eligibility Answers
					grantApplication.HasRequestedAdditionalFunding = model.HasRequestedAdditionalFunding;
					grantApplication.DescriptionOfFundingRequested = model.DescriptionOfFundingRequested;

					grantApplication.MaxReimbursementAmt = grantOpening.GrantStream.MaxReimbursementAmt;
					grantApplication.ReimbursementRate = grantOpening.GrantStream.ReimbursementRate;
					grantApplication.TrainingCost = grantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.EmployerGrant ? new TrainingCost(grantApplication, 0) : new TrainingCost(grantApplication, 1);

					grantApplication.RequireAllParticipantsBeforeSubmission = grantOpening.GrantStream.RequireAllParticipantsBeforeSubmission;

					// set start/end dates to user selected dates
					var earliest = grantApplication.DateSubmitted ?? grantApplication.DateAdded;
					grantApplication.StartDate = model.DeliveryStartDate.HasValue ? new DateTime(model.DeliveryStartYear, model.DeliveryStartMonth, model.DeliveryStartDay, 0, 0, 0, DateTimeKind.Local).ToUtcMorning() : earliest;
					grantApplication.EndDate = model.DeliveryEndDate.HasValue ? new DateTime(model.DeliveryEndYear, model.DeliveryEndMonth, model.DeliveryEndDay, 0, 0, 0, DateTimeKind.Local).ToUtcMidnight() : earliest.AddMonths(1);

					foreach (var question in dbModel.StreamEligibilityQuestions)
					{
						foreach (var clientQuestion in model.GrantStream.StreamEligibilityQuestions)
						{
							if (question.Id != clientQuestion.Id)
								continue;

							if (clientQuestion.EligibilityAnswer == null)
								continue;

							var clientQuestionEligibilityAnswer = clientQuestion.EligibilityAnswer.GetValueOrDefault(false);
							grantApplication.GrantStreamEligibilityAnswers.Add(new GrantStreamEligibilityAnswer
							{
								GrantApplication = grantApplication,
								GrantApplicationId = grantApplication.Id,
								GrantStreamEligibilityQuestionId = question.Id,
								EligibilityAnswer = clientQuestionEligibilityAnswer,
								RationaleAnswer = clientQuestionEligibilityAnswer ? clientQuestion.RationaleAnswer : null
							});
						}
					}
					_grantApplicationService.Add(grantApplication);

					if (model.SeedGrantApplicationId > 0)
                    {
						//update grant app based on seed grant app
						var newId = _grantApplicationService.DuplicateApplication(grantApplication, model.SeedGrantApplicationId);
						grantApplication = _grantApplicationService.Get(newId);
					}

					model = new ApplicationStartViewModel(grantApplication, _grantOpeningService, _grantProgramService, _staticDataService, _grantStreamService)
					{
						RedirectURL = Url.Action("ApplicationOverviewView", new { grantApplicationId = grantApplication.Id })
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
		/// <param name="model"></param>
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
				var currentUser = _userService.GetUser(_siteMinderService.CurrentUserGuid);

				var grantOpening = _grantOpeningService.Get(model.GrantOpeningId.GetValueOrDefault());
				var dbModel = new GrantStreamEligibilityViewModel(grantOpening.GrantStream, _grantStreamService);

				// Handle the eligibility questions. Use the database, valid versions of the questions to test if the client data is correct.
				// passedEligibilityQuestions replaces previous code which set GrantApplication.EligibilityConfirmed to false if
				// there were no eligibility questions, and set it to true if there were (since if there are questions that must be
				// true to add the grant application with the extant eligibility question).
				int numQuestions = 0;
				int numFoundQuestions = 0;
				bool passedEligibilityQuestions = true;
				if (dbModel.StreamEligibilityQuestions.Count() != 0)
				{
					foreach (var question in dbModel.StreamEligibilityQuestions)
					{
						numQuestions++;
						foreach (var clientQuestion in model.GrantStream.StreamEligibilityQuestions)
						{
							if (question.Id == clientQuestion.Id)
							{
								numFoundQuestions++;
								if (clientQuestion.EligibilityAnswer == null || question.EligibilityPositiveAnswerRequired && clientQuestion.EligibilityAnswer != true)
								{
									ModelState.AddModelError("EligibilityQuestion" + question.Id, "The stream eligibility requirements must be met for your application to be submitted and assessed.");
									passedEligibilityQuestions = false;
								}

								if (question.EligibilityRationaleAnswerAllowed
								    && clientQuestion.EligibilityAnswer.HasValue
								    && clientQuestion.EligibilityAnswer.Value
								    && string.IsNullOrWhiteSpace(clientQuestion.RationaleAnswer))
								{
									ModelState.AddModelError("RationaleAnswer" + question.Id, "You must provide a reason when selecting 'Yes' for this question.");
									passedEligibilityQuestions = false;
								}
							}
						}
					}
				}
				if (numFoundQuestions != numQuestions)
				{
					ModelState.AddModelError("EligibilityQuestion", "The stream eligibility requirements must be met for your application to be submitted and assessed.");
					passedEligibilityQuestions = false;
				}

				grantApplication.IsAlternateContact = model.IsAlternateContact;
				grantApplication.InsuranceConfirmed = null;     // InsuranceConfirmed is no longer usable, values stay in GrantApp and are copied to Eligibility Answers
				grantApplication.HasRequestedAdditionalFunding = model.HasRequestedAdditionalFunding;

				// record UTC time only
				grantApplication.StartDate = new DateTime(model.DeliveryStartYear, model.DeliveryStartMonth, model.DeliveryStartDay, 0, 0, 0, DateTimeKind.Local).ToUtcMorning();
				grantApplication.EndDate = new DateTime(model.DeliveryEndYear, model.DeliveryEndMonth, model.DeliveryEndDay, 0, 0, 0, DateTimeKind.Local).ToUtcMidnight();

				// Make the training program dates equal to the delivery dates.
				if (grantApplication.ApplicationStateInternal == ApplicationStateInternal.Draft)
				{
					//grantApplication.TrainingPrograms.Where(tp => tp.StartDate < grantApplication.StartDate || tp.StartDate > grantApplication.EndDate).ForEach(x => x.StartDate = grantApplication.StartDate);
					//grantApplication.TrainingPrograms.Where(tp => tp.EndDate > grantApplication.EndDate || tp.EndDate < grantApplication.StartDate).ForEach(x => x.EndDate = grantApplication.EndDate);
					grantApplication.TrainingPrograms.ForEach(x => x.StartDate = grantApplication.StartDate);
					grantApplication.TrainingPrograms.ForEach(x => x.EndDate = grantApplication.EndDate);
				}

				// update the original entry
				_grantApplicationService.ConvertAndValidate(model, grantApplication, ModelState);

				if (grantApplication.GrantOpening.State == GrantOpeningStates.Closed)
					ModelState.AddModelError("", "Your grant selection is no longer available.  You must make a new grant selection for this application.");

				ModelState.Remove("AlternatePhoneViewModel.PhoneNumber");
				ModelState.Remove("AlternatePhoneViewModel.Phone");

				if (!model.HasRequestedAdditionalFunding ?? true)
					ModelState.Remove("DescriptionOfFundingRequested");

				if (ModelState.IsValid)
				{
					var originalStartDate = (DateTime)_grantApplicationService.OriginalValue(grantApplication, ga => ga.StartDate);
					var originalEndDate = (DateTime)_grantApplicationService.OriginalValue(grantApplication, ga => ga.EndDate);
					var originalGrantOpeningId = (int)_grantApplicationService.OriginalValue(grantApplication, ga => ga.GrantOpeningId);
					var originalEligibilityConfirmed = (bool)_grantApplicationService.OriginalValue(grantApplication, ga => ga.EligibilityConfirmed);
					var originalIsAlternateContact = (bool?)_grantApplicationService.OriginalValue(grantApplication, ga => ga.IsAlternateContact);
					grantApplication.EligibilityConfirmed = passedEligibilityQuestions;

					// updates to alternate contact
					if (grantApplication.IsAlternateContact == true)
					{
						grantApplication.AlternateEmail = model.AlternateEmail;
						grantApplication.AlternateFirstName = model.AlternateFirstName;
						grantApplication.AlternateJobTitle = model.AlternateJobTitle;
						grantApplication.AlternateLastName = model.AlternateLastName;
						grantApplication.AlternatePhoneExtension = model.AlternatePhoneExtension;
						grantApplication.AlternatePhoneNumber = model.AlternatePhone;
						grantApplication.AlternateSalutation = model.AlternateSalutation;

					}
					else
					{
						grantApplication.AlternateEmail = null;
						grantApplication.AlternateFirstName = null;
						grantApplication.AlternateJobTitle = null;
						grantApplication.AlternateLastName = null;
						grantApplication.AlternatePhoneExtension = null;
						grantApplication.AlternatePhoneNumber = null;
						grantApplication.AlternateSalutation = null;
					}

					_grantStreamService.RemoveGrantStreamAnswers(grantApplication.Id);
					foreach (var question in dbModel.StreamEligibilityQuestions)
					{
						foreach (var clientQuestion in model.GrantStream.StreamEligibilityQuestions)
						{
							if (question.Id != clientQuestion.Id)
								continue;

							if (clientQuestion.EligibilityAnswer == null)
								continue;

							var clientQuestionEligibilityAnswer = clientQuestion.EligibilityAnswer.GetValueOrDefault(false);

							grantApplication.GrantStreamEligibilityAnswers.Add(new GrantStreamEligibilityAnswer
							{
								GrantApplication = grantApplication,
								GrantApplicationId = grantApplication.Id,
								GrantStreamEligibilityQuestionId = question.Id,
								EligibilityAnswer = clientQuestionEligibilityAnswer,
								RationaleAnswer = clientQuestionEligibilityAnswer ? clientQuestion.RationaleAnswer : null
							});
						}
					}

					if (originalStartDate != grantApplication.StartDate || originalEndDate != grantApplication.EndDate || originalGrantOpeningId != grantApplication.GrantOpeningId || originalEligibilityConfirmed != grantApplication.EligibilityConfirmed)
					{
						_grantApplicationService.ChangeGrantOpening(grantApplication);

						grantApplication.ParticipantForms.ForEach(x => x.ProgramStartDate = grantApplication.StartDate);

						var originalReimbursement = grantApplication.ReimbursementRate;
						grantApplication.ReimbursementRate = grantApplication.GrantOpening.GrantStream.ReimbursementRate;
						grantApplication.MaxReimbursementAmt = grantApplication.GrantOpening.GrantStream.MaxReimbursementAmt;

						// if the reimbursement rate has changed, then mark the state of the training cost as incomplete and store the new rate
						if (Math.Abs(originalReimbursement - grantApplication.GrantOpening.GrantStream.ReimbursementRate) > TypeExtensions.FloatTolerance)
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
					else if (originalIsAlternateContact != grantApplication.IsAlternateContact)
					{
						_grantApplicationService.Update(grantApplication);
					}
					else
					{
						_grantApplicationService.Update(grantApplication);
					}

					model = new ApplicationStartViewModel(grantApplication, _grantOpeningService, _grantProgramService, _staticDataService, _grantStreamService)
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

		/// <summary>
		/// Add the eligibility answers
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="dbQuestions"></param>
		/// <param name="clientQuestions"></param>
		/// <returns></returns>
		private void AddStreamAnswers(int grantApplicationId, IEnumerable<GrantStreamQuestionViewModel> dbQuestions, IEnumerable<GrantStreamQuestionViewModel> clientQuestions)
		{
			List<GrantStreamEligibilityAnswer> answers = new List<GrantStreamEligibilityAnswer>();
			if (dbQuestions.Count() != 0)
			{
				foreach (var question in dbQuestions)
				{
					foreach (var clientQuestion in clientQuestions)
					{
						if (question.Id == clientQuestion.Id)
						{
							if (clientQuestion.EligibilityAnswer != null)
							{
								answers.Add(new GrantStreamEligibilityAnswer
								{
									GrantApplicationId = grantApplicationId,
									GrantStreamEligibilityQuestionId = question.Id,
									EligibilityAnswer = clientQuestion.EligibilityAnswer.GetValueOrDefault(false)
								});
							}
						}
					}
				}
				_grantStreamService.AddGrantStreamAnswers(answers);
			}
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
			if (grantApplication == null)
			{
				this.SetAlert("The application was not found.", AlertType.Warning, true);
				return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", ""));
			}

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

			//if (grantApplication.TrainingPrograms.Any(x => x.TrainingProgramState == TrainingProgramStates.Incomplete && !x.HasValidDates()))
			//	this.SetAlert("Skills training dates do not fall within your delivery period and will need to be rescheduled. Make sure all your skills training dates are accurate to your plan.", AlertType.Warning);

			if (grantApplication.IsSubmittable())
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
			if (!grantApplication.CanReportParticipants && grantApplication.IsPIFSubmittable())
			{
				grantApplication.EnableParticipantReporting();
				_grantApplicationService.Update(grantApplication);
			}

			var currentUser = _userService.GetUser(_siteMinderService.CurrentUserGuid);

			if (!_organizationService.IsOrganizationNaicsStatusUpdated(currentUser.Organization.Id))
			{
				if (_organizationService.NotSubmittedGrantApplications(currentUser.Organization.Id) > 0)
				{
					// Clear NAICS
					_organizationService.ClearOrganizationOldNaicsCode(currentUser.Organization.Id);
				}

				this.SetAlerts(MessageConstants.Message_BCeID_NAICS_Expired, AlertType.Warning);
			}

			if (_organizationService.RequiresBusinessLicenseDocuments(currentUser.Organization.Id))
			{
				_logger.Info($"The Organization is missing up-to-date Business License Documents - {_siteMinderService.CurrentUserGuid}");
				this.SetAlerts(MessageConstants.Message_BCeID_BusinessDocuments_Required, AlertType.Warning);
			}

			return View(SidebarViewModelFactory.Create(grantApplication, ControllerContext));
		}

		[Route("Application/Resume/{grantApplicationId}")]
		public ActionResult ApplicationResume(int grantApplicationId)
		{
			ViewBag.GrantApplicationId = grantApplicationId;

			var newId = _grantApplicationService.RestartApplicationFromWithdrawn(grantApplicationId);

			return RedirectToAction("ApplicationOverviewView", new { grantApplicationId = newId });
		}

        [Route("Application/Duplicate/{grantApplicationId}")]
        public ActionResult ApplicationDuplicate(int grantApplicationId)
        {
			var grantApplication = _grantApplicationService.Get(grantApplicationId);
			//GrantSelectionView
			return RedirectToAction("GrantSelectionView", new { grantApplicationId = 0, grantProgramId = grantApplication.GrantOpening.GrantStream.GrantProgramId, seedGrantApplicationId = grantApplicationId });
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

				if (grantApplication.GetProgramType() == ProgramTypes.EmployerGrant)
                {
					if (grantApplication.RequireAllParticipantsBeforeSubmission != grantApplication.GrantOpening.GrantStream.RequireAllParticipantsBeforeSubmission)
                    {
						// RequirePIFs flag in this grant application is not in sync with the GrantStream
						if (grantApplication.ApplicationStateExternal == ApplicationStateExternal.NotStarted || grantApplication.ApplicationStateExternal == ApplicationStateExternal.Incomplete)
						{
							grantApplication.RequireAllParticipantsBeforeSubmission = grantApplication.GrantOpening.GrantStream.RequireAllParticipantsBeforeSubmission;
						}
						else if (grantApplication.ApplicationStateExternal == ApplicationStateExternal.Complete)
						{
							//do we need to change the state to Incomplete
							//if the new value is RequirePIFs
							if (grantApplication.GrantOpening.GrantStream.RequireAllParticipantsBeforeSubmission)
							{								
								grantApplication.RequireAllParticipantsBeforeSubmission = grantApplication.GrantOpening.GrantStream.RequireAllParticipantsBeforeSubmission;

								//if all the PIFs are not entered then set state to Incomplete
								if(grantApplication.TrainingCost.EstimatedParticipants != grantApplication.ParticipantForms.Count())
								{
									grantApplication.ApplicationStateExternal = ApplicationStateExternal.Incomplete;
								}								
							}
						}

						//save the change and reload
						_grantApplicationService.Update(grantApplication);
						grantApplication = _grantApplicationService.Get(grantApplicationId);
                    }
				}

				model = new ApplicationOverviewViewModel(grantApplication, _settingService);

				var currentUser = _userService.GetUser(_siteMinderService.CurrentUserGuid);

				if (!_organizationService.IsOrganizationNaicsStatusUpdated(currentUser.Organization.Id))
				{
					model.CanSubmit = false;
				}

				if (_organizationService.RequiresBusinessLicenseDocuments(currentUser.Organization.Id))
				{
					model.CanSubmit = false;
				}				
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


		/// <summary>
		/// Create a new Grant Application View / Edit an exist Grant Application View.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/AlternateUsers/{grantApplicationId}")]
		public JsonResult GetGrantApplicationAlternateContacts(int grantApplicationId)
		{
			var model = new List<KeyValuePair<int, string>>();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				var orgContacts = new List<KeyValuePair<int, string>>
				{
					new KeyValuePair<int, string>(0, "Please select a new application contact")
				};

				var applicationContacts = _grantApplicationService
					.GetAvailableApplicationContacts(grantApplication)
					.Select(a => new KeyValuePair<int, string>(a.Id, $"{a.GetUserFullName()} | {a.JobTitle}"))
					.ToList();
				orgContacts.AddRange(applicationContacts);

				model = orgContacts;
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Launched when the 'Set Outcome' modal on the 'ParticipantsReport' page is clicked
		/// </summary>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Application/AlternateUsers/ChangeUser")]
		public ActionResult ChangeUser(ChangeAlternateContactViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.Id);
				grantApplication.RowVersion = Convert.FromBase64String(model.RowVersion);
				_grantApplicationService.ChangeApplicationAdministrator(grantApplication, model.ApplicantContactId);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			this.SetAlert("The application administrator has been updated.", AlertType.Success, true);
			model.RedirectURL = "/Ext/Home/Index";

			return Json(model, JsonRequestBehavior.AllowGet);
		}
	}
}
