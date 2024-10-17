using CJG.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models.Claims
{
    public class ClaimEligibleCostViewModel
    {
        #region Properties
        public int Id { get; set; }

        public int ClaimId { get; set; }

        public int ClaimVersion { get; set; }

        public int EligibleExpenseTypeId { get; set; }

        public EligibleExpenseTypeViewModel EligibleExpenseType { get; set; }

        public int? EligibleCostId { get; set; }

        public EligibleCostViewModel EligibleCost { get; set; }

        public int? SourceId { get; set; }

        public ClaimEligibleCostViewModel Source { get; set; }

        public decimal ClaimCost { get; set; }

        public int ClaimParticipants { get; set; }

        public double AgreedReimbursementRate { get; set; }

        public decimal AgreedMaxCost { get; set; }

        public int AgreedMaxParticipants { get; set; }

        public decimal AgreedMaxParticipantCost { get; set; }

        public decimal AgreedMaxReimbursement { get; set; }

        public decimal AgreedMaxParticipantReimbursementCost { get; set; }

        public decimal AgreedEmployerContribution { get; set; }

        public decimal AgreedParticipantEmployerContribution { get; set; }

        public decimal ClaimMaxParticipantCost { get; set; }

        public decimal ClaimMaxParticipantReimbursementCost { get; set; }

        public decimal ClaimParticipantEmployerContribution { get; set; }

        public decimal AssessedCost { get; set; }

        public int AssessedParticipants { get; set; }

        public decimal AssessedMaxParticipantCost { get; set; }

        public decimal AssessedMaxParticipantReimbursementCost { get; set; }

        public decimal AssessedParticipantEmployerContribution { get; set; }

        public decimal SumOfParticipantCostUnitsUnassigned { get; set; }

        public byte[] RowVersion { get; set; }

        public IList<ParticipantCostViewModel> ParticipantCosts { get; set; } = new List<ParticipantCostViewModel>();

        public bool AddedByAssessor { get; set; }
        #endregion

        #region Constructors
        public ClaimEligibleCostViewModel()
        {

        }

        public ClaimEligibleCostViewModel(ClaimEligibleCost claimEligibleCost)
        {
            if (claimEligibleCost == null)
                throw new ArgumentNullException(nameof(claimEligibleCost));

            this.Id = claimEligibleCost.Id;
            this.ClaimId = claimEligibleCost.ClaimId;
            this.ClaimVersion = claimEligibleCost.ClaimVersion;
            this.AddedByAssessor = claimEligibleCost.AddedByAssessor;
            this.EligibleExpenseTypeId = claimEligibleCost.EligibleExpenseTypeId;
            this.EligibleExpenseType = new EligibleExpenseTypeViewModel(claimEligibleCost.EligibleExpenseType);
            this.EligibleCostId = claimEligibleCost.EligibleCostId;
            this.EligibleCost = claimEligibleCost.EligibleCost != null
                ? new EligibleCostViewModel(claimEligibleCost.EligibleCost)
                : null;
            this.SourceId = claimEligibleCost.SourceId;
            this.Source = claimEligibleCost.Source != null
                ? new ClaimEligibleCostViewModel(claimEligibleCost.Source)
                : null;

            this.ClaimCost = claimEligibleCost.ClaimCost;
            this.ClaimParticipants = claimEligibleCost.ClaimParticipants;
            this.ClaimMaxParticipantCost = claimEligibleCost.ClaimMaxParticipantCost;
            this.ClaimMaxParticipantReimbursementCost = claimEligibleCost.ClaimMaxParticipantReimbursementCost;
            this.ClaimParticipantEmployerContribution = claimEligibleCost.ClaimParticipantEmployerContribution;

            this.AssessedCost = claimEligibleCost.AssessedCost;
            this.AssessedParticipants = claimEligibleCost.AssessedParticipants;
            this.AssessedMaxParticipantCost = claimEligibleCost.AssessedMaxParticipantCost;
            this.AssessedMaxParticipantReimbursementCost = claimEligibleCost.AssessedMaxParticipantReimbursementCost;
            this.AssessedParticipantEmployerContribution = claimEligibleCost.AssessedParticipantEmployerContribution;

            // These values will only be populated if there was an agreement.
            // These values will use the assessed values of a prior approved claim.

            this.AgreedReimbursementRate = claimEligibleCost.Claim.GrantApplication.ReimbursementRate;
            this.AgreedMaxCost = claimEligibleCost.EligibleCost?.AgreedMaxCost ?? claimEligibleCost.Source?.AssessedCost ?? claimEligibleCost.AssessedCost;
            this.AgreedMaxParticipants = claimEligibleCost.EligibleCost?.AgreedMaxParticipants ?? claimEligibleCost.Source?.AssessedParticipants ?? claimEligibleCost.AssessedParticipants;
            this.AgreedMaxReimbursement = claimEligibleCost.EligibleCost?.AgreedMaxReimbursement ?? claimEligibleCost.Source?.AssessedReimbursementCost ?? claimEligibleCost.AssessedReimbursementCost;
            this.AgreedEmployerContribution = claimEligibleCost.EligibleCost?.AgreedEmployerContribution ?? claimEligibleCost.Source?.CalculateAssessedEmployerContribution() ?? claimEligibleCost.CalculateAssessedEmployerContribution();
            this.AgreedMaxParticipantCost = claimEligibleCost.EligibleCost?.AgreedMaxParticipantCost ?? claimEligibleCost.Source?.AssessedMaxParticipantCost ?? claimEligibleCost.AssessedMaxParticipantCost;
            this.AgreedMaxParticipantReimbursementCost = claimEligibleCost.EligibleCost?.CalculateAgreedPerParticipantReimbursement() ?? claimEligibleCost.Source?.AssessedMaxParticipantReimbursementCost ?? claimEligibleCost.AssessedMaxParticipantReimbursementCost;
            this.AgreedParticipantEmployerContribution = this.AgreedMaxParticipantCost - this.AgreedMaxParticipantReimbursementCost;

            this.ParticipantCosts = claimEligibleCost.ParticipantCosts.OrderBy(pc => pc.ParticipantForm.LastName)
                                                                      .ThenBy(pc => pc.ParticipantForm.FirstName)
                                                                      .Select(pc => new ParticipantCostViewModel(pc)).ToList();

            this.SumOfParticipantCostUnitsUnassigned = (this.ClaimParticipants - this.ParticipantCosts.Count(pc => pc.ClaimParticipantCost > 0)) * this.ClaimMaxParticipantCost;
            this.RowVersion = claimEligibleCost.RowVersion;
        }
        #endregion

        #region Methods
        public static implicit operator ClaimEligibleCost(ClaimEligibleCostViewModel model)
        {
            if (model == null)
                return null;

            var eligible_cost = new ClaimEligibleCost();
            eligible_cost.Id = model.Id;
            eligible_cost.ClaimVersion = model.ClaimVersion;
            eligible_cost.EligibleCostId = model.EligibleCostId;
            eligible_cost.EligibleExpenseTypeId = model.EligibleExpenseTypeId;
            eligible_cost.AssessedCost = model.AssessedCost;
            eligible_cost.AssessedParticipantEmployerContribution = model.AssessedParticipantEmployerContribution;
            eligible_cost.AssessedMaxParticipantCost = model.AssessedMaxParticipantCost;
            //eligible_cost.AssessedMaxParticipantReimbursementCost = model.AssessedMaxParticipantReimbursementCost;
            eligible_cost.AssessedParticipants = model.AssessedParticipants;
            eligible_cost.ClaimCost = model.ClaimCost;
            eligible_cost.ClaimParticipantEmployerContribution = model.ClaimParticipantEmployerContribution;
            eligible_cost.ClaimMaxParticipantCost = model.ClaimMaxParticipantCost;
            //eligible_cost.ClaimMaxParticipantReimbursementCost = model.ClaimMaxParticipantReimbursementCost;
            eligible_cost.ClaimParticipants = model.ClaimParticipants;
            eligible_cost.ParticipantCosts = model.ParticipantCosts.Select(pc => (ParticipantCost)pc).ToList();
            eligible_cost.RowVersion = model.RowVersion;
            return eligible_cost;
        }
        #endregion
    }
}