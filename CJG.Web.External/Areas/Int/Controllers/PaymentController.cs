using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Areas.Int.Models;
using CJG.Web.External.Areas.Int.Models.PaymentRequests;
using CJG.Web.External.Controllers;
using CJG.Web.External.Helpers;
using CJG.Web.External.Helpers.Filters;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace CJG.Web.External.Areas.Int.Controllers
{
	[AuthorizeAction(Privilege.PM1)]
	[RouteArea("Int")]
	[RoutePrefix("Payment/Request")]
	public class PaymentController : BaseController
	{
		#region Variables
		private readonly IPaymentRequestService _paymentRequestService;
		private readonly IClaimService _claimService;
		private readonly IGrantProgramService _grantProgramService;
		private readonly IFiscalYearService _fiscalYearService;
		#endregion

		#region Constructors
		public PaymentController(IControllerService controllerService,
								 IPaymentRequestService paymentRequestService,
								 IClaimService claimService,
								 IGrantProgramService grantProgramService,
								 IFiscalYearService fiscalYearService) : base(controllerService.Logger)
		{
			_paymentRequestService = paymentRequestService;
			_claimService = claimService;
			_grantProgramService = grantProgramService;
			_fiscalYearService = fiscalYearService;
		}
		#endregion

		#region Endpoints
		/// <summary>
		/// Returns the request management view.
		/// </summary>
		/// <returns></returns>
		[HttpGet, Route("Management/View")]
		public ActionResult RequestManagementView()
		{
			return View();
		}

		/// <summary>
		/// Get a list of grant programs.
		/// </summary>
		/// <returns></returns>
		[Route("Programs")]
		public JsonResult GetGrantPrograms()
		{
			IEnumerable<KeyValuePair<int, string>> results = new KeyValuePair<int, string>[0];
			try
			{
				var grantPrograms = _grantProgramService.GetAll();
				results = grantPrograms.OrderBy(o => o.Name).Select(p => new KeyValuePair<int, string>(p.Id, p.Name)).ToArray();
			}
			catch (Exception ex)
			{
				HandleAngularException(ex);
			}
			return Json(results, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get payment request batches for grant program.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="quantity"></param>
		/// <param name="search"></param>
		/// <returns></returns>
		[HttpGet, Route("Batch/Search/{grantProgramId}/{page}/{quantity}")]
		[ValidateRequestHeader]
		[AuthorizeAction(Privilege.PM1)]
		public JsonResult GetRequestBatch(int grantProgramId, int page, int quantity, string search)
		{
			var model = new BaseViewModel();
			try
			{
				var requestBatches = _paymentRequestService.GetRequestBatches(grantProgramId, page, quantity, search);
				var result = new
				{
					RecordsFiltered = requestBatches.Items.Count(),
					RecordsTotal = requestBatches.Total,
					Data = requestBatches.Items.Select(o => new RequestBatchesDataTableModel(o)).ToArray()
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
		/// Get payment requests for grant program.
		/// </summary>
		/// <param name="grantProgramId"></param>
		/// <returns></returns>
		[HttpGet, Route("Payment/Request/{grantProgramId}")]
		[ValidateRequestHeader]
		[AuthorizeAction(Privilege.PM1)]
		public JsonResult GetPaymentRequests(int grantProgramId)
		{
			var model = new BaseViewModel();
			try
			{
				var claims = _claimService.GetApprovedClaimsForPaymentRequest(grantProgramId)
					.Select(c => new PaymentRequestClaimsDataTableModel(c, GetStartDate()))
					.ToList();

				return Json(claims, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get amount owings for grant program.
		/// </summary>
		/// <param name="grantProgramId"></param>
		/// <returns></returns>
		[HttpGet, Route("Amount/Owing/{grantProgramId}")]
		[ValidateRequestHeader]
		[AuthorizeAction(Privilege.PM1)]
		public JsonResult GetAmountOwings(int grantProgramId)
		{
			var model = new BaseViewModel();
			try
			{
				var claims = _claimService.GetApprovedClaimsForAmountOwing(grantProgramId)
					.Select(c => new PaymentRequestClaimsDataTableModel(c, GetStartDate()))
					.ToList();

				return Json(claims, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get payment requests on hold for grant program.
		/// </summary>
		/// <param name="grantProgramId"></param>
		/// <returns></returns>
		[HttpGet, Route("On/Hold/{grantProgramId}")]
		[ValidateRequestHeader]
		[AuthorizeAction(Privilege.PM1)]
		public JsonResult GetPaymentRequestOnHolds(int grantProgramId)
		{
			var model = new BaseViewModel();
			try
			{
				var claims = _claimService.GetClaimsByPaymentRequestHold(grantProgramId, true);

				return Json(claims, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		private DateTime GetStartDate()
		{
			var utcNow = AppDateTime.UtcNow;
			return _fiscalYearService.GetFiscalYear(utcNow)?.StartDate ?? utcNow;
		}

		/// <summary>
		/// Get payment request batches modal view.
		/// </summary>
		/// <param name="requestBatchId"></param>
		/// <returns></returns>
		[HttpGet, Route("Batch/{requestBatchId}")]
		[AuthorizeAction(Privilege.PM1)]
		public ActionResult GetPaymentRequestBatch(int requestBatchId)
		{
			var model = new BaseViewModel();
			try
			{
				var paymentRequestBatch = _paymentRequestService.GetPaymentRequestBatch(requestBatchId);
				
				return PartialView("_PaymentRequestBatch", paymentRequestBatch);
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Generate payment request batch for grant program.
		/// </summary>
		/// <param name="grantProgramId"></param>
		/// <returns></returns>
		[HttpPost]
		[ValidateRequestHeader]
		[AuthorizeAction(Privilege.PM1)]
		[Route("Generate/Payment/Request/{grantProgramId}")]
		public ActionResult GeneratePaymentRequests(int grantProgramId)
		{
			var model = new BaseViewModel();
			try
			{
				var result = _paymentRequestService.GeneratePaymentRequests(grantProgramId);

				if (result != null)
				{
					TempData["InitialRequest"] = true;
					return Json(new { result = "success", id = result.Id, number = result.BatchNumber }, JsonRequestBehavior.AllowGet);
				}

				return Json(new { result = "empty" });
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Generate amount owing batch for grant program.
		/// </summary>
		/// <param name="grantProgramId"></param>
		/// <returns></returns>
		[HttpPost]
		[ValidateRequestHeader]
		[AuthorizeAction(Privilege.PM1)]
		[Route("Generate/Amount/Owing/{grantProgramId}")]
		public ActionResult GenerateAmountOwingRequests(int grantProgramId)
		{
			var model = new BaseViewModel();
			try
			{
				var result = _paymentRequestService.GenerateAmountOwing(grantProgramId);

				if (result != null)
				{
					TempData["InitialRequest"] = true;
					return Json(new { result = "success", id = result.Id, number = result.BatchNumber }, JsonRequestBehavior.AllowGet);
				}

				return Json(new { result = "empty" });
			}
			catch (Exception ex)
			{
				HandleAngularException(ex, model);
			}
			return Json(model, JsonRequestBehavior.AllowGet);
		}
		
		[HttpGet, Route("Print/Payment/Request/Batch/{id}")]
		[AuthorizeAction(Privilege.PM1)]
		public ActionResult PrintPaymentRequestBatch(int id)
		{
			try
			{
				var initial = TempData["InitialRequest"] as bool? == true;
				var model = _paymentRequestService.GetPaymentRequestBatchPDF(id, !initial);
				return PartialView("_PaymentRequestBatchPrint", model);
			}
			catch (Exception e)
			{
				_logger.Error(e, "Couldn't get Payment Request Batch.");
				throw;
			}
		}

		[HttpGet, Route("Print/Amount/Owing/Batch/{id}")]
		[AuthorizeAction(Privilege.PM1)]
		public ActionResult PrintAmountOwingBatch(int id)
		{
			try
			{
				var initial = TempData["InitialRequest"] as bool? == true;
				var model = _paymentRequestService.GetAmountOwingBatchPDF(id, !initial);
				return PartialView("_AmountOwingBatchPrint", model);
			}
			catch (Exception e)
			{
				_logger.Error(e, "Couldn't get Amount Owing Batch.");
				throw;
			}
		}

		[HttpGet, Route("Cancel/Payment/Request/Batch")]
		[AuthorizeAction(Privilege.PM1)]
		public ActionResult CancelPaymentRequestBatch()
		{
			TempData.Remove("InitialRequest");
			return Json(new { result = "success" }, JsonRequestBehavior.AllowGet);
		}
		#endregion
	}
}
