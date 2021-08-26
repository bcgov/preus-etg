using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace CJG.Web.External.Areas.Int.Models.GrantOpenings
{
	public class GrantOpeningViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }
		public int TrainingPeriodId { get; set; }
		public int GrantStreamId { get; set; }
		public decimal DeniedAmt { get; set; }

		public decimal WithdrawnAmt { get; set; }

		public decimal ReductionAmt { get; set; }

		public decimal SlippageAmt { get; set; }

		public decimal CancellationAmt { get; set; }

		public double IntakeTargetRate { get; set; }

		public bool IsScheduleEnabled { get; set; }

		public bool IsFinancialEnabled { get; set; }

		public bool IsPublishDateEnabled { get; set; }

		public bool IsOpeningDateEnabled { get; set; }

		public bool IsClosingDateEnabled { get; set; }

		public bool IsReturnRefundEnabled { get; set; }

		public int NumberUnfundedApplications { get; set; }

		public bool AllowDeleteGrantOpening { get; set; }
		public bool IsUserGM1 { get; set; }
		public bool IsTraingEndDateInThePass { get; set; }
		public string GrantStreamName { get; set; }
		public string TrainingPeriodCaption { get; set; }
		public DateTime TrainingPeriodStartDate { get; set; }
		public DateTime TrainingPeriodEndDate { get; set; }
		public GrantOpeningStates State { get; set; }
		public DateTime ScheduleStartDate { get; set; }
		public DateTime ScheduleEndDate { get; set; }
		public DateTime PublishDate { get; set; }
		public DateTime OpeningDate { get; set; }
		public DateTime ClosingDate { get; set; }
		public Decimal OriginalBudgetAllocationAmt { get; set; }
		public Decimal BudgetAllocationAmt { get; set; }
		public double OriginalPlanDeniedRate { get; set; }
		public double PlanDeniedRate { get; set; }
		public double OriginalPlanWithdrawnRate { get; set; }
		public double PlanWithdrawnRate { get; set; }
		public double OriginalPlanReductionRate { get; set; }
		public double PlanReductionRate { get; set; }
		public double OriginalPlanSlippageRate { get; set; }
		public double PlanSlippageRate { get; set; }
		public double OriginalPlanCancellationRate { get; set; }
		public double PlanCancellationRate { get; set; }
		public decimal IntakeTargetAmt { get; set; }
		#endregion

		#region Constructors
		public GrantOpeningViewModel() { }

		public GrantOpeningViewModel(GrantOpening grantOpening, IGrantOpeningService grantOpeningService, IGrantApplicationService grantApplicationService, IPrincipal user)
		{
			if (grantOpening == null) throw new ArgumentNullException(nameof(grantOpening));
			if (grantOpeningService == null) throw new ArgumentNullException(nameof(grantOpeningService));
			if (grantApplicationService == null) throw new ArgumentNullException(nameof(grantApplicationService));
			if (user == null) throw new ArgumentNullException(nameof(user));

			this.Id = grantOpening.Id;
			this.RowVersion = grantOpening.RowVersion != null ? Convert.ToBase64String(grantOpening.RowVersion) : null;
			this.TrainingPeriodId = grantOpening.TrainingPeriodId;
			this.GrantStreamId = grantOpening.GrantStreamId;
			this.AllowDeleteGrantOpening = grantOpening.Id != 0 && grantOpeningService.CanDeleteGrantOpening(grantOpening);

			this.GrantStreamName = grantOpening.GrantStream.Name;
			this.TrainingPeriodCaption = grantOpening.TrainingPeriod.Caption;
			this.TrainingPeriodStartDate = grantOpening.TrainingPeriod.StartDate;
			this.TrainingPeriodEndDate = grantOpening.TrainingPeriod.EndDate;
			this.State = grantOpening.State;
			this.PublishDate = grantOpening.PublishDate;
			this.OpeningDate = grantOpening.OpeningDate;
			this.ClosingDate = grantOpening.ClosingDate;

			this.BudgetAllocationAmt = grantOpening.BudgetAllocationAmt;

			this.PlanDeniedRate = grantOpening.PlanDeniedRate;
			this.PlanWithdrawnRate = grantOpening.PlanWithdrawnRate;
			this.PlanReductionRate = grantOpening.PlanReductionRate;
			this.PlanSlippageRate = grantOpening.PlanSlippageRate;
			this.PlanCancellationRate = grantOpening.PlanCancellationRate;

			this.DeniedAmt = (decimal)this.PlanDeniedRate * this.BudgetAllocationAmt;
			this.WithdrawnAmt = (decimal)this.PlanWithdrawnRate * this.BudgetAllocationAmt;
			this.ReductionAmt = (decimal)this.PlanReductionRate * this.BudgetAllocationAmt;
			this.SlippageAmt = (decimal)this.PlanSlippageRate * this.BudgetAllocationAmt;
			this.CancellationAmt = (decimal)this.PlanCancellationRate * this.BudgetAllocationAmt;

			this.IntakeTargetAmt =
				this.BudgetAllocationAmt +
				this.DeniedAmt +
				this.WithdrawnAmt +
				this.ReductionAmt +
				this.SlippageAmt +
				this.CancellationAmt;

			this.IntakeTargetRate =
				this.PlanDeniedRate +
				this.PlanWithdrawnRate +
				this.PlanReductionRate +
				this.PlanSlippageRate +
				this.PlanCancellationRate;

			this.IsUserGM1 = user.HasPrivilege(Privilege.GM1);
			this.IsTraingEndDateInThePass = grantOpening.TrainingPeriod.EndDate >= AppDateTime.UtcNow.ToLocalMidnight();

			this.IsScheduleEnabled = false;
			this.IsFinancialEnabled = false;
			this.IsPublishDateEnabled = false;
			this.IsOpeningDateEnabled = false;
			this.IsClosingDateEnabled = false;
			this.NumberUnfundedApplications = grantOpening.Id == 0 ? 0 : grantApplicationService.GetTotalGrantApplications(new List<ApplicationStateInternal> { ApplicationStateInternal.New }, 0, grantOpening.Id, 0, 0, 0, 0, null, null);
			this.IsReturnRefundEnabled = this.NumberUnfundedApplications > 0 && user.HasPrivilege(Privilege.GM1);

			switch (grantOpening.State)
			{
				case GrantOpeningStates.Unscheduled:
					this.IsScheduleEnabled = true;
					this.IsFinancialEnabled = true;
					this.IsPublishDateEnabled = true;
					this.IsOpeningDateEnabled = true;
					this.IsClosingDateEnabled = true;
					break;

				case GrantOpeningStates.Scheduled:
					this.IsScheduleEnabled = true;
					this.IsFinancialEnabled = true;
					this.IsPublishDateEnabled = true;
					this.IsOpeningDateEnabled = true;
					this.IsClosingDateEnabled = true;
					break;

				case GrantOpeningStates.Published:
					this.IsScheduleEnabled = true;
					this.IsFinancialEnabled = true;
					this.IsOpeningDateEnabled = true;
					this.IsClosingDateEnabled = true;
					break;

				case GrantOpeningStates.Open:
					this.IsScheduleEnabled = true;
					this.IsFinancialEnabled = true;
					this.IsClosingDateEnabled = true;
					break;

				case GrantOpeningStates.Closed:
					this.IsFinancialEnabled = true;
					break;

				case GrantOpeningStates.OpenForSubmit:
					this.IsFinancialEnabled = true;
					break;
				default:
					break;
			}
		}
		#endregion
	}
}
