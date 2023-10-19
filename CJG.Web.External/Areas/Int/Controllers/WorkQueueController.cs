using CJG.Application.Services;
using CJG.Core.Entities.Helpers;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Int.Controllers
{
	/// <summary>
	/// WorkQueueController class, provides endpoints to select and search for grant applications.
	/// </summary>
	[RouteArea("Int")]
	[Authorize(Roles = "Assessor, System Administrator, Director, Financial Clerk")]
	public class WorkQueueController : BaseController
	{
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IAuthorizationService _authorizationService;
		private readonly IInternalUserFilterService _internalUserFilterService;
		private readonly IFiscalYearService _fiscalYearService;
		private readonly IGrantProgramService _grantProgramService;
		private readonly IGrantStreamService _grantStreamService;

		public WorkQueueController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			IAuthorizationService authorizationService,
			IInternalUserFilterService internalUserFilterService,
			IFiscalYearService fiscalYearService,
			IGrantProgramService grantProgramService,
			IGrantStreamService grantStreamService
		   ) : base(controllerService.Logger)
		{
			_grantApplicationService = grantApplicationService;
			_internalUserFilterService = internalUserFilterService;
			_authorizationService = authorizationService;
			_fiscalYearService = fiscalYearService;
			_grantProgramService = grantProgramService;
			_grantStreamService = grantStreamService;
		}

		#region Endpoints
		#region Work Queue
		[HttpGet]
		[Route("Work/Queue/View", Name = nameof(WorkQueueView))]
		public ActionResult WorkQueueView()
		{
			var userId = User.GetUserId();
			return View(userId);
		}

		[HttpGet]
		[Route("Work/Queue/Assessors")]
		public JsonResult GetAssessors()
		{
			var results = new KeyValuePair<int, string>[0];
			try
			{
				var assessors = _authorizationService.GetAssessors();
				results = assessors.Select(a => new KeyValuePair<int, string>(a.Id, $"{a.FirstName} {a.LastName}")).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(results, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Route("Work/Queue/Fiscal/Years")]
		public JsonResult GetFiscalYears()
		{
			var results = new KeyValuePair<int, string>[0];
			try
			{
				var fiscalYears = _fiscalYearService.GetFiscalYears();
				results = fiscalYears.Select(fy => new KeyValuePair<int, string>(fy.Id, fy.Caption)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(results, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Route("Work/Queue/Training/Periods/{fiscalYearId:int?}/{grantStreamId:int?}")]
		public JsonResult GetTrainingPeriods(int? fiscalYearId, int? grantStreamId)
		{
			IEnumerable<KeyValuePair<string, string>> results = new KeyValuePair<string, string>[0];
			try
			{
				results = _fiscalYearService.GetTrainingPeriodLabels(fiscalYearId, grantStreamId);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(results, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Route("Work/Queue/Grant/Programs")]
		public JsonResult GetGrantPrograms()
		{
			var results = new KeyValuePair<int, string>[0];
			try
			{
				var grantPrograms = _grantProgramService.GetAll();
				results = grantPrograms.Select(p => new KeyValuePair<int, string>(p.Id, p.Name)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(results, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Route("Work/Queue/Grant/Streams/{grantProgramId?}")]
		public JsonResult GetGrantStreams(int? grantProgramId)
		{
			var results = new KeyValuePair<int, string>[0];
			try
			{
				var grantStreams = grantProgramId.HasValue ? _grantStreamService.GetGrantStreamsForProgram(grantProgramId.Value) : _grantStreamService.GetAll();
				results = grantStreams.Select(s => new KeyValuePair<int, string>(s.Id, s.Name)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(results, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[ValidateRequestHeader]
		[Route("Work/Queue")]
		public JsonResult GetGrantApplications(Models.WorkQueue.WorkQueueFilterViewModel filter)
		{
			var model = new PageList<Models.WorkQueue.GrantApplicationViewModel>();
			try
			{
				int.TryParse(Request.QueryString["page"], out int pageNumber);
				int.TryParse(Request.QueryString["quantity"], out int quantityNumber);

				var query = filter.GetFilter(User);
				var applications = _grantApplicationService.GetGrantApplications(pageNumber, quantityNumber, query);

				model.Page = applications.Page;
				model.Quantity = applications.Quantity;
				model.Total = applications.Total;
				model.Items = applications.Items.Select(a => new Models.WorkQueue.GrantApplicationViewModel(a));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, filter);
				return Json(filter);
			}
			return Json(model);
		}
		#endregion

		#region Filters
		/// <summary>
		/// Get a view to edit or add the specified filter.
		/// </summary>
		/// <param name="filterId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Work/Queue/Filter/{filterId}")]
		public ActionResult FilterView(int filterId)
		{
			return PartialView();
		}

		/// <summary>
		/// Get all the filters for the current user.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[AuthorizeAction(Privilege.IA1)]
		[Route("Work/Queue/Filters")]
		public JsonResult GetFilters()
		{
			var model = new Models.WorkQueue.WorkQueueUserFiltersViewModel();
			try
			{
				var filters = _internalUserFilterService.GetForUser();
				model = new Models.WorkQueue.WorkQueueUserFiltersViewModel(filters);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Create a new filter for the current user.
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[PreventSpam]
		[ValidateRequestHeader]
		[AuthorizeAction(Privilege.IA1)]
		[Route("Work/Queue/Filter")]
		public JsonResult AddFilter(Models.WorkQueue.WorkQueueUserFilterViewModel model)
		{
			try
			{
				var filter = model.GetFilter(User);
				_internalUserFilterService.Add(filter);
				model = new Models.WorkQueue.WorkQueueUserFilterViewModel(filter);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model);
		}

		/// <summary>
		/// Update the specified filter.
		/// </summary>
		/// <param name="filterParameters"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[AuthorizeAction(Privilege.IA1)]
		[ValidateRequestHeader]
		[Route("Work/Queue/Filter")]
		public JsonResult UpdateFilter(Models.WorkQueue.WorkQueueUserFilterViewModel model)
		{
			try
			{
				var filter = _internalUserFilterService.Get(model.Id);
				Utilities.MapProperties(model, filter);

				// Update/Remove attributes.
				var attributes = filter.Attributes.ToArray();
				foreach (var attribute in attributes)
				{
					var update = model.Attributes.FirstOrDefault(a => a.Id == attribute.Id);
					if (update != null)
					{
						Utilities.MapProperties(update, attribute);
					}
					else
					{
						filter.Attributes.Remove(attribute);
					}
				}

				// Add new attributes.
				foreach (var attribute in model.Attributes.Where(a => a.Id == 0))
				{
					filter.AddAttribute(attribute.Key, attribute.Value, attribute.Operator);
				}

				_internalUserFilterService.Update(filter);
				model = new Models.WorkQueue.WorkQueueUserFilterViewModel(filter);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model);
		}

		/// <summary>
		/// Delete the specified filter.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[AuthorizeAction(Privilege.IA1)]
		[ValidateRequestHeader]
		[Route("Work/Queue/Filter/Delete")]
		public JsonResult DeleteFilter(Models.WorkQueue.WorkQueueUserFilterViewModel model)
		{
			try
			{
				var filter = _internalUserFilterService.Get(model.Id);
				_internalUserFilterService.Delete(filter);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model);
		}
		#endregion
		#endregion
	}
}