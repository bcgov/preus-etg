using CJG.Application.Services;
using CJG.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models
{
	public class EstimatedCostViewModel
	{
		#region Properties
		public int Id { get; set; }
		public int? ServiceType { get; set; }
		public int? ServiceCategoryId { get; set; }
		public string EligibleExpenseTypeCaption { get; set; }
		public decimal EstimatedParticipantCost { get; set; }
		public int EstimatedParticipants { get; set; }
		public decimal TotalCost { get; set; }
		public decimal EstimatedEmployerContribution { get; set; }
		public decimal EstimatedReimbursement { get; set; }
		public decimal AgreedMaxParticipantCost { get; set; }
		public int AgreedMaxParticipants { get; set; }
		public decimal AgreedEmployerContribution { get; set; }
		public decimal AgreedMaxReimbursement { get; set; }
		public bool HasOfferBeenIssued { get; set; }
		public IEnumerable<Breakdowns> Breakdowns { get; set; }
		#endregion

		#region Constructors
		public EstimatedCostViewModel()
		{

		}

		public EstimatedCostViewModel(EligibleCost eligibleCost, GrantApplication grantApplication)
		{
			Utilities.MapProperties(eligibleCost, this);
			this.EligibleExpenseTypeCaption = eligibleCost.EligibleExpenseType.Caption;
			this.ServiceCategoryId = eligibleCost.EligibleExpenseType.ServiceCategory?.Id;
			this.ServiceType = (int?)eligibleCost.EligibleExpenseType.ServiceCategory?.ServiceTypeId;
			this.HasOfferBeenIssued = grantApplication.HasOfferBeenIssued();
			this.TotalCost = !this.HasOfferBeenIssued ?  eligibleCost.EstimatedCost: eligibleCost.AgreedMaxCost;
			if (eligibleCost.EligibleExpenseType.ServiceCategory?.ServiceTypeId == ServiceTypes.SkillsTraining)
			{
				if (!this.HasOfferBeenIssued)
				{
					this.Breakdowns = eligibleCost.Breakdowns.Where(b => !b.AddedByAssessor).OrderBy(b => b.EligibleExpenseBreakdown?.RowSequence).Select(x => new Breakdowns(x)).ToArray();
				}
				else
				{
					this.Breakdowns = eligibleCost.Breakdowns.Where(b => b.IsEligible).OrderBy(b => b.EligibleExpenseBreakdown?.RowSequence).Select(x => new Breakdowns(x)).ToArray();
				}
			}
		}
		#endregion
	}
}