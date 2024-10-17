using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAnnotationsExtensions;

namespace CJG.Core.Entities
{
	public class TrainingPeriodBudgetRate : EntityBase
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public virtual TrainingPeriod TrainingPeriod { get; set; }

		public TrainingPeriodBudgetType BudgetType { get; set; }

		[Min(0, ErrorMessage = "The withdrawn rate cannot be less than 0."), Max(1, ErrorMessage = "The withdrawn rate cannot be greater than 1.")]
		public decimal WithdrawnRate { get; set; }

		[Min(0, ErrorMessage = "The refusal rate cannot be less than 0."), Max(1, ErrorMessage = "The refusal rate cannot be greater than 1.")]
		public decimal RefusalRate { get; set; }

		[Min(0, ErrorMessage = "The refusal rate cannot be less than 0."), Max(1, ErrorMessage = "The refusal rate cannot be greater than 1.")]
		public decimal ApprovedSlippageRate { get; set; }

		[Min(0, ErrorMessage = "The refusal rate cannot be less than 0."), Max(1, ErrorMessage = "The refusal rate cannot be greater than 1.")]
		public decimal ClaimedSlippageRate { get; set; }
	}
}