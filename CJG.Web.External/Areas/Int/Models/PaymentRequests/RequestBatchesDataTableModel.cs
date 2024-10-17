using CJG.Core.Entities;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models.PaymentRequests
{
	public class RequestBatchesDataTableModel
	{
		#region Properties
		public int BatchId { get; set; }
		public bool IsPaymentRequest { get; set; }
		public string Batch { get; set; }
		public string BatchType { get; set; }
		public string IssuedDate { get; set; }
		public string IssuedBy { get; set; }
		public int NumberOfRequests { get; set; }
		public int NumberOfPendingRequests { get; set; }
		#endregion

		#region Constructors
		public RequestBatchesDataTableModel(PaymentRequestBatch paymentRequestBatch)
		{
			this.BatchId = paymentRequestBatch.Id;
			this.IsPaymentRequest = paymentRequestBatch.BatchType == PaymentBatchTypes.PaymentRequest;
			this.Batch = paymentRequestBatch.BatchNumber;
			this.BatchType = paymentRequestBatch.BatchType.GetDescription();

			this.IssuedDate = paymentRequestBatch.IssuedDate.ToLocalMorning().ToString("yyyy-MM-dd") ?? "N/A";

			this.IssuedBy = $"{paymentRequestBatch.IssuedBy.FirstName} {paymentRequestBatch.IssuedBy.LastName}";

			this.NumberOfRequests = paymentRequestBatch.RequestCount;
			this.NumberOfPendingRequests = paymentRequestBatch.PendingRequests.Count();
		}
		#endregion
	}
}
