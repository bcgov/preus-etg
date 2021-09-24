using CJG.Application.Services;
using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models.Claims
{
	public class EligibleExpenseTypeViewModel
	{
		#region Properties
		public int Id { get; set; }
		public string Caption { get; set; }
		public string Description { get; set; }
		public bool AllowMultiple { get; set; }
		public bool IsActive { get; set; }
		public int RowSequence { get; set; }
		public ExpenseTypes ExpenseTypeId { get; set; }
		public IEnumerable<EligibleExpenseBreakdownViewModel> EligibleExpenseBreakdowns { get; set; } 
		#endregion

		#region Constructors
		public EligibleExpenseTypeViewModel()
		{

		}

		public EligibleExpenseTypeViewModel(EligibleExpenseType eligibleExpenseType)
		{
			if (eligibleExpenseType == null) throw new ArgumentNullException(nameof(eligibleExpenseType));

			Utilities.MapProperties(eligibleExpenseType, this);

			this.EligibleExpenseBreakdowns = eligibleExpenseType.Breakdowns.Where(b => b.IsActive).Select(eeb => new EligibleExpenseBreakdownViewModel(eeb)).ToArray();
		}

		public EligibleExpenseTypeViewModel(EligibleExpenseType eligibleExpenseType, EligibleCost eligibleCost)
		{
			if (eligibleExpenseType == null) throw new ArgumentNullException(nameof(eligibleExpenseType));

			Utilities.MapProperties(eligibleExpenseType, this);

			this.EligibleExpenseBreakdowns = eligibleExpenseType.Breakdowns.Where(b => b.IsActive).Select(eeb =>
			{
				var lineItem = new EligibleExpenseBreakdownViewModel(eeb);

				if (eligibleCost != null)
				{
					lineItem.Selected = eligibleCost.Breakdowns.Any(t => t.EligibleExpenseBreakdown.ServiceLineId == eeb.ServiceLineId);
				}

				return lineItem;
			});
		}
		#endregion
	}



}