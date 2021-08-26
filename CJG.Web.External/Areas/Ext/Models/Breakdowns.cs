using CJG.Core.Entities;
using System;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models
{
    public class Breakdowns
    {
        #region Properties
        public int Id { get; set; }
        public decimal TotalCost { get; set; }
        public int? ServiceLineId { get; set; }
        public string Caption { get; set; }
        #endregion

        #region Constructors
        public Breakdowns(EligibleCostBreakdown eligibleCostBreakdown)
        {
            if (eligibleCostBreakdown == null)
                throw new ArgumentNullException(nameof(eligibleCostBreakdown));

            this.Id = eligibleCostBreakdown.Id;
            this.TotalCost = !eligibleCostBreakdown.EligibleCost.TrainingCost.GrantApplication.HasOfferBeenIssued() ? eligibleCostBreakdown.EstimatedCost : eligibleCostBreakdown.AssessedCost;
            this.ServiceLineId = eligibleCostBreakdown.EligibleExpenseBreakdown.ServiceLine?.Id;
            var isSkillsTraining = eligibleCostBreakdown.EligibleExpenseBreakdown.ServiceLine?.ServiceCategory?.ServiceTypeId == ServiceTypes.SkillsTraining;
            this.Caption = !isSkillsTraining ? eligibleCostBreakdown.EligibleExpenseBreakdown.Caption : eligibleCostBreakdown.TrainingPrograms.FirstOrDefault().CourseTitle;
        }
        #endregion
    }
}