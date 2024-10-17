using CJG.Application.Services;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class ApplicationGrantOpeningViewModel
	{
		public int Id { get; set; }
		public GrantOpeningStates State { get; set; }
		public decimal IntakeTargetAmt { get; set; }
		public decimal BudgetAllocationAmt { get; set; }
		public double PlanDeniedRate { get; set; }
		public double PlanWithdrawnRate { get; set; }
		public double PlanReductionRate { get; set; }
		public double PlanSlippageRate { get; set; }
		public double PlanCancellationRate { get; set; }
		public string PublishDate { get; set; }
		public string OpeningDate { get; set; }
		public string ClosingDate { get; set; }
		public int TrainingPeriodId { get; set; }
		public int GrantStreamId { get; set; }
		public ApplicationTrainingPeriodViewModel TrainingPeriod { get; set; }
		public ApplicationGrantStreamViewModel GrantStream { get; set; }

		public double ReimbursementRate { get; set; }
		public int? StartDate { get; set; }
		public int? EndDate { get; set; }

		public Region Provinces { get; set; }

		public Country Countries { get; set; }

		public ApplicationGrantOpeningViewModel()
		{
		}

		public ApplicationGrantOpeningViewModel(GrantOpening entity)
		{
			Utilities.MapProperties(entity, this, o => new { o.PublishDate, o.OpeningDate, o.ClosingDate });
			TrainingPeriod = new ApplicationTrainingPeriodViewModel(entity.TrainingPeriod);
			GrantStream = new ApplicationGrantStreamViewModel(entity.GrantStream);
			ReimbursementRate = GrantStream.ReimbursementRate;
			StartDate = entity.TrainingPeriod.StartDate.ToUnixTimeSeconds();
			EndDate = entity.TrainingPeriod.EndDate.ToUnixTimeSeconds();
			PublishDate = entity.PublishDate.ToLocalMorning().ToString("MMMMM d, yyyy");
			OpeningDate = entity.OpeningDate.ToLocalMorning().ToString("MMMMM d, yyyy");
			ClosingDate = entity.ClosingDate.ToLocalMidnight().ToString("MMMMM d, yyyy");
		}
	}
}
