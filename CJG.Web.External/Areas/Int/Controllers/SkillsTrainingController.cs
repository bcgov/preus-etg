using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Int.Controllers
{
	[RouteArea("Int")]
	[Authorize(Roles = "Assessor, System Administrator, Director, Financial Clerk")]
	public class SkillsTrainingController : BaseController
	{
		#region Variables
		private readonly IStaticDataService _staticDataService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly ITrainingProgramService _trainingProgramService;
		private readonly ITrainingProviderService _trainingProviderService;
		private readonly IEligibleCostService _eligibleCostService;
		private readonly IEligibleExpenseTypeService _eligibleExpenseTypeService;
		private readonly IEligibleExpenseBreakdownService _eligibleExpenseBreakdownService;
		private readonly IServiceLineBreakdownService _serviceLineBreakdownService;
		private readonly IApplicationAddressService _applicationAddressService;
		private readonly IAttachmentService _attachmentService;
		#endregion

		#region Constructors
		public SkillsTrainingController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			ITrainingProgramService trainingProgramService,
			ITrainingProviderService trainingProviderService,
			IEligibleCostService eligibleCostService,
			IEligibleExpenseTypeService eligibleExpenseTypeService,
			IEligibleExpenseBreakdownService eligibleExpenseBreakdownService,
			IServiceLineBreakdownService serviceLineBreakdownService,
			IApplicationAddressService applicationAddressService,
			IAttachmentService attachmentService
		   ) : base(controllerService.Logger)
		{
			_staticDataService = controllerService.StaticDataService;
			_grantApplicationService = grantApplicationService;
			_trainingProgramService = trainingProgramService;
			_trainingProviderService = trainingProviderService;
			_eligibleCostService = eligibleCostService;
			_eligibleExpenseTypeService = eligibleExpenseTypeService;
			_eligibleExpenseBreakdownService = eligibleExpenseBreakdownService;
			_serviceLineBreakdownService = serviceLineBreakdownService;
			_applicationAddressService = applicationAddressService;
			_attachmentService = attachmentService;
		}
		#endregion

		#region Endpoints
		#region Training Program
		/// <summary>
		/// Get the training program for the skills training component.
		/// </summary>
		/// <param name="trainingProgramId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Skills/Training/Program/{trainingProgramId}")]
		public JsonResult GetTrainingProgram(int trainingProgramId)
		{
			var model = new Models.SkillsTraining.SkillsTrainingProgramViewModel();

			try
			{
				var trainingProgram = _trainingProgramService.Get(trainingProgramId);
				model = new Models.SkillsTraining.SkillsTrainingProgramViewModel(trainingProgram, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Update the training program (and training provider) for the skills training component.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Application/Skills/Training/Program")]
		public JsonResult UpdateTrainingProgram(Models.SkillsTraining.SkillsTrainingProgramViewModel model)
		{
			try
			{
				var trainingProgram = _trainingProgramService.Get(model.Id);

				if (ModelState.IsValid)
				{
					Utilities.MapProperties(model, trainingProgram);
					trainingProgram.StartDate = model.StartDate.Value.ToUtcMorning();
					trainingProgram.EndDate = model.EndDate.Value.ToUtcMidnight();
					_trainingProgramService.UpdateDeliveryMethods(trainingProgram, model.SelectedDeliveryMethodIds);

					var eligibleCostBreakdown = trainingProgram.EligibleCostBreakdown;
					eligibleCostBreakdown.RowVersion = Convert.FromBase64String(model.EligibleCostBreakdownRowVersion);
					var eligibleExpenseBreakdownId = eligibleCostBreakdown.EligibleCost.EligibleExpenseType.Breakdowns.FirstOrDefault(b => b.ServiceLineId == trainingProgram.ServiceLineId)?.Id ?? throw new InvalidOperationException("An invalid service line has been selected.");
					if (eligibleCostBreakdown.EligibleExpenseBreakdownId != eligibleExpenseBreakdownId)
					{
						// The expense type breakdown (service line) has been changed.
						eligibleCostBreakdown.EligibleExpenseBreakdown = null;
						eligibleCostBreakdown.EligibleExpenseBreakdownId = eligibleExpenseBreakdownId;
					}

					_trainingProgramService.Update(trainingProgram);

					model = new Models.SkillsTraining.SkillsTrainingProgramViewModel(trainingProgram, User);
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
		/// Either make eligible or not eligible the specified skills training component.
		/// </summary>
		/// <param name="trainingProgramId"></param>
		/// <param name="rowVersion">The eligible breakdown row version.</param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Application/Skills/Training/Eligibility/{trainingProgramId}")]
		public JsonResult IsEligible(int trainingProgramId, string rowVersion) // TODO: Replace rowVersion implementaiton with model.
		{
			var model = new Models.SkillsTraining.SkillsTrainingProgramViewModel();
			try
			{
				var trainingProgram = _trainingProgramService.Get(trainingProgramId);
				var breakdown = trainingProgram.EligibleCostBreakdown;
				breakdown.RowVersion = Convert.FromBase64String(rowVersion.Replace(" ", "+"));

				_trainingProgramService.ChangeEligibility(trainingProgram);

				model = new Models.SkillsTraining.SkillsTrainingProgramViewModel(trainingProgram, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model);
		}

		/// <summary>
		/// Add a new skills training component to the grant application.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Application/Skills/Training")]
		public JsonResult AddComponent(HttpPostedFileBase[] files, string component)
		{
			var viewModel = new Models.SkillsTraining.SkillsTrainingProgramViewModel();
			try
			{
				var model = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.SkillsTraining.UpdateSkillsTrainingProgramViewModel>(component);
				model.TrainingProvider.ContactPhone = $"({model.TrainingProvider.ContactPhoneAreaCode}) {model.TrainingProvider.ContactPhoneExchange}-{model.TrainingProvider.ContactPhoneNumber}";

				if (model.TrainingProvider.CountryId != Core.Entities.Constants.CanadaCountryId)
				{
					// Create or fetch the associated region id.
					var region = _applicationAddressService.VerifyOrCreateRegion(model.TrainingProvider.Region, model.TrainingProvider.CountryId);
					model.TrainingProvider.RegionId = region.Id;
				}
				TryUpdateModel(model);
				ModelState.Remove(nameof(model.TrainingProvider.Region));

				if (model.TrainingProvider.CountryId == Core.Entities.Constants.CanadaCountryId)
				{
					ModelState.Remove(nameof(model.TrainingProvider.ZipCode));
				}
				else
				{
					ModelState.Remove(nameof(model.TrainingProvider.PostalCode));
				}

				var grantApplication = _grantApplicationService.Get(model.GrantApplicationId);
				var eligibleCost = _eligibleCostService.Get(model.EligibleCostId);

				if (ModelState.IsValid)
				{
					var trainingProgram = new TrainingProgram(grantApplication);
					var trainingProvider = new TrainingProvider(trainingProgram);
					var eligibleExpenseBreakdown = _eligibleExpenseBreakdownService.GetForServiceLine(model.ServiceLineId);
					var eligibleCostBreakdown = new EligibleCostBreakdown(eligibleCost, eligibleExpenseBreakdown, 0)
					{
						AssessedCost = model.AgreedCost,
						IsEligible = true,
						AddedByAssessor = true
					};
					trainingProgram.EligibleCostBreakdown = eligibleCostBreakdown;

					Utilities.MapProperties(model, trainingProgram);
					trainingProgram.StartDate = model.StartDate.Value.ToUtcMorning();
					trainingProgram.EndDate = model.EndDate.Value.ToUtcMidnight();
					_trainingProgramService.UpdateDeliveryMethods(trainingProgram, model.SelectedDeliveryMethodIds);

					Utilities.MapProperties(model.TrainingProvider, trainingProvider);
					trainingProvider.TrainingProviderState = TrainingProviderStates.Complete;
					trainingProvider.ContactPhoneNumber = model.TrainingProvider.ContactPhone;
					var trainingAddress = new ApplicationAddress
					{
						AddressLine1 = model.TrainingProvider.AddressLine1,
						AddressLine2 = model.TrainingProvider.AddressLine2,
						City = model.TrainingProvider.City,
						RegionId = model.TrainingProvider.RegionId,
						CountryId = model.TrainingProvider.CountryId,
						PostalCode = model.TrainingProvider.CountryId == Core.Entities.Constants.CanadaCountryId ? model.TrainingProvider.PostalCode : model.TrainingProvider.ZipCode
					};
					trainingProvider.TrainingAddress = trainingAddress;
					trainingProvider.TrainingProviderState = TrainingProviderStates.Complete;

					// Recalculate costs.
					eligibleCost.Breakdowns.Add(eligibleCostBreakdown);

					UpdateAttachments(files, model.TrainingProvider, trainingProvider);

					_trainingProgramService.Add(trainingProgram);

					viewModel = new Models.SkillsTraining.SkillsTrainingProgramViewModel(trainingProgram, User);
				}
				else
				{
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
		/// Delete the skills training component from the application.
		/// </summary>
		/// <param name="trainingProgramId"></param>
		/// <param name="rowVersion"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Application/Skills/Training/Component/Delete/{trainingProgramId}")]
		public JsonResult DeleteComponent(int trainingProgramId, string rowVersion)
		{
			var model = new BaseViewModel();
			try
			{
				var trainingProgram = _trainingProgramService.Get(trainingProgramId);
				trainingProgram.RowVersion = Convert.FromBase64String(rowVersion.Replace(" ", "+"));

				if (!(trainingProgram.EligibleCostBreakdown?.AddedByAssessor ?? false))
					throw new InvalidOperationException($"User does not have permission to take this action on the application {trainingProgram.GrantApplicationId}.");

				var grantApplication = trainingProgram.GrantApplication;
				_trainingProgramService.Delete(trainingProgram);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		/// <summary>
		/// Get an array of available service lines for the specified eligible expense type.
		/// </summary>
		/// <param name="eligibleExpenseTypeId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Skills/Training/Service/Lines/{eligibleExpenseTypeId}")]
		public JsonResult GetServiceLines(int eligibleExpenseTypeId)
		{
			var model = new BaseViewModel();

			try
			{
				var eligibleExpenseType = _eligibleExpenseTypeService.Get(eligibleExpenseTypeId);
				var result = eligibleExpenseType.Breakdowns.Select(o => new
				{
					Id = o.ServiceLineId,
					o.Caption,
					o.ServiceLine.BreakdownCaption
				}).Distinct().ToArray();

				return Json(result, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
				return Json(model, JsonRequestBehavior.AllowGet);
			}
		}

		/// <summary>
		/// Get all the service line breakdowns for the specified service line id.
		/// </summary>
		/// <param name="serviceLineId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Skills/Training/Service/Line/Breakdowns/{serviceLineId}")]
		public JsonResult GetServiceLineBreakdowns(int serviceLineId)
		{
			var model = new BaseViewModel();

			try
			{
				var serviceLineBreakdowns = serviceLineId == 0 ? _serviceLineBreakdownService.GetAll() : _serviceLineBreakdownService.GetAllForServiceLine(serviceLineId, true);
				var result = serviceLineBreakdowns.Select(o => new
				{
					o.Id,
					o.Caption,
					o.ServiceLineId
				}).ToArray();

				return Json(result, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
				return Json(model, JsonRequestBehavior.AllowGet);
			}
		}
		#endregion

		#region Training Provider
		/// <summary>
		/// Get the training provider for the skills training component.
		/// </summary>
		/// <param name="trainingProvider"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Skills/Training/Provider/{trainingProviderId}")]
		public JsonResult GetTrainingProvider(int trainingProviderId)
		{
			var model = new Models.SkillsTraining.SkillsTrainingProviderViewModel();

			try
			{
				var trainingProvider = _trainingProviderService.Get(trainingProviderId);
				model = new Models.SkillsTraining.SkillsTrainingProviderViewModel(trainingProvider, User, _staticDataService);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Update the training program in the skills training component.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Application/Skills/Training/Provider")]
		public JsonResult UpdateTrainingProvider(HttpPostedFileBase[] files, string provider)
		{
			var viewModel = new Models.SkillsTraining.SkillsTrainingProviderViewModel();
			try
			{
				var model = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.SkillsTraining.UpdateSkillsTrainingProviderViewModel>(provider);

				if (!String.IsNullOrWhiteSpace(model.ContactPhoneAreaCode) || !String.IsNullOrWhiteSpace(model.ContactPhoneExchange) || !String.IsNullOrWhiteSpace(model.ContactPhoneNumber))
				{
					model.ContactPhone = $"({model.ContactPhoneAreaCode}) {model.ContactPhoneExchange}-{model.ContactPhoneNumber}";
				}
				var trainingProvider = _trainingProviderService.Get(model.Id);

				TryUpdateModel(model);

				if (model.CountryId == Core.Entities.Constants.CanadaCountryId)
				{
					ModelState.Remove(nameof(model.ZipCode));
					ModelState.Remove(nameof(model.Region));
				}
				else
				{
					ModelState.Remove(nameof(model.PostalCode));
					ModelState.Remove(nameof(model.RegionId));
				}

				if (ModelState.IsValid)
				{
					Utilities.MapProperties(model, trainingProvider);
					trainingProvider.ContactPhoneNumber = model.ContactPhone;
					trainingProvider.TrainingAddress.AddressLine1 = model.AddressLine1;
					trainingProvider.TrainingAddress.AddressLine2 = model.AddressLine2;
					trainingProvider.TrainingAddress.City = model.City;
					trainingProvider.TrainingAddress.RegionId = model.RegionId;
					trainingProvider.TrainingAddress.CountryId = model.CountryId;
					trainingProvider.TrainingAddress.PostalCode = model.PostalCode;
					if (model.CountryId != Core.Entities.Constants.CanadaCountryId)
					{
						trainingProvider.TrainingAddress.RegionId = _applicationAddressService.VerifyOrCreateRegion(model.Region, model.CountryId).Id;
					}

					UpdateAttachments(files, model, trainingProvider);

					_trainingProviderService.Update(trainingProvider);

					viewModel = new Models.SkillsTraining.SkillsTrainingProviderViewModel(trainingProvider, User, _staticDataService);
				}
				else
				{
					HandleModelStateValidation(viewModel, ModelState.GetErrorMessages("<br />"));
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}

			return Json(viewModel);
		}

		/// <summary>
		/// Approve the change request service provider.
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Application/Skills/Training/Provider/Approve")]
		public JsonResult ApproveProvider(Models.SkillsTraining.SkillsTrainingProviderViewModel model)
		{
			try
			{
				var trainingProvider = _trainingProviderService.Get(model.Id);
				trainingProvider.RowVersion = Convert.FromBase64String(model.RowVersion);
				trainingProvider.TrainingProviderState = trainingProvider.TrainingProviderState == TrainingProviderStates.RequestApproved ? TrainingProviderStates.Requested : TrainingProviderStates.RequestApproved;
				_trainingProviderService.Update(trainingProvider);

				model = new Models.SkillsTraining.SkillsTrainingProviderViewModel(trainingProvider, User, _staticDataService);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		/// <summary>
		/// Deny the change request service provider.
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Application/Skills/Training/Provider/Deny")]
		public JsonResult DenyProvider(Models.SkillsTraining.SkillsTrainingProviderViewModel model)
		{
			try
			{
				var trainingProvider = _trainingProviderService.Get(model.Id);
				trainingProvider.RowVersion = Convert.FromBase64String(model.RowVersion);
				trainingProvider.TrainingProviderState = trainingProvider.TrainingProviderState == TrainingProviderStates.RequestDenied ? TrainingProviderStates.Requested : TrainingProviderStates.RequestDenied;
				_trainingProviderService.Update(trainingProvider);

				model = new Models.SkillsTraining.SkillsTrainingProviderViewModel(trainingProvider, User, _staticDataService);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}
		#endregion

		#region Helpers
		private void UpdateAttachments(HttpPostedFileBase[] files, Models.SkillsTraining.UpdateSkillsTrainingProviderViewModel model, TrainingProvider trainingProvider)
		{

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
		}
		#endregion
		#endregion
	}
}
