using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Ext.Models.TrainingProviders;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;
using Newtonsoft.Json;
using Constants = CJG.Core.Entities.Constants;

namespace CJG.Web.External.Areas.Ext.Controllers
{
    /// <summary>
    /// TrainingProviderController class, controller provides a way CRUD API for training providers.
    /// </summary>
    [RouteArea("Ext")]
	[ExternalFilter]
	public class TrainingProviderController : BaseController
	{
		private readonly IStaticDataService _staticDataService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly ITrainingProgramService _trainingProgramService;
		private readonly ITrainingProviderService _trainingProviderService;
		private readonly IAttachmentService _attachmentService;
		private readonly IApplicationAddressService _applicationAddressService;

		/// <summary>
		/// Creates a new instance of a TrainingProviderController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="staticDataService"></param>
		/// <param name="grantApplicationService"></param>
		/// <param name="trainingProgramService"></param>
		/// <param name="trainingProviderService"></param>
		/// <param name="attachmentService"></param>
		/// <param name="applicationAddressService"></param>
		public TrainingProviderController(
			IControllerService controllerService,
			IStaticDataService staticDataService,
			IGrantApplicationService grantApplicationService,
			ITrainingProgramService trainingProgramService,
			ITrainingProviderService trainingProviderService,
			IAttachmentService attachmentService,
			IApplicationAddressService applicationAddressService
			) : base(controllerService.Logger)
		{
			_staticDataService = staticDataService;
			_grantApplicationService = grantApplicationService;
			_trainingProgramService = trainingProgramService;
			_trainingProviderService = trainingProviderService;
			_attachmentService = attachmentService;
			_applicationAddressService = applicationAddressService;
		}

		#region Endpoints
		/// <summary>
		/// Return a view for training provider.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="trainingProviderId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Training/Provider/View/{grantApplicationId:int}/{trainingProviderId:int}")]
		public ActionResult TrainingProviderView(int grantApplicationId, int trainingProviderId)
		{
			ViewBag.GrantApplicationId = grantApplicationId;
			ViewBag.TrainingProviderId = trainingProviderId;
			return View();
		}

		/// <summary>
		/// Get the data for the training provider.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="trainingProviderId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Training/Provider/{grantApplicationId:int}/{trainingProviderId:int}")]
		public JsonResult GetTrainingProvider(int grantApplicationId, int trainingProviderId)
		{
			var viewModel = new TrainingProviderViewModel();
			try
			{
				viewModel.GrantApplicationId = grantApplicationId;
				var grantApplication = _grantApplicationService.Get(grantApplicationId);

				TrainingProvider trainingProvider;

				if (trainingProviderId == 0)
				{
					if (grantApplication.TrainingPrograms.Any())
					{
						trainingProvider = new TrainingProvider();
						trainingProvider.TrainingPrograms.Add(grantApplication.TrainingPrograms.First());
						trainingProvider.GrantApplicationId = grantApplication.Id;
					}
					else
					{
						trainingProvider = new TrainingProvider(grantApplication);
					}
				}
				else
				{
					trainingProvider = _trainingProviderService.Get(trainingProviderId);
				}
				viewModel = new TrainingProviderViewModel(trainingProvider);
				viewModel.AlternativeTrainingOptions = trainingProvider.AlternativeTrainingOptions;
				viewModel.ChoiceOfTrainerOrProgram = trainingProvider.ChoiceOfTrainerOrProgram;

				// Convert existing Training Provider Types that have been deactivated back to null to make the user reselect.
				if (trainingProvider.TrainingProviderType != null && !trainingProvider.TrainingProviderType.IsActive)
					viewModel.TrainingProviderTypeId = null;
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}
			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Add a new training provider to the datasource.
		/// </summary>
		/// <param name="files"></param>
		/// <param name="component"></param>
		/// <returns></returns>
		[HttpPost]
		[PreventSpam]
		[ValidateRequestHeader]
		[ValidateInput(false)]   // We're expecting HTML, so this needs to be here.
		[Route("Training/Provider")]
		public JsonResult AddTrainingProvider(HttpPostedFileBase[] files, string component)
		{
			var model = new TrainingProviderViewModel();

			try
			{
				model = JsonConvert.DeserializeObject<TrainingProviderViewModel>(component);

				ModelState.Clear();
				TryUpdateModel(model);

				AdjustModelStateErrors(model);

				//if (model.TrainingProviderTypeId.HasValue)
				//{
				//	var trainingProviderType = _trainingProviderService.Get<TrainingProviderType>(model.TrainingProviderTypeId);

				//	if (trainingProviderType.ProofOfInstructorQualifications == 1)
				//	{
				//		if (String.IsNullOrWhiteSpace(model.ProofOfQualificationsDocument.FileName))
				//		{
				//			ModelState.AddModelError("ProofOfQualificationsDocument", "Proof of qualifications document required.");
				//		}
				//	}
				//	if (trainingProviderType.CourseOutline == 1)
				//	{
				//		if (String.IsNullOrWhiteSpace(model.CourseOutlineDocument.FileName))
				//		{
				//			ModelState.AddModelError("CourseOutlineDocument", "Course outline document required.");
				//		}
				//	}
				//}

				if (ModelState.IsValid)
				{
					var trainingProvider = model.MapProperties(_grantApplicationService, _trainingProgramService, _trainingProviderService, _attachmentService, _applicationAddressService, _staticDataService, User, files);
					_trainingProviderService.Add(trainingProvider);
					model = new TrainingProviderViewModel(trainingProvider);
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
		/// Update the training provider on the datasource.
		/// </summary>
		/// <param name="files"></param>
		/// <param name="component"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[ValidateInput(false)]   // We're expecting HTML, so this needs to be here.
		[Route("Training/Provider")]
		public JsonResult UpdateTrainingProvider(HttpPostedFileBase[] files, string component)
		{
			var model = new TrainingProviderViewModel();

			try
			{
				model = JsonConvert.DeserializeObject<TrainingProviderViewModel>(component);

				ModelState.Clear();
				TryUpdateModel(model);

				AdjustModelStateErrors(model);

				//if (model.TrainingProviderTypeId.HasValue)
				//{
				//	var trainingProviderType = _trainingProviderService.Get<TrainingProviderType>(model.TrainingProviderTypeId);
                    
    //                if (trainingProviderType.ProofOfInstructorQualifications == 1)
    //                {
    //                    if (String.IsNullOrWhiteSpace(model.ProofOfQualificationsDocument.FileName))
    //                    {
    //                        ModelState.AddModelError("ProofOfQualificationsDocument", "Proof of qualifications document required.");
    //                    }
    //                }
    //                if (trainingProviderType.CourseOutline == 1)
    //                {
    //                    if (String.IsNullOrWhiteSpace(model.CourseOutlineDocument.FileName))
    //                    {
    //                        ModelState.AddModelError("CourseOutlineDocument", "Course outline document required.");
    //                    }
    //                }
    //            }

				if (ModelState.IsValid)
				{
					var trainingProvider = model.MapProperties(_grantApplicationService, _trainingProgramService, _trainingProviderService, _attachmentService, _applicationAddressService, _staticDataService, User, files);
					_trainingProviderService.Update(trainingProvider);
					model = new TrainingProviderViewModel(trainingProvider);
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

		private void AdjustModelStateErrors(TrainingProviderViewModel model)
		{
			if (model.SelectedDeliveryMethodIds == null)
			{
				ModelState.Remove(nameof(TrainingProviderViewModel.AddressLine1));
				ModelState.Remove(nameof(TrainingProviderViewModel.City));
				ModelState.Remove(nameof(TrainingProviderViewModel.PostalCode));
			}

			else if (model.SelectedDeliveryMethodIds.Contains(Constants.Delivery_Online)
			         && !model.SelectedDeliveryMethodIds.Contains(Constants.Delivery_Classroom)
			         && !model.SelectedDeliveryMethodIds.Contains(Constants.Delivery_Workplace))
			{
				ModelState.Remove(nameof(TrainingProviderViewModel.AddressLine1));
				ModelState.Remove(nameof(TrainingProviderViewModel.City));
				ModelState.Remove(nameof(TrainingProviderViewModel.PostalCode));
			}

			if (model.IsCanadianAddress)
			{
				ModelState.Remove(nameof(TrainingProviderViewModel.OtherZipCode));
				ModelState.Remove(nameof(TrainingProviderViewModel.OtherRegion));
			}
			else
			{
				ModelState.Remove(nameof(TrainingProviderViewModel.PostalCode));
				ModelState.Remove(nameof(TrainingProviderViewModel.RegionId));
			}

			if (model.IsCanadianAddressTrainingProvider)
			{
				ModelState.Remove(nameof(TrainingProviderViewModel.OtherZipCodeTrainingProvider));
				ModelState.Remove(nameof(TrainingProviderViewModel.OtherRegionTrainingProvider));
			}
			else
			{
				ModelState.Remove(nameof(TrainingProviderViewModel.PostalCodeTrainingProvider));
				ModelState.Remove(nameof(TrainingProviderViewModel.RegionIdTrainingProvider));
			}

			if (model.RegionIdTrainingProvider.ToLower() == "bc")
			{
				ModelState.Remove(nameof(TrainingProviderViewModel.OutOfProvinceLocationRationale));
			}
		}

		/// <summary>
		/// Delete the training provider from the datasource.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="rowVersion"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Training/Provider/Delete")]
		public JsonResult DeleteTrainingProvider(int id, string rowVersion)
		{
			var model = new BaseViewModel();
			try
			{
				var trainingProvider = _trainingProviderService.Get(id);
				trainingProvider.RowVersion = Convert.FromBase64String(rowVersion.Replace("+", " "));
				_trainingProviderService.Delete(trainingProvider);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		#region Dropdowns
		/// <summary>
		/// Get an array of training provider types.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Training/Provider/Types")]
		public JsonResult GetTrainingProviderTypes()
		{
			var types = new List<TrainingProviderTypeViewModel>();
			try
			{
				types = _staticDataService.GetTrainingProviderTypes().Select(x => new TrainingProviderTypeViewModel(x)).ToList();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(types, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region Attachment
		/// <summary>
		/// Download the specified document.
		/// </summary>
		/// <param name="trainingProviderId"></param>
		/// <param name="attachmentId"></param>
		/// <returns></returns>
		[HttpGet]
		[PreventSpam]
		[Route("Training/Provider/{trainingProviderId:int}/Attachment/Download/{attachmentId:int}")]
		public ActionResult DownloadAttachment(int trainingProviderId, int attachmentId)
		{
			var trainingProvider = _trainingProviderService.Get(trainingProviderId);
			var attachment = _attachmentService.Get(attachmentId);
			if (trainingProvider.BusinessCaseDocumentId != attachmentId
				&& trainingProvider.CourseOutlineDocumentId != attachmentId
				&& trainingProvider.ProofOfQualificationsDocumentId != attachmentId) throw new NotAuthorizedException("User does not have access to document.");

			return File(attachment.AttachmentData, MediaTypeNames.Application.Octet, $"{attachment.FileName}{attachment.FileExtension}");
		}
		#endregion
		#endregion
	}
}