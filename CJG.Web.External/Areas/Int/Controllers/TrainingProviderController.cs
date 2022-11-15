using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Ext.Models.TrainingProviders;
using CJG.Web.External.Areas.Int.Models.TrainingProviders;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Int.Controllers
{
	/// <summary>
	/// TrainingProviderController class, controller provides API endpoints to manage training providers.
	/// </summary>
	[RouteArea("Int")]
	[Authorize(Roles = "Assessor, System Administrator, Director, Financial Clerk")]
	public class TrainingProviderController : BaseController
	{
		#region Variables
		private readonly IStaticDataService _staticDataService;
		private readonly IAttachmentService _attachmentService;
		private readonly ITrainingProviderService _trainingProviderService;
		private readonly ITrainingProviderInventoryService _trainingProviderInventoryService;
		private readonly IApplicationAddressService _applicationAddressService;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a TrainingProviderController object, and initializes it with the specified services.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="attachmentService"></param>
		/// <param name="trainingProviderService"></param>
		/// <param name="trainingProviderInventoryService"></param>
		/// <param name="applicationAddressService"></param>
		public TrainingProviderController(
			IControllerService controllerService,
			IAttachmentService attachmentService,
			ITrainingProviderService trainingProviderService,
			ITrainingProviderInventoryService trainingProviderInventoryService,
			IApplicationAddressService applicationAddressService) : base(controllerService.Logger)
		{
			_staticDataService = controllerService.StaticDataService;
			_attachmentService = attachmentService;
			_trainingProviderService = trainingProviderService;
			_trainingProviderInventoryService = trainingProviderInventoryService;
			_applicationAddressService = applicationAddressService;
		}
		#endregion

		#region Endpoints
		/// <summary>
		/// Get the specified training provider.
		/// </summary>
		/// <param name="trainingProviderId"></param>
		/// <returns></returns>
		[HttpGet, Route("Application/Training/Provider/{trainingProviderId:int}")]
		public JsonResult GetTrainingProvider(int trainingProviderId)
		{
			var model = new Models.TrainingProviders.TrainingProviderViewModel();
			try
			{
				var trainingProvider = _trainingProviderService.Get(trainingProviderId);
				var grantApplication = trainingProvider.GetGrantApplication();

				model = new Models.TrainingProviders.TrainingProviderViewModel(trainingProvider, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get the specified training provider extra information. Mostly HTMl content we need to display, but not edit/post.
		/// </summary>
		/// <param name="trainingProviderId"></param>
		/// <returns></returns>
		[HttpGet, Route("Application/Training/Provider/ExtraInfo/{trainingProviderId:int}")]
		public JsonResult GetTrainingProviderExtraInfo(int trainingProviderId)
		{
			var model = new TrainingProviderExtraInfoViewModel();
			try
			{
				var trainingProvider = _trainingProviderService.Get(trainingProviderId);
				var grantApplication = trainingProvider.GetGrantApplication();

				model = new TrainingProviderExtraInfoViewModel(trainingProvider, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Update the specified training provider in the datasource.
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="files"></param>
		/// <returns></returns>
		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut]
		[Route("Application/Training/Provider")]
		public JsonResult UpdateTrainingProvider(string provider, HttpPostedFileBase[] files)
		{
			var viewModel = new Models.TrainingProviders.TrainingProviderViewModel();
			try
			{
				var model = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateTrainingProviderViewModel>(provider);
				var trainingProvider = _trainingProviderService.Get(model.Id);

				if (model.TrainingLocationListViewModel != null)
				{
					model.TrainingLocationListViewModel.IsCanadianAddress = model.TrainingLocationListViewModel.CountryId == Core.Entities.Constants.CanadaCountryId;
					TryValidateModel(model.TrainingLocationListViewModel, "TrainingLocationListViewModel");
				}

				if (model.TrainingProviderLocationListViewModel != null)
				{
					model.TrainingProviderLocationListViewModel.IsCanadianAddress = model.TrainingProviderLocationListViewModel.CountryId == Core.Entities.Constants.CanadaCountryId;
					TryValidateModel(model.TrainingProviderLocationListViewModel, "TrainingProviderLocationListViewModel");// TODO: I don't think this works.
				}

				TryValidateModel(model.TrainingOutsideBcListViewModel, "TrainingOutsideBcListViewModel"); // TODO: I don't think this works.
				TryValidateModel(model.TrainingTrainerDetailsListViewModel, "TrainingTrainerDetailsListViewModel"); // TODO: I don't think this works.
				TryValidateModel(model);

				if (model.SelectedDeliveryMethodIds.Contains(Core.Entities.Constants.Delivery_Classroom)
					|| model.SelectedDeliveryMethodIds.Contains(Core.Entities.Constants.Delivery_Workplace))
				{
					if (model.TrainingLocationListViewModel == null)
					{
						ModelState.AddModelError(nameof(model.TrainingLocationListViewModel.AddressLine1), "Address1 is required");
						ModelState.AddModelError(nameof(model.TrainingLocationListViewModel.City), "City is required");
						ModelState.AddModelError(nameof(model.TrainingLocationListViewModel.PostalCode), "PostalCode is required");
						ModelState.AddModelError(nameof(model.TrainingLocationListViewModel.Country), "Country is required");
						ModelState.AddModelError(nameof(model.TrainingLocationListViewModel.RegionId), "Province is required");
					}
				}

				if (ModelState.IsValid)
				{
					Utilities.MapProperties(model, trainingProvider);
					trainingProvider.TrainingProviderTypeId = model.TrainingProviderType.Id;
					trainingProvider.ContactEmail = model.TrainingTrainerDetailsListViewModel.ContactEmail;
					trainingProvider.ContactFirstName = model.TrainingTrainerDetailsListViewModel.ContactFirstName;
					trainingProvider.ContactLastName = model.TrainingTrainerDetailsListViewModel.ContactLastName;
					trainingProvider.ContactPhoneExtension = model.TrainingTrainerDetailsListViewModel.ContactNumberExtension;
					trainingProvider.ContactPhoneNumber = $"({model.TrainingTrainerDetailsListViewModel.ContactNumberAreaCode}) {model.TrainingTrainerDetailsListViewModel.ContactNumberExchange}-{model.TrainingTrainerDetailsListViewModel.ContactNumberNumber}";
                    if (model.TrainingProviderLocationListViewModel != null)
                        model.TrainingProviderLocationListViewModel.MapToApplicationAddress(trainingProvider.TrainingProviderAddress, _staticDataService, _applicationAddressService);

					if (model.SelectedDeliveryMethodIds.Contains(Core.Entities.Constants.Delivery_Classroom)
						|| model.SelectedDeliveryMethodIds.Contains(Core.Entities.Constants.Delivery_Workplace))
					{
						if (trainingProvider.TrainingAddress == null)
						{
							trainingProvider.TrainingAddress = new ApplicationAddress();
							model.TrainingLocationListViewModel.MapToApplicationAddress(trainingProvider.TrainingAddress, _staticDataService, _applicationAddressService);
						}
						else
						{
							model.TrainingLocationListViewModel.MapToApplicationAddress(trainingProvider.TrainingAddress, _staticDataService, _applicationAddressService);
						}
					}
					trainingProvider.TrainingOutsideBC = model.TrainingOutsideBcListViewModel.TrainingOutsideBC.Value;

					if (files != null && files.Any())
					{
						if (model.CourseOutlineDocument != null && model.CourseOutlineDocument.Index.HasValue && files.Count() > model.CourseOutlineDocument.Index)
						{
							var attachment = files[model.CourseOutlineDocument.Index.Value].UploadFile(model.CourseOutlineDocument.Description, model.CourseOutlineDocument.FileName);
							attachment.Id = model.CourseOutlineDocument.Id;
							if (model.CourseOutlineDocument.Id == 0)
							{
								trainingProvider.CourseOutlineDocument = attachment;
								_attachmentService.Add(attachment);
							}
							else
							{
								attachment.RowVersion = Convert.FromBase64String(model.CourseOutlineDocument.RowVersion);
								_attachmentService.Update(attachment);
							}
						}
						if (model.ProofOfQualificationsDocument != null && model.ProofOfQualificationsDocument.Index.HasValue && files.Count() > model.ProofOfQualificationsDocument.Index)
						{
							var attachment = files[model.ProofOfQualificationsDocument.Index.Value].UploadFile(model.ProofOfQualificationsDocument.Description, model.ProofOfQualificationsDocument.FileName);
							attachment.Id = model.ProofOfQualificationsDocument.Id;
							if (model.ProofOfQualificationsDocument.Id == 0)
							{
								trainingProvider.ProofOfQualificationsDocument = attachment;
								_attachmentService.Add(attachment);
							}
							else
							{
								attachment.RowVersion = Convert.FromBase64String(model.ProofOfQualificationsDocument.RowVersion);
								_attachmentService.Update(attachment);
							}
						}
						if (model.BusinessCaseDocument != null && model.BusinessCaseDocument.Index.HasValue && files.Count() > model.BusinessCaseDocument.Index)
						{
							var attachment = files[model.BusinessCaseDocument.Index.Value].UploadFile(model.BusinessCaseDocument.Description, model.BusinessCaseDocument.FileName);
							attachment.Id = model.BusinessCaseDocument.Id;
							if (model.BusinessCaseDocument.Id == 0)
							{
								trainingProvider.BusinessCaseDocument = attachment;
								_attachmentService.Add(attachment);
							}
							else
							{
								attachment.RowVersion = Convert.FromBase64String(model.BusinessCaseDocument.RowVersion);
								_attachmentService.Update(attachment);
							}
						}
					}

					_trainingProviderService.Update(trainingProvider);

					viewModel = new Models.TrainingProviders.TrainingProviderViewModel(trainingProvider, User);
				}
				else
				{
					HandleModelStateValidation(viewModel, ModelState.GetErrorMessages("<br />"));
					//HandleModelStateValidation(viewModel);
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}

			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Download the specified attachment.
		/// </summary>
		/// <param name="trainingProviderId"></param>
		/// <param name="attachmentId"></param>
		/// <returns></returns>
		[HttpGet, Route("Application/Training/Provider/{trainingProviderId}/Download/Attachment/{attachmentId}")]
		public ActionResult DownloadAttachment(int trainingProviderId, int attachmentId)
		{
			var model = new BaseViewModel();
			try
			{
				var trainingProvider = _trainingProviderService.Get(trainingProviderId);
				var attachment = _attachmentService.Get(attachmentId);

				var files = new List<int>();
				if (trainingProvider.CourseOutlineDocumentId.HasValue) files.Add(trainingProvider.CourseOutlineDocumentId.Value);
				if (trainingProvider.ProofOfQualificationsDocumentId.HasValue) files.Add(trainingProvider.ProofOfQualificationsDocumentId.Value);
				if (trainingProvider.BusinessCaseDocumentId.HasValue) files.Add(trainingProvider.BusinessCaseDocumentId.Value);

				if (!files.Contains(attachment.Id)) throw new InvalidOperationException($"AttachmentId {attachmentId} is not valid for Training Provider {trainingProviderId}");

				return File(attachment.AttachmentData, MediaTypeNames.Application.Octet, $"{attachment.FileName}{attachment.FileExtension}");
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get training providers from inventory.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="quantity"></param>
		/// <param name="search"></param>
		/// <returns></returns>
		[HttpGet, Route("Application/Training/Provider/Inventory/{page}/{quantity?}")]
		public JsonResult GetInventory(int page, int quantity, string search)
		{
			var model = new BaseViewModel();
			try
			{
				var inventory = _trainingProviderInventoryService.GetInventory(page, quantity, search, true);
				var result = new
				{
					Draw = 1,
					RecordsFiltered = inventory.Items.Count(),
					RecordsTotal = inventory.Total,
					Data = inventory.Items.Select(o => new TrainingProviderInventoryViewModel(o)).ToArray()
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
		/// Returns a view of training provider inventory.
		/// </summary>
		/// <param name="trainingProviderId"></param>
		/// <returns></returns>
		[HttpGet, Route("Application/Training/Provider/Validate/View/{trainingProviderId}")]
		public ActionResult GetValidationView(int trainingProviderId)
		{
			var provider = _trainingProviderService.Get(trainingProviderId);

			var model = new TrainingProviderInventoryViewModel(provider);

			return PartialView("_SelectTrainingProvider", model);
		}

		/// <summary>
		/// Validate the specified training provider with the one from the inventory of validated training providers.
		/// </summary>
		/// <param name="trainingProviderId"></param>
		/// <param name="trainingProviderInventoryId"></param>
		/// <param name="rowVersion"></param>
		/// <returns></returns>
		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPut]
		[Route("Application/Training/Provider/Validate")]
		public JsonResult Validate(int trainingProviderId, int trainingProviderInventoryId, string rowVersion)
		{
			var model = new Models.TrainingProviders.TrainingProviderViewModel();
			try
			{
				var provider = _trainingProviderService.Get(trainingProviderId);
				var grantApplication = provider.GetGrantApplication();
				provider.RowVersion = Convert.FromBase64String(rowVersion.Replace(" ", "+"));

				var trainingProvider = _trainingProviderService.ValidateTrainingProvider(provider, trainingProviderInventoryId);

				model = new Models.TrainingProviders.TrainingProviderViewModel(trainingProvider, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Add the training provider name to the inventory of validated training providers.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="isActive"></param>
		/// <param name="isEligible"></param>
		/// <param name="notes"></param>
		/// <returns></returns>
		[PreventSpam]
		[ValidateRequestHeader]
		[HttpPost, Route("Application/Training/Provider/Add/To/Inventory")]
		public JsonResult AddTrainingProviderToInventory(string name, bool? isActive, bool? isEligible, string notes)
		{
			var model = new BaseViewModel();
			try
			{
				if (name == null) { name = ""; }
				if (notes == null) { notes = ""; }
				if (isActive == null) { isActive = true; }
				if (isEligible == null) { isEligible = true; }


				var existing = _trainingProviderInventoryService.GetTrainingProviderFromInventory(name);
				if (existing != null) throw new InvalidOperationException($"The Training Provider name ({name}) already exists.");

				var inventory = new TrainingProviderInventory(name, isEligible.Value, isActive.Value)
				{
					Notes = string.IsNullOrWhiteSpace(notes) ? null : Regex.Replace(notes.Trim(), "([^\r])\n", "$1\r\n")
				};

				_trainingProviderInventoryService.Add(inventory);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region Dropdowns
		/// <summary>
		/// Get an array of training provider types.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Application/Training/Provider/Types/Details")]
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
	}
}
