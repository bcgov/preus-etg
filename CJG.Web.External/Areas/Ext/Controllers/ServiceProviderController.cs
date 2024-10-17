using CJG.Core.Interfaces.Service;
using CJG.Core.Interfaces.Service.Settings;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Areas.Ext.Models.TrainingProviders;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Ext.Controllers
{
	/// <summary>
	/// ServiceProviderController class, controller for CRUD API for service providers.
	/// </summary>
	[RouteArea("Ext")]
	[ExternalFilter]
	public class ServiceProviderController : BaseController
	{
		#region Variables
		private readonly ITrainingProviderSettings _trainingProviderSettings;
		private readonly IStaticDataService _staticDataService;
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly ITrainingProgramService _trainingProgramService;
		private readonly ITrainingProviderService _trainingProviderService;
		private readonly IAttachmentService _attachmentService;
		private readonly IEligibleCostService _eligibleCostService;
		private readonly IEligibleExpenseTypeService _eligibleExpenseTypeService;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a ServiceProviderController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="staticDataService"></param>
		/// <param name="grantApplicationService"></param>
		/// <param name="trainingProviderSettings"></param>
		/// <param name="trainingProviderService"></param>
		/// <param name="trainingProgramService"></param>
		/// <param name="attachmentService"></param>
		/// <param name="eligibleCostService"></param>
		/// <param name="eligibleExpenseTypeService"></param>
		public ServiceProviderController(
			IControllerService controllerService,
			IStaticDataService staticDataService,
			IGrantApplicationService grantApplicationService,
			ITrainingProviderSettings trainingProviderSettings,
			ITrainingProviderService trainingProviderService,
			ITrainingProgramService trainingProgramService,
			IAttachmentService attachmentService,
			IEligibleCostService eligibleCostService,
			IEligibleExpenseTypeService eligibleExpenseTypeService
			) : base(controllerService.Logger)
		{
			_trainingProviderSettings = trainingProviderSettings;
			_staticDataService = staticDataService;
			_grantApplicationService = grantApplicationService;
			_trainingProgramService = trainingProgramService;
			_trainingProviderService = trainingProviderService;
			_attachmentService = attachmentService;
			_eligibleCostService = eligibleCostService;
			_eligibleExpenseTypeService = eligibleExpenseTypeService;
		}
		#endregion

		#region Endpoints
		/// <summary>
		/// Return a view for service provider.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="eligibleExpenseTypeId"></param>
		/// <param name="trainingProviderId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/{grantApplicationId:int}/Service/Provider/View/{eligibleExpenseTypeId:int}/{trainingProviderId:int}")]
		public ActionResult ServiceProviderView(int grantApplicationId, int eligibleExpenseTypeId, int trainingProviderId)
		{
			_grantApplicationService.Get(grantApplicationId);
			ViewBag.GrantApplicationId = grantApplicationId;
			ViewBag.EligibleExpenseTypeId = eligibleExpenseTypeId;
			ViewBag.TrainingProviderId = trainingProviderId;
			return View();
		}

		/// <summary>
		/// Get the data for the service provider.
		/// </summary>
		/// <param name="trainingProviderId"></param>
		/// <param name="grantApplicationId"></param>
		/// <param name="eligibleExpenseTypeId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Service/Provider/{trainingProviderId:int}/{grantApplicationId:int}/{eligibleExpenseTypeId:int}")]
		public JsonResult GetServiceProvider(int trainingProviderId, int grantApplicationId, int eligibleExpenseTypeId)
		{
			var viewModel = new TrainingServiceProviderViewModel();
			try
			{
				if (trainingProviderId > 0)
				{
					var provider = _trainingProviderService.Get(trainingProviderId);
					var eligibleExpenseType = _eligibleExpenseTypeService.Get(eligibleExpenseTypeId);
					viewModel = new TrainingServiceProviderViewModel(provider, eligibleExpenseType);

				}
				else
				{
					var grantApplication = _grantApplicationService.Get(grantApplicationId);
					var eligibleExpenseType = _eligibleExpenseTypeService.Get(eligibleExpenseTypeId);
					viewModel = new TrainingServiceProviderViewModel(grantApplication, eligibleExpenseType);
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}
			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Update the service provider on the datasource.
		/// </summary>
		/// <param name="files"></param>
		/// <param name="component"></param>
		/// <returns></returns>
		[HttpPut]
		[Route("Service/Provider")]
		public JsonResult UpdateServiceProvider(HttpPostedFileBase[] files, string component)
		{
			var model = new TrainingServiceProviderViewModel();
			try
			{
				model = JsonConvert.DeserializeObject<TrainingServiceProviderViewModel>(component);
				var grantApplication = _grantApplicationService.Get(model.GrantApplicationId);

				model.ServiceProvider.ContactPhone = string.Format(
					"({0}) {1}-{2}",
					model.ServiceProvider.ContactPhoneAreaCode,
					model.ServiceProvider.ContactPhoneExchange,
					model.ServiceProvider.ContactPhoneNumber);

				TryUpdateModel(model);
				TryUpdateModel(model.ServiceProvider);

				if (model.ServiceProvider.IsCanadianAddress)
				{
					ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.OtherZipCode));
					ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.OtherRegion));
				}
				else
				{
					ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.PostalCode));
					ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.RegionId));
				}

				ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.AddressLine1TrainingProvider));
				ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.AddressLine2TrainingProvider));
				ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.CityTrainingProvider));
				ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.RegionIdTrainingProvider));
				ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.PostalCodeTrainingProvider));
				ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.OtherZipCodeTrainingProvider));
				ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.OtherRegionTrainingProvider));

				if (ModelState.IsValid)
				{
					grantApplication.RowVersion = Convert.FromBase64String(model.RowVersion);
					var trainingProvider = model.ServiceProvider.MapProperties(
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
					model = new TrainingServiceProviderViewModel(trainingProvider);
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
		/// Add a new service provider to the datasource.
		/// </summary>
		/// <param name="files"></param>
		/// <param name="component"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("Service/Provider")]
		public JsonResult AddServiceProvider(HttpPostedFileBase[] files, string component)
		{
			var model = new TrainingServiceProviderViewModel();
			try
			{
				model = JsonConvert.DeserializeObject<TrainingServiceProviderViewModel>(component);
				var grantApplication = _grantApplicationService.Get(model.GrantApplicationId);

				model.ServiceProvider.ContactPhone = string.Format(
					"({0}) {1}-{2}",
					model.ServiceProvider.ContactPhoneAreaCode,
					model.ServiceProvider.ContactPhoneExchange,
					model.ServiceProvider.ContactPhoneNumber);

				TryUpdateModel(model);
				TryUpdateModel(model.ServiceProvider);

				if (model.ServiceProvider.IsCanadianAddress)
				{
					ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.OtherZipCode));
					ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.OtherRegion));
				}
				else
				{
					ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.PostalCode));
					ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.RegionId));
				}

				ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.AddressLine1TrainingProvider));
				ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.AddressLine2TrainingProvider));
				ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.CityTrainingProvider));
				ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.RegionIdTrainingProvider));
				ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.PostalCodeTrainingProvider));
				ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.OtherZipCodeTrainingProvider));
				ModelState.Remove(nameof(Models.TrainingProviders.TrainingProviderViewModel.OtherRegionTrainingProvider));

				if (ModelState.IsValid)
				{
					grantApplication.RowVersion = Convert.FromBase64String(model.RowVersion);
					var trainingProvider = model.ServiceProvider.MapProperties(
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
					model = new TrainingServiceProviderViewModel(trainingProvider);
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
		/// Get an array of training provide types.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[ValidateRequestHeader]
		[PreventSpam]
		[Route("Service/Provider/Types")]
		public JsonResult GetTrainingProviderTypes()
		{
			var _dataSource = new List<TrainingProviderTypeViewModel>();
			try
			{
				_dataSource = _staticDataService.GetTrainingProviderTypes().Select(x => new TrainingProviderTypeViewModel(x)).ToList();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(_dataSource, JsonRequestBehavior.AllowGet);
		}
		#endregion
	}
}