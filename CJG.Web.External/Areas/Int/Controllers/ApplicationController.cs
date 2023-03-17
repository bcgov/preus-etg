using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Ext.Models.Attachments;
using CJG.Web.External.Areas.Int.Models;
using CJG.Web.External.Areas.Int.Models.Notifications;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Controllers
{
    [RouteArea("Int")]
	[Authorize(Roles = "Assessor, System Administrator, Director, Financial Clerk")]
	public class ApplicationController : BaseController
	{
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IAttachmentService _attachmentService;
		private readonly INoteService _noteService;
		private readonly IPrioritizationService _prioritizationService;
		private readonly IAuthorizationService _authorizationService;

		#region Constructors
		public ApplicationController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			IAttachmentService attachmentService,
			INoteService noteService,
			IPrioritizationService prioritizationService,
			IAuthorizationService authorizationService
		   ) : base(controllerService.Logger)
		{
			_grantApplicationService = grantApplicationService;
			_attachmentService = attachmentService;
			_noteService = noteService;
			_prioritizationService = prioritizationService;
			_authorizationService = authorizationService;
		}
		#endregion

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
			ViewBag.GrantFileNumber = grantApplication.FileNumber;

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
				if (list != null && list.Any())
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
		/// <param name="grantApplicationId"></param>
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
		/// <param name="grantApplicationId"></param>
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
						file.AttachmentType = attachment.AttachmentType;
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
				return File(attachment.AttachmentData, MediaTypeNames.Application.Octet, $"{attachment.FileName}{attachment.FileExtension}");
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Downloads specified business license document
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="attachmentId"></param>
		/// <returns></returns>
		[HttpGet]
		[PreventSpam]
		[Route("Application/BusinessLicense/Download/{grantApplicationId}/{attachmentId}")]
		public ActionResult DownloadBusinessLicense(int grantApplicationId, int attachmentId)
		{
			var model = new BaseViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				var organization = grantApplication.Organization;
				var attachment = _attachmentService.GetBusinessLicenseAttachment(organization.Id, attachmentId);
				return File(attachment.AttachmentData, MediaTypeNames.Application.Octet, $"{attachment.FileName}{attachment.FileExtension}");
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

		/// <summary>
		/// Get the application prioritization view data.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Prioritization/{grantApplicationId}")]
		public JsonResult GetPrioritizationInfo(int grantApplicationId)
		{
			var model = new ApplicationPrioritizationInfoViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new ApplicationPrioritizationInfoViewModel(grantApplication);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Forces the Intake Queue to recalculate all Prioritization Scores
		/// </summary>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Application/Prioritization/Recalculate/{grantApplicationId}")]
		public JsonResult RecalculatePriorities(int grantApplicationId)
		{
			var model = new BaseViewModel();
			try
			{
				if (ModelState.IsValid)
				{
					_prioritizationService.RecalculatePriorityScores(grantApplicationId);
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

			model.RedirectURL = "/Int/Admin/Prioritization/Thresholds/View";
			return Json(model);
		}
	}
}
