using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using CJG.Core.Entities;
using CJG.Core.Entities.Extensions;
using CJG.Core.Entities.Helpers;
using CJG.Core.Interfaces;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using NLog;

namespace CJG.Application.Services
{
	/// <summary>
	/// PaymentReconciliationService class, service provides methods to manage reconciliation reports.
	/// </summary>
	public class PaymentReconciliationService : Service, IPaymentReconciliationService
	{
		private string[] _validPrefix;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="httpContext"></param>
		/// <param name="logger"></param>
		public PaymentReconciliationService(IDataContext context, HttpContextBase httpContext, ILogger logger)
			: base(context, httpContext, logger)
		{
		}

		/// <summary>
		/// Get a page of reconciliation reports and sort them by the specified property.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="quantity"></param>
		/// <param name="sort"></param>
		/// <returns></returns>
		public PageList<ReconciliationReport> GetReports(int page = 1, int quantity = 10, string sort = nameof(ReconciliationReport.DateAdded) + " desc")
		{
			if (page <= 0)
				page = 1;

			if (quantity <= 0 || quantity > 100)
				quantity = 10;

			var query = _dbContext.ReconciliationReports.Where(rc => true);
			var total = query.Count();

			var orderBy = !string.IsNullOrWhiteSpace(sort)
				? sort
				: $"{nameof(ReconciliationReport.DateAdded)} desc";

			query = query.OrderByProperty(orderBy)
				.Skip((page - 1) * quantity)
				.Take(quantity);

			return new PageList<ReconciliationReport>(page, quantity, total, query.ToArray());
		}

		/// <summary>
		/// Create a new reconciliation report or update an existing on by parsing the excel file stream.
		/// This will extract each line item and determine whether it is a payment and whether it is reconciled (or what type of issue it has).
		/// </summary>
		/// <param name="stream">The stream to the excel document.</param>
		/// <param name="createNew">Whether to create a new report, or to update an existing if it exists.</param>
		public ReconciliationReport Reconcile(Stream stream, bool createNew = false)
		{
			try
			{
				var user = _dbContext.InternalUsers.Find(_httpContext.User.GetUserId())
				           ?? throw new InvalidOperationException("User does not exist");
				var defaultGrantProgramId = GetDefaultGrantProgramId();

				_validPrefix = _dbContext.GrantPrograms
					.Where(gp => gp.State == GrantProgramStates.Implemented)
					.Where(gp => gp.Id == defaultGrantProgramId)
					.Select(gp => gp.DocumentPrefix)
					.Distinct()
					.ToArray();

				var reconciliationReport = new ReconciliationReport(user);

				using (var document = SpreadsheetDocument.Open(stream, false))
				{
					var workbookPart = document.WorkbookPart;
					var sheet = workbookPart.WorksheetParts.FirstOrDefault() ?? throw new InvalidOperationException("Excel contains no sheets.");
					var rows = sheet.Worksheet.Descendants<Row>().ToList();

					var clientCol = "A";
					var finalBalanceCol = "E";  // Used to detect the value of 'Final Balance' which signifies the end of the data rows
					var supplierNameCol = "F";
					var periodNameCol = "G";
					var creationDateCol = "H";
					var documentNumberCol = "I";
					var amountCol = "J";

					var bypassedHeaderRows = false;
					var keepProcessing = true;

					var trackedPeriods = new List<DateTime>();

					foreach (var row in rows)
					{
						var rowIndex = row.RowIndex;
						var cells = row.Descendants<Cell>().ToList();

						// Assumption that the 'Client' column header will always be the first column.
						var clientCell = cells.FirstOrDefault(c => c.CellReference == $"{clientCol}{rowIndex}");
						var cellValue = GetCellValue<string>(workbookPart, clientCell);

						if (!keepProcessing)
							break;

						if (!bypassedHeaderRows)
						{
							var cellData = cellValue.Trim().ToLower();

							if (cellData.StartsWith("time run:"))
							{
								var dateTimeString = cellData.Replace("time run: ", "").Trim();
								var format = "d/M/yyyy h:mm:ss tt";

								if (DateTime.TryParseExact(dateTimeString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDateTime))
								{
									reconciliationReport.DateRun = parsedDateTime.ToUniversalTime();
									continue;
								}

								throw new InvalidOperationException($"The reconciliation report 'run date' is invalid - '{cellValue}'.");
							}

							if (cellData == "client")
								bypassedHeaderRows = true;

							continue;
						}

						var finalBalanceCell = cells.FirstOrDefault(c => c.CellReference == $"{finalBalanceCol}{rowIndex}");
						var finalBalanceCellValue = GetCellValue<string>(workbookPart, finalBalanceCell);

						// TODO: Could optimise this to break out here and not do another loop if we want to stop all processing
						if (finalBalanceCellValue.Trim().ToLower() == "final balance")
						{
							keepProcessing = false;
							continue;
						}

						var validClientId = int.TryParse(cellValue, out int clientId);  // Parses the first cell to see if it's an integer (we've hit the client id cell)
						if (!validClientId)
							continue;

						var amountCell = cells.FirstOrDefault(c => c.CellReference == $"{amountCol}{rowIndex}");
						var supplierNameCell = cells.FirstOrDefault(c => c.CellReference == $"{supplierNameCol}{rowIndex}");
						var creationDateCell = cells.FirstOrDefault(c => c.CellReference == $"{creationDateCol}{rowIndex}");
						var documentNumberCell = cells.FirstOrDefault(c => c.CellReference == $"{documentNumberCol}{rowIndex}");
						var periodNameCall = cells.FirstOrDefault(c => c.CellReference == $"{periodNameCol}{rowIndex}");

						// Extract the values from the cells.
						var supplierName = GetCellValue<string>(workbookPart, supplierNameCell);
						var creationDate = GetCellValue<DateTime?>(workbookPart, creationDateCell);
						var documentNumber = GetCellValue<string>(workbookPart, documentNumberCell);
						var amount = GetCellValue<decimal>(workbookPart, amountCell, 2);
						var batchName = string.Empty;
						var supplierNumber = string.Empty;
						var periodName = GetCellValue<string>(workbookPart, periodNameCall);

						if (creationDate == null)
							continue;

						var periodDate = ParsePeriodNameToDate(periodName);

						if (periodDate != null && !trackedPeriods.Contains(periodDate.Value))
							trackedPeriods.Add(periodDate.Value);

						// Determine if this is a payment or accrual journal entry.  If it is, import it.
						var payment = GetReconciliationPayment(reconciliationReport, batchName, creationDate.Value, documentNumber, supplierName, supplierNumber, amount);

						if (payment != null)
							reconciliationReport.Payments.Add(payment);
					}

					// Figure out the Period From/To from the payments
					if (trackedPeriods.Any())
					{ 
						var earliestPeriod = trackedPeriods.Min();
						var lastPeriod = trackedPeriods.Max().AddMonths(1).AddDays(-1);

						reconciliationReport.PeriodFrom = earliestPeriod.ToUniversalTime();
						reconciliationReport.PeriodTo = lastPeriod.ToUniversalTime();
					}

					// Since we don't have a requestor on the CAS File, we can use the current logged in user
					if (string.IsNullOrWhiteSpace(reconciliationReport.Requestor))
						reconciliationReport.Requestor = user.IDIR;

					if (!createNew)
					{
						// Check if there is a report that already exists for this range.
						var existingReport = _dbContext.ReconciliationReports
							.OrderByDescending(rr => rr.DateAdded)
							.FirstOrDefault(rp =>
								rp.DateRun == reconciliationReport.DateRun && rp.Requestor == reconciliationReport.Requestor &&
								rp.PeriodFrom == reconciliationReport.PeriodFrom && rp.PeriodTo == reconciliationReport.PeriodTo);

						if (existingReport != null)
						{
							var paymentIds = existingReport.Payments.Select(p => p.Id).ToArray();
							foreach (var payment in reconciliationReport.Payments)
							{
								// Only add payments that are not already linked to this report.
								if (!paymentIds.Contains(payment.Id))
									existingReport.Payments.Add(payment);
							}
							reconciliationReport = existingReport; // Replace the new one with the existing one.
						}
					}

					// Reconcile only line items that are currently not reconciled (which should be any line item that "might be" reconcilable in this report).
					foreach (var recPayment in reconciliationReport.Payments.Where(p => p.ReconcilationState == ReconciliationStates.NotReconciled))
					{
						if (recPayment.PaymentRequest != null)
						{
							var paymentRequest = recPayment.PaymentRequest;
							if (recPayment.DocumentNumber != paymentRequest.DocumentNumber)
							{
								recPayment.ReconcilationState = ReconciliationStates.InvalidDocumentNumber;
							}
							else if (recPayment.IsValidSupplierName(recPayment.GrantApplication))
							{
								recPayment.ReconcilationState = ReconciliationStates.InvalidSupplierName;
							}
							else
							{
								var sumOfPayments = paymentRequest.ReconciliationPayments.Sum(rp => rp.Amount);

								// If the sum in CAS is greater and there is a prior reconciliation payment with the same amount, it is a duplicate.
								if (sumOfPayments > paymentRequest.PaymentAmount && paymentRequest.ReconciliationPayments.Count(rp => rp.Amount == recPayment.Amount) > 1)
								{
									paymentRequest.ReconciliationPayments
										.Where(rp => rp.ReconcilationState == ReconciliationStates.Reconciled)
										.ForEach(rp => rp.ReconcilationState = ReconciliationStates.InvalidAmount);
									recPayment.ReconcilationState = ReconciliationStates.Duplicate;
									paymentRequest.IsReconciled = false;
								}
								else if (paymentRequest.PaymentAmount != sumOfPayments)
								{
									paymentRequest.ReconciliationPayments
										.Where(rp => rp.ReconcilationState == ReconciliationStates.Reconciled)
										.ForEach(rp => rp.ReconcilationState = ReconciliationStates.InvalidAmount);
									recPayment.ReconcilationState = ReconciliationStates.InvalidAmount;
									paymentRequest.IsReconciled = false;
								}
								else
								{
									recPayment.ReconcilationState = ReconciliationStates.Reconciled; // The line item is reconciled with STG and CAS!
									paymentRequest.ReconciliationPayments
										.Where(rp => rp.ReconcilationState != ReconciliationStates.Reconciled)
										.ForEach(rp => rp.ReconcilationState = ReconciliationStates.Reconciled);
									paymentRequest.IsReconciled = true;
								}
							}

							// Update the related reports based on the state of their payments states.
							// A new ReconciliationPayment will not have reports.
							foreach (var report in paymentRequest.ReconciliationPayments.SelectMany(rp => rp.Reports).Distinct())
								report.IsReconciled = report.Payments.All(p => p.ReconcilationState == ReconciliationStates.Reconciled);

							// Update the claim state.
							recPayment.PaymentRequest.Claim.ClaimState = recPayment.PaymentRequest.Claim.AmountPaidOrOwing() > 0
								? ClaimState.ClaimPaid
								: ClaimState.AmountReceived;
						}
						else
						{
							// All reconciliation payments that have no matching payment request are not reconciled.
							recPayment.ReconcilationState = ReconciliationStates.NoMatch;
						}
					}

					foreach (var recPayment in reconciliationReport.Payments.Where(p => p.ReconcilationState == ReconciliationStates.NoMatch))
					{
						// Determine if this line item has related ReconciliationPayment line items that may result in it being reconciled.
						// This can occur if a payment was made but should not have been made.  CAS will than have another line item to the same supplier and it should reconcile.
						var recPayments = _dbContext.ReconciliationPayments.Where(rp => rp.FromCAS && rp.Id != recPayment.Id && rp.DocumentNumber == recPayment.DocumentNumber && rp.SupplierName == recPayment.SupplierName).ToList();

						// Need to add the current report payments because they are not yet in the datasource.
						// Only add payments for the same document number and supplier name that originated from CAS.
						recPayments.AddRange(reconciliationReport.Payments.Where(rp => rp.FromCAS && rp.ReconcilationState == ReconciliationStates.NoMatch && rp.DocumentNumber == recPayment.DocumentNumber && rp.SupplierName == recPayment.SupplierName).ToArray());

						if (!recPayments.Any() || recPayments.Sum(rp => rp.Amount) != 0)
							continue;

						// The assumption is that the sum of these similar line items from CAS zero out and therefore are reconciled.
						recPayment.ReconcilationState = ReconciliationStates.Reconciled;

						recPayments.Where(rp => rp.ReconcilationState != ReconciliationStates.Reconciled).ForEach(rp =>
						{
							// All reports that contain the Reconciliation Payment needs to be checked to determine if they are now reconciled too.
							rp.ReconcilationState = ReconciliationStates.Reconciled;
							rp.Reports.Where(r => !r.IsReconciled).ForEach(r => r.IsReconciled = r.Payments.All(p => p.ReconcilationState == ReconciliationStates.Reconciled));
						});
					}

					// Fetch all payment requests for the reported period that do not currently have a linked reconciliation payment.
					var paymentRequests = _dbContext.PaymentRequests
						.Where(pr => pr.DateAdded >= reconciliationReport.PeriodFrom && pr.DateAdded <= reconciliationReport.PeriodTo && !pr.ReconciliationPayments.Any());

					foreach (var paymentRequest in paymentRequests)
					{
						// Only need to add line items that have no associated CAS line item.
						// This is done to inform the user that STG has a payment request that CAS does not.
						reconciliationReport.Payments.Add(new ReconciliationPayment(paymentRequest));
					}

					// Update all reports related to the reconciliation payments in this report.
					// When a ReconciliationPayment does not link to any reports, it is because it is just being added by this report.
					var reports = reconciliationReport.Payments
						.Where(rp => rp.ReconcilationState == ReconciliationStates.Reconciled)
						.SelectMany(rp => rp.Reports)
						.Distinct()
						.ToList();

					reports.Add(reconciliationReport);
					foreach (var report in reports)
						report.IsReconciled = report.Payments.All(p => p.ReconcilationState == ReconciliationStates.Reconciled);

					if (!reconciliationReport.Payments.Any())
						throw new InvalidOperationException("The CAS report contains no valid line items to import.");

					if (reconciliationReport.Id == 0)
						_dbContext.ReconciliationReports.Add(reconciliationReport);
					else
						_dbContext.Update(reconciliationReport);

					CommitTransaction();

					return reconciliationReport;
				}
			}
			catch
			{
				// Caller must close the stream.
				throw;
			}
		}

		private DateTime? ParsePeriodNameToDate(string periodName)
		{
			if (string.IsNullOrWhiteSpace(periodName))
				return null;

			// The period name is provided as "APR-25" meaning April 2025
			if (DateTime.TryParseExact($"01-{periodName}", "dd-MMM-yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var foundDate))
				return foundDate;

			throw new InvalidOperationException($"The reconciliation report row 'Period Name' is invalid - '{periodName}'.");
		}

		/// <summary>
		/// Manually reconcile the reconciliation payment and the associated reconciliation report(s).
		/// This allows for reconciling a payment request that may have a different 'Document Number' or 'Supplier Name', but will unreconcile if the 'Amount' is different.
		/// This also unreconciles a reconciliation payment if it is no longer linked to a payment request.
		/// </summary>
		/// <param name="payment"></param>
		public void ManualReconcile(ReconciliationPayment payment)
		{
			if (payment == null)
				throw new ArgumentNullException(nameof(payment));

			if (payment.Id == 0)
				throw new InvalidOperationException("Only existing payments can be reconciled.");

			var originalPaymentRequestBatchId = OriginalValue(payment, pr => pr.PaymentRequestBatchId);
			var originalClaimId = OriginalValue(payment, pr => pr.ClaimId);
			var originalClaimVersion = OriginalValue(payment, pr => pr.ClaimVersion);

			// When removing the link to a payment request, it must unreconcile that payment request.
			if (payment.PaymentRequest == null)
			{
				payment.ReconcilationState = _dbContext.PaymentRequests.Any(pr => pr.DocumentNumber == payment.DocumentNumber)
					? ReconciliationStates.NotReconciled
					: ReconciliationStates.NoMatch;

				if (originalPaymentRequestBatchId != null && originalClaimId != null && originalClaimVersion != null)
					UnlinkPayment(payment, Get<PaymentRequest>(originalPaymentRequestBatchId, originalClaimId, originalClaimVersion));
			}
			else
			{
				// Must link this reconciliation payment with the selected payment request.
				if (!payment.PaymentRequest.ReconciliationPayments.Any(rp => rp.Id == payment.Id))
					payment.PaymentRequest.ReconciliationPayments.Add(payment);

				if (payment.PaymentRequest.PaymentAmount != payment.PaymentRequest.ReconciliationPayments.Sum(rp => rp.Amount))
				{
					payment.ReconcilationState = ReconciliationStates.InvalidAmount;
				}
				else
				{
					payment.ReconcilationState = ReconciliationStates.Reconciled;
					// Update the claim state.
					payment.PaymentRequest.Claim.ClaimState = payment.PaymentRequest.Claim.AmountPaidOrOwing() > 0 ? ClaimState.ClaimPaid : ClaimState.AmountReceived;
				}

				payment.PaymentRequest.IsReconciled = payment.ReconcilationState == ReconciliationStates.Reconciled;

				// Need to manually update any other reconciliation payment that is linked through a related payment request.
				foreach (var recPayment in payment.PaymentRequest.ReconciliationPayments.Where(rp => rp.Id != payment.Id))
				{
					var state = ReconciliationStates.NotReconciled;
					if (payment.ReconcilationState == ReconciliationStates.Reconciled)
						state = ReconciliationStates.Reconciled;

					else if (payment.PaymentRequest.PaymentAmount != payment.PaymentRequest.ReconciliationPayments.Sum(rp => rp.Amount))
						state = ReconciliationStates.InvalidAmount;

					else if (recPayment.DocumentNumber != payment.PaymentRequest.DocumentNumber)
						state = ReconciliationStates.InvalidDocumentNumber;

					else if (recPayment.IsValidSupplierName(payment.PaymentRequest.GrantApplication))
						state = ReconciliationStates.InvalidSupplierName;

					recPayment.ReconcilationState = state;
					if (state == ReconciliationStates.Reconciled)
					{
						recPayment.PaymentRequest.Claim.ClaimState = recPayment.PaymentRequest.Claim.AmountPaidOrOwing() > 0
							? ClaimState.ClaimPaid
							: ClaimState.AmountReceived;
					}
					else
					{
						recPayment.PaymentRequest.Claim.ClaimState = recPayment.PaymentRequest.Claim.AmountPaidOrOwing() > 0
							? ClaimState.PaymentRequested
							: ClaimState.AmountOwing;
					}
				}

				// The previously linked payment request may need to be added to the report as NoMatch.
				if (originalPaymentRequestBatchId != null && originalClaimId != null && originalClaimVersion != null
					&& (originalPaymentRequestBatchId != payment.PaymentRequestBatchId || originalClaimId != payment.ClaimId || originalClaimVersion != payment.ClaimVersion))
				{
					UnlinkPayment(payment, Get<PaymentRequest>(originalPaymentRequestBatchId, originalClaimId, originalClaimVersion));
				}
			}

			foreach (var report in payment.Reports)
			{
				if (payment.ReconcilationState == ReconciliationStates.Reconciled)
				{
					report.IsReconciled = report.Payments.All(p => p.ReconcilationState == ReconciliationStates.Reconciled);
				}
				else
				{
					// This report is no longer reconciled if the payment is no longer reconciled.
					if (report.IsReconciled)
						report.IsReconciled = false;
				}
			}

			_dbContext.Update(payment);
			CommitTransaction();
		}

		/// <summary>
		/// Remove the link from the payment request to the reconciliation payment.
		/// This occurs when manually reconciling a reconciliation payment and switching from one payment request to another.
		/// All reconciliation payments linked to the original payment request must also update their state based on the current situation.
		/// All reports associated with the reconciliation payments must also be updated.
		/// When unlinking a payment request results in a report missing a reconciliation payment referencing the payment request, it must be created and added.
		/// </summary>
		/// <param name="payment"></param>
		/// <param name="paymentRequest"></param>
		private void UnlinkPayment(ReconciliationPayment payment, PaymentRequest paymentRequest)
		{
			if (paymentRequest == null)
				return;

			paymentRequest.IsReconciled = paymentRequest.ReconciliationPayments.Sum(rp => rp.Amount) == paymentRequest.PaymentAmount;

			// It is possible that a Payment Request for the period is now no longer associated with the report.
			// Make sure all Payment Requests are accounted for.
			var relatedPayment = _dbContext.ReconciliationPayments.FirstOrDefault(p => p.Id != payment.Id && p.PaymentRequestBatchId == paymentRequest.PaymentRequestBatchId && p.ClaimId == paymentRequest.ClaimId && p.ClaimVersion == paymentRequest.ClaimVersion) ?? new ReconciliationPayment(paymentRequest);

			if (!relatedPayment.FromCAS && relatedPayment.PaymentRequest.ReconciliationPayments.Count() == 1)
				relatedPayment.ReconcilationState = ReconciliationStates.NoMatch;
			else if (relatedPayment.FromCAS)
				relatedPayment.ReconcilationState = relatedPayment.PaymentRequest.ReconciliationPayments.Sum(p => p.Amount) == relatedPayment.PaymentRequest.PaymentAmount ? ReconciliationStates.Reconciled : ReconciliationStates.InvalidAmount;

			var addPayment = false;

			// Look for reports that do not have the original payment request linked any longer.
			// The assumption is that it should be linked to the report and became unlinked during manual reconciliation.
			foreach (var report in payment.Reports.Where(r => !r.Payments.Any(p => p.PaymentRequestBatchId == paymentRequest.PaymentRequestBatchId && p.ClaimId == paymentRequest.ClaimId && p.ClaimVersion == paymentRequest.ClaimVersion)))
			{
				if (paymentRequest.DateAdded >= report.PeriodFrom && paymentRequest.DateAdded <= report.PeriodTo)
				{
					report.Payments.Add(relatedPayment);
					addPayment = true;
				}
			}

			// Only need to add the reconciliation payment if it was added to a report.
			if (addPayment && relatedPayment.Id == 0)
				_dbContext.ReconciliationPayments.Add(relatedPayment);

			// Need to manually update any other reconciliation payment that is linked through a related payment request.
			foreach (var recPayment in paymentRequest.ReconciliationPayments.Where(rp => rp.Id != payment.Id))
			{
				var state = payment.ReconcilationState;
				if (paymentRequest.IsReconciled)
					state = ReconciliationStates.Reconciled;

				else if (!recPayment.FromCAS && paymentRequest.ReconciliationPayments.Count() == 1)
					state = ReconciliationStates.NoMatch;

				else if (paymentRequest.PaymentAmount != paymentRequest.ReconciliationPayments.Sum(rp => rp.Amount))
					state = ReconciliationStates.InvalidAmount;

				else if (recPayment.DocumentNumber != paymentRequest.DocumentNumber)
					state = ReconciliationStates.InvalidDocumentNumber;

				else if (recPayment.IsValidSupplierName(paymentRequest.GrantApplication))
					state = ReconciliationStates.InvalidSupplierName;

				recPayment.ReconcilationState = state;
				if (recPayment.ReconcilationState == ReconciliationStates.Reconciled)
				{
					recPayment.PaymentRequest.Claim.ClaimState = recPayment.PaymentRequest.Claim.AmountPaidOrOwing() > 0
						? ClaimState.ClaimPaid
						: ClaimState.AmountReceived;
				}
				else
				{
					recPayment.PaymentRequest.Claim.ClaimState = recPayment.PaymentRequest.Claim.AmountPaidOrOwing() > 0
						? ClaimState.PaymentRequested
						: ClaimState.AmountOwing;
				}

				recPayment.Reports.ForEach(r => r.IsReconciled = r.Payments.All(p => p.ReconcilationState == ReconciliationStates.Reconciled));
			}
		}

		/// <summary>
		/// Delete the reconciliation report from the datasource.
		/// This will essentially result in a reversal reconciliation.
		/// </summary>
		/// <param name="report"></param>
		public void Delete(ReconciliationReport report)
		{
			foreach (var payment in report.Payments.ToArray())
			{
				if (payment.Reports.Count != 1)
					continue;

				foreach (var paymentReport in payment.Reports)
				{
					if (paymentReport.IsReconciled)
						paymentReport.IsReconciled = false;
				}

				if (payment.PaymentRequest != null && payment.PaymentRequest.IsReconciled)
				{
					payment.PaymentRequest.IsReconciled = false;
					payment.PaymentRequest.Claim.ClaimState = payment.PaymentRequest.Claim.TotalAssessedReimbursement > 0 ? ClaimState.PaymentRequested : ClaimState.AmountOwing;
				}

				_dbContext.ReconciliationPayments.Remove(payment);
			}

			_dbContext.ReconciliationReports.Remove(report);
			CommitTransaction();
		}

		/// <summary>
		/// Find a matching reconciliation payment from a prior reconciliation report, or generate a new reconciliation payment.
		/// If the line item isn't a reconciliation payment, then return null.
		/// </summary>
		/// <param name="reconciliationReport"></param>
		/// <param name="batchName"></param>
		/// <param name="dateCreated"></param>
		/// <param name="documentNumber"></param>
		/// <param name="supplierName"></param>
		/// <param name="supplierNumber"></param>
		/// <param name="amount"></param>
		/// <returns>A new instance of a ReconciliationPayment if the line item does not exist in the datasource.  If it does exist, fetch the current record.  Of if the line item isn't a payment, return null.</returns>
		private ReconciliationPayment GetReconciliationPayment(ReconciliationReport reconciliationReport, string batchName, DateTime dateCreated, string documentNumber, string supplierName, string supplierNumber, decimal amount)
		{
			// A double entry occurs if identical payment(s) shows up in the same CAS report.  We need to create a separate line item for it otherwise it would get hidden by an existing line item.
			var doubleEntry = reconciliationReport.Payments
				.Count(p => p.BatchName == batchName
				            && p.DateCreated == dateCreated
				            && p.DocumentNumber == documentNumber
				            && p.SupplierName == supplierName
				            && p.Amount == amount);

			// If this payment has already been imported then use it instead of creating a new one (look for an exact match).
			// An assumption is made that if there are double entries that they will be in the same order in each report.  We try and match that order so that we associated existing reconciliation payments to the newly uploaded report.
			var existing = _dbContext.ReconciliationPayments
				.Where(rp => rp.BatchName == batchName
				             && rp.DateCreated == dateCreated
				             && rp.DocumentNumber == documentNumber
				             && rp.SupplierName == supplierName
				             && rp.Amount == amount)
				.OrderBy(rp => rp.DateAdded)
				.Skip(doubleEntry)
				.Take(1)
				.SingleOrDefault();

			if (existing != null)
				return existing;

			// Look for the payment request associated with this payment.
			var paymentRequest = _dbContext.PaymentRequests.SingleOrDefault(pr => pr.DocumentNumber == documentNumber);

			// When a paymentRequest is not found it means one of the following;
			// a) The line item imported is not a valid payment and should be ignored.
			// b) The document Number is invalid and CAS must be informed.
			if (paymentRequest == null)
			{
				// Look for possible payment requests that match this line item.
				var paymentRequests = _dbContext.PaymentRequests
					.Where(pr => pr.GrantApplication.OrganizationLegalName == supplierName
					             && pr.PaymentAmount == amount
					             && pr.DateAdded <= dateCreated // Only look for payment requests before the payment was issued.
								 && !pr.IsReconciled); // Assume this new reconciliation payment would not be associated with a previously reconciled payment request.

				if (paymentRequests.Count() == 1) // A possible matching payment request was found, but the document number doesn't match.
					return new ReconciliationPayment(paymentRequests.First(), batchName, dateCreated, documentNumber, supplierName, supplierNumber, amount)
					{
						ReconcilationState = ReconciliationStates.InvalidDocumentNumber
					};

				if (paymentRequests.Count() > 1) // Many possible matching payment requests were found, but the document number doesn't match.
					return new ReconciliationPayment(batchName, dateCreated, documentNumber, supplierName, supplierNumber, amount)
					{
						ReconcilationState = ReconciliationStates.InvalidDocumentNumber
					};

				if (string.IsNullOrWhiteSpace(supplierNumber)) // Determine if the line item is a payment request.
				{
					// A valid prefix indicates it is an actual payment request.
					if (_validPrefix == null)
					{
						var defaultGrantProgramId = GetDefaultGrantProgramId();

						_validPrefix = _dbContext.GrantPrograms
							.Where(gp => gp.State == GrantProgramStates.Implemented)
							.Where(gp => gp.Id == defaultGrantProgramId)
							.Select(gp => gp.DocumentPrefix).Distinct()
							.ToArray();
					}

					if (_validPrefix.Any(p => documentNumber.StartsWith(p, StringComparison.InvariantCultureIgnoreCase)))
						return new ReconciliationPayment(batchName, dateCreated, documentNumber, supplierName, supplierNumber, amount);

					// Check for an valid supplier name.  If one exists, assume this payment needs to be reconciled event though it does not have a valid document number.
					var supplier = _dbContext.Organizations.FirstOrDefault(o => o.LegalName == supplierName);

					if (supplier != null)
						return new ReconciliationPayment(batchName, dateCreated, documentNumber, supplierName, supplierNumber, amount);

					return null;
				}

				if (doubleEntry > 0) // When a CAS report has identical entries assume it's a duplicate.
					return new ReconciliationPayment(batchName, dateCreated, documentNumber, supplierName, supplierNumber, amount)
					{
						ReconcilationState = ReconciliationStates.Duplicate
					};

				return new ReconciliationPayment(batchName, dateCreated, documentNumber, supplierName, supplierNumber, amount);
			}

			// A matching payment request was found and should be associated with this reconciliation payment.
			return new ReconciliationPayment(paymentRequest, batchName, dateCreated, documentNumber, supplierName, supplierNumber, amount);
		}

		/// <summary>
		/// Get the value from the cell and convert it to the specified type.
		/// </summary>
		/// <typeparam name="T">Type to convert the cell value to.</typeparam>
		/// <param name="workbookPart">The workbook.</param>
		/// <param name="cell">The cell.</param>
		/// <param name="position">For certain types it will truncate the value.</param>
		/// <returns>The value from the cell.</returns>
		private static T GetCellValue<T>(WorkbookPart workbookPart, Cell cell, int? position = null)
		{
			if (cell == null)
				return default;

			try
			{
				var value = cell.InnerText;

				if (cell?.DataType?.Value == CellValues.SharedString)
				{
					if (cell.InnerText != null && cell.InnerText.Trim() == string.Empty)
						return TruncateOrRound((T)System.Convert.ChangeType(value, typeof(T)), position);

					var stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
					if (stringTable != null)
						return TruncateOrRound((T) System.Convert.ChangeType(stringTable.SharedStringTable.ElementAt(int.Parse(value)).InnerText, typeof(T)), position);
				}

				if (typeof(T) == typeof(DateTime))
					return (T)System.Convert.ChangeType(DateTime.FromOADate(double.Parse(value)).ToLocalTime().ToUniversalTime(), typeof(T));

				if (typeof(T) == typeof(DateTime?))
				{
					if (string.IsNullOrEmpty(value))
						return default;

					Type t = typeof(T);
					t = Nullable.GetUnderlyingType(t) ?? t;

					return (T)System.Convert.ChangeType(DateTime.FromOADate(double.Parse(value)).ToLocalTime().ToUniversalTime(), t);
				}

				return TruncateOrRound((T)System.Convert.ChangeType(value, typeof(T)), position);
			}
			catch
			{
				throw new InvalidOperationException($"The data in cell '{cell.CellReference}' was not valid.");
			}
		}

		/// <summary>
		/// Truncate the value to the specified position if it's a string, or round if it's a decimal.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <param name="position">When it's a string it will truncate to this many characters.  If it is a decimal it will truncate to this many decimal points.</param>
		/// <returns></returns>
		private static T TruncateOrRound<T>(T value, int? position = null)
		{
			if (position == null || value == null)
				return value;

			if (position < 0)
				throw new InvalidOperationException($"The argument '{position}' must be equal to or greater than 0.");

			if (typeof(T) == typeof(string))
				return (T)System.Convert.ChangeType(((string)(object)value).Substring(0, position.Value), typeof(T));

			var decimals = (decimal)Math.Pow(10, position.Value); // Should convert 2 to 100.
			if (typeof(T) == typeof(decimal)
			    || typeof(T) == typeof(float)
			    || typeof(T) == typeof(double))
				return (T)System.Convert.ChangeType(Math.Round((decimal)(object)value, position.Value), typeof(T));

			return value;
		}

		/// <summary>
		/// Determine the column letter for the specified index.
		/// </summary>
		/// <param name="colIndex"></param>
		/// <returns></returns>
		private static string ColumnIndexToColumnLetter(int? colIndex)
		{
			if (colIndex == null)
				return null;

			int div = (int)colIndex;
			string colLetter = string.Empty;
			while (div > 0)
			{
				int mod = (div - 1) % 26;
				colLetter = (char)(65 + mod) + colLetter;
				div = (int)((div - mod) / 26);
			}

			return colLetter;
		}

		/// <summary>
		/// Determine the column index for the specified reference.
		/// </summary>
		/// <param name="cellReference"></param>
		/// <returns></returns>
		private static int? GetColumnIndex(string cellReference)
		{
			if (string.IsNullOrEmpty(cellReference))
				return null;

			//remove digits
			string columnReference = Regex.Replace(cellReference.ToUpper(), @"[\d]", string.Empty);

			int columnNumber = -1;
			int multiplier = 1;

			//working from the end of the letters take the ASCII code less 64 (so A = 1, B =2...etc)
			//then multiply that number by our multiplier (which starts at 1)
			//multiply our multiplier by 26 as there are 26 letters
			foreach (char c in columnReference.ToCharArray().Reverse())
			{
				columnNumber += multiplier * ((int)c - 64);

				multiplier = multiplier * 26;
			}

			//the result is zero based so return columnnumber + 1 for a 1 based answer
			//this will match Excel's COLUMN function
			return columnNumber + 1;
		}
	}
}
