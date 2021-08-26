using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="ReconciliationPayment"/> class, provides the ORM a way to manage reconciliation payments.
	/// </summary>
	public class ReconciliationPayment : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - The primary key to identify this reconcilation payment.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Index("IX_ReconciliationPayment", Order = 1)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - The foreign key to the payment request and payment request batch.
		/// </summary>
		[Column(Order = 2)]
		public int? PaymentRequestBatchId { get; set; }

		/// <summary>
		/// get/set - The payment request batch.
		/// </summary>
		[ForeignKey(nameof(PaymentRequestBatchId))]
		public virtual PaymentRequestBatch PaymentRequestBatch { get; set; }

		/// <summary>
		/// get/set - The foreign key to the claim and payment request this payment is associated with.
		/// </summary>
		[ForeignKey(nameof(Claim)), Column(Order = 3)]
		public int? ClaimId { get; set; }

		/// <summary>
		/// get/set - The foreign key to the claim and payment request this payment is associated with.
		/// </summary>
		[ForeignKey(nameof(Claim)), Column(Order = 4)]
		public int? ClaimVersion { get; set; }

		/// <summary>
		/// get/set - The claim associated with this payment.
		/// </summary>
		public Claim Claim { get; set; }

		/// <summary>
		/// get/set - The payment request associated to this reconciliation payment.
		/// </summary>
		public virtual PaymentRequest PaymentRequest { get; set; }

		/// <summary>
		/// get/set - The foreign key to the grant application and payment request this payment is associated with.
		/// </summary>
		[Index("IX_ReconciliationPayment", Order = 5)]
		public int? GrantApplicationId { get; set; }

		/// <summary>
		/// get/set - The grant application this payment is associated with.
		/// </summary>
		[ForeignKey(nameof(GrantApplicationId))]
		public virtual GrantApplication GrantApplication { get; set; }

		/// <summary>
		/// get/set - The document number to identify the grant application and claim it is associated with, which comes from CAS.
		/// </summary>
		[Index("IX_ReconciliationPayment", Order = 6)]
		[Required(ErrorMessage = "The document number is required."), MaxLength(75, ErrorMessage = "The document number can only be a maximum of 75 characters.")]
		public string DocumentNumber { get; set; }

		/// <summary>
		/// get/set - The supplier name from CAS report.
		/// </summary>
		[Index("IX_ReconciliationPayment", Order = 7)]
		[Required(ErrorMessage = "The supplier name is required."), MaxLength(250, ErrorMessage = "The supplier name can only be a maximum of 250 characters.")]
		public string SupplierName { get; set; }

		/// <summary>
		/// get/set - The supplier number from CAS>
		/// </summary>
		[MaxLength(30, ErrorMessage = "The supplier number can only be a maximum of 30 characters.")]
		public string SupplierNumber { get; set; }

		/// <summary>
		/// get/set - The batch name from CAS.
		/// </summary>
		[Index("IX_ReconciliationPayment", Order = 8)]
		[MaxLength(100, ErrorMessage = "The batch name can only be a maximum of 100 characters.")]
		public string BatchName { get; set; }

		/// <summary>
		/// get/set - The amount from CAS.
		/// </summary>
		[Index("IX_ReconciliationPayment", Order = 9)]
		public decimal Amount { get; set; }

		/// <summary>
		/// get/set - The date the payment was created from CAS.
		/// </summary>
		[Index("IX_ReconciliationPayment", Order = 10)]
		[Column(TypeName = "DATETIME2")]
		public DateTime? DateCreated { get; set; }

		/// <summary>
		/// get/set - The payment type.
		/// </summary>
		public PaymentTypes PaymentType { get; set; }

		/// <summary>
		/// get/set - The state of this payment.
		/// </summary>
		[Index("IX_ReconciliationPayment", Order = 11)]
		public ReconciliationStates ReconcilationState { get; set; }

		/// <summary>
		/// get/set - Whether this reconciliation payment originated from a CAS Report.
		/// </summary>
		public bool FromCAS { get; set; }

		/// <summary>
		/// get - A collection of all the reconciliation reports related to this payment.
		/// </summary>
		public virtual ICollection<ReconciliationReport> Reports { get; } = new List<ReconciliationReport>();
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a ReconciliationPayment object.
		/// </summary>
		public ReconciliationPayment()
		{

		}

		/// <summary>
		/// Creates a new instance of a ReconciliationPayment object and initializes it with the specified property values.
		/// </summary>
		/// <param name="paymentRequest"></param>
		public ReconciliationPayment(PaymentRequest paymentRequest)
		{
			this.PaymentRequestBatchId = paymentRequest?.PaymentRequestBatchId ?? throw new ArgumentNullException(nameof(paymentRequest));
			this.PaymentRequestBatch = paymentRequest.PaymentRequestBatch;
			this.GrantApplicationId = paymentRequest?.GrantApplicationId ?? throw new ArgumentNullException(nameof(paymentRequest));
			this.GrantApplication = paymentRequest.GrantApplication;
			this.ClaimId = paymentRequest?.ClaimId ?? throw new ArgumentNullException(nameof(paymentRequest));
			this.ClaimVersion = paymentRequest?.ClaimVersion ?? throw new ArgumentNullException(nameof(paymentRequest));
			this.PaymentRequest = paymentRequest;
			paymentRequest.ReconciliationPayments.Add(this);
			this.DocumentNumber = paymentRequest.DocumentNumber;
			this.SupplierName = paymentRequest.GrantApplication.OrganizationLegalName;
			this.SupplierNumber = paymentRequest.RecipientBusinessNumber;
			this.Amount = 0; // Must be $0 so that it will reconcile when an actual CAS line item is provided.
			this.FromCAS = false;

			this.PaymentType = paymentRequest.PaymentType;
			this.ReconcilationState = ReconciliationStates.NoMatch;
		}

		/// <summary>
		/// Creates a new instance of a ReconciliationPayment object and initializes it with the specified property values.
		/// </summary>
		/// <param name="paymentRequest"></param>
		/// <param name="batchName"></param>
		/// <param name="dateCreated"></param>
		/// <param name="documentNumber"></param>
		/// <param name="supplierName"></param>
		/// <param name="supplierNumber"></param>
		/// <param name="amount"></param>
		public ReconciliationPayment(PaymentRequest paymentRequest, string batchName, DateTime dateCreated, string documentNumber, string supplierName, string supplierNumber, decimal amount)
		{
			this.PaymentRequestBatchId = paymentRequest?.PaymentRequestBatchId ?? throw new ArgumentNullException(nameof(paymentRequest));
			this.PaymentRequestBatch = paymentRequest.PaymentRequestBatch;
			this.GrantApplicationId = paymentRequest?.GrantApplicationId ?? throw new ArgumentNullException(nameof(paymentRequest));
			this.GrantApplication = paymentRequest.GrantApplication;
			this.ClaimId = paymentRequest?.ClaimId ?? throw new ArgumentNullException(nameof(paymentRequest));
			this.ClaimVersion = paymentRequest?.ClaimVersion ?? throw new ArgumentNullException(nameof(paymentRequest));
			this.PaymentRequest = paymentRequest;
			paymentRequest.ReconciliationPayments.Add(this);
			this.BatchName = batchName;
			this.DateCreated = dateCreated;
			this.DocumentNumber = documentNumber;
			this.SupplierName = supplierName;
			this.SupplierNumber = supplierNumber;
			this.Amount = amount;
			this.FromCAS = true;

			this.PaymentType = this.DeterminePaymentType();
			this.ReconcilationState = ReconciliationStates.NotReconciled;
		}

		/// <summary>
		/// Creates a new instance of a ReconciliationPayment object and initializes it with the specified property values.
		/// The state will be 'NoMatch' because there is no associated payment request.
		/// </summary>
		/// <param name="batchName"></param>
		/// <param name="dateCreated"></param>
		/// <param name="documentNumber"></param>
		/// <param name="supplierName"></param>
		/// <param name="supplierNumber"></param>
		/// <param name="amount"></param>
		public ReconciliationPayment(string batchName, DateTime dateCreated, string documentNumber, string supplierName, string supplierNumber, decimal amount)
		{
			this.BatchName = batchName;
			this.DateCreated = dateCreated;
			this.DocumentNumber = documentNumber;
			this.SupplierName = supplierName;
			this.SupplierNumber = supplierNumber;
			this.Amount = amount;
			this.FromCAS = true;

			this.PaymentType = this.DeterminePaymentType();
			this.ReconcilationState = ReconciliationStates.NoMatch;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Validates the reconcilation payment.
		/// </summary>
		/// <param name="validationContext"></param>
		/// <returns></returns>
		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var context = validationContext.GetDbContext();
			var entry = validationContext.GetDbEntityEntry();

			// This is done to stop errors from being thrown when developers use EF entities in ViewModels.
			if (entry == null)
				yield break;

			foreach (var validation in base.Validate(validationContext))
			{
				yield return validation;
			}
		}
		#endregion
	}
}
