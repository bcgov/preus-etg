using System;
using CJG.Core.Entities;

namespace CJG.Application.Business
{
	public class ParticipantCostModel
	{
		public int Id { get; set; }
		public string RowVersion { get; set; }
		public string Name { get; set; }
		public decimal ClaimParticipantCost { get; set; }
		public decimal ClaimEmployerContribution { get; set; }
		public decimal ClaimReimbursement { get; set; }
		public decimal AssessedParticipantCost { get; set; }
		public decimal AssessedEmployerContribution { get; set; }
		public decimal AssessedReimbursement { get; set; }
		public double Rate { get; set; }
		public int ParticipantFormId { get; set; }

		public ParticipantCostModel()
		{

		}
		public ParticipantCostModel(ParticipantCost participantCost, double rate)
		{
			if (participantCost == null)
				throw new ArgumentNullException(nameof(participantCost));

			Id = participantCost.Id;
			RowVersion = participantCost.RowVersion != null ? Convert.ToBase64String(participantCost.RowVersion) : null;
			Name = $"{participantCost.ParticipantForm?.LastName}, {participantCost.ParticipantForm?.FirstName}";
			ClaimParticipantCost = participantCost.ClaimParticipantCost;
			ClaimEmployerContribution = participantCost.ClaimEmployerContribution;
			ClaimReimbursement = participantCost.ClaimReimbursement;
			AssessedParticipantCost = participantCost.AssessedParticipantCost;
			AssessedEmployerContribution = participantCost.AssessedEmployerContribution;
			AssessedReimbursement = participantCost.AssessedReimbursement;
			ParticipantFormId = participantCost.ParticipantFormId;

			Rate = rate;
		}
	}
}
