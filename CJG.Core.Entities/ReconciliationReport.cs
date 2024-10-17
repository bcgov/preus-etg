using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="ReconciliationReport"/> class, provides the ORM a way to manage reconciliation reports.
	/// </summary>
	public class ReconciliationReport : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - The primary key to identify this reconcilation report.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Index("IX_ReconciliationReport", Order = 1)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - The foreign key to the user who created the reconciliation report.
		/// </summary>
		public int InternalUserId { get; set; }

		/// <summary>
		/// get/set - The user who created the reconciliation report.
		/// </summary>
		[ForeignKey(nameof(InternalUserId))]
		public virtual InternalUser Creator { get; set; }

		/// <summary>
		/// get/set - The date the report was run in CAS.
		/// </summary>
		[Required(ErrorMessage = "The date the report was run in CAS is required.")]
		[Column(TypeName = "DATETIME2")]
		public DateTime DateRun { get; set; }

		/// <summary>
		/// get/set - The name of the requestor from CAS.
		/// </summary>
		[Required(ErrorMessage = "The requestor is required")]
		[MaxLength(100, ErrorMessage = "The requestor cannot have more than a maximum of 100 characters.")]
		public string Requestor { get; set; }

		/// <summary>
		/// get/set - The period from in the CAS report.
		/// </summary>
		[Required(ErrorMessage = "The period from is required.")]
		[Index("IX_ReconciliationReport", Order = 3)]
		[Column(TypeName = "DATETIME2")]
		public DateTime PeriodFrom { get; set; }

		/// <summary>
		/// get/set - The period to in the CAS report.
		/// </summary>
		[Required(ErrorMessage = "The period to is required.")]
		[Index("IX_ReconciliationReport", Order = 4)]
		[Column(TypeName = "DATETIME2")]
		public DateTime PeriodTo { get; set; }

		/// <summary>
		/// get/set - Whether this reconciliation report is reconciled.
		/// </summary>
		[DefaultValue(false)]
		[Index("IX_ReconciliationReport", Order = 2)]
		public bool IsReconciled { get; set; }

		/// <summary>
		/// get - A collection of all the payments related to this report.
		/// </summary>
		public virtual ICollection<ReconciliationPayment> Payments { get; } = new List<ReconciliationPayment>();
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a ReconciliationReport object.
		/// </summary>
		public ReconciliationReport()
		{

		}

		/// <summary>
		/// Creates a new instance of a ReconciliationReport object and initializes it with the specified property values.
		/// </summary>
		/// <param name="creator"></param>
		public ReconciliationReport(InternalUser creator)
		{
			if (creator == null) throw new ArgumentNullException(nameof(creator));

			this.Creator = creator;
		}

		/// <summary>
		/// Creates a new instance of a ReconciliationReport object and initializes it with the specified property values.
		/// </summary>
		/// <param name="creator"></param>
		/// <param name="dateRun"></param>
		/// <param name="requestor"></param>
		/// <param name="periodFrom"></param>
		/// <param name="periodTo"></param>
		public ReconciliationReport(InternalUser creator, DateTime dateRun, string requestor, DateTime periodFrom, DateTime periodTo) : this(creator)
		{
			this.DateRun = dateRun;
			this.Requestor = requestor;
			this.PeriodFrom = periodFrom;
			this.PeriodTo = periodTo;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Validates the reconcilation report.
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
