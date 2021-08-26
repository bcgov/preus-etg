using CJG.Core.Entities;
using CJG.Core.Interfaces;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Ext.Controllers
{
	/// <summary>
	/// ChangeRequestController class, provides a controller for grant agreement management.
	/// </summary>
	[RouteArea("Ext")]
	[ExternalFilter]
	public class ChangeRequestController : BaseController
	{
		#region Variables
		private readonly IStaticDataService _staticDataService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly INoteService _noteService;
		private readonly IGrantProgramService _grantProgramService;
		private readonly ITrainingProgramService _trainingProgramService;
		private readonly ITrainingProviderService _trainingProviderService;
		private readonly IEligibleExpenseTypeService _eligibleExpenseTypeService;
		private readonly IAttachmentService _attachmentService;
		private readonly IApplicationAddressService _applicationAddressService;
		private readonly IEligibleCostService _eligibleCostService;
		private readonly IGrantAgreementService _grantAgreementService;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a ChangeRequestController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantApplicationService"></param>
		/// <param name="noteService"></param>
		/// <param name="grantProgramService"></param>
		/// <param name="trainingProgramService"></param>
		/// <param name="trainingProviderService"></param>
		/// <param name="eligibleExpenseTypeService"></param>
		/// <param name="attachmentService"></param>
		/// <param name="applicationAddressService"></param>
		/// <param name="eligibleCostService"></param>
		/// <param name="grantAgreementService"></param>
		public ChangeRequestController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			INoteService noteService,
			IGrantProgramService grantProgramService,
			ITrainingProgramService trainingProgramService,
			ITrainingProviderService trainingProviderService,
			IEligibleExpenseTypeService eligibleExpenseTypeService,
			IAttachmentService attachmentService,
			IApplicationAddressService applicationAddressService,
			IEligibleCostService eligibleCostService,
			IGrantAgreementService grantAgreementService
			) : base(controllerService.Logger)
		{
			_staticDataService = controllerService.StaticDataService;
			_grantApplicationService = grantApplicationService;
			_noteService = noteService;
			_grantProgramService = grantProgramService;
			_trainingProgramService = trainingProgramService;
			_trainingProviderService = trainingProviderService;
			_eligibleExpenseTypeService = eligibleExpenseTypeService;
			_attachmentService = attachmentService;
			_applicationAddressService = applicationAddressService;
			_eligibleCostService = eligibleCostService;
			_grantAgreementService = grantAgreementService;
		}
		#endregion

		#region Endpoints
		/// <summary>
		/// Submit the current change request.
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Agreement/Submit/Change/Request")]
		public JsonResult SubmitChangeRequest(Models.Agreements.AgreementOverviewViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.Id);
				grantApplication.RowVersion = Convert.FromBase64String(model.RowVersion);
				_grantApplicationService.SubmitChangeRequest(grantApplication, "");

				model = new Models.Agreements.AgreementOverviewViewModel(grantApplication, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Cancel the current change request and remove any training providers that were being requested.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Agreement/Cancel/Change/Request")]
		public JsonResult CancelChangeRequest(Models.Agreements.AgreementOverviewViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.Id);
				grantApplication.RowVersion = Convert.FromBase64String(model.RowVersion);
				if (grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramTypeId == ProgramTypes.EmployerGrant)
				{
					var entity = grantApplication.TrainingPrograms.FirstOrDefault().RequestedTrainingProvider;
					if (entity != null)
						_trainingProviderService.DeleteRequestedTrainingProvider(entity);
				}
				else
				{
					foreach (var trainingProgram in grantApplication.TrainingPrograms)
					{
						var entity = trainingProgram.RequestedTrainingProvider;
						if (entity != null)
							_trainingProviderService.DeleteRequestedTrainingProvider(entity);
					}

					var removeRequestedProvider = grantApplication.TrainingProviders.Where(x => x.TrainingProviderState == TrainingProviderStates.Requested).ToList();
					foreach (var trainingProvider in grantApplication.TrainingProviders.Where(x => x.TrainingProviderState == TrainingProviderStates.Requested).ToList())
					{
						_trainingProviderService.DeleteRequestedTrainingProvider(trainingProvider);
					}
				}

				model = new Models.Agreements.AgreementOverviewViewModel(grantApplication, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		#region Delivery Date
		/// <summary>
		/// Return a view to edit the delivery dates.
		/// </summary>
		/// <returns></returns>
		[Route("Agreement/Change/Delivery/Dates/View")]
		public ActionResult ChangeDeliveryDatesView()
		{
			return PartialView("_DeliveryDatesView");
		}

		/// <summary>
		/// Update the delivery dates and any training programs that are outside of the delivery dates.
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Agreement/Change/Delivery/Dates")]
		public JsonResult ChangeDeliveryDates(DeliveryDateViewModel model)
		{
			var viewModel = new Models.Agreements.AgreementOverviewViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(model.Id);
				viewModel = new Models.Agreements.AgreementOverviewViewModel(grantApplication, User);

				var start = model.StartDate.ToUtcMorning();
				var end = model.EndDate.ToUtcMidnight();

				// If a Claim is currently in a submitted state they can't make changes to the Training Provider or Training Program.
				if (!grantApplication.CanMakeChangeRequest())
					throw new InvalidOperationException("A claim is currently being processed, you cannot submit a change request.");

				if (grantApplication.StartDate != start || grantApplication.EndDate != end)
				{
					grantApplication.RowVersion = Convert.FromBase64String(model.RowVersion);

					// Update any training program dates that are outside of the delivery dates.
					grantApplication.TrainingPrograms.Where(tp => tp.StartDate < start || tp.StartDate > end).ForEach(tp => tp.StartDate = start);
					grantApplication.TrainingPrograms.Where(tp => tp.EndDate > end || tp.EndDate < tp.StartDate).ForEach(tp => tp.EndDate = end);

					grantApplication.StartDate = start;
					grantApplication.EndDate = end;

					_grantApplicationService.UpdateDeliveryDates(grantApplication);
					viewModel = new Models.Agreements.AgreementOverviewViewModel(grantApplication, User);
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}
			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region Program Dates
		/// <summary>
		/// Modal popup view to change training program dates.
		/// </summary>
		/// <returns></returns>
		[Route("Agreement/Change/Program/Dates/View")]
		public ActionResult GetProgramDatesView()
		{
			return PartialView("_ProgramDatesView");
		}

		/// <summary>
		/// Update the selected training program dates.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Agreement/Change/Program/Dates")]
		public JsonResult ChangeProgramDates(Models.ChangeRequest.ChangeRequestProgramTrainingDateViewModel model)
		{
			var viewModel = new Models.Agreements.AgreementOverviewViewModel();
			try
			{
				var trainingProgram = _trainingProgramService.Get(model.Id);
				viewModel = new Models.Agreements.AgreementOverviewViewModel(trainingProgram.GrantApplication, User);

				trainingProgram.RowVersion = Convert.FromBase64String(model.RowVersion);

				var start = model.StartDate.ToUtcMorning();
				var end = model.EndDate.ToUtcMidnight();

				if (trainingProgram.StartDate != start || trainingProgram.EndDate != end)
				{
					trainingProgram.StartDate = start;
					trainingProgram.EndDate = end;

					_trainingProgramService.UpdateProgramDates(trainingProgram);
					viewModel = new Models.Agreements.AgreementOverviewViewModel(trainingProgram.GrantApplication, User);
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}
			return Json(viewModel);
		}
		#endregion

		#region Training Provider
		/// <summary>
		/// Return a partial view for training provider change request.
		/// </summary>
		/// <param name="trainingProviderId">The currently approved training provider Id</param>
		/// <returns></returns>
		[Route("Agreement/Change/Training/Provider/View/{trainingProviderId}")]
		public ActionResult ChangeTrainingProviderView(int trainingProviderId)
		{
			var trainingProvider = _trainingProviderService.Get(trainingProviderId);

			ViewBag.OriginalTrainingProviderId = trainingProviderId;
			ViewBag.TrainingProviderId = trainingProvider.RequestedTrainingProvider?.Id ?? 0;
			return PartialView("_ChangeTrainingProviderView");
		}

		/// <summary>
		/// Get the data for the training provider change request.
		/// </summary>
		/// <param name="trainingProviderId">The currently approved training provider Id</param>
		/// <returns></returns>
		[HttpGet]
		[Route("Agreement/Change/Training/Provider/{trainingProviderId}")]
		public JsonResult GetTrainingProvider(int trainingProviderId)
		{
			var model = new Models.ChangeRequest.RequestedTrainingProviderViewModel();
			try
			{
				var trainingProvider = _trainingProviderService.Get(trainingProviderId);
				model = new Models.ChangeRequest.RequestedTrainingProviderViewModel(trainingProvider);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Add the specified training provider change request to the datasource.
		/// </summary>
		/// <param name="files"></param>
		/// <param name="provider"></param>
		/// <returns></returns>
		[HttpPost]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Agreement/Change/Training/Provider")]
		public JsonResult AddTrainingProvider(HttpPostedFileBase[] files, string provider)
		{
			var viewModel = new Models.Agreements.AgreementOverviewViewModel();

			try
			{
				var model = JsonConvert.DeserializeObject<Models.ChangeRequest.RequestedTrainingProviderViewModel>(provider);
				
				ModelState.Clear();
				model.ContactPhone = model.ContactPhoneAreaCode + model.ContactPhoneExchange + model.ContactPhoneNumber;

				TryUpdateModel(model);
				TryUpdateModel(model.TrainingAddress);

				if (model.TrainingOutsideBC == true && String.IsNullOrWhiteSpace(model.BusinessCase) && (String.IsNullOrWhiteSpace(model.BusinessCaseDocument.FileName) || files.Length < model.BusinessCaseDocument.Index))
					ModelState.AddModelError("BusinessCase", "Business case is required when training is outside BC.");
				if (model.ContactPhone.Length < 10)
					ModelState.AddModelError("ContactPhone", "Contact phone number is required.");

				if (model.TrainingProviderTypeId.HasValue)
				{
					var originalTrainingProvider = _trainingProviderService.Get(model.OriginalTrainingProviderId);
					var trainingProviderType = _trainingProviderService.Get<TrainingProviderType>(model.TrainingProviderTypeId);
					if (trainingProviderType.PrivateSectorValidationType.IsPrivateSectorType(originalTrainingProvider.GetGrantApplication().DateSubmitted, AppDateTime.UtcNow))
					{
						if (String.IsNullOrWhiteSpace(model.CourseOutlineDocument.FileName) || files.Length < model.CourseOutlineDocument.Index)
							ModelState.AddModelError("CourseOutlineDocument", "Course outline document is required.");
						if (String.IsNullOrWhiteSpace(model.ProofOfQualificationsDocument.FileName) || files.Length < model.ProofOfQualificationsDocument.Index)
							ModelState.AddModelError("ProofOfQualificationsDocument", "Proof of qualifications document is required.");
					}
				}

				if (ModelState.IsValid)
				{
					var trainingProvider = model.MapProperties(files, _trainingProviderService);

					// If the eligible cost breakdown associated with the training program is no longer eligible, throw an exception.
					if (!(trainingProvider.TrainingProgram?.EligibleCostBreakdown?.IsEligible ?? true))
					{
						throw new InvalidOperationException($"The training program '{trainingProvider.TrainingProgram.CourseTitle}' is not eligible for this grant and cannot be part of a change request.");
					}

					_trainingProviderService.Add(trainingProvider);
					viewModel = new Models.Agreements.AgreementOverviewViewModel(trainingProvider.GetGrantApplication(), User);
				}
				else
				{
					var trainingProvider = _trainingProviderService.Get(model.OriginalTrainingProviderId);
					viewModel = new Models.Agreements.AgreementOverviewViewModel(trainingProvider.GetGrantApplication(), User);

					HandleModelStateValidation(viewModel);
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}

			return Json(viewModel);
		}

		/// <summary>
		/// Update the specified training provider change request in the datasource.
		/// </summary>
		/// <param name="files"></param>
		/// <param name="provider"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Agreement/Change/Training/Provider")]
		public JsonResult UpdateTrainingProvider(HttpPostedFileBase[] files, string provider)
		{
			var viewModel = new Models.Agreements.AgreementOverviewViewModel();

			try
			{
				var model = JsonConvert.DeserializeObject<Models.ChangeRequest.RequestedTrainingProviderViewModel>(provider);

				ModelState.Clear();
				model.ContactPhone = model.ContactPhoneAreaCode + model.ContactPhoneExchange + model.ContactPhoneNumber;

				TryUpdateModel(model);
				TryUpdateModel(model.TrainingAddress);

				if (model.TrainingOutsideBC == true && String.IsNullOrWhiteSpace(model.BusinessCase) && (String.IsNullOrWhiteSpace(model.BusinessCaseDocument.FileName) || files.Length < model.BusinessCaseDocument.Index))
					ModelState.AddModelError("BusinessCase", "Business case is required when training is outside BC.");
				if (model.ContactPhone.Length < 10)
					ModelState.AddModelError("ContactPhone", "Contact phone number is required.");

				if (model.TrainingProviderTypeId.HasValue)
				{
					var trainingProvider = _trainingProviderService.Get(model.Id);
					var trainingProviderType = _trainingProviderService.Get<TrainingProviderType>(model.TrainingProviderTypeId);
					if (trainingProviderType.PrivateSectorValidationType.IsPrivateSectorType(trainingProvider.GetGrantApplication().DateSubmitted, AppDateTime.UtcNow))
					{
						if (String.IsNullOrWhiteSpace(model.CourseOutlineDocument.FileName) || files.Length < model.CourseOutlineDocument.Index)
							ModelState.AddModelError("CourseOutlineDocument", "Course outline document is required.");
						if (String.IsNullOrWhiteSpace(model.ProofOfQualificationsDocument.FileName) || files.Length < model.ProofOfQualificationsDocument.Index)
							ModelState.AddModelError("ProofOfQualificationsDocument", "Proof of qualifications document is required.");
					}
				}

				if (ModelState.IsValid)
				{
					var trainingProvider = model.MapProperties(files, _trainingProviderService);

					// If the eligible cost breakdown associated with the training program is no longer eligible, throw an exception.
					if (!(trainingProvider.TrainingProgram?.EligibleCostBreakdown?.IsEligible ?? true))
					{
						throw new InvalidOperationException($"The training program '{trainingProvider.TrainingProgram.CourseTitle}' is not eligible for this grant and cannot be part of a change request.");
					}

					_trainingProviderService.Update(trainingProvider);
					viewModel = new Models.Agreements.AgreementOverviewViewModel(trainingProvider.GetGrantApplication(), User);
				}
				else
				{
					var trainingProvider = _trainingProviderService.Get(model.Id);
					viewModel = new Models.Agreements.AgreementOverviewViewModel(trainingProvider.GetGrantApplication(), User);
					HandleModelStateValidation(viewModel);
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}

			return Json(viewModel);
		}

		/// <summary>
		/// Delete the specified training provider change request from the datasource.
		/// </summary>
		/// <returns></returns>
		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Agreement/Change/Training/Provider/Delete")]
		public JsonResult DeleteTrainingProvider(Models.ChangeRequest.RequestedTrainingProviderViewModel model)
		{
			var viewModel = new Models.Agreements.AgreementOverviewViewModel();

			try
			{
				var trainingProvider = _trainingProviderService.Get(model.Id);
				var grantApplication = trainingProvider.GetGrantApplication();
				viewModel = new Models.Agreements.AgreementOverviewViewModel(grantApplication, User);

				trainingProvider.RowVersion = Convert.FromBase64String(model.RowVersion);
				_trainingProviderService.DeleteRequestedTrainingProvider(trainingProvider);
				viewModel = new Models.Agreements.AgreementOverviewViewModel(grantApplication, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}
			return Json(viewModel);
		}
		#endregion

		#region Service Provider 
		/// <summary>
		/// Return a partial view for service provider change request.
		/// </summary>
		/// <param name="trainingProviderId">The currently approved service provider Id</param>
		/// <returns></returns>
		[Route("Agreement/Change/Service/Provider/View/{trainingProviderId}")]
		public ActionResult ChangeServiceProviderView(int trainingProviderId)
		{
			var trainingProvider = _trainingProviderService.Get(trainingProviderId);

			ViewBag.OriginalTrainingProviderId = trainingProviderId;
			ViewBag.TrainingProviderId = trainingProvider.RequestedTrainingProvider?.Id ?? 0;
			return PartialView("_ChangeServiceProviderView");
		}

		/// <summary>
		/// Get the data for the service provider change request.
		/// </summary>
		/// <param name="trainingProviderId">The currently approved service provider Id</param>
		/// <returns></returns>
		[HttpGet]
		[Route("Agreement/Change/Service/Provider/{trainingProviderId}")]
		public JsonResult GetServiceProvider(int trainingProviderId)
		{
			var model = new Models.ChangeRequest.RequestedTrainingProviderViewModel();
			try
			{
				var trainingProvider = _trainingProviderService.Get(trainingProviderId);
				model = new Models.ChangeRequest.RequestedTrainingProviderViewModel(trainingProvider)
				{
					TrainingOutsideBC = false
				};
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}
		#endregion
		#endregion
	}
}