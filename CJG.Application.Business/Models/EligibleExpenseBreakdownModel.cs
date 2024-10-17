using CJG.Core.Entities;

namespace CJG.Application.Business.Models
{
    public class EligibleExpenseBreakdownModel : CollectionItemModel
    {
        #region Properties
        public bool EnableCost { get; set; }
        #endregion

        #region Constructors
        public EligibleExpenseBreakdownModel()
        {

        }

        public EligibleExpenseBreakdownModel(EligibleExpenseBreakdown eligibleExpenseBreakdown)
        {
            this.Id = eligibleExpenseBreakdown.Id;
            this.Caption = eligibleExpenseBreakdown.Caption;
            this.EnableCost = eligibleExpenseBreakdown.EnableCost;
        }
        #endregion
    }
}
