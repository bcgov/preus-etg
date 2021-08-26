using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Core.Interfaces.Service.Settings;
using CJG.Web.External.Areas.Ext.Models.TrainingProviders;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Ext.Controllers
{
	/// <summary>
	/// TrainingProviderController class, controller provides a way CRUD API for training providers.
	/// </summary>
	[RouteArea("Ext")]
	[ExternalFilter]
	public class TrainingProviderController : BaseController
	{
		#region Variables
		private readonly ITrainingProviderSettings _trainingProviderSettings;
		private readonly IStaticDataService _staticDataService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly ITrainingProgramService _trainingProgramService;
		private readonly ITrainingProviderService _trainingProviderService;
		private readonly IAttachmentService _attachmentService;
		private readonly IApplicationAddressService _applicationAddressService;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a TrainingProviderController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="trainingProviderSettings"></param>
		/// <param name="staticDataService"></param>
		/// <param name="grantApplicationService"></param>
		/// <param name="trainingProgramService"></param>
		/// <param name="trainingProviderService"></param>
		/// <param name="controllerService"></param>
		/// <param name="attachmentService"></param>
		/// <param name="applicationAddressService"></param>
		public TrainingProviderController(
			IControllerService controllerService,
			ITrainingProviderSettings trainingProviderSettings,
			IStaticDataService staticDataService,
			IGrantApplicationService grantApplicationService,
			ITrainingProgramService trainingProgramService,
			ITrainingProviderService trainingProviderService,
			IAttachmentService attachmentService,
			IApplicationAddressService applicationAddressService
			) : base(controllerService.Logger)
		{
			_trainingProviderSettings = trainingProviderSettings;
			_staticDataService = staticDataService;
			_grantApplicationService = grantApplicationService;
			_trainingProgramService = trainingProgramService;
			_trainingProviderService = trainingProviderService;
			_attachmentService = attachmentService;
			_applicationAddressService = applicationAddressService;
		}
		#endregion

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
			var viewModel = new Models.TrainingProviders.TrainingProviderViewModel();
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
				viewModel = new Models.TrainingProviders.TrainingProviderViewModel(trainingProvider);
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
		[Route("Training/Provider")]
		public JsonResult AddTrainingProvider(HttpPostedFileBase[] files, string component)
		{
			var model = new Models.TrainingProviders.TrainingProviderViewModel();

			try
			{
				model = JsonConvert.DeserializeObject<Models.TrainingProviders.TrainingProviderViewModel>(component);

				ModelState.Clear();
				TryUpdateModel(model);

				if (model.IsCanadianAddress)
				{
					ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.OtherZipCode));
					ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.OtherRegion));
				}
				else
				{
					ModelState.Remove(nameof(TrainingProviderViewModel.PostalCode));
					ModelState.Remove(nameof(TrainingProviderViewModel.RegionId));
				}

				if (ModelState.IsValid)
				{
					var trainingProvider = model.MapProperties(_grantApplicationService, _trainingProgramService, _trainingProviderService, _attachmentService, _applicationAddressService, _staticDataService, User, files);
					_trainingProviderService.Add(trainingProvider);
					model = new Models.TrainingProviders.TrainingProviderViewModel(trainingProvider);
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
		[Route("Training/Provider")]
		public JsonResult UpdateTrainingProvider(HttpPostedFileBase[] files, string component)
		{
			var model = new Models.TrainingProviders.TrainingProviderViewModel();

			try
			{
				model = JsonConvert.DeserializeObject<Models.TrainingProviders.TrainingProviderViewModel>(component);

				ModelState.Clear();
				TryUpdateModel(model);

				if (model.IsCanadianAddress)
				{
					ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.OtherZipCode));
					ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.OtherRegion));
				}
				else
				{
					ModelState.Remove(nameof(TrainingProviderViewModel.PostalCode));
					ModelState.Remove(nameof(TrainingProviderViewModel.RegionId));
				}

				if (ModelState.IsValid)
				{
					var trainingProvider = model.MapProperties(_grantApplicationService, _trainingProgramService, _trainingProviderService, _attachmentService, _applicationAddressService, _staticDataService, User, files);
					_trainingProviderService.Update(trainingProvider);
					model = new Models.TrainingProviders.TrainingProviderViewModel(trainingProvider);
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
		/// Delete the training provider from the datasource.
		/// </summary>
		/// <param name="trainingProviderId"></param>
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

			return File(attachment.AttachmentData, System.Net.Mime.MediaTypeNames.Application.Octet, $"{attachment.FileName}{attachment.FileExtension}");
		}
		#endregion
		#endregion
	}
}