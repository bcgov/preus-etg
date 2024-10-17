using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;

namespace CJG.Web.External.Areas.Int.Models.Reconciliation
{
	public class PaymentRequestViewModel : BaseViewModel
	{
		#region Properties
		public int PaymentRequestBatchId { get; set; }
		public int GrantApplicationId { get; set; }
		public int ClaimId { get; set; }
		public int ClaimVersion { get; set; }
		public string RowVersion { get; set; }
		public string DocumentNumber { get; set; }
		public PaymentTypes PaymentType { get; set; }
		public string PaymentTypeCaption { get; set; }
		public bool IsReconciled { get; set; }
		public decimal PaymentAmount { get; set; }
		public string SupplierName { get; set; }
		public DateTime DateAdded { get; set; }
		public DateTime? IssuedDate { get; private set; }
		public int? ReconciliationPaymentId { get; set; }
		#endregion

		#region Constructors
		public PaymentRequestViewModel() { }

		public PaymentRequestViewModel(PaymentRequest paymentRequest)
		{
			if (paymentRequest == null) throw new ArgumentNullException(nameof(paymentRequest));

			this.PaymentRequestBatchId = paymentRequest.PaymentRequestBatchId;
			this.GrantApplicationId = paymentRequest.GrantApplicationId;
			this.ClaimId = paymentRequest.ClaimId;
			this.ClaimVersion = paymentRequest.ClaimVersion;
			this.RowVersion = Convert.ToBase64String(paymentRequest.RowVersion);
			this.DocumentNumber = paymentRequest.DocumentNumber;
			this.PaymentType = paymentRequest.PaymentType;
			this.PaymentTypeCaption = paymentRequest.PaymentType.GetDescription();
			this.IsReconciled = paymentRequest.IsReconciled;
			this.PaymentAmount = paymentRequest.PaymentAmount;
			this.SupplierName = paymentRequest.GrantApplication.OrganizationLegalName;
			this.DateAdded = paymentRequest.DateAdded;
			this.IssuedDate = paymentRequest?.PaymentRequestBatch?.IssuedDate;
		}
		#endregion

		#region Methods
		public override int GetHashCode()
		{
			return this.PaymentRequestBatchId.GetHashCode()
				+ this.GrantApplicationId.GetHashCode()
				+ this.ClaimId.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var paymentRequest = obj as PaymentRequestViewModel;
			return this.PaymentRequestBatchId == paymentRequest?.PaymentRequestBatchId
				&& this.GrantApplicationId == paymentRequest?.GrantApplicationId
				&& this.ClaimId == paymentRequest?.ClaimId;
		}
		#endregion
	}
}