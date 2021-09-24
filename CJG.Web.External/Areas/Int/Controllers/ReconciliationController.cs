using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using CJG.Core.Interfaces.Service;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Int.Controllers
{
	/// <summary>
	/// ReconciliationController class, provides a way to manage reconciliation reports between CAS and STG.
	/// </summary>
	[RouteArea("Int")]
	public class ReconciliationController : BaseController
	{
		#region Variables
		private readonly IPaymentReconciliationService _paymentReconciliationService;
		private readonly IPaymentRequestService _paymentRequestService;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a ReconciliationController object.
		/// </summary>
		/// <param name="controllerService"></param>
		/// <param name="paymentReconciliationService"></param>
		/// <param name="paymentRequestService"></param>
		public ReconciliationController(
			IControllerService controllerService,
			IPaymentReconciliationService paymentReconciliationService,
			IPaymentRequestService paymentRequestService) : base(controllerService.Logger)
		{
			_paymentReconciliationService = paymentReconciliationService;
			_paymentRequestService = paymentRequestService;
		}
		#endregion

		#region Endpoints
		#region Reconciliation Reports
		/// <summary>
		/// A page to view reconciliation reports.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Payment/Reconciliation/View")]
		public ActionResult ReconciliationReportsView()
		{
			return View();
		}

		/// <summary>
		/// The data for the reconciliation report view.
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		[HttpPost]
		[ValidateRequestHeader]
		[Route("Payment/Reconciliation/Reconcile")]
		public JsonResult ReconcileReport(HttpPostedFileBase file, bool createNew = false)
		{
			var model = new Models.Reconciliation.ReconciliationReportsViewModel();
			int maxUploadSize = int.Parse(ConfigurationManager.AppSettings["MaxUploadSizeInBytes"]);
			string[] permittedAttachmentTypes = ConfigurationManager.AppSettings["ReconciliationPermittedAttachmentTypes"].Split('|');
			try
			{
				var report = _paymentReconciliationService.Reconcile(
					file.Validate(maxUploadSize, permittedAttachmentTypes).InputStream,
					createNew);
				model = new Models.Reconciliation.ReconciliationReportsViewModel(report, User);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}

		/// <summary>
		/// Get a page of reconciliation reports and sort them.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="quantity"></param>
		/// <param name="sort"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Payment/Reconciliation/Reports/{page?}/{quantity?}/{sort?}")]
		public JsonResult GetReconciliationReports(int page = 1, int quantity = 10, string sort = nameof(ReconciliationReport.DateAdded))
		{
			var model = new PageList<Models.Reconciliation.ReconciliationReportsViewModel>();
			try
			{
				var reports = _paymentReconciliationService.GetReports(page, quantity, sort);

				model.Page = reports.Page;
				model.Quantity = reports.Quantity;
				model.Total = reports.Total;
				model.Items = reports.Items.Select(r => new Models.Reconciliation.ReconciliationReportsViewModel(r, User));
			}
			catch (Exception ex)
			{
				var errorModel = new BaseViewModel();
				HandleAngularException(ex, errorModel);
				return Json(errorModel, JsonRequestBehavior.AllowGet);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		[HttpPut]
		[Route("Payment/Reconciliation/Report/Delete")]
		public JsonResult DeleteReconciliationReport(Models.Reconciliation.ReconciliationReportViewModel model)
		{
			try
			{
				var report = _paymentReconciliationService.Get<ReconciliationReport>(model.Id);
				report.RowVersion = Convert.FromBase64String(model.RowVersion);
				_paymentReconciliationService.Delete(report);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model);
		}
		#endregion

		#region Reconciliation Report
		/// <summary>
		/// A page to view the reconciliation report for the specified 'id'.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Payment/Reconciliation/Report/View/{id}")]
		public ActionResult ReconciliationReportView(int id)
		{
			ViewBag.ReconciliationReportId = id;
			return View();
		}

		/// <summary>
		/// Get the report for the specified 'id' and return the JSON data.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Payment/Reconciliation/Report/{id}")]
		public JsonResult GetReconciliationReport(int id)
		{
			var model = new Models.Reconciliation.ReconciliationReportViewModel();
			try
			{
				var report = _paymentReconciliationService.Get<ReconciliationReport>(id);
				model = new Models.Reconciliation.ReconciliationReportViewModel(report);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region Reconciliation Payment
		/// <summary>
		/// A partial view modal popup to reconcile a payment with a payment request.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Payment/Reconcile/Payment/Request/View/{id}")]
		public ActionResult ReconcilePaymentRequestView(int id)
		{
			ViewBag.ReconciliationPaymentId = id;
			return PartialView("_ReconcilePaymentRequestView");
		}

		/// <summary>
		/// Get a page of payment requests for the specified filter.
		/// </summary>
		/// <param name="reconciliationPaymentId"></param>
		/// <param name="page"></param>
		/// <param name="quantity"></param>
		/// <param name="sort"></param>
		/// <param name="search"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("Payment/Reconciliation/Payment/Requests/{reconciliationPaymentId?}/{page?}/{quantity?}/{sort?}/{search?}")]
		public JsonResult GePaymentRequests(int reconciliationPaymentId = 0, int page = 1, int quantity = 10, string sort = "DocumentNumber", string search = null)
		{
			var model = new PageList<Models.Reconciliation.PaymentRequestViewModel>();
			try
			{
				var reconciliationPayment = reconciliationPaymentId != 0 ? _paymentReconciliationService.Find<ReconciliationPayment>(reconciliationPaymentId) : null;
				var paymentRequests = _paymentRequestService.GetPaymentRequests(page, quantity, search, sort);

				var items = new List<Models.Reconciliation.PaymentRequestViewModel>();
				if (reconciliationPayment?.PaymentRequest != null) items.Add(new Models.Reconciliation.PaymentRequestViewModel(reconciliationPayment.PaymentRequest) { ReconciliationPaymentId = reconciliationPaymentId });
				items.AddRange(paymentRequests.Items.Select(p => new Models.Reconciliation.PaymentRequestViewModel(p)));

				model.Page = paymentRequests.Page;
				model.Quantity = paymentRequests.Quantity;
				model.Total = paymentRequests.Total;
				model.Items = items.Distinct();
			}
			catch (Exception ex)
			{
				var errorModel = new BaseViewModel();
				HandleAngularException(ex, errorModel);
				return Json(errorModel, JsonRequestBehavior.AllowGet);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Reconcile the specified reconciliation payment with the linked payment request.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPut]
		[Route("Payment/Reconciliation/Reconcile/Payment")]
		public JsonResult ReconcilePaymentRequest(Models.Reconciliation.ReconciliationPaymentViewModel model)
		{
			try
			{
				var reconciliationPayment = _paymentReconciliationService.Get<ReconciliationPayment>(model.Id);
				reconciliationPayment.RowVersion = Convert.FromBase64String(model.RowVersion);

				if (model.PaymentRequestBatchId == null && model.FromCAS)
				{
					reconciliationPayment.PaymentRequest = null;
					reconciliationPayment.PaymentRequestBatchId = null;
					reconciliationPayment.GrantApplicationId = null;
					reconciliationPayment.ClaimId = null;
					reconciliationPayment.ClaimVersion = null;
				}
				else if (model.PaymentRequestBatchId != null)
				{
					var paymentRequest = _paymentRequestService.Get<PaymentRequest>(model.PaymentRequestBatchId, model.ClaimId, model.ClaimVersion);
					reconciliationPayment.PaymentRequest = paymentRequest;
					reconciliationPayment.PaymentRequestBatchId = model.PaymentRequestBatchId;
					reconciliationPayment.GrantApplicationId = model.GrantApplicationId;
					reconciliationPayment.ClaimId = model.ClaimId;
					reconciliationPayment.ClaimVersion = model.ClaimVersion;
				}

				_paymentReconciliationService.ManualReconcile(reconciliationPayment);

				model = new Models.Reconciliation.ReconciliationPaymentViewModel(reconciliationPayment);
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