using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using CJG.Core.Entities.Attributes;

namespace CJG.Core.Entities
{
	/// <summary>
	/// PaymentRequestBatch class, provides a way to create a batch of payment requests.
	/// </summary>
	public class PaymentRequestBatch : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - Primary key uses IDENTITY
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - A unique batch number to provide a printable identifier.
		/// </summary>
		[Required, MaxLength(4), Index("IX_PaymentRequestBatch", 1),]
		public string BatchNumber { get; set; }

		/// <summary>
		/// get/set - Identify the type of payment batch [paid|owing].
		/// </summary>
		[Required]
		public PaymentBatchTypes BatchType { get; set; }

		/// <summary>
		/// get - The number of payment requests in the batch.
		/// </summary>
		[Required]
		public int RequestCount => PaymentRequests.Count;

		/// <summary>
		/// get/set - The date the batch was issued.
		/// </summary>
		[Required, DateTimeKind(DateTimeKind.Utc)]
		[Column(TypeName = "DATETIME2")]
		public DateTime IssuedDate { get; set; }

		/// <summary>
		/// get/set - Foreign key to user who issued the batch.
		/// </summary>
		public int IssuedById { get; set; }

		/// <summary>
		/// get/set - The user who issued the batch.
		/// </summary>
		public virtual InternalUser IssuedBy { get; set; }

		/// <summary>
		/// get/set - The full name of the user who issued the batch.  Used for historical accuracy.
		/// </summary>
		[Required, MaxLength(250)]
		public string IssuedByName { get; set; }

		/// <summary>
		/// get/set - A description of who requested the batch.  Derived from the grant program.  Used for historical accuracy.
		/// </summary>
		[MaxLength(250)]
		public string RequestedBy { get; set; }

		/// <summary>
		/// get/set - The phone number associated with the batch.  Derived from the grant program. Used for historical accuracy.
		/// </summary>
		[MaxLength(50)]
		public string ProgramPhone { get; set; }

		/// <summary>
		/// get/set - A unique document prefix used when printing.  Derived from the grant program.  Used for historical accuracy.
		/// </summary>
		[MaxLength(5)]
		public string DocumentPrefix { get; set; }

		/// <summary>
		/// get/set - Foreign key to the user who can authorize the expense.  Derived from the grant program.  Used for historical accuracy.
		/// </summary>
		public int? ExpenseAuthorityId { get; set; }

		/// <summary>
		/// get/set - The user who can authorize the expense.  Derived from the grant program.  Used for historical accuracy.
		/// </summary>
		[ForeignKey(nameof(ExpenseAuthorityId))]
		public virtual InternalUser ExpenseAuthority { get; set; }

		/// <summary>
		/// get/set - The expense authorities full name.  Derived from the grant program.  Used for historical accuracy.
		/// </summary>
		[Required, MaxLength(250)]
		public string ExpenseAuthorityName { get; set; }

		/// <summary>
		/// get/set - Foreign key to the grant program associated with this batch.
		/// </summary>
		[Required]
		public int GrantProgramId { get; set; }

		/// <summary>
		/// get/set - The grant program associated with this batch.
		/// </summary>
		[ForeignKey(nameof(GrantProgramId))]
		public virtual GrantProgram GrantProgram { get; set; }

		/// <summary>
		/// get/set - The batch request description.  Derived from the grant program.  Used for historical accuracy.
		/// </summary>
		[MaxLength(2000)]
		public string BatchRequestDescription { get; set; }

		/// <summary>
		/// get/set - All the payment requested associated with this batch.
		/// </summary>
		public virtual ICollection<PaymentRequest> PaymentRequests { get; set; } = new List<PaymentRequest>();

		/// <summary>
		/// get - All the pending requests associated with this batch.
		/// </summary>
		[NotMapped]
		public IEnumerable<PaymentRequest> PendingRequests => this.PaymentRequests.Where(pr => !pr.IsReconciled).OrderBy(o => o.DocumentNumber).ToArray();
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a PaymentRequestBatch object.
		/// </summary>
		public PaymentRequestBatch()
		{

		}

		/// <summary>
		/// Creates a new instance of a PaymentRequestBatch object and initializes it.
		/// </summary>
		/// <param name="grantProgram"></param>
		/// <param name="batchNumber"></param>
		/// <param name="type"></param>
		/// <param name="issuedBy"></param>
		public PaymentRequestBatch(GrantProgram grantProgram, string batchNumber, PaymentBatchTypes type, InternalUser issuedBy)
		{
			if (String.IsNullOrWhiteSpace(batchNumber))
				throw new ArgumentException("The {0} is a required argument value.", nameof(batchNumber));

			this.GrantProgram = grantProgram ?? throw new ArgumentNullException(nameof(grantProgram));
			this.GrantProgramId = grantProgram.Id;
			this.BatchNumber = batchNumber;
			this.BatchType = type;
			this.IssuedBy = issuedBy ?? throw new ArgumentNullException(nameof(issuedBy));
			this.IssuedById = issuedBy.Id;
			this.IssuedByName = $"{issuedBy.LastName}, {issuedBy.FirstName}";
			this.IssuedDate = AppDateTime.UtcNow;

			this.RequestedBy = grantProgram.RequestedBy;
			this.ExpenseAuthority = grantProgram.ExpenseAuthority;
			this.ExpenseAuthorityId = grantProgram.ExpenseAuthorityId;
			this.ExpenseAuthorityName = $"{grantProgram.ExpenseAuthority.FirstName} {grantProgram.ExpenseAuthority.LastName}";
			this.BatchRequestDescription = grantProgram.BatchRequestDescription;
			this.ProgramPhone = grantProgram.ProgramPhone;
			this.DocumentPrefix = grantProgram.DocumentPrefix;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Validates the PaymentRequestBatch property values.
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
