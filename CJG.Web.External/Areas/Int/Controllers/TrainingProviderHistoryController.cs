using CJG.Application.Services;
using CJG.Core.Entities.Helpers;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Int.Models.TrainingProviders;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Int.Controllers
{
	/// <summary>
	/// <typeparamref name="TrainingProviderHistoryController"/> class, provides endpoints to manage Training Provider History.
	/// </summary>
	[RouteArea("Int")]
	[RoutePrefix("Training/Provider")]
	[AuthorizeAction(Privilege.AM1, Privilege.AM2, Privilege.AM3, Privilege.AM4, Privilege.AM5)]
	public class TrainingProviderHistoryController : BaseController
	{
		private readonly ITrainingProviderInventoryService _trainingProviderInventoryService;
		private readonly IAuthorizationService _authorizationService;
		private readonly IGrantApplicationService _grantApplicationService;

		/// <summary>
		/// Creates a new instance of a <typeparamref name="TrainingProviderHistoryController"/> object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="trainingProviderInventoryService"></param>
		/// <param name="authorizationService"></param>
		/// <param name="grantApplicationService"></param>
		public TrainingProviderHistoryController(
			IControllerService controllerService,
			ITrainingProviderInventoryService trainingProviderInventoryService,
			IAuthorizationService authorizationService,
			IGrantApplicationService grantApplicationService) : base(controllerService.Logger)
		{
			_trainingProviderInventoryService = trainingProviderInventoryService;
			_authorizationService = authorizationService;
			_grantApplicationService = grantApplicationService;
		}

		#region Endpoints
		/// <summary>
		/// Returns the training provider grant file history view.
		/// </summary>
		/// <param name="trainingProviderId"></param>
		/// <returns></returns>
		[HttpGet, Route("History/View/{trainingProviderId}")]
		public ActionResult TrainingProviderGrantFileHistoryView(int trainingProviderId)
		{
			ViewBag.TrainingProviderId = trainingProviderId;
			return View();
		}

		/// <summary>
		/// Get the training provider grant file history view data.
		/// </summary>
		/// <param name="trainingProviderId"></param>
		/// <returns></returns>
		[HttpGet, Route("History/{trainingProviderId}")]
		public JsonResult GetTrainingProviderGrantFileHistory(int trainingProviderId)
		{
			var model = new TrainingProviderGrantFileHistoryViewModel();
			try
			{
				var trainingProviderInventory = _trainingProviderInventoryService.Get(trainingProviderId);

				model = new TrainingProviderGrantFileHistoryViewModel(trainingProviderInventory)
				{
					AllowDeleteTrainingProvider = User.HasPrivilege(Privilege.TP2)
					                              && _grantApplicationService.GetTotalGrantApplications(trainingProviderId) == 0,
					UrlReferrer = Request.UrlReferrer?.AbsolutePath ?? new UrlHelper(ControllerContext.RequestContext).Action("TrainingProvidersView", "TrainingProviderInventory")
				};
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get grant file histories for training provider inventory.
		/// </summary>
		/// <param name="trainingProviderInventoryId"></param>
		/// <param name="page"></param>
		/// <param name="quantity"></param>
		/// <param name="search"></param>
		/// <returns></returns>
		[HttpGet, Route("History/Search/{trainingProviderInventoryId}/{page}/{quantity}")]
		[ValidateRequestHeader]
		[AuthorizeAction(Privilege.IA1)]
		public JsonResult GetTrainingProviderGrantFileHistory(int trainingProviderInventoryId, int page, int quantity, string search, string sortby, bool sortDesc)
		{
			var model = new BaseViewModel();
			try
			{
				var grantApplications = _grantApplicationService.GetGrantApplications(trainingProviderInventoryId, search);
				var orderBy = "FileNumber desc";

				if (string.IsNullOrEmpty(sortby) == false)
					orderBy = sortDesc ? sortby + " desc" : sortby + " asc";

				var trainingProviderGrants = grantApplications.ToList()
					.Select(o => new TrainingProviderGrantFileHistoryDataTableModel(o))
					.AsQueryable();

				var filtered = trainingProviderGrants
					.OrderByProperty(orderBy)
					.Skip((page - 1) * quantity)
					.Take(quantity)
					.ToArray();

				var result = new
				{
					RecordsFiltered = filtered.Count(),
					RecordsTotal = trainingProviderGrants.Count(),
					Data = filtered
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
		/// Update the training provider inventory notes.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="notesText"></param>
		/// <param name="riskFlag"></param>
		/// <param name="rowVersion"></param>
		/// <returns></returns>
		[HttpPut, Route("History/Note/{id}")]
		[AuthorizeAction(Privilege.TP1, Privilege.TP2)]
		public JsonResult UpdateNote(int id, string notesText, bool riskFlag, string rowVersion)
		{
			var model = new TrainingProviderGrantFileHistoryViewModel();
			try
			{
				var trainingProviderInventory = _trainingProviderInventoryService.Get(id);

				trainingProviderInventory.RowVersion = Convert.FromBase64String(rowVersion.Replace(" ", "+"));
				trainingProviderInventory.Notes = notesText;
				trainingProviderInventory.RiskFlag = riskFlag;

				_trainingProviderInventoryService.Update(trainingProviderInventory);

				model.RowVersion = Convert.ToBase64String(trainingProviderInventory.RowVersion);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}
		#endregion
	}
}
