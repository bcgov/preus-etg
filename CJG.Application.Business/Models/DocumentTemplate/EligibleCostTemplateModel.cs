using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Application.Business.Models.DocumentTemplate
{
	public class EligibleCostTemplateModel
	{
		#region Properties
		public int Id { get; set; }
		public int EligibleExpenseTypeId { get; set; }
		public string EligibleExpenseTypeCaption { get; set; }
		public string EligibleExpenseTypeServiceCategoryCaption { get; set; }
		public ServiceTypes? EligibleExpenseTypeServiceCategoryServiceTypeId { get; set; }

		public decimal AgreedMaxCost { get; set; }
		public int AgreedMaxParticipants { get; set; }
		public decimal AgreedMaxParticipantCost { get; set; }
		public decimal AgreedEmployerContribution { get; set; }
		public decimal AgreedMaxReimbursement { get; set; }

		public decimal EstimatedCost { get; set; }
		public int EstimatedParticipants { get; set; }
		public decimal EstimatedParticipantCost { get; set; }
		public decimal EstimatedEmployerContribution { get; set; }
		public decimal EstimatedReimbursement { get; set; }
		public int RowSequence { get; set; }
		public IEnumerable<TrainingProviderTemplateModel> TrainingProviders { get; set; }
		public IEnumerable<EligibleCostBreakdownTemplateModel> Breakdowns { get; set; }
		#endregion

		#region Constructors
		public EligibleCostTemplateModel()
		{
		}

		public EligibleCostTemplateModel(EligibleCost eligibleCost)
		{
			if (eligibleCost == null)
				throw new ArgumentNullException(nameof(eligibleCost));

			this.Id = eligibleCost.Id;
			this.RowSequence = eligibleCost.EligibleExpenseType.RowSequence;
			this.EligibleExpenseTypeId = eligibleCost.EligibleExpenseTypeId;
			this.EligibleExpenseTypeCaption = eligibleCost.EligibleExpenseType.Caption;
			this.EligibleExpenseTypeServiceCategoryCaption = eligibleCost.EligibleExpenseType.ServiceCategory?.Caption;
			this.EligibleExpenseTypeServiceCategoryServiceTypeId = eligibleCost.EligibleExpenseType.ServiceCategory?.ServiceTypeId;

			this.AgreedMaxCost = eligibleCost.AgreedMaxCost;
			this.AgreedMaxParticipants = eligibleCost.AgreedMaxParticipants;
			this.AgreedMaxParticipantCost = eligibleCost.AgreedMaxParticipantCost;
			this.AgreedEmployerContribution = eligibleCost.AgreedEmployerContribution;
			this.AgreedMaxReimbursement = eligibleCost.AgreedMaxReimbursement;

			this.EstimatedCost = eligibleCost.EstimatedCost;
			this.EstimatedParticipants = eligibleCost.EstimatedParticipants;
			this.EstimatedParticipantCost = eligibleCost.EstimatedParticipantCost;
			this.EstimatedEmployerContribution = eligibleCost.EstimatedEmployerContribution;
			this.EstimatedReimbursement = eligibleCost.EstimatedReimbursement;

			this.TrainingProviders = eligibleCost.TrainingProviders.Where(tp => tp.TrainingProviderState == TrainingProviderStates.Complete).Select(tp => tp.ApprovedTrainingProvider).Distinct().Select(o => new TrainingProviderTemplateModel(o)).ToArray();
			this.Breakdowns = eligibleCost.Breakdowns.Where(b => b.IsEligible).Select(o => new EligibleCostBreakdownTemplateModel(o)).ToArray();
		}
		#endregion
	}
}
