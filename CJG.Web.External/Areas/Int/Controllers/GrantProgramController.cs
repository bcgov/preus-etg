using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Int.Models.GrantPrograms;
using CJG.Web.External.Areas.Int.Models.Notifications;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;

namespace CJG.Web.External.Areas.Int.Controllers
{
	/// <summary>
	/// <paramtyperef name="GrantProgramController"/> class, provides endpoints to manage grant programs.
	/// </summary>
	[AuthorizeAction(Privilege.GM1, Privilege.SM)]
	[RouteArea("Int")]
	[RoutePrefix("Admin/Grant")]
	public class GrantProgramController : BaseController
	{
		#region Variables
		private readonly IStaticDataService _staticDataService;
		private readonly IGrantProgramService _grantProgramService;
		private readonly IGrantStreamService _grantStreamService;
		private readonly IGrantOpeningService _grantOpeningService;
		private readonly IProgramConfigurationService _programConfigurationService;
		private readonly IServiceCategoryService _serviceCategoryService;
		private readonly IServiceLineService _serviceLineService;
		private readonly IExpenseTypeService _expenseTypeService;
		private readonly IEligibleExpenseBreakdownService _eligibleExpenseBreakdownService;
		private readonly IDeliveryPartnerService _deliveryPartnerService;
		private readonly INotificationTypeService _notificationTypeService;
		private readonly INotificationService _notificationService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IFiscalYearService _fiscalYearService;
		private readonly IUserService _userService;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <paramtyperef name="GrantProgramController"/> object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantProgramService"></param>
		/// <param name="grantStreamService"></param>
		/// <param name="grantOpeningService"></param>
		/// <param name="programConfigurationService"></param>
		/// <param name="serviceCategoryService"></param>
		/// <param name="serviceLineService"></param>
		/// <param name="expenseTypeService"></param>
		/// <param name="eligibleExpenseBreakdownService"></param>
		/// <param name="deliveryPartnerService"></param>
		/// <param name="notificationTypeService"></param>
		/// <param name="notificationService"></param>
		/// <param name="grantApplicationService"></param>
		/// <param name="fiscalYearService"></param>
		/// <param name="userService"></param>
		public GrantProgramController(
			IControllerService controllerService,
			IGrantProgramService grantProgramService,
			IGrantStreamService grantStreamService,
			IGrantOpeningService grantOpeningService,
			IProgramConfigurationService programConfigurationService,
			IServiceCategoryService serviceCategoryService,
			IServiceLineService serviceLineService,
			IExpenseTypeService expenseTypeService,
			IEligibleExpenseBreakdownService eligibleExpenseBreakdownService,
			IDeliveryPartnerService deliveryPartnerService,
			INotificationTypeService notificationTypeService,
			INotificationService notificationService,
			IGrantApplicationService grantApplicationService,
			IFiscalYearService fiscalYearService,
			IUserService userService
		) : base(controllerService.Logger)
		{
			_staticDataService = controllerService.StaticDataService;
			_grantProgramService = grantProgramService;
			_grantStreamService = grantStreamService;
			_grantOpeningService = grantOpeningService;
			_programConfigurationService = programConfigurationService;
			_serviceCategoryService = serviceCategoryService;
			_serviceLineService = serviceLineService;
			_expenseTypeService = expenseTypeService;
			_eligibleExpenseBreakdownService = eligibleExpenseBreakdownService;
			_deliveryPartnerService = deliveryPartnerService;
			_notificationTypeService = notificationTypeService;
			_notificationService = notificationService;
			_grantApplicationService = grantApplicationService;
			_fiscalYearService = fiscalYearService;
			_userService = userService;
		}
		#endregion

		#region Endpoints
		/// <summary>
		/// Returns the Grant Program Management Dashboard View.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Program/View")]
		public ActionResult GrantProgramView()
		{
			return View();
		}

		/// <summary>
		/// Returns a simple page to preview how the message will be displayed in a browser.
		/// </summary>
		/// <param name="title"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("Program/Preview")]
		public PartialViewResult Preview(string title, string message)
		{
			ViewData["GrantProgramTitle"] = title;
			ViewData["GrantProgramMessage"] = message;
			return PartialView();
		}

		/// <summary>
		/// Returns all of the grant programs in a JSON object.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Programs")]
		public JsonResult GetGrantPrograms()
		{
			IEnumerable<dynamic> list = null;
			try
			{
				list = _grantProgramService.GetAll().Select(t => new
				{
					t.Id,
					Caption = t.Name,
					t.State,
					ProgramType = t.ProgramTypeId,
					t.ProgramConfigurationId,
					t.AccountCodeId
				});
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(list, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns a grant program for the specified id in a JSON object.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Program/{id:int}")]
		public JsonResult GetGrantProgram(int id)
		{
			var model = new Models.GrantPrograms.GrantProgramViewModel();
			try
			{
				var program = _grantProgramService.Get(id);
				model = new Models.GrantPrograms.GrantProgramViewModel(program, _grantStreamService, _grantOpeningService, _deliveryPartnerService, _expenseTypeService);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, null, () => { if (ex is NoContentException) Response.StatusCode = 204; });
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		#region Grant Program
		/// <summary>
		/// Adds the specified grant program to the datasource and returns the updated information as a JSON object.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Program")]
		public JsonResult AddGrantProgram(Models.GrantPrograms.GrantProgramViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var grantProgram = new GrantProgram();
					Utilities.MapProperties(model, grantProgram);

					_grantProgramService.Add(grantProgram);

					model = new Models.GrantPrograms.GrantProgramViewModel(grantProgram, _grantStreamService, _grantOpeningService, _deliveryPartnerService, _expenseTypeService);
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
						if (ex?.InnerException?.InnerException.Message.Contains("IX_GrantProgram_Name") ?? false
							|| ((DbUpdateException)ex).Entries.Any(x => x.Entity.GetType().Name == "Name"))
						{
							model.AddError("Name", "Grant program name or program code must be unique.");
							model.AddError("ProgramCode", "Grant program name or program code must be unique.");
						}
					}
				});
			}

			return Json(model);
		}

		/// <summary>
		/// Deletes the specified grant program from the datasource and returns a success as a JSON object.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="rowVersion"></param>
		/// <returns></returns>
		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Program/Delete/{id:int}")]
		public JsonResult DeleteGrantProgram(int id, string rowVersion)
		{
			var model = new BaseViewModel();
			try
			{
				var grantProgram = _grantProgramService.Get(id);
				grantProgram.RowVersion = Convert.FromBase64String(rowVersion.Replace(" ", "+"));

				_grantProgramService.Delete(grantProgram);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Changes the state of the grant program to implemented and returns the grant program as a JSON object.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="rowVersion"></param>
		/// <returns></returns>
		[HttpPut]
		[ValidateRequestHeader]
		[Route("Program/Implement/{id:int}")]
		public JsonResult ImplementGrantProgram(int id, string rowVersion)
		{
			var model = new Models.GrantPrograms.GrantProgramViewModel();

			try
			{
				var grantProgram = _grantProgramService.Get(id);
				grantProgram.RowVersion = Convert.FromBase64String(rowVersion.Replace(" ", "+"));

				_grantProgramService.Implement(grantProgram);

				model = new Models.GrantPrograms.GrantProgramViewModel(grantProgram, _grantStreamService, _grantOpeningService, _deliveryPartnerService, _expenseTypeService);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model);
		}

		/// <summary>
		/// Changes the state of the grant program to not implemented and returns the grant program as a JSON object.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="rowVersion"></param>
		/// <returns></returns>
		[HttpPut]
		[ValidateRequestHeader]
		[Route("Program/Terminate/{id:int}")]
		public JsonResult TerminateGrantProgram(int id, string rowVersion)
		{
			var model = new Models.GrantPrograms.GrantProgramViewModel();

			try
			{
				var grantProgram = _grantProgramService.Get(id);
				grantProgram.RowVersion = Convert.FromBase64String(rowVersion.Replace(" ", "+"));

				_grantProgramService.Terminate(grantProgram);

				model = new Models.GrantPrograms.GrantProgramViewModel(grantProgram, _grantStreamService, _grantOpeningService, _deliveryPartnerService, _expenseTypeService);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model);
		}
		#endregion

		#region Updates Sections
		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Program/Definition")]
		public JsonResult UpdateProgramDefinition(Models.GrantPrograms.GrantProgramViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var grantProgram = _grantProgramService.Get(model.Id);
					grantProgram.RowVersion = Convert.FromBase64String(model.RowVersion);

					grantProgram.Name = model.Name;
					grantProgram.ProgramCode = model.ProgramCode;
					grantProgram.ProgramTypeId = model.ProgramTypeId;
					grantProgram.ProgramConfigurationId = (int)model.ProgramConfigurationId;
					grantProgram.UseFIFOReservation = model.UseFIFOReservation;

					_grantProgramService.Update(grantProgram);

					model = new Models.GrantPrograms.GrantProgramViewModel(grantProgram, _grantStreamService, _grantOpeningService, _deliveryPartnerService, _expenseTypeService);
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
		[Route("Program/Home/Page/Message")]
		public JsonResult UpdateHomePageMessage(Models.GrantPrograms.GrantProgramViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var grantProgram = _grantProgramService.Get(model.Id);
					grantProgram.RowVersion = Convert.FromBase64String(model.RowVersion);

					grantProgram.ShowMessage = model.ShowMessage;
					grantProgram.Message = model.Message;

					_grantProgramService.Update(grantProgram);

					model = new Models.GrantPrograms.GrantProgramViewModel(grantProgram, _grantStreamService, _grantOpeningService, _deliveryPartnerService, _expenseTypeService);
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
		[Route("Program/Eligibility")]
		public JsonResult UpdateEligibility(Models.GrantPrograms.GrantProgramViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var grantProgram = _grantProgramService.Get(model.Id);
					grantProgram.RowVersion = Convert.FromBase64String(model.RowVersion);

					grantProgram.EligibilityDescription = model.EligibilityDescription;

					_grantProgramService.Update(grantProgram);

					model = new Models.GrantPrograms.GrantProgramViewModel(grantProgram, _grantStreamService, _grantOpeningService, _deliveryPartnerService, _expenseTypeService);
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
		[Route("Program/Delivery/Partners")]
		public JsonResult UpdateDeliveryPartners(Models.GrantPrograms.GrantProgramViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var grantProgram = _grantProgramService.Get(model.Id);
					grantProgram.RowVersion = Convert.FromBase64String(model.RowVersion);

					grantProgram.IncludeDeliveryPartner = model.IncludeDeliveryPartner;

					if (grantProgram.IncludeDeliveryPartner)
					{
						foreach (var item in model.DeliveryPartners)
						{
							if (item.Id == 0)
							{
								var deliveryPartner = new DeliveryPartner(grantProgram, item.Caption, item.RowSequence) { IsActive = item.IsActive };
								grantProgram.DeliveryPartners.Add(deliveryPartner);
							}
							else
							{
								var deliveryPartner = grantProgram.DeliveryPartners.FirstOrDefault(o => o.Id == item.Id);
								if (deliveryPartner != null)
								{
									if (item.Delete)
									{
										grantProgram.DeliveryPartners.Remove(deliveryPartner);
										_deliveryPartnerService.Remove(deliveryPartner);
									}
									else
									{
										Utilities.MapProperties(item, deliveryPartner);
									}
								}
							}
						}

						foreach (var item in model.DeliveryPartnerServices)
						{
							if (item.Id == 0)
							{
								var deliveryPartnerService = new Core.Entities.DeliveryPartnerService(grantProgram, item.Caption, item.RowSequence) { IsActive = item.IsActive };
								grantProgram.DeliveryPartnerServices.Add(deliveryPartnerService);
							}
							else
							{
								var deliveryPartnerService = grantProgram.DeliveryPartnerServices.FirstOrDefault(o => o.Id == item.Id);
								if (deliveryPartnerService != null)
								{
									if (item.Delete)
									{
										grantProgram.DeliveryPartnerServices.Remove(deliveryPartnerService);
										_deliveryPartnerService.Remove(deliveryPartnerService);
									}
									else
									{
										Utilities.MapProperties(item, deliveryPartnerService);
									}
								}
							}
						}
					}

					_grantProgramService.Update(grantProgram);

					model = new Models.GrantPrograms.GrantProgramViewModel(grantProgram, _grantStreamService, _grantOpeningService, _deliveryPartnerService, _expenseTypeService);
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
		[Route("Program/User/Guidance")]
		public JsonResult UpdateUserGuidance(Models.GrantPrograms.GrantProgramViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var grantProgram = _grantProgramService.Get(model.Id);
					grantProgram.RowVersion = Convert.FromBase64String(model.RowVersion);

					grantProgram.ProgramConfiguration.UserGuidanceCostEstimates = model.ProgramConfiguration.UserGuidanceCostEstimates;
					grantProgram.ProgramConfiguration.UserGuidanceClaims = model.ProgramConfiguration.UserGuidanceClaims;

					_grantProgramService.Update(grantProgram);

					model = new Models.GrantPrograms.GrantProgramViewModel(grantProgram, _grantStreamService, _grantOpeningService, _deliveryPartnerService, _expenseTypeService);
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
		[Route("Program/Payment/Requests")]
		public JsonResult UpdatePaymentRequests(Models.GrantPrograms.GrantProgramViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var grantProgram = _grantProgramService.Get(model.Id);
					grantProgram.RowVersion = Convert.FromBase64String(model.RowVersion);

					grantProgram.RequestedBy = model.RequestedBy;
					grantProgram.ProgramPhone = model.ProgramPhone;
					grantProgram.BatchRequestDescription = model.BatchRequestDescription;
					grantProgram.ExpenseAuthorityId = model.ExpenseAuthorityId;
					grantProgram.DocumentPrefix = model.DocumentPrefix;

					if (grantProgram.AccountCode == null) grantProgram.AccountCode = new AccountCode();
					Utilities.MapProperties(model.AccountCode, grantProgram.AccountCode);

					_grantProgramService.Update(grantProgram);

					model = new Models.GrantPrograms.GrantProgramViewModel(grantProgram, _grantStreamService, _grantOpeningService, _deliveryPartnerService, _expenseTypeService);
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
		[Route("Program/WDA/Service/Expense/Types")]
		public JsonResult UpdateWDAServiceExpenseTypes(Models.GrantPrograms.GrantProgramViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var grantProgram = _grantProgramService.Get(model.Id);
					grantProgram.RowVersion = Convert.FromBase64String(model.RowVersion);

					var programConfiguration = _programConfigurationService.Get(model.ProgramConfiguration.Id);
					programConfiguration.RowVersion = Convert.FromBase64String(model.ProgramConfiguration.RowVersion);

					model.ProgramConfiguration.MapTo(grantProgram, _serviceCategoryService, _serviceLineService);

					_grantProgramService.Update(grantProgram);

					model = new Models.GrantPrograms.GrantProgramViewModel(grantProgram, _grantStreamService, _grantOpeningService, _deliveryPartnerService, _expenseTypeService);
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
		[Route("Program/WDA/Service/Sync")]
		public JsonResult SyncWDAServices(Models.GrantPrograms.GrantProgramViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var grantProgram = _grantProgramService.Get(model.Id);
					grantProgram.RowVersion = Convert.FromBase64String(model.RowVersion);

					var programConfiguration = _programConfigurationService.Get(model.ProgramConfiguration.Id);
					programConfiguration.RowVersion = Convert.FromBase64String(model.ProgramConfiguration.RowVersion);

					_programConfigurationService.SyncWDAService(programConfiguration);

					model = new Models.GrantPrograms.GrantProgramViewModel(grantProgram, _grantStreamService, _grantOpeningService, _deliveryPartnerService, _expenseTypeService);
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
		[Route("Program/Employer/Grant/Expense/Types")]
		public JsonResult UpdateEmployerGrantExpenseTypes(Models.GrantPrograms.GrantProgramViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var grantProgram = _grantProgramService.Get(model.Id);
					grantProgram.RowVersion = Convert.FromBase64String(model.RowVersion);

					var programConfiguration = _programConfigurationService.Get(model.ProgramConfigurationId.Value);
					grantProgram.ProgramConfigurationId = programConfiguration.Id;
					grantProgram.ProgramConfiguration = programConfiguration;

					// Add or update each expense type within the Program Configuration.
					foreach (var item in model.ProgramConfiguration.EligibleExpenseTypes)
					{
						var expenseType = _staticDataService.GetExpenseType(item.ExpenseTypeId);
						var eligibleExpenseType = item.Id == 0 ? new EligibleExpenseType(item.Caption, expenseType) : grantProgram.ProgramConfiguration.EligibleExpenseTypes.FirstOrDefault(eet => eet.Id == item.Id) ?? throw new NoContentException($"Unable to find eligible expense type '{item.Id}'.");

						if (item.Delete)
						{
							// Delete the eligible expense type.
							grantProgram.ProgramConfiguration.EligibleExpenseTypes.Remove(eligibleExpenseType);
							_expenseTypeService.Remove(eligibleExpenseType);
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

								grantProgram.ProgramConfiguration.EligibleExpenseTypes.Add(eligibleExpenseType);
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

					_grantProgramService.Update(grantProgram);

					model = new Models.GrantPrograms.GrantProgramViewModel(grantProgram, _grantStreamService, _grantOpeningService, _deliveryPartnerService, _expenseTypeService);
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
		/// Update the grant program notifications.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Program/Notifications")]
		public JsonResult UpdateNotifications(Models.GrantPrograms.GrantProgramNotificationsViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var grantProgram = _grantProgramService.Get(model.Id);
					grantProgram.RowVersion = Convert.FromBase64String(model.RowVersion);

					foreach (var nvm in model.Notifications)
					{
						var notification = grantProgram.GrantProgramNotificationTypes.FirstOrDefault(n => n.NotificationTypeId == nvm.NotificationTypeId);
						if (notification != null)
						{
							notification.IsActive = nvm.IsActive;
						}
					}

					_grantProgramService.Update(grantProgram);

					model = new Models.GrantPrograms.GrantProgramNotificationsViewModel(grantProgram);
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
		/// Update the grant program document templates.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Program/Document/Templates")]
		public JsonResult UpdateDocumentTemplates(Models.GrantPrograms.GrantProgramDocumentTemplatesViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var grantProgram = _grantProgramService.Get(model.Id);
					grantProgram.RowVersion = Convert.FromBase64String(model.RowVersion);

					if (model.ApplicantDeclarationTemplate != grantProgram.ApplicantDeclarationTemplate.Body)
					{
						grantProgram.ApplicantDeclarationTemplate.Body = model.ApplicantDeclarationTemplate;
						grantProgram.ApplicantDeclarationTemplate.RowVersion = Convert.FromBase64String(model.ApplicantDeclarationTemplateRowVersion);
					}
					if (model.ApplicantCoverLetterTemplate != grantProgram.ApplicantCoverLetterTemplate.Body)
					{
						grantProgram.ApplicantCoverLetterTemplate.Body = model.ApplicantCoverLetterTemplate;
						grantProgram.ApplicantCoverLetterTemplate.RowVersion = Convert.FromBase64String(model.ApplicantCoverLetterTemplateRowVersion);
					}
					if (model.ApplicantScheduleATemplate != grantProgram.ApplicantScheduleATemplate.Body)
					{
						grantProgram.ApplicantScheduleATemplate.Body = model.ApplicantScheduleATemplate;
						grantProgram.ApplicantScheduleATemplate.RowVersion = Convert.FromBase64String(model.ApplicantScheduleATemplateRowVersion);
					}
					if (model.ApplicantScheduleBTemplate != grantProgram.ApplicantScheduleBTemplate.Body)
					{
						grantProgram.ApplicantScheduleBTemplate.Body = model.ApplicantScheduleBTemplate;
						grantProgram.ApplicantScheduleBTemplate.RowVersion = Convert.FromBase64String(model.ApplicantScheduleBTemplateRowVersion);
					}
					if (model.ParticipantConsentTemplate != grantProgram.ParticipantConsentTemplate.Body)
					{
						grantProgram.ParticipantConsentTemplate.Body = model.ParticipantConsentTemplate;
						grantProgram.ParticipantConsentTemplate.RowVersion = Convert.FromBase64String(model.ParticipantConsentTemplateRowVersion);
					}

					_grantProgramService.ValidateTemplates(grantProgram);

					_grantProgramService.Update(grantProgram);

					model = new Models.GrantPrograms.GrantProgramDocumentTemplatesViewModel(grantProgram);
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

		#region Configuration
		/// <summary>
		/// Returns an array of expense authority users.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[OutputCache(CacheProfile = "StaticData", VaryByParam = "none", Location = OutputCacheLocation.ServerAndClient)]
		[Route("Program/Expense/Authorities")]
		public JsonResult GetExpenseAuthorities()
		{
			IEnumerable<KeyValuePair<int, string>> list = null;
			try
			{
				list = _grantProgramService.GetExpenseAuthorities().Select(t => new KeyValuePair<int, string>(t.Id, $"{t.LastName}, {t.FirstName}")).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(list, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns the account codes for the specified grant program id as a JSON object.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Route("Program/Account/Code/{id}")]
		public JsonResult GetAccountCode(int id)
		{
			var viewModel = new AccountCodeViewModel();
			try
			{
				var program = _grantProgramService.Get(id);

				Utilities.MapProperties(program.AccountCode, viewModel);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}

			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns the program configuration for the grant program specified by the id.
		/// </summary>
		/// <param name="id">The grant program id.</param>
		/// <returns></returns>
		[HttpGet]
		[Route("Program/Configuration/{id}")]
		public JsonResult GetProgramConfiguration(int id)
		{
			var viewModel = new ProgramConfigurationViewModel();

			try
			{
				var program = _grantProgramService.Get(id);
				var programConfiguration = program.ProgramConfiguration;
				var eligibleExpenseTypes = programConfiguration.EligibleExpenseTypes.ToArray();

				viewModel = new ProgramConfigurationViewModel(programConfiguration);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}

			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns the template for the specified grant program id as a JSON object.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Route("Program/Template/{id}")]
		public JsonResult GetGrantProgramTemplate(int id)
		{
			var viewModel = new Models.GrantPrograms.GrantProgramViewModel { };
			try
			{
				var program = _grantProgramService.Get(id);
				viewModel.IncludeDeliveryPartner = program.IncludeDeliveryPartner;
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}

			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns the document templates for the specified grant program.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Route("Program/Templates/{id}")]
		public JsonResult GetGrantProgramTemplates(int id)
		{
			var model = new GrantProgramDocumentTemplatesViewModel();

			try
			{
				var program = _grantProgramService.Get(id);
				model = new GrantProgramDocumentTemplatesViewModel(program);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get an array of program configurations.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Program/Configurations")]
		public JsonResult GetProgramConfigurations()
		{
			IEnumerable<KeyValuePair<int, string>> result = null;

			try
			{
				result = _programConfigurationService.GetAll().Select(pc => new KeyValuePair<int, string>(pc.Id, pc.Caption));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}

			return Json(result, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region Notifications
		/// <summary>
		/// Return a partial view to manage the notification type.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Program/Notification/Type/View/{id}")]
		public PartialViewResult GrantProgramNotificationTypeView(int id)
		{
			ViewBag.GrantProgramNotificationTypeId = id;
			return PartialView("_NotificationType");
		}

		/// <summary>
		/// Return a partial view to manage the notification queue.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Program/Notification/View/{id}")]
		public PartialViewResult GrantProgramNotificationView(int id)
		{
			ViewBag.GrantProgramNotificationQueueId = id;
			return PartialView("_Notification");
		}

		/// <summary>
		/// Returns an arary of notification trigger types.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Program/Notification/Triggers")]
		public JsonResult GetNotificationTriggers()
		{
			var model = new BaseViewModel();
			try
			{
				var triggers = _notificationTypeService.GetTriggerTypes(true);
				var result = triggers.Select(t => new NotificationTriggerViewModel(t)).ToArray();
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns an array of all the notification types.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Program/Notification/Types")]
		public JsonResult GetNotificationTypes()
		{
			var notificationTypes = new NotificationTypeListViewModel();
			try
			{
				notificationTypes = new NotificationTypeListViewModel(_notificationTypeService.Get(true));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, notificationTypes);
			}

			return Json(notificationTypes, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns an array of all the notification types associated to the specified grant program.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Program/Notification/Types/{grantProgramId}")]
		public JsonResult GetGrantProgramNotificationTypes(int grantProgramId)
		{
			var grantProgramNotificationTypes = new GrantProgramNotificationTypeListViewModel();
			try
			{
				var grantProgram = _grantProgramService.Get(grantProgramId);

				grantProgramNotificationTypes = new GrantProgramNotificationTypeListViewModel(grantProgram.GrantProgramNotificationTypes, grantProgram.Id);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, grantProgramNotificationTypes);
			}

			return Json(grantProgramNotificationTypes, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns the data for the specified grant program notification type.
		/// </summary>
		/// <param name="grantProgramId"></param>
		/// <param name="notificationTypeId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Program/Notification/Type/{grantProgramId}/{notificationTypeId}")]
		public JsonResult GetGrantProgramNotificationType(int grantProgramId, int notificationTypeId)
		{
			var grantProgramNotificationType = new GrantProgramNotificationTypeViewModel();
			try
			{
				var grantProgram = _grantProgramService.Get(grantProgramId);

				grantProgramNotificationType = new GrantProgramNotificationTypeViewModel(grantProgram.GrantProgramNotificationTypes.FirstOrDefault(g => g.NotificationTypeId == notificationTypeId));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, grantProgramNotificationType);
			}

			return Json(grantProgramNotificationType, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Updates the grant program notification types and their custom templates.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Program/Notification/Types")]
		public JsonResult UpdateGrantProgramNotificationTypes(GrantProgramNotificationTypeListViewModel model)
		{
			try
			{
				var grantProgram = _grantProgramService.Get(model.Id);

				foreach (var notificationType in model.GrantProgramNotificationTypes)
				{
					var originalNotificationType = grantProgram.GrantProgramNotificationTypes.FirstOrDefault(t => t.NotificationTypeId == notificationType.NotificationTypeId);
					if (originalNotificationType == null)
					{
						// Add new grant program notification type
						var grantProgramNotificationType = new GrantProgramNotificationType();
						var notificationTemplate = new NotificationTemplate(notificationType.NotificationTemplate.Caption, notificationType.NotificationTemplate.EmailSubject, notificationType.NotificationTemplate.EmailBody);

						grantProgramNotificationType.GrantProgramId = grantProgram.Id;
						grantProgramNotificationType.NotificationTypeId = notificationType.Id;
						grantProgramNotificationType.NotificationTemplate = notificationTemplate;
						grantProgramNotificationType.IsActive = notificationType.IsActive;

						grantProgram.GrantProgramNotificationTypes.Add(grantProgramNotificationType);
					}
					else if (notificationType.ToBeDeleted)
					{
						// Delete 
						var notificationTypeToBeDeleted = grantProgram.GrantProgramNotificationTypes.FirstOrDefault(n => n.NotificationTypeId == notificationType.NotificationTypeId);

						grantProgram.GrantProgramNotificationTypes.Remove(notificationTypeToBeDeleted);
					}
					else
					{
						// Update
						// Template has been updated
						if ((notificationType.NotificationTemplate.EmailSubject != originalNotificationType.NotificationTemplate.EmailSubject) ||
							(notificationType.NotificationTemplate.EmailBody != originalNotificationType.NotificationTemplate.EmailBody))
						{
							var notificationTemplate = new NotificationTemplate(notificationType.NotificationTemplate.Caption ?? notificationType.Caption, notificationType.NotificationTemplate.EmailSubject, notificationType.NotificationTemplate.EmailBody);

							if (!String.IsNullOrWhiteSpace(originalNotificationType.NotificationTemplate.Caption)) notificationTemplate.Id = originalNotificationType.NotificationTemplate.Id; // If caption is not empty, it's a custom template

							originalNotificationType.NotificationTemplate = notificationTemplate;
						}

						Utilities.MapProperties(notificationType, originalNotificationType, x => new { x.NotificationTemplate });
					}
				}

				_grantProgramService.Update(grantProgram);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model);
		}

		/// <summary>
		/// Returns an array of all the notifications associated to the specified grant program and the filter.
		/// </summary>
		/// <param name="grantProgramId"></param>
		/// <param name="page"></param>
		/// <param name="quantity"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("Program/Notifications/{grantProgramId}/{page}/{quantity}")]
		public JsonResult GetNotifications(int grantProgramId, int page, int quantity, NotificationQueueFilterViewModel filter)
		{
			var model = new BaseViewModel();
			try
			{
				var notifications = _notificationService.GetGrantProgramNotifications(grantProgramId, page, quantity, filter.GetFilter());
				var result = new
				{
					RecordsFiltered = notifications.Items.Count(),
					RecordsTotal = notifications.Total,
					Data = notifications.Items.Select(n => new NotificationQueueViewModel(n)).ToArray()
				};
				return Json(result);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		/// <summary>
		/// Returns the notification filter data for the grant program.
		/// </summary>
		/// <param name="grantProgramId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Program/Notification/Filters/{grantProgramId}")]
		public JsonResult GetNotificationFilters(int grantProgramId)
		{
			var model = new BaseViewModel();
			try
			{
				var grantProgram = _grantProgramService.Get(grantProgramId);

				var organizations = _notificationService.GetGrantProgramNotificationOrganizations(grantProgram.Id);
				var notificationTypes = _notificationService.GetGrantProgramNotificationNotificationTypes(grantProgram.Id);

				var result = new
				{
					Organizations = organizations.Where(o => o != null).Select(n => n.LegalName).ToArray(),
					NotificationTypes = notificationTypes.Where(o => o != null).Select(n => n.Caption).ToArray()
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns the data for the specified grant program notification.
		/// </summary>
		/// <param name="notificationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Program/Notification/{notificationId}")]
		public JsonResult GetNotification(int notificationId)
		{
			var notification = new NotificationQueueViewModel();
			try
			{
				notification = new NotificationQueueViewModel(_notificationService.GetApplicationNotification(notificationId));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, notification);
			}
			return Json(notification, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns an array of internal application states.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Program/Notification/Application/States")]
		public JsonResult GetApplicationStates()
		{
			var model = new BaseViewModel();
			try
			{
				var result = _grantApplicationService.GetInternalStates(true);
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns the current user email.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Program/Notification/User/Email")]
		public JsonResult GetUserEmail()
		{
			var model = new BaseViewModel();
			try
			{
				var user = _userService.GetInternalUser(User.GetUserId().Value);
				var result = user.Email;
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region Document Templates
		/// <summary>
		/// Return a view for the notification preview.
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Program/Document/Template/Preview/View")]
		public ActionResult DocumentTemplatePreview(GrantProgramDocumentTemplatePreviewModel model)
		{
			TempData["_RemoveHeader"] = true;
			return View(model);
		}

		/// <summary>
		/// Returns a view to display the specified notification parsed template.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Program/Document/Template/Preview")]
		public ActionResult GetDocumentTemplatePreview(GrantProgramDocumentTemplatePreviewModel model)
		{
			try
			{
				var data = model.GenerateTestEntities(User, _userService, _grantProgramService, _fiscalYearService);

				model.Body = _grantProgramService.GetType().GetMethod(model.Template).Invoke(_grantProgramService, new object[] { data.GrantApplication }) as string;
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
