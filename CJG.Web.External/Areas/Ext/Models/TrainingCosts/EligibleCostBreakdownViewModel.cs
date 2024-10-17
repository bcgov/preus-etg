using CJG.Core.Entities;
using System;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models.TrainingCosts
{
	public class EligibleCostBreakdownViewModel
	{
		#region Properties
		public int Id { get; set; }
		public bool IsEligible { get; set; }
		public decimal AssessedCost { get; set; }
		public decimal EstimatedCost { get; set; }
		public EligibleExpenseBreakdownViewModel EligibleExpenseBreakdown { get; set; }

		public string TrainingProgramTitle { get; set; }
		#endregion

		#region Constructors
		public EligibleCostBreakdownViewModel()
		{

		}

		public EligibleCostBreakdownViewModel(EligibleCostBreakdown eligibleCostBreakdown)
		{
			if (eligibleCostBreakdown == null) throw new ArgumentNullException(nameof(eligibleCostBreakdown));

			this.Id = eligibleCostBreakdown.Id;

			this.IsEligible = eligibleCostBreakdown.IsEligible;
			this.EstimatedCost = eligibleCostBreakdown.EstimatedCost;
			this.AssessedCost = eligibleCostBreakdown.AssessedCost;

			this.EligibleExpenseBreakdown = new EligibleExpenseBreakdownViewModel(eligibleCostBreakdown.EligibleExpenseBreakdown);
			this.TrainingProgramTitle = eligibleCostBreakdown.TrainingPrograms.FirstOrDefault()?.CourseTitle;
		}
		#endregion
	}
}