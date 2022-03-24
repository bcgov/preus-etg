using CJG.Application.Services;
using CJG.Core.Interfaces;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Ext.Models.Attachments;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Ext.Controllers
{
	/// <summary>
	/// ApplicationAttachmentController class, provides a controller endpoints for managing external grant application attachments.
	/// </summary>
	[RouteArea("Ext")]
	[ExternalFilter]
	public class ApplicationAttachmentController : BaseController
	{
		#region Variables
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IAttachmentService _attachmentService;
		private readonly IFileWrapper _fileWrapper;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a ApplicationAttachmentController object.
		/// </summary>
		/// <param name="grantApplicationService"></param>
		/// <param name="attachmentService"></param>
		/// <param name="controllerService"></param>
		/// <param name="fileWrapper"></param>
		public ApplicationAttachmentController(
			IGrantApplicationService grantApplicationService,
			IAttachmentService attachmentService,
			IControllerService controllerService,
			IFileWrapper fileWrapper) : base(controllerService.Logger)
		{
			_grantApplicationService = grantApplicationService;
			_attachmentService = attachmentService;
			_fileWrapper = fileWrapper;
		}
		#endregion

		#region Endpoints
		/// <summary>
		/// Display the Application Attachment View.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[Route("Application/Attachments/View/{grantApplicationId}")]
		public ActionResult AttachmentsView(int grantApplicationId)
		{
			var grantApplication = _grantApplicationService.Get(grantApplicationId);
			ViewBag.GrantApplicationId = grantApplicationId;
			return View();
		}

		/// <summary>
		/// Display the Application Business Case View.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[Route("Application/Business/Case/View/{grantApplicationId}")]
		public ActionResult BusinessCaseView(int grantApplicationId)
		{
			var grantApplication = _grantApplicationService.Get(grantApplicationId);
			ViewBag.GrantApplicationId = grantApplicationId;
			return View();
		}

		/// <summary>
		/// Get the data for the AttachmentsView page.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Attachments/{grantApplicationId}")]
		public JsonResult GetAttachments(int grantApplicationId)
		{
			var model = new GrantApplicationAttachmentsViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new GrantApplicationAttachmentsViewModel(grantApplication);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get the data for the BusinessCaseView page.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Business/Case/{grantApplicationId}")]
		public JsonResult GetBusinessCase(int grantApplicationId)
		{
			var model = new GrantApplicationBusinessCaseViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new GrantApplicationBusinessCaseViewModel(grantApplication);
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
				var attachment = _attachmentService.Get(attachmentId);
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
		[Route("Application/Attachments/{grantApplicationId}")]
		public JsonResult UpdateAttachment(int grantApplicationId, HttpPostedFileBase[] files, string attachments)
		{
			var model = new GrantApplicationAttachmentsViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);

				// Deserialize model.  This is required because it isn't easy to deserialize an array when including files in a multipart data form.
				var data = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<UpdateAttachmentViewModel>>(attachments);

				foreach (var attachment in data)
				{
					// Applicant cannot update Assessor-uploaded files
					if (attachment.AttachmentType == AttachmentType.Document)
						continue;
					
					if (attachment.Delete) // Delete
					{
						var existing = _attachmentService.Get(attachment.Id);
						existing.RowVersion = Convert.FromBase64String(attachment.RowVersion);
						grantApplication.Attachments.Remove(existing);

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

						_attachmentService.Add(file, true);
					}
					else if (files.Length > attachment.Index.Value && files[attachment.Index.Value] != null && attachment.Id != 0) // Update with file
					{
						var file = files[attachment.Index.Value].UploadFile(attachment.Description, attachment.FileName);
						var existing = _attachmentService.Get(attachment.Id);
						existing.RowVersion = Convert.FromBase64String(attachment.RowVersion);

						attachment.MapToEntity(existing);
						existing.AttachmentData = file.AttachmentData;
						_attachmentService.Update(existing, true);
					}
				}

				model = new GrantApplicationAttachmentsViewModel(grantApplication);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Update the business case (update/create) for the specified grant application.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="files"></param>
		/// <param name="attachments"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Application/Business/Case/{grantApplicationId}")]
		public JsonResult UpdateBusinessCase(int grantApplicationId, HttpPostedFileBase file, string description)
		{
			var grantApplication = _grantApplicationService.Get(grantApplicationId);
			var model = new GrantApplicationBusinessCaseViewModel(grantApplication);

			try
			{
				if (file != null)
				{
					var attachment = file.UploadFile(description, file.FileName);

					if (grantApplication.BusinessCaseDocument == null || grantApplication.BusinessCaseDocument.Id == 0)
					{
						grantApplication.BusinessCaseDocument = attachment;
						_attachmentService.Add(attachment, true);
					}
					else
					{
						attachment.Id = grantApplication.BusinessCaseDocument.Id;
						attachment.RowVersion = grantApplication.BusinessCaseDocument.RowVersion;
						_attachmentService.Update(attachment, true);
					}
				}
				model = new GrantApplicationBusinessCaseViewModel(grantApplication);
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
		/// Download the specified file.
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/DownloadResource/{filename}")]
		public ActionResult DownloadResource(string filename, string message)
		{
			try
			{
				filename += ".pdf";
				var fullPath = Server.MapPath($"~/App_Data/PDF/{filename}");

				if (_fileWrapper.Exists(fullPath))
				{
					return File(fullPath, System.Net.Mime.MediaTypeNames.Application.Octet, $"{filename}");
				}
				else
				{
					this.SetAlert(string.Format("The sample {0} could not be found.", string.IsNullOrEmpty(message) ? "agreement" : message), AlertType.Warning, true);
					return Redirect(Request.UrlReferrer.ToString());
				}
			}
			catch (Exception e)
			{
				_logger.Error(e, e.GetAllMessages());
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest, e.GetAllMessages()).HttpStatusCodeResultWithAlert(Response, AlertType.Error);
			}
		}
		#endregion
	}
}
