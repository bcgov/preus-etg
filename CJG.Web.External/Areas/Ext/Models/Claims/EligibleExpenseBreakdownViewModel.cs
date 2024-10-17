using CJG.Application.Services;
using CJG.Core.Entities;
using System;

namespace CJG.Web.External.Areas.Ext.Models.Claims
{
	public class EligibleExpenseBreakdownViewModel
	{
		#region Properties
		public int Id { get; set; }
		public string RowVersion { get; set; }
		public string Caption { get; set; }
		public string Description { get; set; }
		public bool IsActive { get; set; } = true;
		public int RowSequence { get; set; }
		public bool EnableCost { get; set; }
		public int? ServiceLineId { get; set; }

		public bool Selected { get; set; }
		public bool Deleted { get; set; }
		#endregion

		#region Constructors
		public EligibleExpenseBreakdownViewModel()
		{

		}

		public EligibleExpenseBreakdownViewModel(EligibleExpenseBreakdown eligibleExpenseBreakdown)
		{
			if (eligibleExpenseBreakdown == null) throw new ArgumentNullException(nameof(eligibleExpenseBreakdown));

			Utilities.MapProperties(eligibleExpenseBreakdown, this);
		}
		#endregion
	}
}