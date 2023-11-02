using System;
using System.Web.Mvc;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Int.Models;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;

namespace CJG.Web.External.Areas.Int.Controllers
{
    /// <summary>
    /// <paramtyperef name="ClaimController"/> class, provides endpoints to manage the assessment of claims.
    /// </summary>
    [Authorize(Roles = "Assessor, Director, Financial Clerk, System Administrator")]
	[RouteArea("Int")]
	public class ClaimController : BaseController
	{
		private readonly IGrantApplicationService _grantApplicationService;

		/// <summary>
		/// Creates a new instance of a ClaimController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantApplicationService"></param>
		public ClaimController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService
		   ) : base(controllerService.Logger)
		{
			_grantApplicationService = grantApplicationService;
		}

		/// <summary>
		/// Get the claims for the specified grant application on the application details view.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Claims/{grantApplicationId}")]
		public JsonResult GetClaims(int grantApplicationId)
		{
			var model = new ClaimListViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new ClaimListViewModel(grantApplication, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Update the grant application and either hold or release payment requests.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="rowVersion"></param>
		/// <param name="hold"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[AuthorizeAction(Privilege.IA1)]
		[Route("Application/Claim/Hold/Payment/Requests/{grantApplicationId}")]
		public JsonResult HoldPaymentRequests(int grantApplicationId, string rowVersion, bool hold) // TODO: This should be a model so that 'rowVersion' isn't corrupted in the query string.
		{
			var model = new Models.Claims.HoldPaymentRequestViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);

				if (grantApplication.HoldPaymentRequests != hold)
				{
					grantApplication.RowVersion = Convert.FromBase64String(rowVersion.Replace("+", " "));
					grantApplication.HoldPaymentRequests = hold;
					_grantApplicationService.Update(grantApplication, ApplicationWorkflowTrigger.HoldPaymentRequests); 
				}

				model = new Models.Claims.HoldPaymentRequestViewModel(grantApplication);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}
	}
}
