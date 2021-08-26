using CJG.Application.Business.Models;
using CJG.Core.Entities;
using CJG.Core.Interfaces;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;
using CJG.Web.External.Models.Shared.SkillsTrainings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Int.Controllers
{
	/// <summary>
	/// <paramtyperef name="ESSController"/> class, provides endpoints to manage ESS Services.
	/// </summary>
	[RouteArea("Int")]
	[RoutePrefix("Application/ESS")]
	public class ESSController : BaseController
	{
		#region Variables
		private readonly IStaticDataService _staticDataService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IGrantAgreementService _grantAgreementService;
		private readonly IEligibleCostService _eligibleCostService;
		private readonly IEligibleCostBreakdownService _eligibleCostBreakdownService;
		private readonly ITrainingProviderService _trainingProviderService;
		private readonly IEligibleExpenseBreakdownService _eligibleExpenseBreakdownService;
		private readonly ITrainingProgramService _trainingProgramService;
		private readonly IEligibleExpenseTypeService _eligibleExpenseTypeService;
		private readonly IAttachmentService _attachmentService;
		#endregion

		#region Constructors
		public ESSController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			IGrantAgreementService grantAgreementService,
			IEligibleCostService eligibleCostService,
			IEligibleCostBreakdownService eligibleCostBreakdownService,
			IEligibleExpenseBreakdownService eligibleExpenseBreakdownService,
			ITrainingProviderService trainingProviderService,
			ITrainingProgramService trainingProgramService,
			IAttachmentService attachmentService,
			IEligibleExpenseTypeService eligibleExpenseTypeService
		   ) : base(controllerService.Logger)
		{
			_staticDataService = controllerService.StaticDataService;
			_grantApplicationService = grantApplicationService;
			_grantAgreementService = grantAgreementService;
			_eligibleCostService = eligibleCostService;
			_eligibleCostBreakdownService = eligibleCostBreakdownService;
			_eligibleExpenseBreakdownService = eligibleExpenseBreakdownService;
			_trainingProviderService = trainingProviderService;
			_trainingProgramService = trainingProgramService;
			_attachmentService = attachmentService;
			_eligibleExpenseTypeService = eligibleExpenseTypeService;
		}
		#endregion

		#region Endpoints
		#region Services
		/// <summary>
		/// Get the data for the ESS Services section in Application Details View.
		/// </summary>
		/// <param name="eligibleCostId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Services/{eligibleCostId}")]
		public JsonResult GetServices(int eligibleCostId)
		{
			var model = new Models.ESS.EmploymentServicesViewModel();
			try
			{
				var eligibleCost = _eligibleCostService.Get(eligibleCostId);
				model = new Models.ESS.EmploymentServicesViewModel(eligibleCost);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get all the service lines for the specified eligible expense type.
		/// </summary>
		/// <param name="eligibleExpenseTypeId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Service/Lines/{eligibleExpenseTypeId}")]
		public JsonResult GetServiceLines(int eligibleExpenseTypeId)
		{
			KeyValueListItem<int, string>[] model = null;

			try
			{
				var entity = _eligibleExpenseBreakdownService.GetAllForEligibleExpenseType(eligibleExpenseTypeId);
				model = entity.Select(o => new KeyValueListItem<int, string>(o.Id, o.Caption)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Update the eligible cost selected service lines.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Services")]
		public JsonResult UpdateServices(Models.ESS.EmploymentServicesViewModel model)
		{
			try
			{
				var eligibleCost = _eligibleCostService.Get(model.Id);
				eligibleCost.RowVersion = Convert.FromBase64String(model.RowVersion);

				var currentIds = eligibleCost.Breakdowns.Select(b => b.EligibleExpenseBreakdownId).ToArray();

				if (model.SelectedServiceLineIds == null) model.SelectedServiceLineIds = new int[0];
				var idsToRemove = currentIds.Where(id => !model.SelectedServiceLineIds.Contains(id)).ToArray(); ;
				var idsToAdd = model.SelectedServiceLineIds.Where(id => !currentIds.Contains(id)).ToArray();

				idsToRemove.ForEach(eligibleExpenseBreakdownId =>
				{
					var breakdown = eligibleCost.Breakdowns.FirstOrDefault(b => b.EligibleExpenseBreakdownId == eligibleExpenseBreakdownId);
					if (breakdown != null)
					{
						_eligibleCostBreakdownService.Remove(breakdown);
					}
				});

				var itemsToAdd = new List<EligibleCostBreakdown>();
				idsToAdd.ForEach(eligibleExpenseBreakdownId =>
				{
					if (eligibleCost.Breakdowns.All(b => b.EligibleExpenseBreakdownId != eligibleExpenseBreakdownId))
					{
						var breakdownType = _eligibleExpenseBreakdownService.Get(eligibleExpenseBreakdownId);
						var breakdown = new EligibleCostBreakdown(eligibleCost, breakdownType, 0);
						eligibleCost.Breakdowns.Add(breakdown);
					}
				});

				eligibleCost.TrainingCost.RecalculateAgreedCosts();
				_grantApplicationService.Update(eligibleCost.TrainingCost.GrantApplication);

				model = new Models.ESS.EmploymentServicesViewModel(eligibleCost);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region Training Provider
		/// <summary>
		/// Get the training provider for the specified id.
		/// </summary>
		/// <param name="trainingProviderId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Provider/{trainingProviderId}")]
		public JsonResult GetProvider(int trainingProviderId)
		{
			var model = new Models.ESS.ProviderViewModel();
			try
			{
				var trainingProvider = _trainingProviderService.Get(trainingProviderId);
				model = new Models.ESS.ProviderViewModel(trainingProvider, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Update the training provider in the datasource.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Provider")]
		public JsonResult UpdateProvider(HttpPostedFileBase[] files, string provider)
		{
			var returnModel = new Models.ESS.ProviderViewModel();
			try
			{
				var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ServiceProviderDetailsViewModel>(provider);

				if (!String.IsNullOrWhiteSpace(model.ContactPhoneAreaCode) || !String.IsNullOrWhiteSpace(model.ContactPhoneExchange) || !String.IsNullOrWhiteSpace(model.ContactPhoneNumber))
				{
					model.ContactPhone = $"({model.ContactPhoneAreaCode}) {model.ContactPhoneExchange}-{model.ContactPhoneNumber}";
				}
				model.Region = null;
				model.CountryId = "CA";

				ModelState.Clear();
				TryUpdateModel(model);
				ModelState.Remove("OtherZipCode");
				ModelState.Remove("OtherRegion");

				if (ModelState.IsValid)
				{
					var trainingProvider = model.MapProperties(
												_grantApplicationService,
												_trainingProgramService,
												_trainingProviderService,
												_attachmentService,
												_eligibleCostService,
												_eligibleExpenseTypeService,
												_staticDataService,
												User,
												files);

					_trainingProviderService.Update(trainingProvider);
					returnModel = new Models.ESS.ProviderViewModel(trainingProvider, User);
				}
				else
				{
					HandleModelStateValidation(returnModel);
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, returnModel);
			}

			return Json(returnModel);
		}

		/// <summary>
		/// Add the training provider to the datasource.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Provider")]
		public JsonResult AddProvider(HttpPostedFileBase[] files, string provider)
		{
			var returnModel = new Models.ESS.ProviderViewModel();
			try
			{
				var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ServiceProviderDetailsViewModel>(provider);

				if (!String.IsNullOrWhiteSpace(model.ContactPhoneAreaCode) || !String.IsNullOrWhiteSpace(model.ContactPhoneExchange) || !String.IsNullOrWhiteSpace(model.ContactPhoneNumber))
				{
					model.ContactPhone = $"({model.ContactPhoneAreaCode}) {model.ContactPhoneExchange}-{model.ContactPhoneNumber}";
				}
				model.Region = null;
				model.CountryId = "CA";

				ModelState.Clear();
				TryUpdateModel(model);
				ModelState.Remove("OtherZipCode");
				ModelState.Remove("OtherRegion");

				if (ModelState.IsValid)
				{
					var trainingProvider = model.MapProperties(
												_grantApplicationService,
												_trainingProgramService,
												_trainingProviderService,
												_attachmentService,
												_eligibleCostService,
												_eligibleExpenseTypeService,
												_staticDataService,
												User,
												files);

					_trainingProviderService.Add(trainingProvider);
					returnModel = new Models.ESS.ProviderViewModel(trainingProvider, User);
				}
				else
				{
					HandleModelStateValidation(returnModel);
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, returnModel);
			}

			return Json(returnModel);
		}

		/// <summary>
		/// Delete the specified training provider from the datasource.
		/// </summary>
		/// <param name="trainingProviderId"></param>
		/// <param name="rowVersion"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Provider/Delete/{trainingProviderId}")]
		public JsonResult DeleteProvider(int trainingProviderId, string rowVersion)
		{
			var model = new BaseViewModel();
			try
			{
				var trainingProvider = _trainingProviderService.Get(trainingProviderId);
				var grantApplication = trainingProvider.GetGrantApplication();

				trainingProvider.RowVersion = Convert.FromBase64String(rowVersion.Replace(" ", "+"));
				_trainingProviderService.Delete(trainingProvider);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		/// <summary>
		/// Approve the training provider change request.
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Provider/Approve")]
		public JsonResult ApproveProvider(Models.ESS.ProviderViewModel model)
		{
			try
			{
				var trainingProvider = _trainingProviderService.Get(model.Id);
				trainingProvider.RowVersion = Convert.FromBase64String(model.RowVersion);
				trainingProvider.TrainingProviderState = trainingProvider.TrainingProviderState == TrainingProviderStates.RequestApproved ? TrainingProviderStates.Requested : TrainingProviderStates.RequestApproved;
				_trainingProviderService.Update(trainingProvider);

				model = new Models.ESS.ProviderViewModel(trainingProvider, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			var jsonResult = Json(model);
			jsonResult.MaxJsonLength = int.MaxValue;
			return jsonResult;
		}

		/// <summary>
		/// Deny the training provider change request.
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Provider/Deny")]
		public JsonResult DenyProvider(Models.ESS.ProviderViewModel model)
		{
			try
			{
				var trainingProvider = _trainingProviderService.Get(model.Id);
				trainingProvider.RowVersion = Convert.FromBase64String(model.RowVersion);
				trainingProvider.TrainingProviderState = trainingProvider.TrainingProviderState == TrainingProviderStates.RequestDenied ? TrainingProviderStates.Requested : TrainingProviderStates.RequestDenied;
				_trainingProviderService.Update(trainingProvider);

				model = new Models.ESS.ProviderViewModel(trainingProvider, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			var jsonResult = Json(model);
			jsonResult.MaxJsonLength = int.MaxValue;
			return jsonResult;
		}
		#endregion
		#endregion
	}
}
