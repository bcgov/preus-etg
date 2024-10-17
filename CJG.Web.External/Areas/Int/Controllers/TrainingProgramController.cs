using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using CJG.Application.Services;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Int.Models.TrainingPrograms;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Controllers
{
	/// <summary>
	/// TrainingProgramController class, controller provides a way to update training programs.
	/// </summary>
	[RouteArea("Int")]
	[Authorize(Roles = "Assessor, System Administrator, Director, Financial Clerk")]
	public class TrainingProgramController : BaseController
	{
		private readonly IStaticDataService _staticDataService;
		private readonly ITrainingProgramService _trainingProgramService;
		private readonly ICipsCodesService _cipsCodesService;
		private readonly IAttachmentService _attachmentService;

		/// <summary>
		/// Create a new instance of a TrainingProgramController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="trainingProgramService"></param>
		/// <param name="cipsCodesService"></param>
		/// <param name="attachmentService"></param>
		public TrainingProgramController(
			IControllerService controllerService,
			ITrainingProgramService trainingProgramService,
			ICipsCodesService cipsCodesService,
			IAttachmentService attachmentService
		   ) : base(controllerService.Logger)
		{
			_staticDataService = controllerService.StaticDataService;
			_trainingProgramService = trainingProgramService;
			_cipsCodesService = cipsCodesService;
			_attachmentService = attachmentService;

		}

		/// <summary>
		/// Get the training program detail for the specified 'trainingProgramId'.
		/// </summary>
		/// <param name="trainingProgramId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Training/Program/{trainingProgramId}")]
		public JsonResult GetTrainingProgram(int trainingProgramId)
		{
			var model = new TrainingProgramViewModel();

			try
			{
				var trainingProgram = _trainingProgramService.Get(trainingProgramId);
				model = new TrainingProgramViewModel(trainingProgram, _cipsCodesService);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get the specified training program extra information. Mostly HTMl content we need to display, but not edit/post.
		/// </summary>
		/// <param name="trainingProgramId"></param>
		/// <returns></returns>
		[HttpGet, Route("Application/Training/Program/ExtraInfo/{trainingProgramId}")]
		public JsonResult GetTrainingProgramExtraInfo(int trainingProgramId)
		{
			var model = new TrainingProgramExtraInfoViewModel();
			try
			{
				var trainingProgram = _trainingProgramService.Get(trainingProgramId);
				model = new TrainingProgramExtraInfoViewModel(trainingProgram);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Gets the child CIPS record
		/// </summary>
		/// <param name="level"></param>
		/// <param name="parentId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Training/Program/CipsCodes/{level}/{parentId?}")]
		public JsonResult GetCipsCode(int level, int? parentId)
		{
			try
			{
				var model = _cipsCodesService.GetCipsCodeChildren(parentId ?? 0, level).Select(n => new KeyValuePair<int, string>(n.Id, $"{n.Code} | {n.Description}")).ToArray();
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
		/// Update the specified training program in the datasource.
		/// </summary>
		/// <param name="program"></param>
		/// <param name="files"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Application/Training/Program")]
		public JsonResult UpdateTrainingProgram(string program, HttpPostedFileBase[] files)
		{
			var model = new TrainingProgramViewModel();

			try
			{
				model = Newtonsoft.Json.JsonConvert.DeserializeObject<TrainingProgramViewModel>(program);
				TryValidateModel(model);

				if (ModelState.IsValid)
				{
					var trainingProgram = _trainingProgramService.Get(model.Id);
					model.MapTo(trainingProgram, _staticDataService);

					if (files != null && files.Any())
					{
						if (model.CourseOutlineDocument != null && files.Any())
						{
							var attachment = files.First().UploadFile(model.CourseOutlineDocument.Description, model.CourseOutlineDocument.FileName);
							attachment.Id = model.CourseOutlineDocument.Id;
							if (model.CourseOutlineDocument.Id == 0)
							{
								trainingProgram.CourseOutlineDocument = attachment;
								_attachmentService.Add(attachment);
							}
							else
							{
								attachment.RowVersion = Convert.FromBase64String(model.CourseOutlineDocument.RowVersion);
								_attachmentService.Update(attachment);
							}
						}
					}

					_trainingProgramService.Update(trainingProgram);

					model = new TrainingProgramViewModel(trainingProgram, _cipsCodesService);
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

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Download the specified attachment.
		/// </summary>
		/// <param name="trainingProgramId"></param>
		/// <param name="attachmentId"></param>
		/// <returns></returns>
		[HttpGet, Route("Application/Training/Program/{trainingProgramId}/Download/Attachment/{attachmentId}")]
		public ActionResult DownloadAttachment(int trainingProgramId, int attachmentId)
		{
			var model = new BaseViewModel();
			try
			{
				var trainingProgram = _trainingProgramService.Get(trainingProgramId);
				var attachment = _attachmentService.Get(attachmentId);

				var files = new List<int>();
				if (trainingProgram.CourseOutlineDocumentId.HasValue)
					files.Add(trainingProgram.CourseOutlineDocumentId.Value);

				if (!files.Contains(attachment.Id))
					throw new InvalidOperationException($"AttachmentId {attachmentId} is not valid for Training Program {trainingProgramId}");

				return File(attachment.AttachmentData, MediaTypeNames.Application.Octet, $"{attachment.FileName}{attachment.FileExtension}");
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}
	}
}
