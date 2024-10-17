using System;
using System.Collections.Generic;
using System.Security.Principal;
using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Identity;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models.GrantOpenings
{
	public class GrantOpeningViewModel : BaseViewModel
	{
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
		public bool IsTrainingEndDateInThePast { get; set; }
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
		public decimal OriginalBudgetAllocationAmt { get; set; }
		public decimal BudgetAllocationAmt { get; set; }
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

		public GrantOpeningViewModel() { }

		public GrantOpeningViewModel(GrantOpening grantOpening, IGrantOpeningService grantOpeningService, IGrantApplicationService grantApplicationService, IPrincipal user)
		{
			if (grantOpening == null)
				throw new ArgumentNullException(nameof(grantOpening));

			if (grantOpeningService == null)
				throw new ArgumentNullException(nameof(grantOpeningService));

			if (grantApplicationService == null)
				throw new ArgumentNullException(nameof(grantApplicationService));

			if (user == null)
				throw new ArgumentNullException(nameof(user));

			Id = grantOpening.Id;
			RowVersion = grantOpening.RowVersion != null ? Convert.ToBase64String(grantOpening.RowVersion) : null;
			TrainingPeriodId = grantOpening.TrainingPeriodId;
			GrantStreamId = grantOpening.GrantStreamId;
			AllowDeleteGrantOpening = grantOpening.Id != 0 && grantOpeningService.CanDeleteGrantOpening(grantOpening);

			GrantStreamName = grantOpening.GrantStream.Name;
			TrainingPeriodCaption = grantOpening.TrainingPeriod.Caption;
			TrainingPeriodStartDate = grantOpening.TrainingPeriod.StartDate;
			TrainingPeriodEndDate = grantOpening.TrainingPeriod.EndDate;
			State = grantOpening.State;
			PublishDate = grantOpening.PublishDate;
			OpeningDate = grantOpening.OpeningDate;
			ClosingDate = grantOpening.ClosingDate;

			BudgetAllocationAmt = grantOpening.BudgetAllocationAmt;

			PlanDeniedRate = grantOpening.PlanDeniedRate;
			PlanWithdrawnRate = grantOpening.PlanWithdrawnRate;
			PlanReductionRate = grantOpening.PlanReductionRate;
			PlanSlippageRate = grantOpening.PlanSlippageRate;
			PlanCancellationRate = grantOpening.PlanCancellationRate;

			DeniedAmt = (decimal)PlanDeniedRate * BudgetAllocationAmt;
			WithdrawnAmt = (decimal)PlanWithdrawnRate * BudgetAllocationAmt;
			ReductionAmt = (decimal)PlanReductionRate * BudgetAllocationAmt;
			SlippageAmt = (decimal)PlanSlippageRate * BudgetAllocationAmt;
			CancellationAmt = (decimal)PlanCancellationRate * BudgetAllocationAmt;

			IntakeTargetAmt =
				BudgetAllocationAmt +
				DeniedAmt +
				WithdrawnAmt +
				ReductionAmt +
				SlippageAmt +
				CancellationAmt;

			IntakeTargetRate =
				PlanDeniedRate +
				PlanWithdrawnRate +
				PlanReductionRate +
				PlanSlippageRate +
				PlanCancellationRate;

			IsUserGM1 = user.HasPrivilege(Privilege.GM1);
			IsTrainingEndDateInThePast = grantOpening.TrainingPeriod.EndDate >= AppDateTime.UtcNow.ToLocalMidnight();

			IsScheduleEnabled = false;
			IsFinancialEnabled = false;
			IsPublishDateEnabled = false;
			IsOpeningDateEnabled = false;
			IsClosingDateEnabled = false;
			NumberUnfundedApplications = grantOpening.Id == 0 ? 0 : grantApplicationService.GetTotalGrantApplications(new List<ApplicationStateInternal> { ApplicationStateInternal.New }, 0, grantOpening.Id, 0, 0, 0, 0, null, null);
			IsReturnRefundEnabled = NumberUnfundedApplications > 0 && user.HasPrivilege(Privilege.GM1);

			switch (grantOpening.State)
			{
				case GrantOpeningStates.Unscheduled:
					IsScheduleEnabled = true;
					IsFinancialEnabled = true;
					IsPublishDateEnabled = true;
					IsOpeningDateEnabled = true;
					IsClosingDateEnabled = true;
					break;

				case GrantOpeningStates.Scheduled:
					IsScheduleEnabled = true;
					IsFinancialEnabled = true;
					IsPublishDateEnabled = true;
					IsOpeningDateEnabled = true;
					IsClosingDateEnabled = true;
					break;

				case GrantOpeningStates.Published:
					IsScheduleEnabled = true;
					IsFinancialEnabled = true;
					IsOpeningDateEnabled = true;
					IsClosingDateEnabled = true;
					break;

				case GrantOpeningStates.Open:
					IsScheduleEnabled = true;
					IsFinancialEnabled = true;
					IsClosingDateEnabled = true;
					break;

				case GrantOpeningStates.Closed:
					IsFinancialEnabled = true;
					break;

				case GrantOpeningStates.OpenForSubmit:
					IsFinancialEnabled = true;
					break;
			}
		}
	}
}
