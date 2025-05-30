﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Ext.Models.TrainingPrograms;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;
using Newtonsoft.Json;

namespace CJG.Web.External.Areas.Ext.Controllers
{
    /// <summary>
    /// TrainingProgramController class, provides endpoints to manage training programs.
    /// </summary>
    [RouteArea("Ext")]
	[ExternalFilter]
	public class TrainingProgramController : BaseController
	{
		private readonly IStaticDataService _staticDataService;
		private readonly IAttachmentService _attachmentService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly ITrainingProgramService _trainingProgramService;
		private readonly ITrainingProviderService _trainingProviderService;

		/// <summary>
		/// Creates a new instance of a TrainingProgramController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantApplicationService"></param>
		/// <param name="trainingProgramService"></param>
		/// <param name="trainingProviderService"></param>
		/// <param name="attachmentService"></param>
		public TrainingProgramController(IControllerService controllerService,
										 IGrantApplicationService grantApplicationService,
										 ITrainingProgramService trainingProgramService,
										 ITrainingProviderService trainingProviderService,
										 IAttachmentService attachmentService) : base(controllerService.Logger)
		{
			_staticDataService = controllerService.StaticDataService;
			_grantApplicationService = grantApplicationService;
			_trainingProgramService = trainingProgramService;
			_trainingProviderService = trainingProviderService;
			_attachmentService = attachmentService;
		}

		/// <summary>
		/// Returns a view for adding/updating a training program for ETG grant programs.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="trainingProgramId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Training/Program/View/{grantApplicationId:int}/{trainingProgramId:int}")]
		public ActionResult TrainingProgramView(int grantApplicationId, int trainingProgramId)
		{
			_grantApplicationService.Get(grantApplicationId);
			ViewBag.GrantApplicationId = grantApplicationId;
			ViewBag.TrainingProgramId = trainingProgramId;

			return View();
		}

		/// <summary>
		/// Get the data for the training program view.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="trainingProgramId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Training/Program/{grantApplicationId:int}/{trainingProgramId:int}")]
		public JsonResult GetTrainingProgram(int grantApplicationId, int trainingProgramId)
		{
			var model = new TrainingProgramViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				var trainingProgram = trainingProgramId == 0
					? new TrainingProgram(grantApplication)
					: _trainingProgramService.Get(trainingProgramId);

				model = new TrainingProgramViewModel(trainingProgram);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get the application attachments view data.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Training/Program/Attachment/{id:int}")]
		public JsonResult GetApplicationAttachments(int id)
		{
			var viewModel = new GrantApplicationAttachmentDetailViewModel();

			try
			{
				var grantApplication = _grantApplicationService.Get(id);
				var attachments = _grantApplicationService.GetAttachments(id);
				viewModel = new GrantApplicationAttachmentDetailViewModel(grantApplication, attachments);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}
			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Add the specified training program to the datasource.
		/// </summary>
		/// <param name="files"></param>
		/// <param name="component"></param>
		/// <returns></returns>
		[HttpPost]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Training/Program")]
		[ValidateInput(false)]   // We're expecting HTML, so this needs to be here.
		public JsonResult AddTrainingProgram(HttpPostedFileBase[] files, string component)
		{
			var model = new TrainingProgramViewModel();
			try
			{
				model = JsonConvert.DeserializeObject<TrainingProgramViewModel>(component);

				model.StartDate = model.StartDate.Value.ToUtcMorning();
				model.EndDate = model.EndDate.Value.ToUtcMidnight();

				ModelState.Clear();
				TryUpdateModel(model);

				//model.StartDate = new DateTime(model.StartYear, model.StartMonth, model.StartDay, 0, 0, 0, DateTimeKind.Local).ToUtcMorning();
				//model.EndDate = new DateTime(model.EndYear, model.EndMonth, model.EndDay, 0, 0, 0, DateTimeKind.Local).ToUtcMidnight();

				var anyFilesUploaded = files != null && files.Length > 0;
				var existingFileUploaded = model.CourseOutlineDocument.Id > 0;

				if (!existingFileUploaded && !anyFilesUploaded)
					ModelState.AddModelError("CourseOutlineDocument", "You must provide a course outline document.");

				if (!(new int[] { Core.Entities.Constants.SkillsFocus_ApprenticeshipTraining }.Contains(model.SkillFocusId.GetValueOrDefault())))
				{
					ModelState.Remove("InDemandOccupationId");
					ModelState.Remove("TrainingLevelId");
					ModelState.Remove("MemberOfUnderRepresentedGroup");
					ModelState.Remove("SelectedUnderRepresentedGroupIds");
				}

				if (new int[] { Core.Entities.Constants.ExpectedQualifications_None }.Contains(model.ExpectedQualificationId.GetValueOrDefault()) || model.ExpectedQualificationId == null)
					ModelState.Remove("TitleOfQualification");

				if (!model.HasRequestedAdditionalFunding ?? true)
					ModelState.Remove("DescriptionOfFundingRequested");

				if (!model.MemberOfUnderRepresentedGroup.HasValue || model.MemberOfUnderRepresentedGroup == false)
					ModelState.Remove("SelectedUnderRepresentedGroupIds");

				if (ModelState.IsValid)
				{
					var trainingProgram = model.UpdateTrainingProgram(_grantApplicationService, _trainingProgramService, _trainingProviderService, _staticDataService, _attachmentService, User, files);
					model = new TrainingProgramViewModel(trainingProgram);
				}
				else
				{
					HandleModelStateValidation(model);
				}
			}
			catch(Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		/// <summary>
		/// Update the specified training program in the datasource.
		/// </summary>
		/// <param name="files"></param>
		/// <param name="component"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Training/Program")]
		[ValidateInput(false)]   // We're expecting HTML, so this needs to be here.
		public JsonResult UpdateTrainingProgram(HttpPostedFileBase[] files, string component)
		{
			var model = new TrainingProgramViewModel();

			try
			{
				model = JsonConvert.DeserializeObject<TrainingProgramViewModel>(component);

				model.StartDate = model.StartDate.Value.ToUtcMorning();
				model.EndDate = model.EndDate.Value.ToUtcMidnight();

				ModelState.Clear();
				TryUpdateModel(model);

				var anyFilesUploaded = files != null && files.Length > 0;
				var existingFileUploaded = model.CourseOutlineDocument.Id > 0;

				if (!existingFileUploaded && !anyFilesUploaded)
					ModelState.AddModelError("CourseOutlineDocument", "You must provide a course outline document.");

				if (!(new int[] { Core.Entities.Constants.SkillsFocus_ApprenticeshipTraining }.Contains(model.SkillFocusId.GetValueOrDefault())))
				{
					ModelState.Remove("InDemandOccupationId");
					ModelState.Remove("TrainingLevelId");
					ModelState.Remove("MemberOfUnderRepresentedGroup");
					ModelState.Remove("SelectedUnderRepresentedGroupIds");
				}

				if (new int[] { Core.Entities.Constants.ExpectedQualifications_None }.Contains(model.ExpectedQualificationId.GetValueOrDefault()) || model.ExpectedQualificationId == null)
					ModelState.Remove("TitleOfQualification");

				if (!model.HasRequestedAdditionalFunding ?? true)
					ModelState.Remove("DescriptionOfFundingRequested");

				if (!model.MemberOfUnderRepresentedGroup.HasValue || model.MemberOfUnderRepresentedGroup == false)
					ModelState.Remove("SelectedUnderRepresentedGroupIds");

				if (ModelState.IsValid)
				{
					var trainingProgram = model.UpdateTrainingProgram(_grantApplicationService, _trainingProgramService, _trainingProviderService, _staticDataService, _attachmentService, User, files);
					model = new TrainingProgramViewModel(trainingProgram);
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
		/// Delete the training program from the datasource.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="rowVersion"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Training/Program/Delete")]
		public JsonResult DeleteTrainingProgram(int id, string rowVersion) // TODO: Don't pass rowVersion on the query string.
		{
			var model = new BaseViewModel();
			try
			{
				var trainingProgram = _trainingProgramService.Get(id);
				trainingProgram.RowVersion = Convert.FromBase64String(rowVersion.Replace("+", " "));
				_trainingProgramService.Delete(trainingProgram);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		/// <summary>
		/// Download the specified document.
		/// </summary>
		/// <param name="trainingProgramId"></param>
		/// <param name="attachmentId"></param>
		/// <returns></returns>
		[HttpGet]
		[PreventSpam]
		[Route("Training/Program/{trainingProgramId:int}/Attachment/Download/{attachmentId:int}")]
		public ActionResult DownloadAttachment(int trainingProgramId, int attachmentId)
		{
			var trainingProgram = _trainingProgramService.Get(trainingProgramId);
			var attachment = _attachmentService.Get(attachmentId);
			if (trainingProgram.CourseOutlineDocumentId != attachmentId)
				throw new NotAuthorizedException("User does not have access to document.");

			return File(attachment.AttachmentData, MediaTypeNames.Application.Octet, $"{attachment.FileName}{attachment.FileExtension}");
		}

		/// <summary>
		/// Get an array of skill levels.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Training/Program/Skill/Levels")]
		public JsonResult GetSkillLevels()
		{
			var results = new List<CollectionItemModel>();
			try
			{
				results = _staticDataService.GetSkillLevels()
					.Select(x => new CollectionItemModel
					{
						Id = x.Id,
						Caption = x.Caption,
						RowSequence = x.RowSequence
					}).ToList();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(results, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get an array of skills focuses.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Training/Program/Skills/Focuses")]
		public JsonResult GetSkillsFocuses()
		{
			var results = new List<CollectionItemModel>();
			try
			{
				results = _staticDataService.GetSkillsFocuses()
					.Select(x => new CollectionItemModel
					{
						Id = x.Id,
						Caption = x.Caption,
						RowSequence = x.RowSequence
					}).ToList();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(results, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get an array of expected qualifications.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Training/Program/Expected/Qualifications")]
		public JsonResult GetExpectedQualifications()
		{
			var results = new List<CollectionItemModel>();
			try
			{
				results = _staticDataService.GetExpectedQualifications()
					.Select(x => new CollectionItemModel
					{
						Id = x.Id,
						Caption = x.Caption,
						RowSequence = x.RowSequence
					}).ToList();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(results, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get an array of in demand occupations.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Training/Program/Indemand/Occupations")]
		public JsonResult GetInDemandOccupations()
		{
			var results = new List<CollectionItemModel>();
			try
			{
				results = _staticDataService.GetInDemandOccupations()
					.Select(x => new CollectionItemModel
					{
						Id = x.Id,
						Caption = x.Caption,
						RowSequence = x.RowSequence
					}).ToList();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(results, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get an array of training levels.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Training/Program/Training/Levels")]
		public JsonResult GetTrainingLevels()
		{
			var results = new List<CollectionItemModel>();
			try
			{
				results = _staticDataService.GetTrainingLevels()
					.Select(x => new CollectionItemModel
					{
						Id = x.Id,
						Caption = x.Caption,
						RowSequence = x.RowSequence
					}).ToList();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(results, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get an array of under-represented groups.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Training/Program/Underrepresented/Groups")]
		public JsonResult GetUnderrepresentedGroups()
		{
			var results = new List<CollectionItemModel>();
			try
			{
				results = _staticDataService.GetUnderRepresentedGroups()
					.Select(x => new CollectionItemModel
					{
						Id = x.Id,
						Caption = x.Caption,
						RowSequence = x.RowSequence
					}).ToList();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(results, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get an array of delivery methods.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Training/Program/Delivery/Methods")]
		public JsonResult GetDeliveryMethods()
		{
			var results = new List<CollectionItemModel>();
			try
			{
				results = _staticDataService.GetDeliveryMethods()
					.Select(x => new CollectionItemModel
					{
						Id = x.Id,
						Caption = x.Caption,
						RowSequence = x.RowSequence
					}).ToList();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(results, JsonRequestBehavior.AllowGet);
		}
	}
}