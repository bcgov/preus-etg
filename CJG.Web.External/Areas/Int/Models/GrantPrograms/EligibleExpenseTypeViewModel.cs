using CJG.Application.Services;
using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models.GrantPrograms
{
	public class EligibleExpenseTypeViewModel
	{
		#region Properties
		public int Id { get; set; }
		public string RowVersion { get; set; }
		public string Caption { get; set; }
		public string Description { get; set; }
		public bool IsActive { get; set; }
		public int RowSequence { get; set; }
		public ExpenseTypes ExpenseTypeId { get; set; }
		public double? Rate { get; set; }
		public bool AutoInclude { get; set; }
		public bool AllowMultiple { get; set; } = true;


		public int? ServiceCategoryId { get; set; }
		public int MinProviders { get; set; }
		public int MaxProviders { get; set; }
		public int? MinPrograms { get; set; }
		public int? MaxPrograms { get; set; }

		public bool Delete { get; set; }
		public IEnumerable<EligibleExpenseBreakdownViewModel> Breakdowns { get; set; }
		#endregion

		#region Constructors
		public EligibleExpenseTypeViewModel()
		{
			this.Breakdowns = new List<EligibleExpenseBreakdownViewModel>();
		}

		public EligibleExpenseTypeViewModel(EligibleExpenseType expenseType)
		{
			if (expenseType == null) throw new ArgumentNullException(nameof(expenseType));

			Utilities.MapProperties(expenseType, this);
			this.MinPrograms = expenseType.ServiceCategory?.MinPrograms;
			this.MaxPrograms = expenseType.ServiceCategory?.MaxPrograms;
			this.Breakdowns = expenseType.Breakdowns.OrderBy(eet => eet.RowSequence).ThenBy(eet => eet.Caption).Select(b => new EligibleExpenseBreakdownViewModel(b));
		}
		#endregion
	}
}