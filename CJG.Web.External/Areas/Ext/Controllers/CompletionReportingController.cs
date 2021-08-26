using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Core.Interfaces.Service.Settings;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Areas.Ext.Models.Reports;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;
using CJG.Web.External.Models.Shared.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Ext.Controllers
{
	/// <summary>
	/// CompletionReportingController class, provides endpoints for completion reporting.
	/// </summary>
	[RouteArea("Ext")]
	[ExternalFilter]
	public class CompletionReportingController : BaseController
	{
		#region Variables
		private const int FinalCompletionReportStep = 4;

		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IClaimService _claimService;
		private readonly IParticipantService _participantService;
		private readonly ITrainingProviderSettings _trainingProviderSettings;
		private readonly ISettingService _settingService;
		private readonly ICompletionReportService _completionReportService;
		private readonly IEligibleCostBreakdownService _eligibleCostBreakdownService;
		private readonly INaIndustryClassificationSystemService _naIndustryClassificationSystemService;
		private readonly INationalOccupationalClassificationService _nationalOccupationalClassificationService;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="CompletionReportingController"/> object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantApplicationService"></param>
		/// <param name="claimService"></param>
		/// <param name="participantService"></param>
		/// <param name="trainingProviderSettings"></param>
		/// <param name="settingService"></param>
		/// <param name="completionReportService"></param>
		/// <param name="eligibleCostBreakdownService"></param>
		/// <param name="naIndustryClassificationSystemService"></param>
		/// <param name="nationalOccupationalClassificationService"></param>
		public CompletionReportingController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			IClaimService claimService,
			IParticipantService participantService,
			ITrainingProviderSettings trainingProviderSettings,
			ISettingService settingService,
			ICompletionReportService completionReportService,
			IEligibleCostBreakdownService eligibleCostBreakdownService,
			INaIndustryClassificationSystemService naIndustryClassificationSystemService,
			INationalOccupationalClassificationService nationalOccupationalClassificationService) : base(controllerService.Logger)
		{
			_grantApplicationService = grantApplicationService;
			_claimService = claimService;
			_participantService = participantService;
			_trainingProviderSettings = trainingProviderSettings;
			_settingService = settingService;
			_completionReportService = completionReportService;
			_eligibleCostBreakdownService = eligibleCostBreakdownService;
			_naIndustryClassificationSystemService = naIndustryClassificationSystemService;
			_nationalOccupationalClassificationService = nationalOccupationalClassificationService;
		}
		#endregion

		#region Endpoints
		#region Completion Reporting
		/// <summary>
		/// Return a view to report a completion report.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Reporting/Completion/Report/View/{grantApplicationId}")]
		public ActionResult CompletionReportView(int grantApplicationId)
		{
			ViewBag.GrantApplicationId = grantApplicationId;
			var grantApplication = _grantApplicationService.Get(grantApplicationId);

			if (grantApplication.ApplicationStateExternal == ApplicationStateExternal.Closed)
			{
				this.SetAlert("You are not authorized to edit the completion report.", AlertType.Warning, true);
				return RedirectToAction(nameof(CompletionReportingController.CompletionReportDetailsView), new { grantApplicationId });
			}

			if (!User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.SubmitCompletionReport))
			{
				this.SetAlert("You are not authorized to submit the completion report.", AlertType.Warning, true);
				return RedirectToAction(nameof(ReportingController.GrantFileView), nameof(ReportingController).Replace("Controller", ""), new { grantApplicationId });
			}

			return View();
		}

		/// <summary>
		/// Get the data for the completion report view.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Reporting/Completion/Report/{grantApplicationId}")]
		public JsonResult GetCompletionReport(int grantApplicationId)
		{
			var model = new CompletionReportViewModel();

			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new CompletionReportViewModel(grantApplication, _completionReportService);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get the completion report group data.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="completionReportGroupId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Reporting/Completion/Report/Group/{grantApplicationId}/{completionReportGroupId}")]
		public JsonResult GetCompletionReportGroup(int grantApplicationId, int completionReportGroupId)
		{
			var model = new CompletionReportGroupViewModel();

			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				var completionReportGroup = _completionReportService.GetCompletionReportGroup(completionReportGroupId);
				model = new CompletionReportGroupViewModel(grantApplication, completionReportGroup, _naIndustryClassificationSystemService, _nationalOccupationalClassificationService);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get the completion report ESS data.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Reporting/Completion/Report/ESS/{grantApplicationId}")]
		public JsonResult GetEmploymentServicesAndSupports(int grantApplicationId)
		{
			var model = new CompletionReportDynamicCheckboxViewModel();

			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new CompletionReportDynamicCheckboxViewModel(grantApplication);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Update and save the specified completion report group anwsers in the grant application.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Reporting/Completion/Report/")]
		public JsonResult UpdateCompletionReport(CompletionReportGroupViewModel model)
		{
			try
			{
				var participantAnswers = new List<ParticipantCompletionReportAnswer>();
				var employerAnswers = new List<EmployerCompletionReportAnswer>();
				var grantApplication = _grantApplicationService.Get(model.GrantApplicationId);
				var completionReportGroupId = model.Id;
				var doNotIncludeParticipants = new int[0];
				var participantFormsForReport = new int[0];
				var participantIdsForStep = new int[0];

				var claim = grantApplication.GetCurrentClaim();
				var claimType = grantApplication.GetClaimType();

				if (!User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.SubmitCompletionReport))
				{
					throw new NotAuthorizedException("You are not authorized to edit the completion report.");
				}

				if (completionReportGroupId == 1)
				{
					var participants = grantApplication.ParticipantForms;
					participantFormsForReport = participants.Select(pf => pf.Id).ToArray();
				}
				else if (claimType == ClaimTypes.SingleAmendableClaim && claim != null)
				{
					participantFormsForReport = claim.EligibleCosts.SelectMany(ec => ec.ParticipantCosts.Select(pc => pc.ParticipantForm.Id)).Distinct().Where(peId => !doNotIncludeParticipants.Contains(peId)).ToArray();
				}
				else
				{
					participantFormsForReport = grantApplication.ParticipantForms.Where(pf => !pf.IsExcludedFromClaim && !doNotIncludeParticipants.Contains(pf.Id)).Select(pe => pe.Id).ToArray();
				}

				switch (completionReportGroupId)
				{
					case 1:
					case 2:
						foreach (var question in model.Questions)
						{
							var answer = question.Level1Answers.First();
							if (!answer.BoolAnswer)
							{
								// record any entries where the answer was not in the affirmative
								foreach (var participantAnswer in question.Level2Answers.Where(o => o.BoolAnswer))
								{
									// make sure we don't record the same answer twice for a participant (currently only applies to reasons for training outcomes)
									if (participantAnswers.Count(pa => pa.AnswerId == participantAnswer.IntAnswer && pa.ParticipantFormId == participantAnswer.ParticipantFormId) == 0)
									{
										if (participantAnswer.IntAnswer == 0)
										{
											switch (completionReportGroupId)
											{
												case 1:
													throw new InvalidOperationException("A reason must be specified for all participants not completed training.");
												case 2:
													throw new InvalidOperationException("A reason must be specified for all participants not employed after completing training.");
											}
										}

										participantAnswers.Add(new ParticipantCompletionReportAnswer
										{
											GrantApplicationId = grantApplication.Id,
											QuestionId = participantAnswer.QuestionId,
											AnswerId = participantAnswer.IntAnswer,
											ParticipantFormId = participantAnswer.ParticipantFormId ?? 0,
											OtherAnswer = participantAnswer.StringAnswer ?? string.Empty
										});
										if (completionReportGroupId == 2)
										{
											var participant = grantApplication.ParticipantForms.FirstOrDefault(o => o.Id == participantAnswer.ParticipantFormId);
											participant.NocId = null;
											participant.NaicsId = null;
											participant.EmployerName = null;
										}
									}
								}

								// get the ids of those that didn't answer in the affirmative
								doNotIncludeParticipants = participantAnswers.Select(pa => pa.ParticipantFormId).Distinct().ToArray();
							}
						}
						break;
					case 4:
						foreach (var question in model.Questions)
						{
							foreach (var employerAnswer in question.Level1Answers.OrEmptyIfNull())
							{
								employerAnswers.Add(new EmployerCompletionReportAnswer
								{
									QuestionId = employerAnswer.QuestionId,
									AnswerId = employerAnswer.IntAnswer == 0 ? question.DefaultAnswerId : employerAnswer.IntAnswer,
									GrantApplicationId = grantApplication.Id,
									OtherAnswer = employerAnswer.StringAnswer
								});
							}
						}
						break;
				}

				// either get the completion report for which participant answers have already been given, or get the current one
				var completionReport = _completionReportService.GetCompletionReportForParticipants(doNotIncludeParticipants.Length > 0 ? doNotIncludeParticipants : new int[] { 0 }) ?? _completionReportService.GetCurrentCompletionReport();
				var questions = _completionReportService.GetCompletionReportQuestionsForStep(completionReport.Id, completionReportGroupId);

				if (completionReportGroupId == 1)
				{
					// exclude any participants that didn't answer in the affirmative from those on the claim
					participantIdsForStep = participantFormsForReport.ToList().Where(id => !doNotIncludeParticipants.Contains(id)).ToArray();

					// Delete any answers for these participants that are currently saved.
					_completionReportService.DeleteAnswersFor(doNotIncludeParticipants);

					// Delete saved answer it they originally were not included.
					var questionIds = questions.Select(q => q.Id).ToArray();
					_completionReportService.DeleteAnswersFor(participantFormsForReport, questionIds);
				}
				else if (completionReportGroupId == 5)
				{
					// exclude any participants that didn't answer in the affirmative from those on the claim
					participantIdsForStep = participantFormsForReport.ToList().Where(id => !doNotIncludeParticipants.Contains(id)).ToArray();
				}
				else
				{
					// need to exclude any participants that didn't give affirmative answers in the previous step
					var previousQuestion = _completionReportService.GetCompletionReportQuestionsForStep(completionReport.Id, 1).First();
					if (previousQuestion.DefaultAnswerId.HasValue)
					{
						participantIdsForStep = _completionReportService.GetAffirmativeCompletionReportParticipants(participantFormsForReport, previousQuestion.DefaultAnswerId.Value).Select(pe => pe.Id).Where(peId => !doNotIncludeParticipants.Contains(peId)).ToArray();
					}
				}

				// now add all of the other participants with the affirmative answer
				foreach (var participantId in participantIdsForStep)
				{
					switch (completionReportGroupId)
					{
						case 1:
							{
								participantAnswers.Add(new ParticipantCompletionReportAnswer
								{
									GrantApplicationId = grantApplication.Id,
									QuestionId = questions.First().Id,
									AnswerId = questions.First().DefaultAnswerId ?? 0,
									ParticipantFormId = participantId,
									OtherAnswer = string.Empty
								});
							}
							break;
						case 2:
							{
								var participant = grantApplication.ParticipantForms.FirstOrDefault(o => o.Id == participantId);
								var answer = model.Questions.First().Level2Answers.FirstOrDefault(o => o.ParticipantFormId == participant.Id);
								participant.NocId = answer.Noc4Id ?? answer.Noc3Id ?? answer.Noc2Id ?? answer.Noc1Id;
								participant.NaicsId = answer.Naics5Id ?? answer.Naics4Id ?? answer.Naics3Id ?? answer.Naics2Id ?? answer.Naics1Id;
								participant.EmployerName = answer.EmployerName;
								participantAnswers.Add(new ParticipantCompletionReportAnswer
								{
									GrantApplicationId = grantApplication.Id,
									QuestionId = questions.First().Id,
									AnswerId = questions.First().DefaultAnswerId ?? 0,
									ParticipantFormId = participantId,
									OtherAnswer = string.Empty
								});
								break;
							}
						case Core.Entities.Constants.CompletionStepWithMultipleQuestions:
							{
								foreach (var question in model.Questions)
								{
									var participantAnswer = question.Level1Answers.FirstOrDefault(o => o.QuestionId == question.Id && o.ParticipantFormId == participantId);
									participantAnswers.Add(new ParticipantCompletionReportAnswer
									{
										GrantApplicationId = grantApplication.Id,
										QuestionId = participantAnswer.QuestionId,
										AnswerId = participantAnswer.IntAnswer == 0 ? question.DefaultAnswerId : participantAnswer.IntAnswer,
										ParticipantFormId = participantAnswer.ParticipantFormId ?? 0,
										OtherAnswer = participantAnswer.StringAnswer ?? string.Empty
									});
								}
								break;
							}
						case 5:
							{
								var participant = grantApplication.ParticipantForms.FirstOrDefault(o => o.Id == participantId);
								foreach (var question in model.Questions)
								{
									var eligibleCostBreakdowns = participant.EligibleCostBreakdowns.Select(o => o.Id).ToArray();
									var eligibleCostBreakdownIds = question.Level1Answers.Where(o => o.ParticipantFormId == participantId).Select(o => o.EligibleCostBreakdownId ?? 0).ToArray();
									var eligibleCostBreakdownsAdded = eligibleCostBreakdownIds.Except(eligibleCostBreakdowns);
									var eligibleCostBreakdownsRemoved = eligibleCostBreakdowns.Except(eligibleCostBreakdownIds);

									foreach (var eligibleCostBreakdownId in eligibleCostBreakdownsAdded)
									{
										var eligibleCostBreakdown = _eligibleCostBreakdownService.Get(eligibleCostBreakdownId);
										participant.EligibleCostBreakdowns.Add(eligibleCostBreakdown);
									}

									foreach (var eligibleCostBreakdownId in eligibleCostBreakdownsRemoved)
									{
										var eligibleCostBreakdown = participant.EligibleCostBreakdowns.FirstOrDefault(o => o.Id == eligibleCostBreakdownId);
										participant.EligibleCostBreakdowns.Remove(eligibleCostBreakdown);
									}
								}
								break;
							}
					}
				}

				if (_completionReportService.RecordCompletionReportAnswersForStep(completionReportGroupId, participantAnswers, employerAnswers, completionReport.Id, participantIdsForStep))
				{
					var allParticipantsHaveCompletedReport = false;

					if (claimType == ClaimTypes.SingleAmendableClaim && claim != null)
					{
						// need to get all of the participants on the final claim
						var participantsOnClaim = claim.EligibleCosts.SelectMany(ec => ec.ParticipantCosts.Select(pc => pc.ParticipantFormId)).Distinct().Where(peId => !doNotIncludeParticipants.Contains(peId)).OrderBy(peId => peId).ToArray();
						allParticipantsHaveCompletedReport = _completionReportService.AllParticipantsHaveCompletedReport(participantsOnClaim, completionReport.Id);
					}
					else
					{

						allParticipantsHaveCompletedReport = _completionReportService.AllParticipantsHaveCompletedReport(participantIdsForStep, completionReport.Id);
					}

					if (!model.SaveOnly
						&& completionReportGroupId == FinalCompletionReportStep
						&& grantApplication.ApplicationStateExternal == ApplicationStateExternal.ReportCompletion
						&& allParticipantsHaveCompletedReport)
					{
						_grantApplicationService.SubmitCompletionReportToCloseGrantFile(grantApplication);
					}
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get an array of NAICS for the specified level and parent.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="parentId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Reporting/Completion/Report/NAICS/{level}/{parentId?}")]
		public JsonResult GetNAICS(int level, int? parentId)
		{
			try
			{
				var model = _naIndustryClassificationSystemService.GetNaIndustryClassificationSystemChildren(parentId ?? 0, level).Select(n => new KeyValuePair<int, string>(n.Id, $"{n.Code} | {n.Description}")).ToArray();
				return Json(model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				var model = new BaseViewModel();
				HandleAngularException(ex, model);
				return Json(model, JsonRequestBehavior.AllowGet);
			}
		}

		/// <summary>
		/// Get an array of NOCS for the specified level and parent.
		/// </summary>
		/// <param name="level"></param>
		/// <param name="parentId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Reporting/Completion/Report/NOCs/{level}/{parentId?}")]
		public JsonResult GetNOCs(int level, int? parentId)
		{
			try
			{
				var model = _nationalOccupationalClassificationService.GetNationalOccupationalClassificationChildren(parentId ?? 0, level).Select(n => new KeyValuePair<int, string>(n.Id, $"{n.Code} | {n.Description}")).ToArray();
				return Json(model, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				var model = new BaseViewModel();
				HandleAngularException(ex, model);
				return Json(model, JsonRequestBehavior.AllowGet);
			}
		}
		#endregion

		#region Complete Report Details
		/// <summary>
		/// Get the read only view for the completion report.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Reporting/Completion/Report/Details/{grantApplicationId}")]
		public ActionResult CompletionReportDetailsView(int grantApplicationId)
		{
			ViewBag.GrantApplicationId = grantApplicationId;
			return View();
		}

		/// <summary>
		/// Get the data for the completion report view.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Reporting/Completion/Report/Details/Data/{grantApplicationId}")]
		public JsonResult GetCompletionReportDetails(int grantApplicationId)
		{
			var model = new CompletionReportDetailsViewModel();

			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);

				model = new CompletionReportDetailsViewModel(grantApplication, _completionReportService);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Show cmpletion report status
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Reporting/Completion/Status/{grantApplicationId}")]
		public PartialViewResult ShowCompletionReportStatus(int grantApplicationId)
		{
			var model = new CompletionReportStatusViewModel();

			var completionReportStatus = _completionReportService.GetCompletionReportStatus(grantApplicationId);

			model.CompletionReportStatus = (CompletionReportStatus)Enum.Parse(typeof(CompletionReportStatus), completionReportStatus.Replace(" ", string.Empty));

			return PartialView("_ShowCompletionReportStatus", model);
		}
		#endregion
		#endregion
	}
}
