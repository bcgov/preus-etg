using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using CJG.Core.Entities.Attributes;
using CJG.Infrastructure.Entities;

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
		/// get/set - The foreign key to the Grant Stream this Training Period is associated with.
		/// </summary>
		[Index("IX_TrainingPeriod", 4), DefaultValue(true)]
		public int? GrantStreamId { get; set; }

		/// <summary>
		/// get/set - The Grant Stream this Training Period is associated with.
		/// </summary>
		[ForeignKey(nameof(GrantStreamId))]
		public virtual GrantStream GrantStream { get; set; }

		/// <summary>
		/// get/set - Whether this training period is available for grant openings.
		/// </summary>
		[Required]
		[Index("IX_TrainingPeriod", 5), DefaultValue(true)]
		public bool IsActive { get; set; } = true;

		/// <summary>
		/// get/set - Overpayment amount entered by user for display on Claims Dashboard.
		/// </summary>
		[DisplayName("Overpayment Amount")]
		[DefaultValue(0)]
		public decimal OverpaymentAmount { get; set; }

		/// <summary>
		/// get - All the grant openings associated with this training period.
		/// </summary>
		public virtual ICollection<GrantOpening> GrantOpenings { get; set; } = new List<GrantOpening>();
		#endregion

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
			if (string.IsNullOrEmpty(caption))
				throw new ArgumentNullException(nameof(caption));

			if (startOn >= endOn)
				throw new ArgumentException("The start date must be before the closing date.", nameof(startOn));

			if (defaultPublishOn > startOn)
				throw new ArgumentException($"The default publish date must be before or on the start date '{startOn:yyyy-MM-dd}'.", nameof(defaultPublishOn));

			if (defaultPublishOn > defaultOpeningOn)
				throw new ArgumentException($"The default opening date must be after or on the publish date '{defaultPublishOn:yyyy-MM-dd}'.", nameof(defaultOpeningOn));

			FiscalYear = fiscalYear ?? throw new ArgumentNullException(nameof(fiscalYear));
			FiscalYearId = fiscalYear.Id;
			Caption = caption;
			StartDate = startOn.ToUniversalTime();
			EndDate = endOn.ToUniversalTime();
			DefaultPublishDate = defaultPublishOn.ToUniversalTime();
			DefaultOpeningDate = defaultOpeningOn.ToUniversalTime();
		}

		public bool HasOpenGrantOpenings()
		{
			var openStatus = new List<GrantOpeningStates>
			{
				GrantOpeningStates.Open,
				GrantOpeningStates.OpenForSubmit,
				GrantOpeningStates.Published,
				GrantOpeningStates.Scheduled,
				GrantOpeningStates.Unscheduled
			};

			return GrantOpenings.Any(go => openStatus.Contains(go.State));
		}

		public bool HasClosedGrantOpenings()
		{
			return GrantOpenings.Any(go => go.State == GrantOpeningStates.Closed);
		}

		/// <summary>
		/// Validates the TrainingPeriod property values.
		/// </summary>
		/// <param name="validationContext"></param>
		/// <returns></returns>
		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var entry = validationContext.GetDbEntityEntry();
			var context = validationContext.GetDbContext();

			// This is done to stop errors from being thrown when developers use EF entities in ViewModels.
			if (entry == null)
				yield break;

			// Must be associated with a FiscalYear.
			if (FiscalYear == null && FiscalYearId == 0)
				yield return new ValidationResult("The training period must be associated with a fiscal year.", new[] { nameof(FiscalYear) });
			
			// StartDate must be before EndDate.
			if (StartDate >= EndDate)
				yield return new ValidationResult($"The start date must be before the end date '{EndDate.ToLocalMidnight():yyyy-MM-dd}'.", new[] { nameof(StartDate) });

			// The DefaultPublishDate cannot be after the DefaultOpeningDate
			if (DefaultPublishDate > DefaultOpeningDate)
				yield return new ValidationResult($"The default publish date cannot be after the default opening date '{DefaultOpeningDate.ToLocalMorning():yyyy-MM-dd}'.", new[] { nameof(DefaultPublishDate) });

			// The DefaultPublishDate must be before the EndDate.
			if (DefaultPublishDate >= EndDate)
				yield return new ValidationResult($"The default publish date must be before the end date '{EndDate.ToLocalMidnight():yyyy-MM-dd}'.", new[] { nameof(DefaultPublishDate) });

			// The DefaultOpeningDate must fall within the the DefaultPublishDate and EndDate.
			if (DefaultOpeningDate < DefaultPublishDate || DefaultOpeningDate >= EndDate)
				yield return new ValidationResult($"The default opening date must be between '{DefaultPublishDate.ToLocalMorning():yyyy-MM-dd}' and '{EndDate.ToLocalMidnight():yyyy-MM-dd}'.", new[] { nameof(DefaultOpeningDate) });

			// Intake Period Start Date cannot be earlier than fiscal year start date.
			if (FiscalYear != null && StartDate < FiscalYear.StartDate)
				yield return new ValidationResult("The start date cannot be before the fiscal year start date.", new[] { nameof(StartDate) });

			// Intake Period End Date cannot be later than fiscal year end date.
			if (FiscalYear != null && EndDate > FiscalYear.EndDate)
				yield return new ValidationResult("The end date cannot be past the fiscal year end date.", new[] { nameof(EndDate) });

			var matchesExisting = GetExistingTrainingPeriod(context);
			if (matchesExisting != null)
				yield return new ValidationResult($"The dates entered match the existing intake period '{matchesExisting.Caption}'.", new[] { nameof(StartDate), nameof(EndDate) });

			if (entry.State == EntityState.Added)
			{
				// The EndDate cannot be before today.
				if (EndDate.Date < AppDateTime.UtcNow.Date)
					yield return new ValidationResult("The end date cannot be in the past.", new[] { nameof(EndDate) });

				// Intake Period Start Date cannot be earlier than the start date for the previous intake period.
				var previousIntakePeriods = context.Set<TrainingPeriod>()
					.Where(tp => tp.FiscalYear.Id == FiscalYear.Id)
					.Where(tp => tp.GrantStream.GrantProgram.Id == GrantStream.GrantProgram.Id)
					.Where(tp => tp.GrantStream.Id == GrantStream.Id)
					.OrderBy(fy => fy.Caption)
					.ToList();

				if (previousIntakePeriods.Any())
				{
					var lastIntakePeriod = previousIntakePeriods.LastOrDefault();
					if (lastIntakePeriod != null && StartDate < lastIntakePeriod.StartDate)
						yield return new ValidationResult($"The start date cannot be earlier than the start date for the previous intake period '{lastIntakePeriod.Caption}'.", new[] {nameof(StartDate)});
				}

				// Intake periods must be added sequentially for fiscal years.
				//   For example, if the highest fiscal year to have intake periods defined, for the selected program, is 2021,
				//   do not allow the user to create intake periods for any years greater than 2022 (which is the next sequential fiscal year).
				var fiscalIntakeYear = context.Set<TrainingPeriod>()
					.Where(tp => tp.GrantStream.GrantProgram.Id == GrantStream.GrantProgram.Id)
					.Where(tp => tp.GrantStream.Id == GrantStream.Id)
					.OrderByDescending(tp => tp.FiscalYear.StartDate)
					.Select(tp => tp.FiscalYear)
					.Distinct()
					.ToList();

				if (fiscalIntakeYear.Any())
				{
					var latestYear = fiscalIntakeYear.Last();
					var maximumAllowedStart = latestYear.StartDate.AddYears(1);
					var maximumAllowedEnd = latestYear.EndDate.AddYears(1);
					if (StartDate > maximumAllowedStart || EndDate > maximumAllowedEnd)
						yield return new ValidationResult($"Intake periods must be added sequentially for fiscal years. Enter periods for {maximumAllowedStart.Year}/{maximumAllowedEnd.Year} fiscal year first.", new[] { nameof(StartDate), nameof(EndDate) });
				}
			}
			else if (entry.State == EntityState.Modified)
			{
				var originalStartDate = (DateTime)entry.OriginalValues[nameof(StartDate)];
				var originalEndDate = (DateTime)entry.OriginalValues[nameof(EndDate)];

				//	o	If this intake period is not associated with a grant opening, the Start and End dates can be altered as required by the user.

				//	o	If this intake period is associated with a closed grant opening: 
				//			Start and End dates cannot be changed.

				var datesNotUpdated = originalStartDate == StartDate && originalEndDate == EndDate;
				var hasClosedGrantOpening = HasClosedGrantOpenings();
				if (hasClosedGrantOpening && !datesNotUpdated)
					yield return new ValidationResult("This Intake Period is associated with a closed grant opening. The start and end dates cannot be modified.");

				// If we don't have Closed Grant openings, we may be able to edit the start or end dates
				if (!hasClosedGrantOpening)
				{
					// Intake Period Start Date cannot be earlier than the start date for the previous intake period.
					var intakePeriodSiblings = context.Set<TrainingPeriod>()
						.Where(tp => tp.FiscalYear.Id == FiscalYear.Id)
						.Where(tp => tp.GrantStream.GrantProgram.Id == GrantStream.GrantProgram.Id)
						.Where(tp => tp.GrantStream.Id == GrantStream.Id)
						.OrderBy(fy => fy.Caption)
						.ToList();

					if (intakePeriodSiblings.Any())
					{
						var intakePeriodSequence = intakePeriodSiblings.IndexOf(this);

						var firstPeriod = intakePeriodSiblings.First();
						var isFirstPeriod = Id == firstPeriod.Id;
						if (!isFirstPeriod)
						{
							var previousPeriod = intakePeriodSiblings.ElementAtOrDefault(intakePeriodSequence - 1);
							if (previousPeriod != null && StartDate < previousPeriod.StartDate)
								yield return new ValidationResult($"The start date cannot be earlier than the start date for the previous intake period '{previousPeriod.Caption}'.", new[] { nameof(StartDate) });
						}

						var lastPeriod = intakePeriodSiblings.Last();
						var isLastPeriod = Id == lastPeriod.Id;
						if (!isLastPeriod)
						{
							var nextPeriod = intakePeriodSiblings.ElementAtOrDefault(intakePeriodSequence + 1);
							if (nextPeriod != null && StartDate > nextPeriod.StartDate)
								yield return new ValidationResult($"The start date cannot be later than the start date for the next intake period '{nextPeriod.Caption}'.", new[] { nameof(StartDate) });
						}
					}

					/* 
					o	If this intake period is associated with an open grant opening: 
							The intake period can be extended (End Date can be changed to a later date).
							•	The associated Grant Opening Close date must also be updated to match the new Intake Period End date.
							•	Provide a warning message (does not prevent dates from changing):
								“Please note. Changing the Intake Period End Date will also change the associated Grant Opening Close Date.”
										Start Date cannot change.
										End Date cannot be changed to an earlier date.
										End Date must be later than Start Date.
					 */
					var openGrantOpening = HasOpenGrantOpenings();
					if (openGrantOpening)
					{
						if (StartDate != originalStartDate)
							yield return new ValidationResult(
								"This Intake Period is associated with an open grant opening. The start date cannot be modified.",
								new[] {nameof(StartDate)});

						if (EndDate != originalEndDate && EndDate < originalEndDate)
							yield return new ValidationResult(
								$"This Intake Period is associated with an open grant opening. The end date cannot be set to before '{originalEndDate.ToLocalMidnight():yyyy-MM-dd}'.",
								new[] {nameof(EndDate)});
					}
				}

				if (EndDate != originalEndDate)
				{
					// Intake Period End Date cannot be later than fiscal year end date.
					if (FiscalYear != null && EndDate > FiscalYear.EndDate)
						yield return new ValidationResult("The end date cannot be past the fiscal year end date.", new[] { nameof(EndDate) });
				}
			}

			foreach (var validation in base.Validate(validationContext))
			{
				yield return validation;
			}
		}

		private TrainingPeriod GetExistingTrainingPeriod(IDataContext context)
		{
			if (FiscalYear == null || GrantStream == null)
				return null;

			var existingPeriods = context.Set<TrainingPeriod>()
				.Where(tp => tp.FiscalYearId == FiscalYear.Id)
				.Where(tp => tp.GrantStream.Id == GrantStream.Id)
				.Where(tp => tp.StartDate == StartDate)
				.Where(tp => tp.EndDate == EndDate)
				.Where(tp => tp.IsActive == IsActive);

			if (Id > 0)
				existingPeriods = existingPeriods.Where(tp => tp.Id != Id);

			return existingPeriods.FirstOrDefault();
		}
	}
}
