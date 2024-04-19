using System;
using System.Linq;
using System.Web.Mvc;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Ext.Models;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;

namespace CJG.Web.External.Areas.Ext.Controllers
{
    /// <summary>
    /// ApplicationViewController class, provides a controller endpoints for managing external user grant applications.
    /// </summary>
    [RouteArea("Ext")]
	[ExternalFilter]
	public class ApplicationViewController : BaseController
	{
		private readonly IUserService _userService;
		private readonly IGrantApplicationService _grantApplicationService;

		/// <summary>
		/// Creates a new instance of a ApplicationViewController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantApplicationService"></param>
		public ApplicationViewController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService) : base(controllerService.Logger)
		{
			_userService = controllerService.UserService;
			_grantApplicationService = grantApplicationService;
		}

		/// <summary>
		/// This view is to display the application once it is submitted.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Details/View/{grantApplicationId}")]
		public ActionResult ApplicationDetailsView(int grantApplicationId)
		{
			ViewBag.GrantApplicationId = grantApplicationId;
			var grantApplication = _grantApplicationService.Get(grantApplicationId);
			return View(SidebarViewModelFactory.Create(grantApplication, ControllerContext));
		}

		/// <summary>
		/// Get the data for the ApplicationDetailsView.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Application/Details/{grantApplicationId}")]
		public JsonResult GetApplicationDetails(int grantApplicationId)
		{
			ApplicationViewModel viewModel = null;
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				var userId = grantApplication.BusinessContactRoles.FirstOrDefault()?.UserId;
				viewModel = new ApplicationViewModel(grantApplication, userId == null ? null : _userService.GetUser((int)userId));
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, viewModel);
			}
			var jsonResult = Json(viewModel, JsonRequestBehavior.AllowGet);
			jsonResult.MaxJsonLength = int.MaxValue;
			return jsonResult;
		}

		/// <summary>
		/// Display the Grant Application withdraw View.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Withdraw/View/{grantApplicationId}")]
		public ActionResult WithdrawApplicationView(int grantApplicationId)
		{
			var grantApplication = _grantApplicationService.Get(grantApplicationId);
			ViewBag.GrantApplicationId = grantApplicationId;
			ViewBag.RowVersion = Convert.ToBase64String(grantApplication.RowVersion);
			return PartialView();
		}

		/// <summary>
		/// Update the grant application and attempt to withdraw it.
		/// Display the Grant Application withdraw View.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Application/Withdraw")]
		public JsonResult WithdrawApplication(Models.Applications.WithdrawApplicationViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var grantApplication = _grantApplicationService.Get(model.Id);
					grantApplication.RowVersion = Convert.FromBase64String(model.RowVersion);
					_grantApplicationService.Withdraw(grantApplication, model.WithdrawReason);

					model = new Models.Applications.WithdrawApplicationViewModel(grantApplication);
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
	}
}
