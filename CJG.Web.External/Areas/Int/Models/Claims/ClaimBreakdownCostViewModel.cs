using CJG.Core.Entities;
using System;

namespace CJG.Web.External.Areas.Int.Models.Claims
{
	public class ClaimBreakdownCostViewModel
	{
		#region Properties
		public int Id { get; set; }
		public string RowVersion { get; set; }
		public string Caption { get; set; }
		public decimal ClaimCost { get; set; }
		public decimal AssessedCost { get; set; }
		public decimal AssessedToDate { get; set; }
		#endregion

		#region Constructors
		public ClaimBreakdownCostViewModel() { }

		public ClaimBreakdownCostViewModel(ClaimBreakdownCost breakdown)
		{
			if (breakdown == null) throw new ArgumentNullException(nameof(breakdown));

			this.Id = breakdown.Id;
			this.RowVersion = Convert.ToBase64String(breakdown.RowVersion);
			this.Caption = breakdown.EligibleExpenseBreakdown.Caption;
			this.ClaimCost = breakdown.ClaimCost;
			this.AssessedCost = breakdown.AssessedCost;
			this.AssessedToDate = breakdown.GetTotalAssessed();
		}
		#endregion
	}
}