using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Int.Models;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Int.Controllers
{
	/// <summary>
	/// WDAServiceController class, provides endpoints to manage grant programs.
	/// </summary>
	[AuthorizeAction(Privilege.GM1, Privilege.SM)]
	[RouteArea("Int")]
	[RoutePrefix("WDA")]
	public class WDAServiceController : BaseController
	{
		#region Variables
		private readonly IServiceCategoryService _serviceCategoryService;
		private readonly IServiceLineService _serviceLineService;
		private readonly IServiceLineBreakdownService _serviceLineBreakdownService;
		private readonly IStaticDataService _staticDataService;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a WDAServiceController object and initializes it.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="serviceCategoryService"></param>
		/// <param name="serviceLineService"></param>
		/// <param name="grantOpeningService"></param>
		/// <param name="staticDataService"></param>
		public WDAServiceController(
			IControllerService controllerService,
			IServiceCategoryService serviceCategoryService,
			IServiceLineService serviceLineService,
			IServiceLineBreakdownService serviceLineBreakdownService,
			IStaticDataService staticDataService
		   ) : base(controllerService.Logger)
		{
			_serviceCategoryService = serviceCategoryService;
			_serviceLineService = serviceLineService;
			_serviceLineBreakdownService = serviceLineBreakdownService;
			_staticDataService = staticDataService;

		}
		#endregion

		#region Endpoints
		/// <summary>
		/// This is the default view to perform CRUD on service categories, service lines and service line breakdowns.
		/// </summary>
		/// <returns></returns>
		[Route("Services/View")]
		public ActionResult WDAServicesView()
		{
			return View();
		}

		/// <summary>
		/// Get an array of service types.
		/// </summary>
		/// <returns></returns>
		[Route("Service/Types")]
		public ActionResult GetServiceTypes()
		{
			var data = _staticDataService.GetServiceTypes().Select(t => new 
			{
				t.Id,
				t.Caption
			});

			return Json(data, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// AJAX endpoint to return the WDA services.
		/// </summary>
		/// <returns></returns>
		[Route("Services")]
		public JsonResult GetWDAServices()
		{
			var viewModel = new WDAServiceViewModel();
			try
			{
				var serviceCategories = _serviceCategoryService.Get();
				viewModel = new WDAServiceViewModel(serviceCategories);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}

			return Json(viewModel, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// AJAX endpoint to update the WDA services.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut, ValidateRequestHeader, PreventSpam]
		[Route("Services")]
		public JsonResult UpdateWDAServices(WDAServiceViewModel model)
		{
			try
			{
				var i = 0;
				foreach (var category in model.ServiceCategories)
				{
					if (category.IsActive && category.ServiceTypeId != ServiceTypes.Administration && !category.ServiceLines.Any(sl => sl.IsActive))
					{
						ModelState.AddModelError($"ServiceCategories[{i}].ServiceTypeId", $"At least one active service line is required.");
					}

					// Skills training components require training programs.
					if (category.ServiceTypeId == ServiceTypes.SkillsTraining && category.MaxPrograms <= 0)
					{
						ModelState.AddModelError($"ServiceCategories[{i}].MaxPrograms", "Must be greater than 0.");
					}

					// Employment services and supports may not include providers.
					if (category.ServiceTypeId == ServiceTypes.EmploymentServicesAndSupports && category.MinProviders > 0 && category.MaxProviders <= 0)
					{
						ModelState.AddModelError($"ServiceCategories[{i}].MaxProviders", "Must be greater than 0 if providers are required.");
					}

					i++;
				}

				if (ModelState.IsValid)
				{
					var categories = new List<ServiceCategory>();

					foreach (var serviceCategory in model.ServiceCategories)
					{
						var category = serviceCategory.Id == 0
							? new ServiceCategory(serviceCategory.Caption, serviceCategory.ServiceTypeId.Value, serviceCategory.AutoInclude, serviceCategory.AllowMultiple, serviceCategory.MinProviders, serviceCategory.MaxProviders, serviceCategory.MinPrograms, serviceCategory.MaxPrograms, serviceCategory.CompletionReport)
							: _serviceCategoryService.Get(serviceCategory.Id);

						Utilities.MapProperties(serviceCategory, category);
						if (serviceCategory.Deleted)
						{
							_serviceCategoryService.Remove(category);
						}
						else
						{
							categories.Add(category);
						}

						foreach (var serviceLine in serviceCategory.ServiceLines)
						{
							var line = serviceLine.Id == 0
								? new ServiceLine(serviceLine.Caption, serviceLine.BreakdownCaption, serviceCategory.Id, serviceLine.EnableCost)
								: _serviceLineService.Get(serviceLine.Id);

							Utilities.MapProperties(serviceLine, line);

							if (serviceLine.Id == 0)
							{
								category.ServiceLines.Add(line);
							}
							else if (serviceLine.Deleted)
							{
								_serviceLineService.Remove(line);
							}

							foreach (var breakdown in serviceLine.ServiceLineBreakdowns)
							{
								var down = breakdown.Id == 0
									? new ServiceLineBreakdown(breakdown.Caption, serviceLine.Id)
									: _serviceLineBreakdownService.Get(breakdown.Id);

								Utilities.MapProperties(breakdown, down);

								if (breakdown.Id == 0)
								{
									line.ServiceLineBreakdowns.Add(down);
								}
								else if (breakdown.Deleted)
								{
									_serviceLineBreakdownService.Remove(down);
								}
							}
						}

					}
					var serviceCategories = _serviceCategoryService.BulkAddUpdate(categories);
					model = new WDAServiceViewModel(serviceCategories);
				}
				else
				{
					HandleModelStateValidation(model);
				}   
			}
			catch (Exception ex)
			{
				if (ex is DbUpdateException && (ex.InnerException.InnerException?.Message.Contains("IX_Caption") ?? false))
				{
					ex = new InvalidOperationException("Caption must be unique");
				}

				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		/// <summary>
		/// A view to display the service line HTML text names and descriptions.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost, Route("Service/Preview")]
		public ActionResult PreviewWDAServicesView(WDAServiceViewModel model)
		{
			return PartialView(model);
		}
		#endregion
	}
}
