using System.ComponentModel;

namespace CJG.Core.Entities
{
	public enum TrainingPeriodBudgetType
	{
		[Description("Base Budget")]
		BaseBudget = 0,

		[Description("Old Growth Budget")]
		OldGrowthBudget = 1
	}
}