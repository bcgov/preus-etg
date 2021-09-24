using CJG.Core.Entities;
using System.Linq;

namespace CJG.Application.Business.Models
{
	public class EligibleCostBreakdownModel
	{
		#region Properties
		public int Id { get; set; }
		public bool IsEligible { get; set; }
		public decimal AssessedCost { get; set; }
		public decimal EstimatedCost { get; set; }
		public EligibleExpenseBreakdownModel EligibleExpenseBreakdown { get; set; }

		public string TrainingProgramTitle { get; set; }
		#endregion

		#region Constructors
		public EligibleCostBreakdownModel()
		{

		}

		public EligibleCostBreakdownModel(EligibleCostBreakdown eligibleCostBreakdown)
		{
			this.Id = eligibleCostBreakdown.Id;

			this.IsEligible = eligibleCostBreakdown.IsEligible;
			this.EstimatedCost = eligibleCostBreakdown.EstimatedCost;
			this.AssessedCost = eligibleCostBreakdown.AssessedCost;

			this.EligibleExpenseBreakdown = new EligibleExpenseBreakdownModel(eligibleCostBreakdown.EligibleExpenseBreakdown);
			this.TrainingProgramTitle = eligibleCostBreakdown.TrainingPrograms.FirstOrDefault()?.CourseTitle;
		}
		#endregion
	}
}
