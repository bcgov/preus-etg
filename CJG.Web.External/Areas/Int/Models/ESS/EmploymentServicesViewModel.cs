using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models.ESS
{
	public class EmploymentServicesViewModel : BaseViewModel
	{
		#region Properties
		public string RowVersion { get; set; }
		public IEnumerable<int> SelectedServiceLineIds { get; set; }
		#endregion

		#region Constructors
		public EmploymentServicesViewModel() { }

		public EmploymentServicesViewModel(EligibleCost eligibleCost)
		{
			if (eligibleCost == null) throw new ArgumentNullException(nameof(eligibleCost));

			this.Id = eligibleCost.Id;
			this.RowVersion = Convert.ToBase64String(eligibleCost.RowVersion);
			this.SelectedServiceLineIds = eligibleCost.Breakdowns.Select(b => b.EligibleExpenseBreakdownId).ToArray();
		}
		#endregion
	}
}