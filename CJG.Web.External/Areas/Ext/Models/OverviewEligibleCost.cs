using CJG.Application.Business.Models;
using CJG.Application.Services;
using CJG.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models
{
    public class OverviewEligibleCost
    {
        #region Properties
        public int Id { get; set; }
        public decimal EstimatedCost { get; set; }
        public IEnumerable<CollectionItemModel> ServiceLines { get; set; } = new List<CollectionItemModel>();
        #endregion

        #region Constructors
        public OverviewEligibleCost()
        {

        }
        public OverviewEligibleCost(EligibleCost eligibleCost)
        {
            Utilities.MapProperties(eligibleCost, this);
            if (eligibleCost.Breakdowns.Any())
            {
                this.ServiceLines = eligibleCost.Breakdowns.Select(x => new CollectionItemModel()
                {
                    Id = x.EligibleExpenseBreakdown.ServiceLine.Id,
                    Caption = x.EligibleExpenseBreakdown.ServiceLine.Caption,
                    Description = x.EligibleExpenseBreakdown.ServiceLine.Description
                }).ToArray();
            }
        }
        #endregion
    }
}