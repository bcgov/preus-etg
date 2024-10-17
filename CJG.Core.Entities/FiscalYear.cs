using CJG.Core.Entities.Attributes;
using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="FiscalYear"/> class, provides the ORM a way to manage the fiscal years.
	/// </summary>
	public class FiscalYear : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - The primary key uses IDENTITY.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - The caption to identify the fiscal year.
		/// </summary>
		[Required, MaxLength(250)]
		public string Caption { get; set; }

		/// <summary>
		/// get/set - The starting date of this fiscal year.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Required, Index("IX_FiscalYear_Dates", 1, IsUnique = true)]
		[Column(TypeName = "DATETIME2")]
		public DateTime StartDate { get; set; }

		/// <summary>
		/// get/set - The ending date of this fiscal year.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Required, Index("IX_FiscalYear_Dates", 2, IsUnique = true)]
		[Column(TypeName = "DATETIME2")]
		public DateTime EndDate { get; set; }

		/// <summary>
		/// get/set - The next agreement number.
		/// </summary>
		[Required, Min(1), DefaultValue(1)]
		public int NextAgreementNumber { get; set; } = 1;

		/// <summary>
		/// get - All the training periods associated with this fiscal year.
		/// </summary>
		public ICollection<TrainingPeriod> TrainingPeriods { get; set; } = new List<TrainingPeriod>();

		/// <summary>
		/// get - All the report rates associated with this fiscal year.
		/// </summary>
		public ICollection<ReportRate> ReportRates { get; set; } = new List<ReportRate>();
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="FiscalYear"/> object.
		/// </summary>
		public FiscalYear()
		{ }

		/// <summary>
		/// Creates a new instance of a <typeparamref name="FiscalYear"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		public FiscalYear(string caption, DateTime startDate, DateTime endDate)
		{
			if (String.IsNullOrEmpty(caption))
				throw new ArgumentNullException(nameof(caption));

			if (endDate < startDate)
				throw new ArgumentException("The start date must be before the end date.", nameof(startDate));

			this.Caption = caption;
			this.StartDate = startDate.ToUniversalTime();
			this.EndDate = endDate.ToUniversalTime();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Validates the FiscalYear property values.
		/// </summary>
		/// <param name="validationContext"></param>
		/// <returns></returns>
		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var entry = validationContext.GetDbEntityEntry();

			// This is done to stop errors from being thrown when developers use EF entities in ViewModels.
			if (entry == null)
				yield break;

			// The StartDate must be before the EndDate.
			if (this.EndDate < this.StartDate)
				yield return new ValidationResult("The start date must be before the end date.", new[] { nameof(this.StartDate), nameof(this.EndDate) });

			if (entry.State == EntityState.Added)
			{
				// April 1
				// The StartDate cannot be in the past.
				if (this.StartDate.Date < AppDateTime.UtcNow.Date)
					yield return new ValidationResult("The start date cannot be in the past.", new[] { nameof(this.StartDate) });

				// March 31
				// The EndDate cannot be in the past.
				if (this.EndDate.Date < AppDateTime.UtcNow.Date)
					yield return new ValidationResult("The end date cannot be in the past.", new[] { nameof(this.EndDate) });
			}
			else if (entry.State == EntityState.Modified)
			{
				var original_startDate = (DateTime)entry.OriginalValues[nameof(this.StartDate)];

				// Cannot modify the StartDate to a date prior to the original StartDate.
				if (this.StartDate < original_startDate)
					yield return new ValidationResult($"The start date cannot be before {original_startDate.ToLocalMorning():yyyy-MM-dd}.", new[] { nameof(this.StartDate) });

				// Cannot modify the EndDate to a date prior the original StartDate.
				if (this.EndDate < original_startDate)
					yield return new ValidationResult($"The end date cannot be before {original_startDate.ToLocalMorning():yyyy-MM-dd}.", new[] { nameof(this.EndDate) });
			}

			foreach (var validation in base.Validate(validationContext))
			{
				yield return validation;
			}
		}
		#endregion
	}
}
