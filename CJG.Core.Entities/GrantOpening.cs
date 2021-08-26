using CJG.Core.Entities.Attributes;
using DataAnnotationsExtensions;
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
	/// <typeparamref name="GrantOpening"/> class, provides the ORM a way to manage grant openings.  A grant opening is an allotment of funds for a specific stream and training period.
	/// </summary>
	public class GrantOpening : EntityBase
	{
		#region Properties
		/// <summary>
		/// get/set - The primary key for this Grant Opening.
		/// </summary>
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		/// <summary>
		/// get/set - The current state of this Grant Opening.
		/// </summary>
		[DefaultValue(GrantOpeningStates.Unscheduled)]
		[Index("IX_GrantOpening", 1)]
		public GrantOpeningStates State { get; set; } = GrantOpeningStates.Unscheduled;

		/// <summary>
		/// get/set - The amount of money targeted to use the whole budget based on denied, withdrawn, reduction, slippage and cancellation rates.
		/// </summary>
		[Required, Min(0, ErrorMessage = "The intake target must be greater than or equal to 0.")]
		public decimal IntakeTargetAmt { get; set; }

		/// <summary>
		/// get/set - The amount of money that has been allocated to this Grant Opening.
		/// </summary>
		[Required, Min(0, ErrorMessage = "The budget allocation amount must be greater than or equal to 0.")]
		public decimal BudgetAllocationAmt { get; set; }

		/// <summary>
		/// get/set - The expected percentage of Grant Applications that will be denied.
		/// </summary>
		[Min(0, ErrorMessage = "The plan denied rate cannot be less than 0."), Max(1, ErrorMessage = "The plan denied rate cannot be greater than 1.")]
		public double PlanDeniedRate { get; set; }

		/// <summary>
		/// get/set - The expected percentage of Grant Application that will be withdrawn.
		/// </summary>
		[Min(0, ErrorMessage = "The plan withdrawn rate cannot be less than 0."), Max(1, ErrorMessage = "The plan withdrawn rate cannot be greater than 1.")]
		public double PlanWithdrawnRate { get; set; }

		/// <summary>
		/// get/set - The expected percentage of Grant Applications that will be reduced.
		/// </summary>
		[Min(0, ErrorMessage = "The plan reduction rate cannot be less than 0."), Max(1, ErrorMessage = "The plan reduction rate cannot be greater than 1.")]
		public double PlanReductionRate { get; set; }

		/// <summary>
		/// get/set - The expected percentage of Grant Applications that will have slippage.
		/// </summary>
		[Min(0, ErrorMessage = "The plan slippage rate cannot be less than 0."), Max(1, ErrorMessage = "The plan slippage rate cannot be greater than 1.")]
		public double PlanSlippageRate { get; set; }

		/// <summary>
		/// get/set - The expected percentage of Grant Applications that will be cancelled.
		/// </summary>
		[Min(0, ErrorMessage = "The plan cancellation rate cannot be less than 0."), Max(1, ErrorMessage = "The plan cancellation rate cannot be greater than 1.")]
		public double PlanCancellationRate { get; set; }

		/// <summary>
		/// get/set - When this Grant Opening will be published for the public to pick when creating Grant Applications.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Required]
		[Index("IX_GrantOpening", 2)]
		[Column(TypeName = "DATETIME2")]
		public DateTime PublishDate { get; set; }

		/// <summary>
		/// get/set - When this Grant Opening will be available for submitting Grant Applications.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Required]
		[Index("IX_GrantOpening", 3)]
		[Column(TypeName = "DATETIME2")]
		public DateTime OpeningDate { get; set; }

		/// <summary>
		/// get/set - When this Grant Opening will no longer be available for selecting or submitting Grant Applications.
		/// </summary>
		[DateTimeKind(DateTimeKind.Utc)]
		[Required]
		[Index("IX_GrantOpening", 4)]
		[Column(TypeName = "DATETIME2")]
		public DateTime ClosingDate { get; set; }

		/// <summary>
		/// get/set - The foreign key to the Training Period this Grant Opening will be available in.
		/// </summary>
		[Index("IX_GrantOpening", 5)]
		public int TrainingPeriodId { get; set; }

		/// <summary>
		/// get/set - The Training Period this Grant Opening will be available in.
		/// </summary>
		[ForeignKey(nameof(TrainingPeriodId))]
		public virtual TrainingPeriod TrainingPeriod { get; set; }

		/// <summary>
		/// get/set - The foreign key to the Grant Stream this Grant Opening is associated with.
		/// </summary>
		[Index("IX_GrantOpening", 6)]
		public int GrantStreamId { get; set; }

		/// <summary>
		/// get/set - The Grant Stream this Grant Opening is associated with.
		/// </summary>
		[ForeignKey(nameof(GrantStreamId))]
		public virtual GrantStream GrantStream { get; set; }

		/// <summary>
		/// get/set - The intake information for this Grant Opening.
		/// </summary>
		public virtual GrantOpeningIntake GrantOpeningIntake { get; set; }

		/// <summary>
		/// get/set - The financial information for this Grant Opening.
		/// </summary>
		public virtual GrantOpeningFinancial GrantOpeningFinancial { get; set; }

		/// <summary>
		/// get/set - A collection of all the Grant Applications associated to this Grant Opening.
		/// </summary>
		public ICollection<GrantApplication> GrantApplications { get; set; } = new List<GrantApplication>();
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of a <typeparamref name="GrantOpening" /> object.
		/// </summary>
		public GrantOpening()
		{
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="GrantOpening"/> object and initializes it with the specified property values.
		/// Uses the training period default publish, opening and closing dates.
		/// </summary>
		/// <param name="grantStream"></param>
		/// <param name="trainingPeriod"></param>
		/// <param name="budgetAllocation"></param>
		/// <param name="intakeTarget"></param>
		public GrantOpening(GrantStream grantStream, TrainingPeriod trainingPeriod, decimal budgetAllocation)
		{
			if (budgetAllocation < 0)
				throw new ArgumentException("The budget allocation must be greater than or equal to 0.", nameof(budgetAllocation));

			this.GrantStream = grantStream ?? throw new ArgumentNullException(nameof(grantStream));
			this.GrantStreamId = grantStream.Id;
			this.TrainingPeriod = trainingPeriod ?? throw new ArgumentNullException(nameof(trainingPeriod));
			this.TrainingPeriodId = trainingPeriod.Id;
			this.PublishDate = trainingPeriod.DefaultPublishDate;
			this.OpeningDate = trainingPeriod.DefaultOpeningDate;
			this.ClosingDate = trainingPeriod.EndDate;
			this.BudgetAllocationAmt = budgetAllocation;
			this.State = GrantOpeningStates.Unscheduled;

			this.PlanDeniedRate = grantStream.DefaultDeniedRate;
			this.PlanWithdrawnRate = grantStream.DefaultWithdrawnRate;
			this.PlanReductionRate = grantStream.DefaultReductionRate;
			this.PlanSlippageRate = grantStream.DefaultSlippageRate;
			this.PlanCancellationRate = grantStream.DefaultCancellationRate;
			this.IntakeTargetAmt = this.CalculateIntakeTarget();
			this.GrantOpeningIntake = new GrantOpeningIntake(this);
			this.GrantOpeningFinancial = new GrantOpeningFinancial(this);
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="GrantOpening"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="grantStream"></param>
		/// <param name="trainingPeriod"></param>
		/// <param name="publishOn"></param>
		/// <param name="openOn"></param>
		/// <param name="closeOn"></param>
		/// <param name="budgetAllocation"></param>
		/// <param name="intakeTarget"></param>
		public GrantOpening(GrantStream grantStream, TrainingPeriod trainingPeriod, DateTime publishOn, DateTime openOn, DateTime closeOn, decimal budgetAllocation)
		{
			if (trainingPeriod == null)
				throw new ArgumentNullException(nameof(trainingPeriod));

			if (trainingPeriod.StartDate > openOn)
				throw new ArgumentException($"The opening date must not be before the training period start date '{trainingPeriod.StartDate.ToLocalMorning():yyyy-MM-dd}'.", nameof(openOn));

			if (publishOn > openOn)
				throw new ArgumentException("The publish date must be before or on the opening date.", nameof(publishOn));

			if (openOn >= closeOn)
				throw new ArgumentException("The opening date must be before or on the closing date.", nameof(openOn));

			if (closeOn.Date < AppDateTime.UtcMorning)
				throw new ArgumentException("The closing date cannot be in the past.", nameof(closeOn));

			if (trainingPeriod.EndDate < closeOn)
				throw new ArgumentException($"The closing date cannot be after the training period end date '{trainingPeriod.EndDate.ToLocalMidnight():yyyy-MM-dd}'.", nameof(closeOn));

			if (budgetAllocation < 0)
				throw new ArgumentException("The budget allocation must be greater than or equal to 0.", nameof(budgetAllocation));

			this.GrantStream = grantStream ?? throw new ArgumentNullException(nameof(grantStream));
			this.GrantStreamId = grantStream.Id;
			this.TrainingPeriod = trainingPeriod;
			this.TrainingPeriodId = trainingPeriod.Id;
			this.PublishDate = publishOn.ToUniversalTime();
			this.OpeningDate = openOn.ToUniversalTime();
			this.ClosingDate = closeOn.ToUniversalTime();
			this.BudgetAllocationAmt = budgetAllocation;
			this.State = GrantOpeningStates.Unscheduled;

			this.PlanDeniedRate = grantStream.DefaultDeniedRate;
			this.PlanWithdrawnRate = grantStream.DefaultWithdrawnRate;
			this.PlanReductionRate = grantStream.DefaultReductionRate;
			this.PlanSlippageRate = grantStream.DefaultSlippageRate;
			this.PlanCancellationRate = grantStream.DefaultCancellationRate;
			this.IntakeTargetAmt = this.CalculateIntakeTarget();
		}

		/// <summary>
		/// Creates a new instance of a <typeparamref name="GrantOpening"/> object and initializes it with the specified property values.
		/// </summary>
		/// <param name="grantStream"></param>
		/// <param name="trainingPeriod"></param>
		/// <param name="publishOn"></param>
		/// <param name="openOn"></param>
		/// <param name="closeOn"></param>
		/// <param name="budgetAllocation"></param>
		/// <param name="intakeTarget"></param>
		/// <param name="deniedRate"></param>
		/// <param name="withdrawnRate"></param>
		/// <param name="reductionRate"></param>
		/// <param name="slippageRate"></param>
		/// <param name="cancellationRate"></param>
		public GrantOpening(GrantStream grantStream, TrainingPeriod trainingPeriod, DateTime publishOn, DateTime openOn, DateTime closeOn, decimal budgetAllocation,
			double deniedRate, double withdrawnRate, double reductionRate, double slippageRate, double cancellationRate) : this(grantStream, trainingPeriod, publishOn, openOn, closeOn, budgetAllocation)
		{
			if (deniedRate < 0 || deniedRate > 1)
				throw new ArgumentException("The denied rate cannot be less than 0 or greater than 1.", nameof(deniedRate));

			if (withdrawnRate < 0 || withdrawnRate > 1)
				throw new ArgumentException("The withdrawn rate cannot be less than 0 or greater than 1.", nameof(withdrawnRate));

			if (reductionRate < 0 || reductionRate > 1)
				throw new ArgumentException("The reduction rate cannot be less than 0 or greater than 1.", nameof(reductionRate));

			if (slippageRate < 0 || slippageRate > 1)
				throw new ArgumentException("The slippage rate cannot be less than 0 or greater than 1.", nameof(slippageRate));

			if (cancellationRate < 0 || cancellationRate > 1)
				throw new ArgumentException("The cancellation rate cannot be less than 0 or greater than 1.", nameof(cancellationRate));

			this.PlanDeniedRate = deniedRate;
			this.PlanWithdrawnRate = withdrawnRate;
			this.PlanReductionRate = reductionRate;
			this.PlanSlippageRate = slippageRate;
			this.PlanCancellationRate = cancellationRate;
			this.IntakeTargetAmt = this.CalculateIntakeTarget();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Validate this <typeparamref name="GrantOpening"/> object.
		/// </summary>
		/// <param name="validationContext"></param>
		/// <returns></returns>
		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var context = validationContext.GetDbContext();
			var entry = validationContext.GetDbEntityEntry();

			// This is done to stop errors from being thrown when developers use EF entities in ViewModels.
			if (entry == null || context == null)
				yield break;

			context.Set<GrantOpening>().Include(go => go.TrainingPeriod).FirstOrDefault(go => go.Id == this.Id);

			// The earliest publish date is today.
			var earliest_publish_date = AppDateTime.UtcMorning;

			// TrainingPeriodId is required.
			if (this.TrainingPeriodId == 0)
				yield return new ValidationResult("The grant opening must be associated with a valid training period.", new[] { nameof(this.TrainingPeriodId) });

			if (entry.State == EntityState.Added)
			{
				// A new GrantOpening must begin as Unscheduled.
				if (!((this.PublishDate >= AppDateTime.UtcMorning &&
					   this.OpeningDate >= AppDateTime.UtcMorning &&
					   this.ClosingDate > AppDateTime.UtcNow) ||
					  (this.PublishDate <= AppDateTime.UtcMorning &&
					   this.OpeningDate <= AppDateTime.UtcMorning &&
					   this.ClosingDate < AppDateTime.UtcNow)))
				{
					yield return new ValidationResult("Publish date, opening date and closing date must either all in the pass or all in the future.", new[] { nameof(this.State) });
				}

				if (this.State != GrantOpeningStates.Unscheduled)
					yield return new ValidationResult("A new Grant Opening must begin as unscheduled.", new[] { nameof(this.State) });
			}
			else if (entry.State == EntityState.Modified)
			{
				context.Entry(entry.Entity).GetDatabaseValues();

				var original_budget = (decimal)entry.OriginalValues[nameof(BudgetAllocationAmt)];
				var original_state = (GrantOpeningStates)entry.OriginalValues[nameof(this.State)];
				var original_publish_date = DateTime.SpecifyKind((DateTime)entry.OriginalValues[nameof(this.PublishDate)], DateTimeKind.Utc);
				var original_opening_date = DateTime.SpecifyKind((DateTime)entry.OriginalValues[nameof(this.OpeningDate)], DateTimeKind.Utc);
				var original_closing_date = DateTime.SpecifyKind((DateTime)entry.OriginalValues[nameof(this.ClosingDate)], DateTimeKind.Utc);
				var original_grant_stream_id = (int)entry.OriginalValues[nameof(this.GrantStreamId)];
				var original_training_period_id = (int)entry.OriginalValues[nameof(this.TrainingPeriodId)];
				earliest_publish_date = original_publish_date.Date < AppDateTime.UtcMorning.Date ? original_publish_date : AppDateTime.UtcMorning;

				if (this.GrantStreamId != original_grant_stream_id || this.TrainingPeriodId != original_training_period_id)
					yield return new ValidationResult("The Grant Stream and Training Period cannot be changed on a Grant Opening.", new[] { nameof(this.State) });

				switch (this.State)
				{
					// to UNSCHEDULED
					case GrantOpeningStates.Unscheduled:
						if (original_state != GrantOpeningStates.Unscheduled
						 && original_state != GrantOpeningStates.Scheduled)
							yield return new ValidationResult("The state must go from scheduled or unscheduled, to unscheduled.", new[] { nameof(this.State) });
						break;

					// to SCHEDULED
					case GrantOpeningStates.Scheduled:
						if (original_state != GrantOpeningStates.Scheduled
						 && original_state != GrantOpeningStates.Unscheduled)
							yield return new ValidationResult("The state must go from scheduled or unscheduled, to scheduled.", new[] { nameof(this.State) });
						break;

					// to PUBLISHED
					case GrantOpeningStates.Published:
						if (original_state != GrantOpeningStates.Published
						 && original_state != GrantOpeningStates.Scheduled
						 && original_state != GrantOpeningStates.Unscheduled)
							yield return new ValidationResult("The state must go from published, scheduled, or unscheduled, to published.", new[] { nameof(this.State) });

						if (original_state.In(GrantOpeningStates.Published, GrantOpeningStates.Open, GrantOpeningStates.OpenForSubmit, GrantOpeningStates.Closed)
							&& original_publish_date != this.PublishDate)
							yield return new ValidationResult("Publish date cannot be changed after the Grant Opening has been published.", new[] { nameof(this.State) });

						if (!(this.PublishDate <= AppDateTime.UtcNow && this.OpeningDate > AppDateTime.UtcNow))
							yield return new ValidationResult("Publised date must be today or in the past, opening date must be in the future while in open state.", new[] { nameof(this.ClosingDate) });
						break;
					// to OPEN
					case GrantOpeningStates.Open:
						if (original_state != GrantOpeningStates.Open
						 && original_state != GrantOpeningStates.Scheduled
						 && original_state != GrantOpeningStates.Unscheduled
						 && original_state != GrantOpeningStates.Published
						 && original_state != GrantOpeningStates.Closed)
							yield return new ValidationResult("The state must go from open, closed, scheduled, unscheduled, or published, to open.", new[] { nameof(this.State) });

						if (original_state.In(GrantOpeningStates.Published, GrantOpeningStates.Open, GrantOpeningStates.OpenForSubmit, GrantOpeningStates.Closed)
							&& original_publish_date != this.PublishDate)
							yield return new ValidationResult("Publish date cannot be changed after the Grant Opening has been published.", new[] { nameof(this.State) });

						if (original_state.In(GrantOpeningStates.Open, GrantOpeningStates.OpenForSubmit, GrantOpeningStates.Closed)
							&& original_opening_date != this.OpeningDate)
							yield return new ValidationResult("Opening date cannot be changed after the Grant Opening has been opened.", new[] { nameof(this.State) });

						// The TrainingPeriod has expired
						if (original_state == GrantOpeningStates.Closed && AppDateTime.UtcNow >= this.TrainingPeriod.EndDate)
							yield return new ValidationResult("This grant opening cannot be reopened as the training period end date has past.", new[] { nameof(this.ClosingDate) });

						if (!(this.OpeningDate <= AppDateTime.UtcNow && this.ClosingDate > AppDateTime.UtcNow))
							yield return new ValidationResult("Opening date must be today or in the past, closing date must be in the future while in open state.", new[] { nameof(this.ClosingDate) });
						break;

					// to CLOSED
					case GrantOpeningStates.Closed:
						if (original_state != GrantOpeningStates.Closed
							&& original_state != GrantOpeningStates.Open
							&& original_state != GrantOpeningStates.OpenForSubmit
							&& original_state != GrantOpeningStates.Unscheduled
							&& original_state != GrantOpeningStates.Scheduled
							&& original_state != GrantOpeningStates.Published)
							yield return new ValidationResult("The state must go from open, open for submit, unscheduled, scheduled, or published, to closed.", new[] { nameof(this.State) });

						if (original_state.In(GrantOpeningStates.Published, GrantOpeningStates.Open, GrantOpeningStates.OpenForSubmit, GrantOpeningStates.Closed)
							&& original_publish_date != this.PublishDate)
							yield return new ValidationResult("Publish date cannot be changed after the Grant Opening has been published.", new[] { nameof(this.State) });

						if (original_state.In(GrantOpeningStates.Open, GrantOpeningStates.OpenForSubmit, GrantOpeningStates.Closed)
							&& original_opening_date != this.OpeningDate)
							yield return new ValidationResult("Opening date cannot be changed after the Grant Opening has been opened.", new[] { nameof(this.State) });

						if (original_state.In(GrantOpeningStates.OpenForSubmit, GrantOpeningStates.Closed)
							&& original_closing_date != this.ClosingDate)
							yield return new ValidationResult("Closing date cannot be changed after the Grant Opening has been closed.", new[] { nameof(this.State) });
						break;

					// to OPENFORSUBMIT
					case GrantOpeningStates.OpenForSubmit:
						if (original_state != GrantOpeningStates.OpenForSubmit
							&& original_state != GrantOpeningStates.Closed)
							yield return new ValidationResult("The state must go from closed, to open for submit.", new[] { nameof(this.State) });

						if (this.PublishDate != original_publish_date
						 || this.OpeningDate != original_opening_date
						 || this.ClosingDate != original_closing_date)
							yield return new ValidationResult("Publish date, opening date and closing date cannot be changed while in open for submit state.", new[] { nameof(this.State) });
						break;
				}
			}

			if (this.PublishDate < earliest_publish_date || this.PublishDate > this.TrainingPeriod?.EndDate)
				yield return new ValidationResult($"Publish date must be today or later, and the same or before the training period end date '{this.TrainingPeriod?.EndDate.ToLocalMidnight():yyyy-MM-dd}'.", new[] { nameof(this.PublishDate) });

			if (this.OpeningDate < this.PublishDate || this.OpeningDate.ToLocalTime().Date > this.ClosingDate.ToLocalTime().Date)
				yield return new ValidationResult($"Opening date must be the same or later than the publish date, and before the closing date.", new[] { nameof(this.OpeningDate) });

			if (this.ClosingDate < this.OpeningDate || this.ClosingDate.Date > this.TrainingPeriod?.EndDate || this.ClosingDate.Date < this.TrainingPeriod?.StartDate)
				yield return new ValidationResult($"Closing date must be the same or later than the opening date, and during the training period of '{this.TrainingPeriod?.StartDate.ToLocalMidnight():yyyy-MM-dd} to {this.TrainingPeriod?.EndDate.ToLocalMidnight():yyyy-MM-dd}'.", new[] { nameof(this.ClosingDate) });

			foreach (var validation in base.Validate(validationContext))
			{
				yield return validation;
			}
		}
		#endregion
	}
}
