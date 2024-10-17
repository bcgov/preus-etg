using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Int.Models.TrainingProviders;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Controllers
{
    /// <summary>
    /// TrainingProviderInventoryController class, provides endpoints to manage TrainingProviders.
    /// </summary>
    [RouteArea("Int")]
	[RoutePrefix("Training/Provider")]
	[AuthorizeAction(Privilege.TP1)]
	public class TrainingProviderInventoryController : BaseController
	{
		#region Variables
		public static class TextMessages
		{
			public const string PromptCantDeleteProvider = "There are one or more grant files associated with this training provider. You will need to reassign these files to eligible providers before deletion is possible";
			public const string PromptConfirmDeleteProvider = "There are no grant files associated with this training provider. Deleting will remove this training provider from the master list, making it unavailable from assignment to applications";
			public const string NoPermissions = "You don't have permissions to change status of Training Provider";
		}

		private readonly ITrainingProviderInventoryService _trainingProviderInventoryService;
		private readonly IAuthorizationService _authorizationService;
		private readonly IGrantApplicationService _grantApplicationService;
		#endregion

		#region Constructors
		public TrainingProviderInventoryController(
			IControllerService controllerService,
			ITrainingProviderInventoryService trainingProviderInventoryService,
			IAuthorizationService authorizationService,
			IGrantApplicationService grantApplicationService) : base(controllerService.Logger)
		{
			_trainingProviderInventoryService = trainingProviderInventoryService;
			_authorizationService = authorizationService;
			_grantApplicationService = grantApplicationService;
		}
		#endregion

		#region Endpoints
		[HttpGet, Route("Inventory/View")]
		public ActionResult TrainingProvidersView()
		{
			//
			// Launched when the 'Training Providers' link on the DashBoard is clicked
			//
			return View();
		}

		[HttpGet, Route("Inventory")]
		public JsonResult GetTrainingProvider()
		{
			var model = new TrainingProvidersInventoryViewModel();
			try
			{
				model = new TrainingProvidersInventoryViewModel(User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		[HttpGet, Route("Inventory/Form/View")]
		public ActionResult TrainingProviderView()
		{
			return PartialView("_TrainingProviderDetails");
		}

		/// <summary>
		/// Get training providers from inventory.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="quantity"></param>
		/// <param name="search"></param>
		/// <param name="riskFlag"></param>
		/// <returns></returns>
		[HttpGet, Route("Inventory/Search/{page}/{quantity}")]
		public JsonResult GetInventory(int page, int quantity, string search, bool? riskFlag)
		{
			var model = new BaseViewModel();
			try
			{
				var inventory = _trainingProviderInventoryService.GetInventory(page, quantity, search, isRisk: riskFlag);
				var result = new
				{
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

		[HttpPost, Route("Inventory")]
		[AuthorizeAction(Privilege.TP1, Privilege.TP2)]
		public JsonResult CreateTrainingProviderInventory(TrainingProviderInventoryViewModel model)
		{
			//
			// Launched when the 'Save' button is clicked on the 'Training provider' inline popup
			//
			try
			{
				int existingTrainingProviderId = 0;
				var strActiveState = "";
				var trainingProvider = _trainingProviderInventoryService.GetTrainingProviderFromInventory(model.Name);
				if (trainingProvider != null)
				{
					existingTrainingProviderId = trainingProvider.Id;
					strActiveState = trainingProvider.IsActive ? "active" : "inactive";
				}

				if (existingTrainingProviderId == 0)
				{
					// The user has specified a new name
					var newTrainingProviderInventory = new TrainingProviderInventory
					{
						IsActive = model.IsActive,
						IsEligible = model.IsEligible,
						Name = model.Name,
						Acronym = model.Acronym,
						Notes = CleanMultilineText(model.Notes)
					};

					_trainingProviderInventoryService.Add(newTrainingProviderInventory);
				}
				else
				{
					throw new InvalidOperationException("The Training Provider name (" + model.Name + ")  already exists for an " + strActiveState + " entry.");
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		[HttpPut, Route("Inventory")]
		[AuthorizeAction(Privilege.TP1, Privilege.TP2)]
		public JsonResult UpdateTrainingProviderInventory(TrainingProviderInventoryViewModel model)
		{
			//
			// Launched when the 'Save' button is clicked on the 'Training provider' inline popup
			//
			try
			{
				var trainingProviderInventory = _trainingProviderInventoryService.Get(model.Id);
				trainingProviderInventory.RowVersion = Convert.FromBase64String(model.RowVersion);

				if (trainingProviderInventory.Name == null) { trainingProviderInventory.Name = ""; }
				if (trainingProviderInventory.Notes == null) { trainingProviderInventory.Notes = ""; }

				if ((model.IsActive != trainingProviderInventory.IsActive || model.IsEligible != trainingProviderInventory.IsEligible) && !User.HasPrivilege(Privilege.TP2)
					|| model.Notes != trainingProviderInventory.Notes && !User.HasPrivilege(Privilege.TP1))
				{
					throw new NotAuthorizedException(TextMessages.NoPermissions);
				}

				var duplicateTrainingProvider = _trainingProviderInventoryService.GetTrainingProviderFromInventory(model.Name);
				if (duplicateTrainingProvider == null || duplicateTrainingProvider.Id == model.Id)
				{
					// no duplicate found
					trainingProviderInventory.Name = model.Name;
					trainingProviderInventory.Acronym = model.Acronym;
					trainingProviderInventory.Notes = CleanMultilineText(model.Notes);
					trainingProviderInventory.IsActive = model.IsActive;
					trainingProviderInventory.IsEligible = model.IsEligible;

					_trainingProviderInventoryService.Update(trainingProviderInventory);
				}
				else
				{
					throw new InvalidOperationException("The Training Provider name specified already exists for an " + (duplicateTrainingProvider.IsActive ? "active" : "inactive") + " entry.");
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}
		
		[HttpPut]
		[Route("Inventory/Delete")]
		[AuthorizeAction(Privilege.TP2)]
		public JsonResult DeleteTrainingProvider(TrainingProviderInventoryViewModel model)
		{
			//
			// Launched when the 'Delete' button is clicked on the 'Training provider' inline popup
			//
			string strMsgType = string.Empty;
			string strMsgText = string.Empty;

			try
			{
				var trainingProviderInventory = _trainingProviderInventoryService.Get(model.Id);
				trainingProviderInventory.RowVersion = Convert.FromBase64String(model.RowVersion);

				if (_grantApplicationService.GetTotalGrantApplications(model.Id) != 0)
				{
					throw new InvalidOperationException("The training provider cannot be deleted as there are grant files associated with it.");
				}

				_trainingProviderInventoryService.Delete(model.Id, ref strMsgType, ref strMsgText);

				switch (strMsgType)
				{
					case "W":
					case "E":
						throw new InvalidOperationException(strMsgText);
				}
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region Methods
		private static string CleanMultilineText(string text)
		{
			return string.IsNullOrWhiteSpace(text)
				? null
				: Regex.Replace(text.Trim(), "([^\r])\n", "$1\r\n");
		}
		#endregion
	}
}
