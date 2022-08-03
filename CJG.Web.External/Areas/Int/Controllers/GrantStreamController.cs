using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Int.Models.GrantStreams;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Int.Controllers
{
	/// <summary>
	/// <paramtyperef name="GrantStreamController"/> class, provides endpoints to manage grant streams.
	/// </summary>
	[AuthorizeAction(Privilege.GM1, Privilege.SM)]
	[RouteArea("Int")]
	[RoutePrefix("Admin/Grant")]
	public class GrantStreamController : BaseController
	{
		private readonly IStaticDataService _staticDataService;
		private readonly IGrantProgramService _grantProgramService;
		private readonly IGrantStreamService _grantStreamService;
		private readonly IAccountCodeService _accountCodeService;
		private readonly IProgramConfigurationService _programConfigurationService;
		private readonly IEligibleExpenseBreakdownService _eligibleExpenseBreakdownService;
		private readonly IServiceCategoryService _serviceCategoryService;
		private readonly IServiceLineService _serviceLineService;

		/// <summary>
		/// Creates a new instance of a <paramtyperef name="GrantStreamController"/> object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantProgramService"></param>
		/// <param name="grantStreamService"></param>
		/// <param name="accountCodeService"></param>
		/// <param name="programConfigurationService"></param>
		/// <param name="eligibleExpenseBreakdownService"></param>
		/// <param name="serviceCategoryService"></param>
		/// <param name="serviceLineService"></param>
		public GrantStreamController(
			IControllerService controllerService,
			IGrantProgramService grantProgramService,
			IGrantStreamService grantStreamService,
			IAccountCodeService accountCodeService,
			IProgramConfigurationService programConfigurationService,
			IEligibleExpenseBreakdownService eligibleExpenseBreakdownService,
			IServiceCategoryService serviceCategoryService,
			IServiceLineService serviceLineService) : base(controllerService.Logger)
		{
			_staticDataService = controllerService.StaticDataService;
			_grantProgramService = grantProgramService;
			_grantStreamService = grantStreamService;
			_accountCodeService = accountCodeService;
			_programConfigurationService = programConfigurationService;
			_eligibleExpenseBreakdownService = eligibleExpenseBreakdownService;
			_serviceCategoryService = serviceCategoryService;
			_serviceLineService = serviceLineService;
		}

		/// <summary>
		/// Grant Stream Management Dashboard endpoint.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Stream/View")]
		public ActionResult GrantStreamView(int? id)
		{
			return View();
		}

		/// <summary>
		/// Returns an array of grant streams.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Streams/{grantProgramId}")]
		public JsonResult GetGrantStreams(int grantProgramId)
		{
			IEnumerable<dynamic> list = null;
			try
			{
				list = _grantStreamService.GetGrantStreamsForProgram(grantProgramId, false).Select(t => new
				{
					t.Id,
					Caption = t.Name,
					DateFirstUsed = t.DateFirstUsed?.ToString("yyyy-MM-dd") ?? "N/A",
					CanDelete = t.DateFirstUsed == null && !t.IsActive && t.GrantOpenings.Count == 0
				});
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(list, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns the grant stream for the specified id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Stream/{id:int}")]
		public JsonResult GetGrantStream(int id)
		{
			var model = new GrantStreamViewModel();
			try
			{
				var grantStream = _grantStreamService.Get(id);
				model = new GrantStreamViewModel(grantStream, _serviceCategoryService, _staticDataService, _grantStreamService);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns an array of stream questions.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Streams/questions/{grantStreamId}")]
		public JsonResult GetGrantStreamQuestions(int grantStreamId)
		{
			List<GrantStreamQuestionViewModel> list = null;
			try
			{
				list = _grantStreamService.GetGrantStreamQuestions(grantStreamId)
					.Select(n => new GrantStreamQuestionViewModel(n)).ToList();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(list, JsonRequestBehavior.AllowGet);
		}


		/// <summary>
		/// Returns a simple page to preview how the message will be displayed in a browser.
		/// </summary>
		/// <param name="title"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("Stream/Preview")]
		public PartialViewResult Preview(string title, string message)
		{
			ViewData["GrantStreamTitle"] = title;
			ViewData["GrantStreamMessage"] = message;
			return PartialView();
		}

		#region Grant Stream
		/// <summary>
		/// Adds a new grant stream to the datasource.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Stream")]
		public JsonResult AddGrantStream(GrantStreamViewModel model)
		{
			try
			{
				var objective = new HtmlDocument();
				if (model.EligibilityRequirements == null)
					model.EligibilityRequirements = string.Empty;
				objective.LoadHtml(model.EligibilityRequirements);

				if (objective.ParseErrors.Any())
					model.AddError(nameof(model.EligibilityRequirements), "Invalid HTML");

				if (ModelState.IsValid)
				{
					var grantProgram = _grantProgramService.Get(model.GrantProgramId);
					var grantStream = new GrantStream(model.Name, model.Objective, grantProgram);

					Utilities.MapProperties(model, grantStream);

					_grantStreamService.Add(grantStream);
					model = new GrantStreamViewModel(grantStream, _serviceCategoryService, _staticDataService, _grantStreamService);
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
					if (ex is DbUpdateException)
					{
						if (((DbUpdateException)ex).Entries.Any(x => x.Entity.GetType().Name == "Name"))
						{
							model.AddError("Name", "Grant stream name must be unique.");
						}
					}
				});
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Deletes the grant stream from the datasource.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Stream/Delete")]
		public JsonResult DeleteGrantStream(GrantStreamViewModel model)
		{
			try
			{
				var grantStream = _grantStreamService.Get(model.Id);
				grantStream.RowVersion = Convert.FromBase64String(model.RowVersion);
				_grantStreamService.Delete(grantStream);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region Configuration
		/// <summary>
		/// Returns the account codes for the specified id.
		/// </summary>
		/// <param name="id">The account code id.</param>
		/// <returns></returns>
		[HttpGet]
		[Route("Stream/Account/Code/{id}")]
		public JsonResult GetAccountCode(int id)
		{
			var viewModel = new AccountCodeViewModel();
			try
			{
				var accountCode = _accountCodeService.Get(id);
				viewModel = new AccountCodeViewModel(accountCode);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}

			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns the program configuration for the grant stream specified by the id.
		/// </summary>
		/// <param name="id">The grant stream id.</param>
		/// <returns></returns>
		[HttpGet]
		[Route("Stream/Program/Configuration/{id:int}")]
		public JsonResult GetStreamProgramConfiguration(int id)
		{
			var viewModel = new StreamProgramConfigurationViewModel();

			try
			{
				var stream = _grantStreamService.Get(id);
				var programConfiguration = stream.ProgramConfiguration;
				var eligibleExpenseTypes = programConfiguration.EligibleExpenseTypes.ToArray();

				viewModel = new StreamProgramConfigurationViewModel(programConfiguration);

			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}

			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region Update Sections
		/// <summary>
		/// Updates all stream questions from the model.
		/// </summary>
		/// <returns></returns>
		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Streams/UpdateGrantStreamQuestions")]
		public JsonResult UpdateGrantStreamQuestions(GrantStreamViewModel fullmodel)
		{
			List<GrantStreamEligibilityQuestion> list = null;

			int grantStreamId = fullmodel.Id;
			var model = fullmodel.StreamQuestions;

			try
			{
				if (!ModelState.IsValid)
				{
					HandleModelStateValidation(fullmodel);
				}
				else
				{
					var grantStream = _grantStreamService.Get(fullmodel.Id);

					list = _grantStreamService.GetGrantStreamQuestions(grantStreamId).ToList();
					for (int aIdx = 0; aIdx < model.Count; aIdx++)
					{
						var clientQuestion = model[aIdx];
						if (clientQuestion.Id != 0)
						{
							var dbQuestion = list.Where(x => x.Id == clientQuestion.Id).First();

							dbQuestion.EligibilityRequirements = clientQuestion.EligibilityRequirements ?? "";
							dbQuestion.EligibilityQuestion = clientQuestion.EligibilityQuestion;
							dbQuestion.IsActive = clientQuestion.IsActive;
							dbQuestion.EligibilityPositiveAnswerRequired = clientQuestion.EligibilityPositiveAnswerRequired;
							dbQuestion.EligibilityRationaleAnswerAllowed = clientQuestion.EligibilityRationaleAnswerAllowed;
							dbQuestion.EligibilityRationaleAnswerLabel = clientQuestion.EligibilityRationaleAnswerLabel;
							dbQuestion.RowSequence = clientQuestion.RowSequence;
							dbQuestion.RowVersion = Convert.FromBase64String(clientQuestion.RowVersion);
						}
						else
						{
							GrantStreamEligibilityQuestion newQuestion = new GrantStreamEligibilityQuestion();
							newQuestion.EligibilityRequirements = clientQuestion.EligibilityRequirements ?? "";
							newQuestion.EligibilityQuestion = clientQuestion.EligibilityQuestion;
							newQuestion.IsActive = clientQuestion.IsActive;
							newQuestion.EligibilityPositiveAnswerRequired = clientQuestion.EligibilityPositiveAnswerRequired;
							newQuestion.EligibilityRationaleAnswerAllowed = clientQuestion.EligibilityRationaleAnswerAllowed;
							newQuestion.EligibilityRationaleAnswerLabel = clientQuestion.EligibilityRationaleAnswerLabel;
							newQuestion.RowSequence = clientQuestion.RowSequence;
							newQuestion.GrantStreamId = clientQuestion.GrantStreamId;
							list.Add(newQuestion);
						}
					}
					_grantStreamService.UpdateGrantStreamQuestions(list);
					fullmodel = new GrantStreamViewModel(grantStream, _serviceCategoryService, _staticDataService, _grantStreamService);
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			
			// Fix up validation errors.
			// An array returns validation errors in the text form of "StreamQuestions[0].EligibilityQuestion"
			// and then the error text.
			// Fixup without the [] since that is not easily addressable inside javascript.
			if (fullmodel.ValidationErrors != null)
			{
				var originalCount = fullmodel.ValidationErrors.Count;
				for (int aIdx = 0; aIdx < originalCount; aIdx++)
				{
					var newValue = fullmodel.ValidationErrors[aIdx].Key.Replace('[', '_');
					newValue = newValue.Replace(']', '_');
					fullmodel.ValidationErrors.Add(new KeyValuePair<string, string>(newValue, fullmodel.ValidationErrors[aIdx].Value));
				}
			}
			return Json(fullmodel);
		}

		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Stream/Definition")]
		public JsonResult UpdateStreamDefinition(GrantStreamViewModel model)
		{
			try
			{
				var grantStream = _grantStreamService.Get(model.Id);

				var objective = new HtmlDocument();
				if (!string.IsNullOrEmpty(model.EligibilityRequirements))
					objective.LoadHtml(model.EligibilityRequirements);

				if (objective.ParseErrors.Count() > 0)
				{
					model.ValidationErrors.Add(new KeyValuePair<string, string>(nameof(model.EligibilityRequirements), "Invalid HTML"));
					Response.StatusCode = 400;
					Response.TrySkipIisCustomErrors = true;
				}

				if (ModelState.IsValid)
				{
					grantStream.Name = model.Name;
					grantStream.IsActive = model.IsActive;
					grantStream.IncludeDeliveryPartner = model.IncludeDeliveryPartner;
					grantStream.Objective = model.Objective;
					grantStream.MaxReimbursementAmt = model.MaxReimbursementAmt;
					grantStream.ReimbursementRate = model.ReimbursementRate;

					grantStream.DefaultDeniedRate = model.DefaultDeniedRate;
					grantStream.DefaultWithdrawnRate = model.DefaultWithdrawnRate;
					grantStream.DefaultReductionRate = model.DefaultReductionRate;
					grantStream.DefaultSlippageRate = model.DefaultSlippageRate;
					grantStream.DefaultCancellationRate = model.DefaultCancellationRate;

					_grantStreamService.Update(grantStream);
					model = new GrantStreamViewModel(grantStream, _serviceCategoryService, _staticDataService, _grantStreamService);
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
					if (ex is DbUpdateException)
					{
						if (((DbUpdateException)ex).Entries.Any(x => x.Entity.GetType().Name == "Name"))
						{
							model.AddError("Name", "Grant stream name must be unique.");
						}
					}
				});
			}

			return Json(model);
		}

		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Stream/Eligibility")]
		public JsonResult UpdateEligibility(GrantStreamViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var grantStream = _grantStreamService.Get(model.Id);
					grantStream.RowVersion = Convert.FromBase64String(model.RowVersion);

					grantStream.EligibilityEnabled = model.EligibilityEnabled;
					grantStream.EligibilityRequired = model.EligibilityRequired;
					grantStream.EligibilityRequirements = model.EligibilityRequirements;
					grantStream.EligibilityQuestion = model.EligibilityQuestion;

					_grantStreamService.Update(grantStream);
					model = new GrantStreamViewModel(grantStream, _serviceCategoryService, _staticDataService, _grantStreamService);
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

		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Stream/Payment/Requests")]
		public JsonResult UpdatePaymentRequests(GrantStreamViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var grantStream = _grantStreamService.Get(model.Id);
					grantStream.RowVersion = Convert.FromBase64String(model.RowVersion);

					if (model.SelfAccountCode)
					{
						if (grantStream.AccountCode == null || grantStream.AccountCodeId == grantStream.GrantProgram.AccountCodeId)
						{
							grantStream.AccountCode = new AccountCode();
							_grantStreamService.Add(grantStream.AccountCode);
						}
						Utilities.MapProperties(model.AccountCode, grantStream.AccountCode, m => m.Id);
					}
					else
					{
						if (grantStream.AccountCodeId != grantStream.GrantProgram.AccountCodeId)
						{
							_grantStreamService.Remove(grantStream.AccountCode);
						}
						grantStream.AccountCodeId = grantStream.GrantProgram.AccountCodeId;
					}

					_grantStreamService.Update(grantStream);
					model = new GrantStreamViewModel(grantStream, _serviceCategoryService, _staticDataService, _grantStreamService)
					{
						AccountCode = new AccountCodeViewModel(grantStream.AccountCode)
					};
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

		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Stream/Application/Attachments")]
		public JsonResult UpdateApplicationAttachments(GrantStreamViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var grantStream = _grantStreamService.Get(model.Id);
					grantStream.RowVersion = Convert.FromBase64String(model.RowVersion);

					grantStream.AttachmentsIsEnabled = model.AttachmentsIsEnabled;
					grantStream.AttachmentsRequired = model.AttachmentsRequired;
					grantStream.AttachmentsHeader = model.AttachmentsHeader;
					grantStream.AttachmentsUserGuidance = model.AttachmentsUserGuidance;
					grantStream.AttachmentsMaximum = model.AttachmentsMaximum;

					_grantStreamService.Update(grantStream);
					model = new GrantStreamViewModel(grantStream, _serviceCategoryService, _staticDataService, _grantStreamService);
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

		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Stream/Application/Business/Case")]
		public JsonResult UpdateBusinessCase(GrantStreamViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var grantStream = _grantStreamService.Get(model.Id);
					grantStream.RowVersion = Convert.FromBase64String(model.RowVersion);

					grantStream.BusinessCaseIsEnabled = model.BusinessCaseIsEnabled;
					grantStream.BusinessCaseRequired = model.BusinessCaseRequired;
					grantStream.BusinessCaseInternalHeader = model.BusinessCaseInternalHeader;
					grantStream.BusinessCaseExternalHeader = model.BusinessCaseExternalHeader;
					grantStream.BusinessCaseUserGuidance = model.BusinessCaseUserGuidance;
					grantStream.BusinessCaseTemplateURL = model.BusinessCaseTemplateURL;

					_grantStreamService.Update(grantStream);
					model = new GrantStreamViewModel(grantStream, _serviceCategoryService, _staticDataService, _grantStreamService);
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

		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Stream/WDA/Service/Expense/Types")]
		public JsonResult UpdateWDAServiceExpenseTypes(GrantStreamViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var grantStream = _grantStreamService.Get(model.Id);
					grantStream.RowVersion = Convert.FromBase64String(model.RowVersion);


					// Create a new program configuration for this grant stream.
					if (model.ProgramConfigurationId == 0)
					{
						_programConfigurationService.Generate(grantStream);
					}
					else if (model.ProgramConfigurationId == grantStream.GrantProgram.ProgramConfigurationId)
					{
						// Delete the program configuration for this grant stream, as it no longer will be used.
						if (grantStream.ProgramConfigurationId != grantStream.GrantProgram.ProgramConfigurationId)
						{
							_programConfigurationService.Remove(grantStream.ProgramConfiguration);
						}

						grantStream.ProgramConfigurationId = grantStream.GrantProgram.ProgramConfigurationId;
						grantStream.ProgramConfiguration = grantStream.GrantProgram.ProgramConfiguration;
					}
					else if (model.ProgramConfigurationId == grantStream.ProgramConfigurationId)
					{
						var programConfiguration = _programConfigurationService.Get(model.ProgramConfiguration.Id);
						programConfiguration.RowVersion = Convert.FromBase64String(model.ProgramConfiguration.RowVersion);

						model.ProgramConfiguration.MapTo(grantStream, _serviceCategoryService, _serviceLineService);
					}

					_grantStreamService.Update(grantStream);
					model = new GrantStreamViewModel(grantStream, _serviceCategoryService, _staticDataService, _grantStreamService);
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

		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Stream/WDA/Service/Sync")]
		public JsonResult SyncWDAServices(GrantStreamViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var grantStream = _grantStreamService.Get(model.Id);
					grantStream.RowVersion = Convert.FromBase64String(model.RowVersion);

					var programConfiguration = _programConfigurationService.Get(model.ProgramConfiguration.Id);
					programConfiguration.RowVersion = Convert.FromBase64String(model.ProgramConfiguration.RowVersion);

					_programConfigurationService.SyncWDAService(programConfiguration);

					model = new GrantStreamViewModel(grantStream, _serviceCategoryService, _staticDataService, _grantStreamService);
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

		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Stream/Employer/Grant/Expense/Types")]
		public JsonResult UpdateEmployerGrantExpenseTypes(GrantStreamViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var grantStream = _grantStreamService.Get(model.Id);
					grantStream.RowVersion = Convert.FromBase64String(model.RowVersion);

					// Create a new program configuration for this grant stream.
					if (model.ProgramConfigurationId == 0)
					{
						_programConfigurationService.Generate(grantStream);
					}
					else if (model.ProgramConfigurationId == grantStream.GrantProgram.ProgramConfigurationId)
					{
						// Delete the program configuration for this grant stream, as it no longer will be used.
						if (grantStream.ProgramConfigurationId != grantStream.GrantProgram.ProgramConfigurationId)
						{
							_programConfigurationService.Remove(grantStream.ProgramConfiguration);
						}

						grantStream.ProgramConfigurationId = grantStream.GrantProgram.ProgramConfigurationId;
						grantStream.ProgramConfiguration = grantStream.GrantProgram.ProgramConfiguration;
					}
					else if (model.ProgramConfigurationId == grantStream.ProgramConfigurationId)
					{
						var programConfiguration = _programConfigurationService.Get(model.ProgramConfigurationId);
						grantStream.ProgramConfigurationId = programConfiguration.Id;
						grantStream.ProgramConfiguration = programConfiguration;

						// Add or update each expense type within the Program Configuration.
						foreach (var item in model.ProgramConfiguration.EligibleExpenseTypes)
						{
							var expenseType = _staticDataService.GetExpenseType(item.ExpenseTypeId);
							var eligibleExpenseType = item.Id == 0 ? new EligibleExpenseType(item.Caption, expenseType) : grantStream.ProgramConfiguration.EligibleExpenseTypes.FirstOrDefault(eet => eet.Id == item.Id) ?? throw new NoContentException($"Unable to find eligible expense type '{item.Id}'.");

							if (item.Delete)
							{
								// Delete the eligible expense type.
								grantStream.ProgramConfiguration.EligibleExpenseTypes.Remove(eligibleExpenseType);
								_grantStreamService.Remove(eligibleExpenseType);
							}
							else
							{
								Utilities.MapProperties(item, eligibleExpenseType);
								eligibleExpenseType.Description = eligibleExpenseType.Caption;

								if (item.Id == 0)
								{
									// Add a new eligible expense type.
									foreach (var breakdown in item.Breakdowns)
									{
										var eligibleExpenseBreakdown = new EligibleExpenseBreakdown(breakdown.Caption, eligibleExpenseType);
										Utilities.MapProperties(breakdown, eligibleExpenseBreakdown);
										eligibleExpenseType.Breakdowns.Add(eligibleExpenseBreakdown);
									}

									grantStream.ProgramConfiguration.EligibleExpenseTypes.Add(eligibleExpenseType);
								}
								else
								{
									// Update the eligible expense type.
									foreach (var breakdown in item.Breakdowns)
									{
										var eligibleExpenseBreakdown = breakdown.Id == 0 ? new EligibleExpenseBreakdown(breakdown.Caption, eligibleExpenseType) : eligibleExpenseType.Breakdowns.FirstOrDefault(t => t.Id == breakdown.Id) ?? throw new NoContentException($"Unable to find eligible expense breakdown '{item.Id}'."); ;
										if (breakdown.Delete)
										{
											if (eligibleExpenseBreakdown != null)
											{
												eligibleExpenseType.Breakdowns.Remove(eligibleExpenseBreakdown);
												_eligibleExpenseBreakdownService.Remove(eligibleExpenseBreakdown);
											}
										}
										else
										{
											Utilities.MapProperties(breakdown, eligibleExpenseBreakdown);

											if (breakdown.Id == 0)
												eligibleExpenseType.Breakdowns.Add(eligibleExpenseBreakdown);
										}
									}
								}
							}
						}
					}

					_grantStreamService.Update(grantStream);
					model = new GrantStreamViewModel(grantStream, _serviceCategoryService, _staticDataService, _grantStreamService);
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

		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Stream/Reporting")]
		public JsonResult UpdateReporting(GrantStreamViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var grantStream = _grantStreamService.Get(model.Id);
					grantStream.RowVersion = Convert.FromBase64String(model.RowVersion);

					grantStream.CanApplicantReportParticipants = model.CanApplicantReportParticipants;
					grantStream.HasParticipantOutcomeReporting = model.HasParticipantOutcomeReporting;
					grantStream.RequireAllParticipantsBeforeSubmission = model.RequireAllParticipantsBeforeSubmission;

					_grantStreamService.Update(grantStream);
					model = new GrantStreamViewModel(grantStream, _serviceCategoryService, _staticDataService, _grantStreamService);
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
		#endregion
	}
}
