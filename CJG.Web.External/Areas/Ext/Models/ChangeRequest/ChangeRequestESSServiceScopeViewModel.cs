using CJG.Core.Entities;
using System;

namespace CJG.Web.External.Areas.Ext.Models.ChangeRequest
{
    public class ChangeRequestESSServiceScopeViewModel
    {
        public string Name { get; set; }
        public ChangeRequestESSServiceScopeViewModel()
        {

        }
        public ChangeRequestESSServiceScopeViewModel(EligibleCostBreakdown eligibleCostBreakdown)
        {
            if (eligibleCostBreakdown == null) throw new ArgumentNullException(nameof(eligibleCostBreakdown));
            this.Name = eligibleCostBreakdown.EligibleExpenseBreakdown.Caption;
        }
    }
}