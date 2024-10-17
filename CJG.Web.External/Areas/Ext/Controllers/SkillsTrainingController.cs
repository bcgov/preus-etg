using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared.SkillsTrainings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Ext.Controllers
{
	/// <summary>
	/// SkillsTrainingController class, provides endpoints to manage training programs.
	/// </summary>
	[RouteArea("Ext")]
	[ExternalFilter]
	public class SkillsTrainingController : BaseController
	{
		#region Variables
		private readonly IStaticDataService _staticDataService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IApplicationAddressService _applicationAddressService;
		private readonly IAttachmentService _attachmentService;
		private readonly ITrainingProgramService _trainingProgramService;
		private readonly ITrainingProviderService _trainingProviderService;
		private readonly IEligibleExpenseBreakdownService _eligibleExpenseBreakdownService;
		private readonly IServiceLineBreakdownService _serviceLineBreakdownService;
		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance of a SkillsTrainingController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantApplicationService"></param>
		/// <param name="applicationAddressService"></param>
		/// <param name="attachmentService"></param>
		/// <param name="trainingProgramService"></param>
		/// <param name="trainingProviderService"></param>
		/// <param name="eligibleExpenseBreakdownService"></param>
		/// <param name="serviceLineBreakdownService"></param>
		public SkillsTrainingController(IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			IApplicationAddressService applicationAddressService,
			IAttachmentService attachmentService,
			ITrainingProgramService trainingProgramService,
			ITrainingProviderService trainingProviderService,
			IEligibleExpenseBreakdownService eligibleExpenseBreakdownService,
			IServiceLineBreakdownService serviceLineBreakdownService) : base(controllerService.Logger)
		{
			_staticDataService = controllerService.StaticDataService;
			_grantApplicationService = grantApplicationService;
			_applicationAddressService = applicationAddressService;
			_attachmentService = attachmentService;
			_trainingProgramService = trainingProgramService;
			_trainingProviderService = trainingProviderService;
			_eligibleExpenseBreakdownService = eligibleExpenseBreakdownService;
			_serviceLineBreakdownService = serviceLineBreakdownService;
		}
		#endregion

		#region Endpoints

		/// <summary>
		/// Returns a View to edit a skills training component (which is a training provider and training program pair).
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="eligibleExpenseTypeId"></param>
		/// <param name="trainingProgramId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/{grantApplicationId:int}/Skills/Training/View/{eligibleExpenseTypeId:int}/{trainingProgramId:int}")]
		public ActionResult SkillsTrainingView(int grantApplicationId, int eligibleExpenseTypeId, int trainingProgramId)
		{
			var grantApplication = _grantApplicationService.Get(grantApplicationId);
			if (!User.CanPerformAction(grantApplication, ApplicationWorkflowTrigger.ViewApplication))
				throw new NotAuthorizedException($"User does not have permission to view application '{grantApplicationId}'.");

			ViewBag.GrantApplicationId = grantApplicationId;
			ViewBag.EligibleExpenseTypeId = eligibleExpenseTypeId;
			ViewBag.TrainingProgramId = trainingProgramId;
			return View();
		}

		/// <summary>
		/// Get the data for the SkillsTrainingView endpoint.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="eligibleExpenseTypeId"></param>
		/// <param name="trainingProgramId"></param>
		/// <returns></returns>
		[HttpGet]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Application/{grantApplicationId:int}/Skills/Training/{eligibleExpenseTypeId:int}/{trainingProgramId:int}")]
		public JsonResult GetSkillsTraining(int grantApplicationId, int eligibleExpenseTypeId, int trainingProgramId)
		{
			var viewModel = new SkillTrainingViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);

				if (trainingProgramId == 0)
				{
					viewModel = new SkillTrainingViewModel(grantApplication);
				}
				else
				{
					var trainingProgram = _trainingProgramService.Get(trainingProgramId);
					viewModel = new SkillTrainingViewModel(trainingProgram);
				}

				viewModel.GrantApplicationId = grantApplicationId;
				viewModel.EligibleExpenseTypeId = eligibleExpenseTypeId;

				if (!grantApplication.GrantOpening.GrantStream.ProgramConfiguration.EligibleExpenseTypes.Any(eet => eet.ServiceCategory.ServiceTypeId == ServiceTypes.SkillsTraining && eet.Id == eligibleExpenseTypeId))
					throw new InvalidOperationException($"The skills training eligible expense type is not valid for grant application '{grantApplicationId}'.");

				viewModel.SkillTrainingDetails.EligibleExpenseTypeId = viewModel.EligibleExpenseTypeId;

				if (grantApplication.GrantOpening.GrantStream.GrantProgram.ProgramType.Id == ProgramTypes.EmployerGrant)
				{
					viewModel.SkillTrainingDetails.StartDate = grantApplication.StartDate;
					viewModel.SkillTrainingDetails.EndDate = grantApplication.EndDate;
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}
			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Add the skills training component to the datasource.
		/// </summary>
		/// <param name="files"></param>
		/// <param name="component"></param>
		/// <returns></returns>
		[HttpPost]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Skills/Training")]
		public JsonResult AddSkillsTraining(HttpPostedFileBase[] files, string component)
		{
			var model = new SkillTrainingViewModel();

			try
			{
				model = JsonConvert.DeserializeObject<SkillTrainingViewModel>(component);

				var grantApplication = _grantApplicationService.Get(model.GrantApplicationId);

				TryUpdateModel(model.SkillTrainingDetails);
				TryUpdateModel(model.SkillTrainingDetails.TrainingProvider);

				if (model.SkillTrainingDetails.SelectedDeliveryMethodIds.Contains(Core.Entities.Constants.Delivery_Online)
					&& !model.SkillTrainingDetails.SelectedDeliveryMethodIds.Contains(Core.Entities.Constants.Delivery_Classroom)
					&& !model.SkillTrainingDetails.SelectedDeliveryMethodIds.Contains(Core.Entities.Constants.Delivery_Workplace))
				{
					ModelState.Remove("AddressLine1");
					ModelState.Remove("City");
					ModelState.Remove("PostalCode");
				}

				if (model.SkillTrainingDetails.ExpectedQualificationId == 5)
				{
					ModelState.Remove("TitleOfQualification");
				}

				if (model.SkillTrainingDetails.TrainingProvider.IsCanadianAddress)
				{
					ModelState.Remove(nameof(model.SkillTrainingDetails.TrainingProvider.OtherRegion));
					ModelState.Remove(nameof(model.SkillTrainingDetails.TrainingProvider.OtherZipCode));
				}
				else
				{
					ModelState.Remove(nameof(model.SkillTrainingDetails.TrainingProvider.PostalCode));
					ModelState.Remove(nameof(model.SkillTrainingDetails.TrainingProvider.RegionId));
				}
				if (model.SkillTrainingDetails.TrainingProvider.IsCanadianAddressTrainingProvider)
				{
					ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.OtherZipCodeTrainingProvider));
					ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.OtherRegionTrainingProvider));
				}
				else
				{
					ModelState.Remove(nameof(model.SkillTrainingDetails.TrainingProvider.PostalCodeTrainingProvider));
					ModelState.Remove(nameof(model.SkillTrainingDetails.TrainingProvider.RegionIdTrainingProvider));
				}

				if (model.SkillTrainingDetails.TrainingProvider.TrainingOutsideBC == true && String.IsNullOrWhiteSpace(model.SkillTrainingDetails.TrainingProvider.BusinessCaseDocument.FileName))
				{
					ModelState.AddModelError("BusinessCaseDocument", "Business case document required.");
				}

				if (model.SkillTrainingDetails.TrainingProvider.TrainingProviderTypeId.HasValue)
				{
					var trainingProviderType = _trainingProviderService.Get<TrainingProviderType>(model.SkillTrainingDetails.TrainingProvider.TrainingProviderTypeId);
					var trainingProvider = new TrainingProvider(grantApplication)
					{
						TrainingProviderTypeId = model.SkillTrainingDetails.TrainingProvider.TrainingProviderTypeId.Value,
						TrainingProviderType = trainingProviderType
					};
					if (trainingProvider.TrainingProviderType.ProofOfInstructorQualifications == 1)
					{
						if (String.IsNullOrWhiteSpace(model.SkillTrainingDetails.TrainingProvider.ProofOfQualificationsDocument.FileName))
						{
							ModelState.AddModelError("ProofOfQualificationsDocument", "Proof of qualifications document required.");
						}
					}
					if (trainingProvider.TrainingProviderType.CourseOutline == 1)
					{
						if (String.IsNullOrWhiteSpace(model.SkillTrainingDetails.TrainingProvider.CourseOutlineDocument.FileName))
						{
							ModelState.AddModelError("CourseOutlineDocument", "Course outline document required.");
						}
					}
				}

				if (model.SkillTrainingDetails.TotalCost <= 0)
				{
					ModelState.AddModelError("TotalCost", "Total cost must be greater than $0.00.");
				}

				if (model.SkillTrainingDetails.EligibleExpenseBreakdownId.HasValue)
				{
					// ServiceLineBreakdownId is only required if there are service line breakdown values for the selected eligible expense breakdown.
					var eligibleExpenseBreakdown = _eligibleExpenseBreakdownService.Get(model.SkillTrainingDetails.EligibleExpenseBreakdownId.Value);
					if (!(eligibleExpenseBreakdown.ServiceLine?.ServiceLineBreakdowns.Any() ?? false))
					{
						ModelState.Remove(nameof(model.SkillTrainingDetails.ServiceLineBreakdownId));
					}
				}

				if (ModelState.IsValid)
				{
					grantApplication.RowVersion = Convert.FromBase64String(model.RowVersion);
					var trainingProgram = model.SkillTrainingDetails.MapProperties(
						_grantApplicationService,
						_trainingProgramService,
						_trainingProviderService,
						_attachmentService,
						_applicationAddressService,
						_eligibleExpenseBreakdownService,
						_serviceLineBreakdownService,
						_staticDataService,
						User,
						files);
					trainingProgram.IsSkillsTraining = true;
					_trainingProgramService.Add(trainingProgram);
					model = new SkillTrainingViewModel(trainingProgram);
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
		/// Update the specified skills training component in the datasource.
		/// </summary>
		/// <param name="files"></param>
		/// <param name="component"></param>
		/// <returns></returns>
		[HttpPut]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Skills/Training")]
		public JsonResult UpdateSkillsTraining(HttpPostedFileBase[] files, string component)
		{
			var model = new SkillTrainingViewModel();

			try
			{
				model = JsonConvert.DeserializeObject<SkillTrainingViewModel>(component);

				var grantApplication = _grantApplicationService.Get(model.GrantApplicationId);

				TryUpdateModel(model.SkillTrainingDetails);
				TryUpdateModel(model.SkillTrainingDetails.TrainingProvider);
				if (model.SkillTrainingDetails.SelectedDeliveryMethodIds.Contains(Core.Entities.Constants.Delivery_Online)
					&& !model.SkillTrainingDetails.SelectedDeliveryMethodIds.Contains(Core.Entities.Constants.Delivery_Classroom)
					&& !model.SkillTrainingDetails.SelectedDeliveryMethodIds.Contains(Core.Entities.Constants.Delivery_Workplace))
				{
					ModelState.Remove("AddressLine1");
					ModelState.Remove("City");
					ModelState.Remove("PostalCode");
				}

				if (model.SkillTrainingDetails.ExpectedQualificationId == 5)
				{
					ModelState.Remove("TitleOfQualification");
				}

				if (model.SkillTrainingDetails.TrainingProvider.IsCanadianAddress)
				{
					ModelState.Remove(nameof(model.SkillTrainingDetails.TrainingProvider.OtherRegion));
					ModelState.Remove(nameof(model.SkillTrainingDetails.TrainingProvider.OtherZipCode));
				}
				else
				{
					ModelState.Remove(nameof(model.SkillTrainingDetails.TrainingProvider.PostalCode));
					ModelState.Remove(nameof(model.SkillTrainingDetails.TrainingProvider.RegionId));
				}

				if (model.SkillTrainingDetails.TrainingProvider.TrainingOutsideBC == true && String.IsNullOrWhiteSpace(model.SkillTrainingDetails.TrainingProvider.BusinessCaseDocument.FileName))
				{
					ModelState.AddModelError("BusinessCaseDocument", "Business case document required.");
				}
				if (model.SkillTrainingDetails.TrainingProvider.IsCanadianAddressTrainingProvider)
				{
					ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.OtherZipCodeTrainingProvider));
					ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.OtherRegionTrainingProvider));
				}
				else
				{
					ModelState.Remove(nameof(model.SkillTrainingDetails.TrainingProvider.PostalCodeTrainingProvider));
					ModelState.Remove(nameof(model.SkillTrainingDetails.TrainingProvider.RegionIdTrainingProvider));
				}

				if (model.SkillTrainingDetails.TrainingProvider.TrainingProviderTypeId.HasValue)
				{
					var trainingProviderType = _trainingProviderService.Get<TrainingProviderType>(model.SkillTrainingDetails.TrainingProvider.TrainingProviderTypeId);
					var trainingProvider = new TrainingProvider(grantApplication)
					{
						TrainingProviderTypeId = model.SkillTrainingDetails.TrainingProvider.TrainingProviderTypeId.Value,
						TrainingProviderType = trainingProviderType
					};
					if (trainingProvider.TrainingProviderType.ProofOfInstructorQualifications == 1)
					{
						if (String.IsNullOrWhiteSpace(model.SkillTrainingDetails.TrainingProvider.ProofOfQualificationsDocument.FileName))
						{
							ModelState.AddModelError("ProofOfQualificationsDocument", "Proof of qualifications document required.");
						}
					}
					if (trainingProvider.TrainingProviderType.CourseOutline == 1)
					{
						if (String.IsNullOrWhiteSpace(model.SkillTrainingDetails.TrainingProvider.CourseOutlineDocument.FileName))
						{
							ModelState.AddModelError("CourseOutlineDocument", "Course outline document required.");
						}
					}
				}

				if (model.SkillTrainingDetails.TotalCost <= 0)
				{
					ModelState.AddModelError("TotalCost", "Total cost must be greater than $0.00.");
				}

				if (model.SkillTrainingDetails.EligibleExpenseBreakdownId.HasValue)
				{
					// ServiceLineBreakdownId is only required if there are service line breakdown values for the selected eligible expense breakdown.
					var eligibleExpenseBreakdown = _eligibleExpenseBreakdownService.Get(model.SkillTrainingDetails.EligibleExpenseBreakdownId.Value);
					if (!(eligibleExpenseBreakdown.ServiceLine?.ServiceLineBreakdowns.Any() ?? false))
					{
						ModelState.Remove(nameof(model.SkillTrainingDetails.ServiceLineBreakdownId));
					}
				}

				if (ModelState.IsValid)
				{
					grantApplication.RowVersion = Convert.FromBase64String(model.RowVersion);
					var trainingProgram = model.SkillTrainingDetails.MapProperties(
						_grantApplicationService,
						_trainingProgramService,
						_trainingProviderService,
						_attachmentService,
						_applicationAddressService,
						_eligibleExpenseBreakdownService,
						_serviceLineBreakdownService,
						_staticDataService,
						User,
						files);
					trainingProgram.IsSkillsTraining = true;
					_trainingProgramService.Update(trainingProgram);
					model = new SkillTrainingViewModel(trainingProgram);
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
		/// Get the service lines for the specified eligible expense type.
		/// </summary>
		/// <param name="eligibleExpenseTypeId"></param>
		/// <returns></returns>
		[HttpGet]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Skills/Training/Service/Lines/{eligibleExpenseTypeId:int}")]
		public JsonResult GetServiceLines(int eligibleExpenseTypeId)
		{
			var serviceLines = new List<ServiceLineListModel>();
			try
			{
				serviceLines = _eligibleExpenseBreakdownService.GetAllActiveForEligibleExpenseType(eligibleExpenseTypeId).Select(x => new ServiceLineListModel()
				{
					Id = x.Id,
					Caption = x.Caption,
					RowSequence = x.RowSequence,
					BreakdownCaption = x.ServiceLine.BreakdownCaption
				}).ToList();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(serviceLines, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get the service line breakdowns for the specified eligible expense breakdown.
		/// </summary>
		/// <param name="eligibleExpenseBreakdownId"></param>
		/// <returns></returns>
		[HttpGet]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Skills/Training/Service/Line/Breakdowns/{eligibleExpenseBreakdownId:int}")]
		public JsonResult GetServiceLineBreakdowns(int eligibleExpenseBreakdownId)
		{
			var breakdowns = new List<CollectionItemModel>();
			try
			{
				var eligibleExpenseBreakdown = _eligibleExpenseBreakdownService.Get(eligibleExpenseBreakdownId);
				breakdowns = _serviceLineBreakdownService.GetAllForServiceLine(eligibleExpenseBreakdown.ServiceLineId.Value, true).Select(x => new CollectionItemModel()
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
			return Json(breakdowns, JsonRequestBehavior.AllowGet);
		}
		#endregion
	}
}