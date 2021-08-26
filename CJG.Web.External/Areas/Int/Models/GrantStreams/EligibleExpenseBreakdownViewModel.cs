using CJG.Application.Services;
using CJG.Core.Entities;
using System;

namespace CJG.Web.External.Areas.Int.Models.GrantStreams
{
	public class EligibleExpenseBreakdownViewModel
	{
		#region Properties
		public int Id { get; set; }
		public string RowVersion { get; set; }
		public string Caption { get; set; }
		public string Description { get; set; }
		public ExpenseTypes EligibleExpenseTypeId { get; set; }
		public int? ServiceLineId { get; set; }
		public bool EnableCost { get; set; }
		public bool IsActive { get; set; }
		public int RowSequence { get; set; }
		public bool Delete { get; set; }
		#endregion

		#region Constructors
		public EligibleExpenseBreakdownViewModel() { }

		public EligibleExpenseBreakdownViewModel(EligibleExpenseBreakdown breakdown)
		{
			if (breakdown == null) throw new ArgumentNullException(nameof(breakdown));

			Utilities.MapProperties(breakdown, this);
		}
		#endregion
	}
}