using System.Collections.Generic;
using CJG.Core.Entities;
using CJG.Application.Business.Models;
using CJG.Core.Entities.Helpers;

namespace CJG.Core.Interfaces.Service
{
	public interface IPaymentRequestService : IService
	{
		void AddPaymentRequest(PaymentRequest paymentRequest);

		PaymentRequest GetPaymentRequest(int paymentRequestBatchId, int trainingProgramId);

		PageList<PaymentRequestBatch> GetRequestBatches(int grantProgramId, int page, int quantity, string search);
		
        PaymentRequestBatch GeneratePaymentRequests(int grantProgramId);
        PaymentRequestBatch GenerateAmountOwing(int grantProgramId);
        PaymentRequestBatch GeneratePaymentRequestBatch(int grantProgramId, IEnumerable<Claim> claims, PaymentBatchTypes batchType);
        PaymentRequestBatchPDFModel GetPaymentRequestBatchPDF(int id, bool duplicate);
        AmountOwingBatchPDFModel GetAmountOwingBatchPDF(int id, bool duplicate);
        PaymentRequestBatch GetPaymentRequestBatch(int id);
        PageList<PaymentRequest> GetPaymentRequests(int page = 1, int quantity = 10, string search = null, string sort = nameof(PaymentRequest.DateAdded));
    }
}
