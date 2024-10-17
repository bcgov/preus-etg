using System.Collections.Generic;

namespace CJG.Web.External.Models.Shared
{
    public class EligibleExpenseTypeViewModel : LookupTableViewModel
    {
        public double? Rate { get; set; }
        public bool AutoInclude { get; set; }
        public bool AllowMultiple { get; set; }
        public KeyValueViewModel<int> ExpenseType { get; set; } = new KeyValueViewModel<int>();

        public List<EligibleExpenseBreakdownViewModel> Breakdowns { get; set; } = new List<EligibleExpenseBreakdownViewModel>();
    }
}
