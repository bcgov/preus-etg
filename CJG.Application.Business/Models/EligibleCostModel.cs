using System;
using System.Collections.Generic;
using CJG.Core.Entities;

namespace CJG.Application.Business.Models
{
	public class EligibleCostModel
    {
	    public int Id { get; set; }
        public EligibleExpenseTypeModel EligibleExpenseType { get; set; }
        public int EstimatedParticipants { get; set; }
        public decimal EstimatedParticipantCost { get; set; }
        public decimal EstimatedCost { get; set; }
        public decimal EstimatedEmployerContribution { get; set; }
        public decimal EstimatedReimbursement { get; set; }

        public string ExpenseExplanation { get; set; }

		public int AgreedMaxParticipants { get; set; }
        public decimal AgreedMaxParticipantCost { get; set; }
        public decimal AgreedCost { get; set; }
        public decimal AgreedEmployerContribution { get; set; }
        public decimal AgreedMaxReimbursement { get; set; }
        public ServiceTypes? ServiceType { get; set; }
        public bool ShowBreakdowns { get; set; }

        public bool AddedByAssessor { get; set; }


        public List<EligibleCostBreakdownModel> Breakdowns { get; set; } = new List<EligibleCostBreakdownModel>();

        public EligibleCostModel()
        {

        }

        public EligibleCostModel(EligibleCost eligibleCost)
        {
            if (eligibleCost == null)
                throw new ArgumentNullException(nameof(eligibleCost));

            Id = eligibleCost.Id;
            EstimatedCost = eligibleCost.EstimatedCost;
            EstimatedParticipantCost = eligibleCost.EstimatedParticipantCost;
            EstimatedEmployerContribution = eligibleCost.EstimatedEmployerContribution;
            EstimatedParticipants = eligibleCost.EstimatedParticipants;
            EstimatedReimbursement = eligibleCost.EstimatedReimbursement;

            ExpenseExplanation = eligibleCost.ExpenseExplanation;

            AgreedMaxParticipants = eligibleCost.AgreedMaxParticipants;
            AgreedMaxParticipantCost = eligibleCost.AgreedMaxParticipantCost;
            AgreedCost = eligibleCost.AgreedMaxCost;
            AgreedEmployerContribution = eligibleCost.AgreedEmployerContribution;
            AgreedMaxReimbursement = eligibleCost.AgreedMaxReimbursement;

            EligibleExpenseType = new EligibleExpenseTypeModel(eligibleCost.EligibleExpenseType);
            ShowBreakdowns = !(eligibleCost.EligibleExpenseType.ServiceCategory?.ServiceTypeId.In(ServiceTypes.EmploymentServicesAndSupports, ServiceTypes.Administration) ?? false);
            ServiceType = eligibleCost.EligibleExpenseType.ServiceCategory?.ServiceTypeId;
            AddedByAssessor = eligibleCost.AddedByAssessor;

            foreach (var item in eligibleCost.Breakdowns)
            {
                Breakdowns.Add(new EligibleCostBreakdownModel(item));
            }
        }

        public EligibleCostModel(EligibleExpenseTypeModel eligibleExpenseType)
        {
            EligibleExpenseType = eligibleExpenseType;
        }
    }
}
