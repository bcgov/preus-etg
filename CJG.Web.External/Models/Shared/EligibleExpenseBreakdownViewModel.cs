using CJG.Application.Services;
using CJG.Core.Entities;

namespace CJG.Web.External.Models.Shared
{
    public class EligibleExpenseBreakdownViewModel : LookupTableViewModel
    {
        public int EligibleExpenseTypeId { get; set; }
        public int? ServiceLineId { get; set; }

        public EligibleExpenseBreakdownViewModel()
        {
        }

        public EligibleExpenseBreakdownViewModel(EligibleExpenseBreakdown eligibleExpenseBreakdown)
        {
            Utilities.MapProperties(eligibleExpenseBreakdown, this);
        }
    }
}
