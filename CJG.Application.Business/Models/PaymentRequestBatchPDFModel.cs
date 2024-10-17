using CJG.Core.Entities;
using System;

namespace CJG.Application.Business.Models
{
	public class PaymentRequestBatchPDFModel : RequestBatchPDFBaseModel
	{
		#region Properties
		public string Description { get; set; }
		public string Certification { get; set; } = "Certified that the amount to be paid is correct, is in accordance with appropriate statute or other authority and/or contract and where applicable that the work has been performed and the services rendered or other conditions met.";
		public string RequestBy { get; set; }
		public string NumberOfRequests => $"--{(PaymentRequests?.Count ?? 0):D3}--";
		public string Authority { get; set; }
		public string Phone { get; set; }
		#endregion

		#region Constructors
		public PaymentRequestBatchPDFModel(PaymentRequestBatch batch, bool duplicate) : base(batch)
		{
			if (batch == null) throw new ArgumentNullException(nameof(batch));

			this.Description = batch.GrantProgram.BatchRequestDescription;
			this.RequestBy = batch.RequestedBy;
			this.Authority = string.Format("{0}, {1}", batch.ExpenseAuthority.LastName, batch.ExpenseAuthority.FirstName);
			this.Phone = batch.ProgramPhone;

			foreach (var paymentRequest in batch.PendingRequests)
				this.PaymentRequests.Add(new PaymentRequestPDFModel(paymentRequest, duplicate));
		}
		#endregion
	}
}
