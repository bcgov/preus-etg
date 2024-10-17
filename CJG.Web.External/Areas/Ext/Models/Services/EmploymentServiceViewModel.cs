using CJG.Application.Services;
using CJG.Core.Entities;
using System;

namespace CJG.Web.External.Areas.Ext.Models.Services
{
	public class EmploymentServiceViewModel : External.Models.Shared.LookupTableViewModel
	{
		#region Properties
		public int GrantApplicationId { get; set; }
		public int EligibleCostId { get; set; }
		public decimal EstimatedCost { get; set; }

		public Models.TrainingCosts.EligibleExpenseTypeViewModel EligibleExpenseType { get; set; } = new Models.TrainingCosts.EligibleExpenseTypeViewModel();
		#endregion

		#region Constructors
		public EmploymentServiceViewModel()
		{
		}

		public EmploymentServiceViewModel(EligibleExpenseType eligibleExpenseType, EligibleCost eligibleCost, GrantApplication grantApplication)
		{
			Utilities.MapProperties(eligibleExpenseType, this, t => new { t.RowVersion });

			if (eligibleExpenseType.ServiceCategoryId.HasValue)
			{
				EligibleExpenseType = new Models.TrainingCosts.EligibleExpenseTypeViewModel(eligibleExpenseType, eligibleCost);
			}

			if (eligibleCost != null)
			{
				EligibleCostId = eligibleCost.Id;
				EstimatedCost = eligibleCost.EstimatedCost;
				RowVersion = Convert.ToBase64String(eligibleCost.RowVersion);
			}

			GrantApplicationId = grantApplication.Id;
		}
		#endregion
	}
}
