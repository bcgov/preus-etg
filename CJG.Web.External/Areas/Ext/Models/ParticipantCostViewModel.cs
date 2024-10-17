using CJG.Core.Entities;
using System;
using System.Linq;

namespace CJG.Web.External.Areas.Ext.Models
{
    public class ParticipantCostViewModel
    {
        #region Properties
        public int Id { get; set; }

        public int ClaimEligibleCostId { get; set; }

        public int ParticipantFormId { get; set; }

        public ParticipantFormViewModel ParticipantForm { get; set; }

        public decimal ClaimParticipantCost { get; set; }

        public decimal ClaimReimbursement { get; set; }

        public decimal ClaimEmployerContribution { get; set; }

        public decimal AssessedParticipantCost { get; set; }

        public decimal AssessedReimbursement { get; set; }

        public decimal AssessedEmployerContribution { get; set; }

        public decimal PreviouslyAssessedReimbursement { get; set; }

        public byte[] RowVersion { get; set; }
        #endregion


        #region Constructors
        public ParticipantCostViewModel()
        {

        }

        public ParticipantCostViewModel(ParticipantCost participantCost)
        {
            if (participantCost == null)
                throw new ArgumentNullException(nameof(participantCost));

            this.Id = participantCost.Id;
            this.ClaimEligibleCostId = participantCost.ClaimEligibleCostId;
            this.ParticipantFormId = participantCost.ParticipantFormId;
            this.ParticipantForm = participantCost.ParticipantForm != null ? new ParticipantFormViewModel(participantCost.ParticipantForm) : null;
            this.ClaimParticipantCost = participantCost.ClaimParticipantCost;
            this.ClaimReimbursement = participantCost.ClaimReimbursement;
            this.ClaimEmployerContribution = participantCost.ClaimEmployerContribution;
            this.AssessedParticipantCost = participantCost.AssessedParticipantCost;
            this.AssessedReimbursement = participantCost.AssessedReimbursement;
            this.AssessedEmployerContribution = participantCost.AssessedEmployerContribution;
            this.PreviouslyAssessedReimbursement = participantCost.ClaimEligibleCost.Source?.ParticipantCosts.Where(pc => pc.ParticipantFormId == participantCost.ParticipantFormId).FirstOrDefault()?.AssessedReimbursement ?? 0;
            this.RowVersion = participantCost.RowVersion;
        }
        #endregion

        #region Methods
        public static implicit operator ParticipantCost(ParticipantCostViewModel model)
        {
            if (model == null)
                return null;

            var participant_cost = new ParticipantCost
            {
                Id = model.Id,
                ClaimEligibleCostId = model.ClaimEligibleCostId,
                ParticipantFormId = model.ParticipantFormId,
                AssessedEmployerContribution = model.AssessedEmployerContribution,
                AssessedParticipantCost = model.AssessedParticipantCost,
                AssessedReimbursement = model.AssessedReimbursement,
                ClaimEmployerContribution = model.ClaimEmployerContribution,
                ClaimParticipantCost = model.ClaimParticipantCost,
                ClaimReimbursement = model.ClaimReimbursement,
                RowVersion = model.RowVersion
            };
            return participant_cost;
        }
        #endregion
    }
}