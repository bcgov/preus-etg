using CJG.Core.Entities;

namespace CJG.Application.Business.Models
{
	public class TrainingBudgetModel
	{
		public int TrainingPeriodId { get; set; }
		public TrainingPeriodBudgetType BudgetType { get; set; }
		public decimal WithdrawnRate { get; set; }
		public decimal RefusalRate { get; set; }
		public decimal ApprovedSlippageRate { get; set; }
		public decimal ClaimedSlippageRate { get; set; }
	}
}