using CJG.Core.Entities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace CJG.Core.Entities
{
	/// <summary>
	/// <typeparamref name="TrainingPeriod"/> class, provides the ORM a way to manage training periods.
	/// </summary>
	public class TrainingPeriod : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - The primary key uses IDENTITY.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - The caption name of this training period.
		/// </summary>
		[Required, MaxLength(250)]
		public string Caption { get; set; }

		/// <summary>
		/// get/set - A short name to identify this training period.
		/// </summary>
		[MaxLength(20)]
		public string ShortName { get; set; }

		/// <summary>
		/// get/set - The start date for this training period.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Required]
		[Index("IX_TrainingPeriod", 2, IsUnique = true)]
		[Column(TypeName = "DATETIME2")]
		public DateTime StartDate { get; set; }

		/// <summary>
		/// get/set - The end date for this training period.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Required]
		[Index("IX_TrainingPeriod", 3, IsUnique = true)]
		[Column(TypeName = "DATETIME2")]
		public DateTime EndDate { get; set; }

		/// <summary>
		/// get/set - The default publishing date for grant openings.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Required]
		[Column(TypeName = "DATETIME2")]
		public DateTime DefaultPublishDate { get; set; }

		/// <summary>
		/// get/set - The default opening dates for grant openings.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Required]
		[Column(TypeName = "DATETIME2")]
		public DateTime DefaultOpeningDate { get; set; }

		/// <summary>
		/// get/set - The foreign key to the fiscal year.
		/// </summary>
		[Index("IX_TrainingPeriod", 1, IsUnique = true)]
		public int FiscalYearId { get; set; }

		/// <summary>
		/// get/set - The parent fiscal year.
		/// </summary>
		[ForeignKey(nameof(FiscalYearId))]
		public virtual FiscalYear FiscalYear { get; set; }

		/// <summary>
		/// get - All the grant openings associated with this training period.
		/// </summary>
		public ICollection<GrantOpening> GrantOpenings { get; set; } = new List<GrantOpening>();
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="TrainingPeriod"/> object.
		/// </summary>
		public TrainingPeriod()
		{ }

		/// <summary>
		/// Creates a new instance of a <typeparamref name="TrainingPeriod"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="fiscalYear"></param>
		/// <param name="caption"></param>
		/// <param name="startOn"></param>
		/// <param name="endOn"></param>
		/// <param name="defaultPublishOn"></param>
		/// <param name="defaultOpeningOn"></param>
		public TrainingPeriod(FiscalYear fiscalYear, string caption, DateTime startOn, DateTime endOn, DateTime defaultPublishOn, DateTime defaultOpeningOn)
		{
			if (String.IsNullOrEmpty(caption))
				throw new ArgumentNullException(nameof(caption));

			if (startOn >= endOn)
				throw new ArgumentException("The start date must be before the closing date.", nameof(startOn));

			if (defaultPublishOn > startOn)
				throw new ArgumentException($"The default publish date must be before or on the start date '{startOn:yyyy-MM-dd}'.", nameof(defaultPublishOn));

			if (defaultPublishOn > defaultOpeningOn)
				throw new ArgumentException($"The default opening date must be after or on the publish date '{defaultPublishOn:yyyy-MM-dd}'.", nameof(defaultOpeningOn));

			this.FiscalYear = fiscalYear ?? throw new ArgumentNullException(nameof(fiscalYear));
			this.FiscalYearId = fiscalYear.Id;
			this.Caption = caption;
			this.StartDate = startOn.ToUniversalTime();
			this.EndDate = endOn.ToUniversalTime();
			this.DefaultPublishDate = defaultPublishOn.ToUniversalTime();
			this.DefaultOpeningDate = defaultOpeningOn.ToUniversalTime();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Validates the TrainingPeriod property values.
		/// </summary>
		/// <param name="validationContext"></param>
		/// <returns></returns>
		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var entry = validationContext.GetDbEntityEntry();

			// This is done to stop errors from being thrown when developers use EF entities in ViewModels.
			if (entry == null)
				yield break;

			// Must be associated with a FiscalYear.
			if (this.FiscalYear == null && this.FiscalYearId == 0)
				yield return new ValidationResult("The training period must be associated with a fiscal year.", new[] { nameof(this.FiscalYear) });

			// StartDate must be before EndDate.
			if (this.StartDate >= this.EndDate)
				yield return new ValidationResult($"The start date must be before the end date '{this.EndDate.ToLocalMidnight():yyyy-MM-dd}'.", new[] { nameof(this.StartDate) });

			// The DefaultPublishDate cannot be after the DefaultOpeningDate
			if (this.DefaultPublishDate > this.DefaultOpeningDate)
				yield return new ValidationResult($"The default publish date cannot be after the default opening date '{this.DefaultOpeningDate.ToLocalMorning():yyyy-MM-dd}'.", new[] { nameof(this.DefaultPublishDate) });

			// The DefaultPublishDate must be before the EndDate.
			if (this.DefaultPublishDate >= this.EndDate)
				yield return new ValidationResult($"The default publish date must be before the end date '{this.EndDate.ToLocalMidnight():yyyy-MM-dd}'.", new[] { nameof(this.DefaultPublishDate) });

			// The DefaultOpeningDate must fall within the the DefaultPublishDate and EndDate.
			if (this.DefaultOpeningDate < this.DefaultPublishDate || this.DefaultOpeningDate >= this.EndDate)
				yield return new ValidationResult($"The default opening date must be between '{this.DefaultPublishDate.ToLocalMorning():yyyy-MM-dd}' and '{this.EndDate.ToLocalMidnight():yyyy-MM-dd}'.", new[] { nameof(this.DefaultOpeningDate) });

			if (entry.State == EntityState.Added)
			{
				// The EndDate cannot be before today.
				if (this.EndDate.Date < AppDateTime.UtcNow.Date)
					yield return new ValidationResult("The end date cannot be in the past.", new[] { nameof(this.EndDate) });
			}
			else if (entry.State == EntityState.Modified)
			{
				// The EndDate cannot be modified to a date before the original EndDate.
				var original_endDate = (DateTime)entry.OriginalValues[nameof(this.EndDate)];
				if (this.EndDate != original_endDate && this.EndDate.Date < original_endDate)
					yield return new ValidationResult($"The end date cannot be modified to a date in the before '{original_endDate.ToLocalMidnight():yyyy-MM-dd}'.", new[] { nameof(this.EndDate) });
			}

			foreach (var validation in base.Validate(validationContext))
			{
				yield return validation;
			}
		}
		#endregion
	}
}
