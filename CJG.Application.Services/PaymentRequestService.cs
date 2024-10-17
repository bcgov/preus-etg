using CJG.Application.Business.Models;
using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CJG.Application.Services
{
	public class PaymentRequestService : Service, IPaymentRequestService
	{
		#region Variables
		private readonly IFiscalYearService _fiscalYearService;
		private readonly IClaimService _claimService;
		private readonly IGrantProgramService _grantProgramService;
		private readonly INotificationService _notificationService;

		private object generateRequestsLock = new object();
		#endregion

		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fiscalYearService"></param>
		/// <param name="claimService"></param>
		/// <param name="grantProgramService"></param>
		/// <param name="notificationService"></param>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public PaymentRequestService(
			IFiscalYearService fiscalYearService,
			IClaimService claimService,
			IGrantProgramService grantProgramService,
			INotificationService notificationService,
			IDataContext context,
			HttpContextBase httpContext,
			ILogger logger) : base(context, httpContext, logger)
		{
			_fiscalYearService = fiscalYearService;
			_claimService = claimService;
			_grantProgramService = grantProgramService;
			_notificationService = notificationService;
		}
		#endregion

		#region Methods
		public void AddPaymentRequest(PaymentRequest paymentRequest)
		{
			paymentRequest.DateAdded = AppDateTime.UtcNow;
			_dbContext.PaymentRequests.Add(paymentRequest);
			Commit();
		}

		public PaymentRequest GetPaymentRequest(int paymentRequestBatchId, int trainingProgramId)
		{
			var paymentRequest = _dbContext.PaymentRequests.Find(paymentRequestBatchId, trainingProgramId);
			return paymentRequest;
		}
		
		public PageList<PaymentRequestBatch> GetRequestBatches(int grantProgramId, int page, int quantity, string search)
		{
			var filtered = _dbContext.PaymentRequestBatches.AsNoTracking()
					.Where(o => o.GrantProgramId == grantProgramId && (string.IsNullOrEmpty(search) || o.BatchNumber.Contains(search)))
					.OrderByDescending(prb => prb.IssuedDate);
			var total = filtered.Count();
			var result = filtered.Skip((page - 1) * quantity).Take(quantity);
			return new PageList<PaymentRequestBatch>(page, quantity, total, result.ToArray());
		}
		
		public PaymentRequestBatch GeneratePaymentRequests(int grantProgramId)
		{
			lock (generateRequestsLock)
			{
				var claims = _claimService.GetApprovedClaimsForPaymentRequest(grantProgramId);

				if (claims.Any())
				{
					return GeneratePaymentRequestBatch(grantProgramId, claims, PaymentBatchTypes.PaymentRequest);
				}

				return null;
			}
		}

		public PaymentRequestBatch GenerateAmountOwing(int grantProgramId)
		{
			lock (generateRequestsLock)
			{
				var claims = _claimService.GetApprovedClaimsForAmountOwing(grantProgramId);

				if (claims.Any())
				{
					return GeneratePaymentRequestBatch(grantProgramId, claims, PaymentBatchTypes.AmountOwing);
				}

				return null;
			}
		}

		public string GenerateNewBatchNumber()
		{
			var newestRequestBatch = _dbContext.PaymentRequestBatches.OrderByDescending(prb => prb.Id).FirstOrDefault();
			var batchNumber = newestRequestBatch == null ? 0 : int.Parse(newestRequestBatch.BatchNumber);

			return $"{(batchNumber >= 9999 ? 1 : ++batchNumber):D4}";
		}

		/// <summary>
		/// Create a new instance of a PaymentRequestBatch for the specified grant program and collection of claims.
		/// </summary>
		/// <param name="grantProgramId"></param>
		/// <param name="claims"></param>
		/// <param name="batchType"></param>
		/// <returns></returns>
		public PaymentRequestBatch GeneratePaymentRequestBatch(int grantProgramId, IEnumerable<Claim> claims, PaymentBatchTypes batchType)
		{
			var grantProgram = _grantProgramService.Get(grantProgramId);
			if (grantProgram.ExpenseAuthority == null)
				throw new InvalidOperationException("An Expense Authority must be specified.");

			var currentUser = _httpContext.User;
			var paymentRequestBatch = new PaymentRequestBatch
			{
				IssuedDate = AppDateTime.UtcNow,
				BatchType = batchType,
				IssuedById = currentUser.GetUserId().Value,
				BatchNumber = GenerateNewBatchNumber(),
				GrantProgramId = grantProgram.Id,
				ExpenseAuthorityId = grantProgram.ExpenseAuthorityId,
				RequestedBy = grantProgram.RequestedBy,
				ProgramPhone = grantProgram.ProgramPhone,
				DocumentPrefix = grantProgram.DocumentPrefix,
				ExpenseAuthorityName = $"{grantProgram.ExpenseAuthority.FirstName} {grantProgram.ExpenseAuthority.LastName}",
				IssuedByName = $"{currentUser.GetFirstName()} {currentUser.GetLastName()}",
				BatchRequestDescription = grantProgram.BatchRequestDescription
			};

			foreach (var claim in claims)
			{
				PaymentRequest paymentRequest;

				if (claim.AmountPaidOrOwing() == 0)
				{
					paymentRequest = new PaymentRequest
					{
						ClaimId = claim.Id,
						ClaimVersion = claim.ClaimVersion,
						DocumentNumber = "-",
						PaymentType = PaymentTypes.None,
						PaymentAmount = 0,
						GrantApplicationId = claim.GrantApplicationId,
						GLClientNumber = "N/A",
						GLRESP = "N/A",
						GLServiceLine = "N/A",
						GLSTOB = "N/A",
						GLProjectCode = "N/A"
					};

					claim.ClaimState = ClaimState.ClaimPaid;
				}
				else
				{
					var paymentType = claim.GrantApplication.StartDate < _fiscalYearService.GetFiscalYear(AppDateTime.UtcNow).StartDate ? PaymentTypes.Accrual : PaymentTypes.Normal;
					var account = claim.GrantApplication.GrantOpening.GrantStream.AccountCode;

					paymentRequest = new PaymentRequest
					{
						ClaimId = claim.Id,
						ClaimVersion = claim.ClaimVersion,
						DocumentNumber = $"{paymentRequestBatch.DocumentPrefix}{claim.ClaimNumber}",
						RecipientBusinessNumber = claim.GrantApplication.OrganizationBusinessLicenseNumber,
						PaymentType = paymentType,
						PaymentAmount = claim.AmountPaidOrOwing(),
						GrantApplicationId = claim.GrantApplicationId,
						GLClientNumber = account.GLClientNumber,
						GLRESP = account.GLRESP,
						GLServiceLine = account.GLServiceLine,
						GLSTOB = paymentType == PaymentTypes.Accrual ? account.GLSTOBAccrual : account.GLSTOBNormal,
						GLProjectCode = account.GLProjectCode
					};

					claim.ClaimState = batchType == PaymentBatchTypes.PaymentRequest ? ClaimState.PaymentRequested : ClaimState.AmountOwing;
				}

				_dbContext.Update(claim);

				paymentRequestBatch.PaymentRequests.Add(paymentRequest);
			}

			_dbContext.PaymentRequestBatches.Add(paymentRequestBatch);

			// Add notification for each application in payment request.
			var notification_errors = new List<Exception>();
			foreach (var claim in claims)
			{
				try
				{
					var application = claim.GrantApplication;
					var notifications = application.GrantOpening.GrantStream.GrantProgram.GrantProgramNotificationTypes.Where(nt => nt.IsActive && nt.NotificationType.IsActive && nt.NotificationType.ClaimReportRule == NotificationClaimReportRules.PaymentRequestIssued);
					foreach (var notification in notifications)
					{
						_notificationService.HandleWorkflowNotification(application, notification);
					}
				}
				catch (Exception ex)
				{
					notification_errors.Add(ex);
				}
			}

			_dbContext.CommitTransaction();

			if (notification_errors.Any())
				throw new NotificationException("Error(s) occured while attempting to send email notifications", new NotificationException(String.Join("\n", notification_errors.Select(e => e.Message))));

			return paymentRequestBatch;
		}

		/// <summary>
		/// Retrieve a <typeparamref name="PaymentRequestBatch"/> by Id.
		/// </summary>
		/// <param name="id">Id of the <typeparamref name="PaymentRequestBatch"/></param>
		/// <returns></returns>
		public PaymentRequestBatch GetPaymentRequestBatch(int id)
		{
			return _dbContext.PaymentRequestBatches.Find(id);
		}

		public PaymentRequestBatchPDFModel GetPaymentRequestBatchPDF(int id, bool duplicate)
		{
			var batch = GetPaymentRequestBatch(id);
			return new PaymentRequestBatchPDFModel(batch, duplicate);
		}

		public AmountOwingBatchPDFModel GetAmountOwingBatchPDF(int id, bool duplicate)
		{
			var batch = GetPaymentRequestBatch(id);
			return new AmountOwingBatchPDFModel(batch, duplicate);
		}

		/// <summary>
		/// Get a page of payment requests for the specified filter.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="quantity"></param>
		/// <param name="search"></param>
		/// <param name="sort"></param>
		/// <returns></returns>
		public PageList<PaymentRequest> GetPaymentRequests(int page = 1, int quantity = 10, string search = null, string sort = nameof(PaymentRequest.DateAdded))
		{
			if (page <= 0) page = 1;
			if (quantity <= 0 || quantity > 100) quantity = 10;

			var isAmount = decimal.TryParse(search, out decimal amount);
			var query = !String.IsNullOrWhiteSpace(search) ? _dbContext.PaymentRequests.Where(pr => pr.DocumentNumber.Contains(search) || pr.GrantApplication.OrganizationLegalName.Contains(search) || (isAmount && pr.PaymentAmount == amount)) : _dbContext.PaymentRequests.Where(pr => true);

			var total = query.Count();

			var orderBy = !String.IsNullOrWhiteSpace(sort) ? sort : $"{nameof(PaymentRequest.DateAdded)} desc";
			query = query.OrderByProperty(orderBy).Skip((page - 1) * quantity).Take(quantity);

			return new PageList<PaymentRequest>(page, quantity, total, query.ToArray());
		}
		#endregion
	}
}
