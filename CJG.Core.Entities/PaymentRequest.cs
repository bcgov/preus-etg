using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace CJG.Core.Entities
{
	/// <summary>
	/// PaymentRequest class, provides a way to keep payment request information.
	/// </summary>
	public class PaymentRequest : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - The primary key and foreign key to the payment request batch.
		/// </summary>
		[Key, ForeignKey(nameof(PaymentRequestBatch)), Column(Order = 0)]
		public int PaymentRequestBatchId { get; set; }

		/// <summary>
		/// get/set - The payment request batch that contains this payment request.
		/// </summary>
		public virtual PaymentRequestBatch PaymentRequestBatch { get; set; }

		/// <summary>
		/// get/set - The foreign key to the claim.
		/// </summary>
		[Key, ForeignKey(nameof(Claim)), Column(Order = 1)]
		public int ClaimId { get; set; }

		/// <summary>
		/// get/set - The foreign key to the claim.
		/// </summary>
		[Key, ForeignKey(nameof(Claim)), Column(Order = 2)]
		public int ClaimVersion { get; set; }

		/// <summary>
		/// get/set - The claim that is associated with this payment request.
		/// </summary>
		public virtual Claim Claim { get; set; }

		/// <summary>
		/// get/set - The primary key and foreign key to the grant application.
		/// </summary>
		public int GrantApplicationId { get; set; }

		/// <summary>
		/// get/set - The parent grant application.
		/// </summary>
		[ForeignKey(nameof(GrantApplicationId))]
		public virtual GrantApplication GrantApplication { get; set; }

		/// <summary>
		/// get/set - The payment amount.
		/// </summary>
		[Required]
		[Index("IX_PaymentRequest", Order = 3)]
		public decimal PaymentAmount { get; set; }

		/// <summary>
		/// get/set - The payment type.
		/// </summary>
		[Required]
		public PaymentTypes PaymentType { get; set; }

		/// <summary>
		/// get/set - A unique way to identify this payment request.
		/// </summary>
		[Required(AllowEmptyStrings = false, ErrorMessage = "A document number is required."), MaxLength(100)]
		[Index("IX_PaymentRequest", Order = 1)]
		public string DocumentNumber { get; set; }

		/// <summary>
		/// get/set - Whether this is reconciled to CAS.
		/// </summary>
		[DefaultValue(false)]
		[Index("IX_PaymentRequest", Order = 2)]
		public bool IsReconciled { get; set; }

		/// <summary>
		/// get/set - The recipient business number.
		/// </summary>
		[MaxLength(100)]
		public string RecipientBusinessNumber { get; set; }
		
		/// <summary>
		/// get/set - The GL account client number.
		/// </summary>
		[Required(AllowEmptyStrings = false, ErrorMessage = "The general ledger client number is required."), MaxLength(50)]
		public string GLClientNumber { get; set; }

		/// <summary>
		/// get/set - The GL RESP account.
		/// </summary>
		[Required(AllowEmptyStrings = false, ErrorMessage = "The general ledger RESP account number is required."), MaxLength(20)]
		public string GLRESP { get; set; }

		/// <summary>
		/// get/set - The GL service line account.
		/// </summary>
		[Required(AllowEmptyStrings = false, ErrorMessage = "The general ledger service line account number is required."), MaxLength(20)]
		public string GLServiceLine { get; set; }

		/// <summary>
		/// get/set - The GL STOB account.
		/// </summary>
		[Required(AllowEmptyStrings = false, ErrorMessage = "The general ledger STOB account number is required."), MaxLength(20)]
		public string GLSTOB { get; set; }

		/// <summary>
		/// get/set - The GL project code account.
		/// </summary>
		[Required(AllowEmptyStrings = false, ErrorMessage = "The general ledger project code is required."), MaxLength(20)]
		public string GLProjectCode { get; set; }

		/// <summary>
		/// get - A collection of reconciliation payments associated to this payment request.
		/// </summary>
		public virtual ICollection<ReconciliationPayment> ReconciliationPayments { get; } = new List<ReconciliationPayment>();
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a PaymentRequest object.
		/// </summary>
		public PaymentRequest() { }

		/// <summary>
		/// Creates a new instance of a PaymentRequest object and initializes it.
		/// </summary>
		/// <param name="grantApplication"></param>
		/// <param name="batch"></param>
		/// <param name="claim"></param>
		public PaymentRequest(GrantApplication grantApplication, PaymentRequestBatch batch, Claim claim)
		{
			this.GrantApplication = grantApplication ?? throw new ArgumentNullException(nameof(grantApplication));
			this.GrantApplicationId = grantApplication.Id;
			this.PaymentRequestBatch = batch ?? throw new ArgumentNullException(nameof(batch));
			this.PaymentRequestBatchId = batch.Id;
			this.Claim = claim ?? throw new ArgumentNullException(nameof(claim));
			this.ClaimId = claim.Id;
			this.ClaimVersion = claim.ClaimVersion;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Validates the PaymentRequest property values.
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


			if (this.Claim == null) this.Claim = context.Claims.Include(c => c.GrantApplication).Include(c => c.GrantApplication.PaymentRequests).FirstOrDefault(c => c.Id == this.ClaimId && c.ClaimVersion == this.ClaimVersion);
			if (this.Claim.GrantApplication?.PaymentRequests == null || !this.Claim.GrantApplication.PaymentRequests.Any()) this.GrantApplication = context.GrantApplications.Include(ga => ga.PaymentRequests).FirstOrDefault(ga => ga.Id == this.GrantApplicationId);

			// Payment amount cannot exceed the amount owing.
			var amountPaidOrOwing = this.Claim.AmountPaidOrOwing();
			if (this.PaymentAmount != amountPaidOrOwing)
				yield return new ValidationResult($"The payment amount is invalid and should be {amountPaidOrOwing.ToString("c2")}.", new[] { nameof(this.PaymentAmount) });

			foreach (var validation in base.Validate(validationContext))
			{
				yield return validation;
			}
		}
		#endregion
	}
}
