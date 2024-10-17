using CJG.Core.Entities;
using DataAnnotationsExtensions;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class GrantOpeningViewModel
	{
		#region Properties
		public int Id { get; set; }

		public string RowVersion { get; set; }

		[DefaultValue(GrantOpeningStates.Unscheduled)]
		public GrantOpeningStates State { get; set; } = GrantOpeningStates.Unscheduled;

		[Required, Min(0, ErrorMessage = "The intake target must be greater than or equal to 0.")]
		public decimal IntakeTargetAmt { get; set; }

		[Required, Min(0, ErrorMessage = "The budget allocation amount must be greater than or equal to 0.")]
		public decimal BudgetAllocationAmt { get; set; }

		[Min(0, ErrorMessage = "The plan denied rate cannot be less than 0."), Max(1, ErrorMessage = "The plan denied rate cannot be greater than 1.")]
		public double PlanDeniedRate { get; set; }

		[Min(0, ErrorMessage = "The plan withdrawn rate cannot be less than 0."), Max(1, ErrorMessage = "The plan withdrawn rate cannot be greater than 1.")]
		public double PlanWithdrawnRate { get; set; }

		[Min(0, ErrorMessage = "The plan reduction rate cannot be less than 0."), Max(1, ErrorMessage = "The plan reduction rate cannot be greater than 1.")]
		public double PlanReductionRate { get; set; }

		[Min(0, ErrorMessage = "The plan slippage rate cannot be less than 0."), Max(1, ErrorMessage = "The plan slippage rate cannot be greater than 1.")]
		public double PlanSlippageRate { get; set; }

		[Min(0, ErrorMessage = "The plan cancellation rate cannot be less than 0."), Max(1, ErrorMessage = "The plan cancellation rate cannot be greater than 1.")]
		public double PlanCancellationRate { get; set; }

		[Required]
		public DateTime PublishDate { get; set; }

		[Required]
		public DateTime OpeningDate { get; set; }

		[Required]
		public DateTime ClosingDate { get; set; }

		public int TrainingPeriodId { get; set; }

		[NotMapped]
		public virtual TrainingPeriodViewModel TrainingPeriod { get; set; }

		public int GrantStreamId { get; set; }

		[NotMapped]
		public virtual GrantStream GrantStream { get; set; }
		public ApplicationGrantStreamViewModel ApplicationGrantStreamViewModel { get; set; }

		[NotMapped]
		public virtual GrantOpeningIntake GrantOpeningIntake { get; set; }

		[NotMapped]
		public virtual GrantOpeningFinancial GrantOpeningFinancial { get; set; }
		#endregion

		#region Constructors
		public GrantOpeningViewModel()
		{

		}

		public GrantOpeningViewModel(GrantOpening grantOpening)
		{
			if (grantOpening == null)
				throw new ArgumentNullException(nameof(grantOpening));

			this.Id = grantOpening.Id;
			this.RowVersion = Convert.ToBase64String(grantOpening.RowVersion);
			this.State = grantOpening.State;
			this.IntakeTargetAmt = grantOpening.IntakeTargetAmt;
			this.BudgetAllocationAmt = grantOpening.BudgetAllocationAmt;
			this.PlanDeniedRate = grantOpening.PlanDeniedRate;
			this.PlanWithdrawnRate = grantOpening.PlanWithdrawnRate;
			this.PlanReductionRate = grantOpening.PlanReductionRate;
			this.PlanSlippageRate = grantOpening.PlanSlippageRate;
			this.PlanCancellationRate = grantOpening.PlanCancellationRate;
			this.PublishDate = grantOpening.PublishDate;
			this.OpeningDate = grantOpening.OpeningDate;
			this.ClosingDate = grantOpening.ClosingDate;
			this.TrainingPeriodId = grantOpening.TrainingPeriodId;
			this.TrainingPeriod = grantOpening.TrainingPeriod != null ? new TrainingPeriodViewModel(grantOpening.TrainingPeriod) : null;
			this.GrantStreamId = grantOpening.GrantStreamId;
			this.GrantStream = grantOpening.GrantStream;
			this.GrantOpeningIntake = grantOpening.GrantOpeningIntake;
			this.GrantOpeningFinancial = grantOpening.GrantOpeningFinancial;
			this.ApplicationGrantStreamViewModel = new ApplicationGrantStreamViewModel(grantOpening.GrantStream);
		}
		#endregion

		#region Methods
		public static implicit operator GrantOpening(GrantOpeningViewModel model)
		{
			if (model == null)
				return null;

			var grantOpening = new GrantOpening
			{
				Id = model.Id,
				RowVersion = Convert.FromBase64String(model.RowVersion),
				State = model.State,
				IntakeTargetAmt = model.IntakeTargetAmt,
				BudgetAllocationAmt = model.BudgetAllocationAmt,
				PlanDeniedRate = model.PlanDeniedRate,
				PlanWithdrawnRate = model.PlanWithdrawnRate,
				PlanReductionRate = model.PlanReductionRate,
				PlanSlippageRate = model.PlanSlippageRate,
				PlanCancellationRate = model.PlanCancellationRate,
				PublishDate = model.PublishDate,
				OpeningDate = model.OpeningDate,
				ClosingDate = model.ClosingDate,
				TrainingPeriodId = model.TrainingPeriodId,
				TrainingPeriod = model.TrainingPeriod,
				GrantStreamId = model.GrantStreamId,
				GrantStream = model.GrantStream,
				GrantOpeningIntake = model.GrantOpeningIntake,
				GrantOpeningFinancial = model.GrantOpeningFinancial
			};
			return grantOpening;
		}
		#endregion
	}
}