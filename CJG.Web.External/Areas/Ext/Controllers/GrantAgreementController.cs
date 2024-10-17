using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using System;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Ext.Controllers
{
	/// <summary>
	/// <typeparamref name="GrantAgreementController"/> class, provides a controller for grant agreement management.
	/// </summary>
	[RouteArea("Ext")]
	[ExternalFilter] 
	public class GrantAgreementController : BaseController
	{
		#region Variables
		private readonly IGrantApplicationService _grantApplicationService;
		private readonly IGrantAgreementService _grantAgreementService;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="GrantAgreementController"/> object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="grantApplicationService"></param>
		/// <param name="grantAgreementService"></param>
		public GrantAgreementController(
			IControllerService controllerService,
			IGrantApplicationService grantApplicationService,
			IGrantAgreementService grantAgreementService
			) : base(controllerService.Logger)
		{
			_grantApplicationService = grantApplicationService;
			_grantAgreementService = grantAgreementService;
		}
		#endregion

		#region Endpoints
		#region Review Agreement - Before Acceptance
		/// <summary>
		/// Return the review agreement view.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Agreement/Review/View/{grantApplicationId}")]
		public ActionResult AgreementReviewView(int grantApplicationId)
		{
			var grantApplication = _grantApplicationService.Get(grantApplicationId);

			if (grantApplication.ApplicationStateInternal.In(ApplicationStateInternal.OfferWithdrawn, ApplicationStateInternal.ApplicationWithdrawn, ApplicationStateInternal.CancelledByMinistry, ApplicationStateInternal.CancelledByAgreementHolder))
			{
				this.SetAlert("No further action can be taken on this application", AlertType.Warning, true);
				return RedirectToAction(nameof(ApplicationViewController.ApplicationDetailsView), nameof(ApplicationViewController).Replace("Controller", ""), new { grantApplicationId });
			}

			if (grantApplication.GrantAgreement != null)
			{
				ViewBag.GrantApplicationId = grantApplicationId;
				var model = SidebarViewModelFactory.Create(grantApplication, ControllerContext);
				return View(model);
			}

			return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", ""));
		}

		/// <summary>
		/// Get the agreement data for the review agreement view.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Agreement/Review/{grantApplicationId}")]
		public JsonResult GetAgreementReview(int grantApplicationId)
		{
			var model = new Models.Agreements.AgreementReviewViewModel();
			try
			{

				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new Models.Agreements.AgreementReviewViewModel(grantApplication);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);

		}

		/// <summary>
		/// Returns a view of the agreement coverletter so that applicant can accept it.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Agreement/CoverLetter/View/{grantApplicationId}")]
		public ActionResult CoverLetterView(int grantApplicationId)
		{
			var grantApplication = _grantApplicationService.Get(grantApplicationId);
			if (grantApplication.ApplicationStateInternal.In(ApplicationStateInternal.OfferWithdrawn, ApplicationStateInternal.ApplicationWithdrawn, ApplicationStateInternal.CancelledByMinistry, ApplicationStateInternal.CancelledByAgreementHolder))
			{
				this.SetAlert("No further action can be taken on this application", AlertType.Warning, true);
				return RedirectToAction(nameof(ApplicationViewController.ApplicationDetailsView), nameof(ApplicationViewController).Replace("Controller", ""), new { grantApplicationId });
			}

			ViewBag.GrantApplicationId = grantApplicationId;
			return View(SidebarViewModelFactory.Create(grantApplication, ControllerContext));
		}

		/// <summary>
		/// Returns a view of the agreement schedule A so that applicant can accept it.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Agreement/ScheduleA/View/{grantApplicationId}")]
		public ActionResult ScheduleAView(int grantApplicationId)
		{
			var grantApplication = _grantApplicationService.Get(grantApplicationId);
			if (grantApplication.ApplicationStateInternal.In(ApplicationStateInternal.OfferWithdrawn, ApplicationStateInternal.ApplicationWithdrawn, ApplicationStateInternal.CancelledByMinistry, ApplicationStateInternal.CancelledByAgreementHolder))
			{
				this.SetAlert("No further action can be taken on this application", AlertType.Warning, true);
				return RedirectToAction(nameof(ApplicationViewController.ApplicationDetailsView), nameof(ApplicationViewController).Replace("Controller", ""), new { grantApplicationId });
			}

			ViewBag.GrantApplicationId = grantApplicationId;
			return View(SidebarViewModelFactory.Create(grantApplication, ControllerContext));
		}

		/// <summary>
		/// Returns a view of the agreement schedule B so that applicant can accept it.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Agreement/ScheduleB/View/{grantApplicationId}")]
		public ActionResult ScheduleBView(int grantApplicationId)
		{
			var grantApplication = _grantApplicationService.Get(grantApplicationId);
			if (grantApplication.ApplicationStateInternal.In(ApplicationStateInternal.OfferWithdrawn, ApplicationStateInternal.ApplicationWithdrawn, ApplicationStateInternal.CancelledByMinistry, ApplicationStateInternal.CancelledByAgreementHolder))
			{
				this.SetAlert("No further action can be taken on this application", AlertType.Warning, true);
				return RedirectToAction(nameof(ApplicationViewController.ApplicationDetailsView), nameof(ApplicationViewController).Replace("Controller", ""), new { grantApplicationId });
			}

			ViewBag.GrantApplicationId = grantApplicationId;
			return View(SidebarViewModelFactory.Create(grantApplication, ControllerContext));
		}

		/// <summary>
		/// Get the agreement document data for the view.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <param name="document"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Agreement/{document}/{grantApplicationId}")]
		public JsonResult GetAgreementDocument(int grantApplicationId, string document)
		{
			var model = new Models.Agreements.GrantAgreementDocumentViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new Models.Agreements.GrantAgreementDocumentViewModel(grantApplication, document);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Accept the specified document.
		/// </summary>
		/// <param name="document"></param>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Agreement/Accept/{document}")]
		public JsonResult AcceptDocument(string document, Models.Agreements.GrantAgreementDocumentViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.Id);
				grantApplication.RowVersion = Convert.FromBase64String(model.RowVersion);
				switch (document)
				{
					case ("CoverLetter"):
						if (!model.Confirmation)
							AddAngularError(model, "Confirmation", "You must confirm the coverletter.");
						grantApplication.GrantAgreement.CoverLetterConfirmed = true;
						break;
					case ("ScheduleA"):
						if (!model.Confirmation)
							AddAngularError(model, "Confirmation", "You must confirm schedule A.");
						grantApplication.GrantAgreement.ScheduleAConfirmed = true;
						break;
					case ("ScheduleB"):
						if (!model.Confirmation)
							AddAngularError(model, "Confirmation", "You must confirm schedule B.");
						grantApplication.GrantAgreement.ScheduleBConfirmed = true;
						break;
				}

				if (ModelState.IsValid)
				{
					_grantAgreementService.Update(grantApplication.GrantAgreement);
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
		/// Accept the agreement and change the application state to Approved.
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Agreement/Accept")]
		public JsonResult AcceptAgreement(Models.Agreements.AgreementReviewViewModel model)
		{
			try
			{
				if (!model.CoverLetterConfirmed)
					ModelState.AddModelError("CoverLetterConfirmed", "You must accept the coverletter before accepting the agreement.");
				if (!model.ScheduleAConfirmed)
					ModelState.AddModelError("ScheduleAConfirmed", "You must accept schedule A before accepting the agreement.");
				if (!model.ScheduleBConfirmed)
					ModelState.AddModelError("ScheduleBConfirmed", "You must accept schedule B before accepting the agreement.");

				ModelState.Remove("IncompleteReason");

				if (ModelState.IsValid)
				{
					var grantApplication = _grantApplicationService.Get(model.GrantApplicationId);
					grantApplication.RowVersion = Convert.FromBase64String(model.RowVersion);
					_grantAgreementService.AcceptGrantAgreement(grantApplication);
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

		#region Reject Agreement
		/// <summary>
		/// A partial view to capture the reason to reject the agreement.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Agreement/Reject/View/{grantApplicationId}")]
		public ActionResult RejectAgreementView(int grantApplicationId)
		{
			ViewBag.GrantApplicationId = grantApplicationId;
			return PartialView("_RejectAgreement");
		}

		/// <summary>
		/// Reject the agreement and change the application state to AgreementRejected.
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Agreement/Reject")]
		public JsonResult RejectAgreement(Models.Agreements.AgreementReviewViewModel model)
		{
			try
			{
				var grantApplication = _grantApplicationService.Get(model.GrantApplicationId);

				if (ModelState.IsValid)
				{
					grantApplication.RowVersion = Convert.FromBase64String(model.RowVersion);
					_grantAgreementService.RejectGrantAgreement(grantApplication, model.IncompleteReason);
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
		#endregion
		#endregion

		#region View Agreement - After Acceptance
		/// <summary>
		/// A view to display the accepted grant agreement.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Agreement/Overview/View/{grantApplicationId}")]
		public ActionResult AgreementOverviewView(int grantApplicationId)
		{
			var grantApplication = _grantApplicationService.Get(grantApplicationId);
			ViewBag.GrantApplicationId = grantApplicationId;
			return View(SidebarViewModelFactory.Create(grantApplication, ControllerContext));
		}

		/// <summary>
		/// Get the grant agreement data.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Agreement/Overview/{grantApplicationId:int}")]
		public JsonResult GetAgreementOverview(int grantApplicationId)
		{
			var model = new Models.Agreements.AgreementOverviewViewModel();
			try
			{
				var grantApplication = _grantApplicationService.Get(grantApplicationId);
				model = new Models.Agreements.AgreementOverviewViewModel(grantApplication, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		#region Cancel Agreement
		/// <summary>
		/// A partial view to capture the reason to cancel the agreement.
		/// </summary>
		/// <param name="grantApplicationId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Agreement/Cancel/View/{grantApplicationId}")]
		public ActionResult CancelAgreementView(int grantApplicationId)
		{
			ViewBag.GrantApplicationId = grantApplicationId;
			return PartialView("_CancelAgreement");
		}

		/// <summary>
		/// Cancel the grant agreement.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[PreventSpam]
		[ValidateRequestHeader]
		[Route("Agreement/Cancel")]
		public JsonResult CancelAgreement(Models.Agreements.CancelAgreementViewModal model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var grantApplication = _grantApplicationService.Get(model.Id);
					grantApplication.RowVersion = Convert.FromBase64String(model.RowVersion);
					_grantAgreementService.CancelGrantAgreement(grantApplication, model.CancelReason);
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
		#endregion
		#endregion
		#endregion
	}
}