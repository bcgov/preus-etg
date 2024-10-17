using CJG.Core.Interfaces.Service;
using CJG.Web.External.Areas.Int.Models.Agreements;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using System;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Int.Controllers
{
	[RouteArea("Int")]
	[Authorize(Roles = "Assessor, System Administrator, Director, Financial Clerk")]
	public class GrantAgreementController : BaseController
	{
		#region Variables
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IGrantAgreementService _grantAgreementService;
		#endregion

		#region Constructors
		public GrantAgreementController(
			IControllerService controllerService,
			IGrantAgreementService grantAgreementService,
			IGrantApplicationService grantApplicationService
		   ) : base(controllerService.Logger)
		{
			_grantAgreementService = grantAgreementService;
			_grantApplicationService = grantApplicationService;
		}
		#endregion

		#region Endpoints
		/// <summary>
		/// Returns the application Agreement view.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet, Route("Application/Agreement/View/{grantApplicationId}")]
		public ActionResult AgreementView(int grantApplicationId)
		{
			ViewBag.GrantApplicationId = grantApplicationId;
			return View();
		}

		/// <summary>
		/// Get the application Agreement view data.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Agreement/{grantApplicationId}")]
		public JsonResult GetAgreement(int grantApplicationId)
		{
			var model = new GrantAgreementViewModel();

			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new GrantAgreementViewModel(grantApplication);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns the Agreement Cover Letter view.
		/// </summary>
		/// <returns></returns>
		[HttpGet, Route("Application/Agreement/Cover/Letter/View")]
		public PartialViewResult CoverLetterView()
		{
			return PartialView();
		}

		/// <summary>
		/// Get the Agreement Cover Letter view data.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Agreement/Cover/Letter/{grantApplicationId}/{version?}")]
		public JsonResult GetCoverLetter(int grantApplicationId, int? version)
		{
			var model = new GrantAgreementCoverLetterViewModel();

			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new GrantAgreementCoverLetterViewModel(grantApplication, version);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns the Agreement Schedule A view.
		/// </summary>
		/// <returns></returns>
		[HttpGet, Route("Application/Agreement/ScheduleA/View")]
		public PartialViewResult ScheduleAView()
		{
			return PartialView();
		}

		/// <summary>
		/// Get the Agreement Schedule A view data.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Agreement/ScheduleA/{grantApplicationId}/{version?}")]
		public JsonResult GetScheduleA(int grantApplicationId, int? version)
		{
			var model = new GrantAgreementScheduleAViewModel();

			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new GrantAgreementScheduleAViewModel(grantApplication, version);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}

			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Returns the Agreement Schedule B view.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet, Route("Application/Agreement/ScheduleB/View")]
		public PartialViewResult ScheduleBView()
		{
			return PartialView();
		}

		/// <summary>
		/// Get the Agreement Schedule B view data.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Application/Agreement/ScheduleB/{grantApplicationId}/{version?}")]
		public JsonResult GetScheduleB(int grantApplicationId, int? version)
		{
			var model = new GrantAgreementScheduleBViewModel();

			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new GrantAgreementScheduleBViewModel(grantApplication, version);
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
