using System;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Ext.Models
{
    public class ParticipantCostModel
    {
        #region Properties
        public int Id { get; set; }
        public string Name { get; set; }
        public string RowVersion { get; set; }
        public decimal ClaimParticipantCost { get; set; }
        public decimal ClaimEmployerContribution { get; set; }
        public decimal ClaimReimbursement { get; set; }
        #endregion

        #region Constructors
        public ParticipantCostModel()
        {
            
        }

        public ParticipantCostModel(ParticipantCost participantCost)
        {
            Id = participantCost.Id;
            Name = $"{participantCost.ParticipantForm?.LastName}, {participantCost.ParticipantForm?.FirstName}";
            ClaimParticipantCost = participantCost.ClaimParticipantCost;
            ClaimEmployerContribution = participantCost.ClaimEmployerContribution;
            ClaimReimbursement = participantCost.ClaimReimbursement;
            RowVersion = Convert.ToBase64String(participantCost.RowVersion);
        }
        #endregion

        #region Methods
        public static explicit operator ParticipantCost(ParticipantCostModel model)
        {
            return new ParticipantCost
            {
                Id = model.Id,
                ClaimParticipantCost = model.ClaimParticipantCost,
                ClaimEmployerContribution = model.ClaimEmployerContribution,
                ClaimReimbursement = model.ClaimReimbursement,
                RowVersion =  Convert.FromBase64String(model.RowVersion)
            };
        }
        #endregion
    }
}