using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Core.Interfaces.Service.Settings;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Ext.Models.Attachments;
using CJG.Web.External.Areas.Int.Models;
using CJG.Web.External.Areas.Int.Models.Notifications;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Int.Controllers
{
	[RouteArea("Int")]
	[Authorize(Roles = "Assessor, System Administrator, Director, Financial Clerk")]
	public class ApplicationController : BaseController
	{
		#region Variables
		private static readonly string PartialViewLocation = "~/Areas/Ext/Views/Shared/";
		private readonly IStaticDataService _staticDataService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IGrantAgreementService _grantAgreementService;
		private readonly IGrantOpeningService _grantOpeningService;
		private readonly IAttachmentService _attachmentService;
		private readonly ITrainingProgramService _trainingProgramService;
		private readonly ITrainingProviderService _trainingProviderService;
		private readonly ITrainingProviderInventoryService _trainingProviderInventoryService;
		private readonly IEligibleCostService _eligibleCostService;
		private readonly IUserService _userService;
		private readonly INoteService _noteService;
		private readonly INaIndustryClassificationSystemService _naicsService;
		private readonly IAuthorizationService _authorizationService;
		private readonly IUserManagerAdapter _userManager;
		private readonly IInternalUserFilterService _internalUserFilterService;
		private readonly ITrainingProviderSettings _trainingProviderSettings;
		private readonly IParticipantService _participantService;
		private readonly INationalOccupationalClassificationService _nationalOccupationalClassificationService;
		private readonly IClaimService _claimService;
		private readonly IFiscalYearService _fiscalYearService;
		private readonly IGrantProgramService _grantProgramService;
		private readonly IGrantStreamService _grantStreamService;
		private readonly IRiskClassificationService _riskClassificationService;
		private readonly IEligibleExpenseTypeService _eligibleExpenseTypeService;
		private readonly ICompletionReportService _completionReportService;
		private readonly ISiteMinderService _siteMinderService;
		#endregion

		#region Constructors
		public ApplicationController(
			IControllerService controllerService,
			IGrantAgreementService grantAgreementService,
			IGrantApplicationService grantApplicationService,
			IGrantOpeningService grantOpeningService,
			IAttachmentService attachmentService,
			ITrainingProgramService trainingProgramService,
			ITrainingProviderService trainingProviderService,
			ITrainingProviderInventoryService trainingProviderInventoryService,
			IEligibleCostService eligibleCostService,
			INoteService noteService,
			INaIndustryClassificationSystemService naicsService,
			IAuthorizationService authorizationService,
			IUserManagerAdapter userManager,
			IInternalUserFilterService internalUserFilterService,
			ITrainingProviderSettings trainingProviderSettings,
			IParticipantService participantService,
			INationalOccupationalClassificationService nationalOccupationalClassificationService,
			IClaimService claimService,
			IFiscalYearService fiscalYearService,
			IGrantProgramService grantProgramService,
			IGrantStreamService grantStreamService,
			IRiskClassificationService riskClassificationService,
			IEligibleExpenseTypeService eligibleExpenseTypeService,
			ICompletionReportService completionReportService,
			ISiteMinderService siteMinderService
		   ) : base(controllerService.Logger)
		{
			_userService = controllerService.UserService;
			_staticDataService = controllerService.StaticDataService;
			_grantAgreementService = grantAgreementService;
			_grantApplicationService = grantApplicationService;
			_grantOpeningService = grantOpeningService;
			_attachmentService = attachmentService;
			_trainingProgramService = trainingProgramService;
			_trainingProviderService = trainingProviderService;
			_trainingProviderInventoryService = trainingProviderInventoryService;
			_eligibleCostService = eligibleCostService;
			_userManager = userManager;
			_internalUserFilterService = internalUserFilterService;
			_noteService = noteService;
			_naicsService = naicsService;
			_authorizationService = authorizationService;
			_trainingProviderSettings = trainingProviderSettings;
			_participantService = participantService;
			_nationalOccupationalClassificationService = nationalOccupationalClassificationService;
			_claimService = claimService;
			_fiscalYearService = fiscalYearService;
			_grantProgramService = grantProgramService;
			_grantStreamService = grantStreamService;
			_riskClassificationService = riskClassificationService;
			_eligibleExpenseTypeService = eligibleExpenseTypeService;
			_completionReportService = completionReportService;
			_siteMinderService = siteMinderService;
		}
		#endregion

		#region Endpoints
		/// <summary>
		/// Returns the application details view.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet, Route("Application/Details/View/{grantApplicationId}")]
		public ActionResult ApplicationDetailsView(int grantApplicationId)
		{
			ViewBag.GrantApplicationId = grantApplicationId;
			var grantApplication = _grantApplicationService.Get(grantApplicationId);
			return View();
		}

		/// <summary>
		/// Get the application details view data.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Details/{grantApplicationId}")]
		public JsonResult GetApplicationDetails(int grantApplicationId)
		{
			var model = new ApplicationDetailsViewModel();

			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new ApplicationDetailsViewModel(grantApplication, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns an array of eligible assessors.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Assessors/{grantApplicationId}")]
		public JsonResult GetAssessors(int grantApplicationId)
		{
			InternalUserViewModel[] assessors = null;
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				var list = _authorizationService.GetAssessors();
				if (list != null && list.Count() > 0)
					assessors = list.Select(x => new InternalUserViewModel { Id = x.Id, LastName = x.LastName, FirstName = x.FirstName }).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(assessors, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Enables or disables applicants from reporting participants.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="rowVersion"></param>
		/// <param name="enable"></param>
		/// <returns></returns>
		[HttpPut, AuthorizeAction(Privilege.AM1)]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Application/Applicant/Reporting/Participants/{grantApplicationId}")]
		public JsonResult UpdateApplicantReportingOfParticipants(int grantApplicationId, string rowVersion, bool enable)
		{
			var model = new Models.Applications.UpdateReportParticipantViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				grantApplication.RowVersion = Convert.FromBase64String(rowVersion.Replace(" ", "+"));
				if (enable)
				{
					grantApplication.EnableApplicantParticipantReporting();
				}
				else
				{
					grantApplication.DisableApplicantParticipantReporting();
				}
				_grantApplicationService.Update(grantApplication, ApplicationWorkflowTrigger.EnableApplicantReportingOfParticipants);
				model = new Models.Applications.UpdateReportParticipantViewModel(grantApplication);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Enables or disables applicants from reporting participants.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="rowVersion"></param>
		/// <param name="enable"></param>
		/// <returns></returns>
		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Application/Participant/Reporting/{grantApplicationId}")]
		public JsonResult EnableParticipantReporting(int grantApplicationId, string rowVersion, bool enable)
		{
			var model = new ApplicationDetailsViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				if (enable)
				{
					grantApplication.EnableParticipantReporting();
				}
				else
				{
					grantApplication.DisableParticipantReporting();
				}
				grantApplication.RowVersion = Convert.FromBase64String(rowVersion.Replace(" ", "+"));
				_grantApplicationService.Update(grantApplication, ApplicationWorkflowTrigger.EnableParticipantReporting);
				model = new ApplicationDetailsViewModel(grantApplication, User, x => Url.RouteUrl(x));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		#region Grant Application Attachments
		/// <summary>
		/// Return all of the attachment data for the specified grant application.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Attachments/{grantApplicationId}")]
		public JsonResult GetGrantApplicationAttachments(int grantApplicationId)
		{
			var model = new Models.Attachments.GrantApplicationAttachmentsViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new Models.Attachments.GrantApplicationAttachmentsViewModel(grantApplication);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Return the attachment data for the specified attachment.
		/// </summary>
		/// <param name="attachmentId"></param>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Attachment/{attachmentId}/{grantApplicationId}")]
		public JsonResult GetAttachment(int attachmentId, int grantApplicationId)
		{
			var model = new GrantApplicationAttachmentViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				var attachment = attachmentId > 0 ? _attachmentService.Get(attachmentId) : new Attachment();
				model = new GrantApplicationAttachmentViewModel(grantApplication, attachment);
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
		[Route("Application/Attachments")]
		public JsonResult UpdateApplicationAttachments(int grantApplicationId, HttpPostedFileBase[] files, string attachments)
		{
			var model = new Models.Attachments.GrantApplicationAttachmentsViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);

				// Deserialize model.  This is required because it isn't easy to deserialize an array when including files in a multipart data form.
				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Models.Attachments.UpdateAttachmentViewModel>>(attachments);

				foreach (var attachment in data)
				{
					if (attachment.Delete) // Delete
					{
						var existing = _attachmentService.Get(attachment.Id);
						existing.RowVersion = Convert.FromBase64String(attachment.RowVersion);
						grantApplication.Attachments.Remove(existing);
						_noteService.AddSystemNote(grantApplication, $"Attachment \'{existing.FileName}\' deleted.");
						_attachmentService.Delete(existing);
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
						grantApplication.Attachments.Add(file);
						_noteService.AddSystemNote(grantApplication, $"Attachment \'{file.FileName}\' uploaded.");
						_attachmentService.Add(file, true);
					}
					else if (files.Length > attachment.Index.Value && files[attachment.Index.Value] != null && attachment.Id != 0) // Update with file
					{
						var file = files[attachment.Index.Value].UploadFile(attachment.Description, attachment.FileName);
						var existing = _attachmentService.Get(attachment.Id);
						existing.RowVersion = Convert.FromBase64String(attachment.RowVersion);
						_noteService.AddSystemNote(grantApplication, $"Attachment \'{existing.FileName}\' replaced with \'{file.FileName}\'.");
						attachment.MapToEntity(existing);
						existing.AttachmentData = file.AttachmentData;
						_attachmentService.Update(existing, true);
					}
				}

				model = new Models.Attachments.GrantApplicationAttachmentsViewModel(grantApplication);
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
		[PreventSpam]
		[Route("Application/Attachment/Download/{grantApplicationId}/{attachmentId}")]
		public ActionResult DownloadAttachment(int grantApplicationId, int attachmentId)
		{
			var model = new BaseViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				var attachment = _attachmentService.Get(attachmentId);
				return File(attachment.AttachmentData, System.Net.Mime.MediaTypeNames.Application.Octet, $"{attachment.FileName}{attachment.FileExtension}");
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns the scheduled notifications details for the specified grant application.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Details/Scheduled/Notifications/{grantApplicationId}")]
		public JsonResult GetApplicationScheduledNotifications(int grantApplicationId)
		{
			var model = new EnableNotificationViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);

				model = new EnableNotificationViewModel(grantApplication);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Enables or disables scheduled notifications for this grant application.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Application/Details/Enable/Scheduled/Notifications")]
		public JsonResult EnableScheduledNotifications(EnableNotificationViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.Id);

				Utilities.MapProperties(model, grantApplication);

				_grantApplicationService.Update(grantApplication);

				model = new EnableNotificationViewModel(grantApplication);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		/// <summary>
		/// Return a partial view to manage notifications.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Notifications/View/{id}")]
		public PartialViewResult NotificationView(int id)
		{
			ViewBag.NotificationId = id;
			return PartialView("_ApplicationNotification");
		}
		#endregion
		#endregion
	}
}
