using System;
using CJG.Core.Entities;

namespace CJG.Web.External.Areas.Int.Models.Claims
{
	public class ParticipantCostViewModel
	{
		public int Id { get; set; }
		public string RowVersion { get; set; }

		public decimal ClaimParticipantCost { get; set; }
		public decimal ClaimReimbursement { get; set; }
		public decimal ClaimEmployerContribution { get; set; }

		public decimal AssessedParticipantCost { get; set; }
		public decimal AssessedReimbursement { get; set; }
		public decimal AssessedEmployerContribution { get; set; }

		public ParticipantViewModel Participant { get; set; }

		public ParticipantCostViewModel() { }

		public ParticipantCostViewModel(ParticipantCost participantCost)
		{
			if (participantCost == null)
				throw new ArgumentNullException(nameof(participantCost));

			Id = participantCost.Id;
			RowVersion = Convert.ToBase64String(participantCost.RowVersion);

			ClaimParticipantCost = participantCost.ClaimParticipantCost;
			ClaimReimbursement = participantCost.ClaimReimbursement;
			ClaimEmployerContribution = participantCost.ClaimEmployerContribution;

			AssessedParticipantCost = participantCost.AssessedParticipantCost;
			AssessedReimbursement = participantCost.AssessedReimbursement;
			AssessedEmployerContribution = participantCost.AssessedEmployerContribution;

			Participant = new ParticipantViewModel(participantCost.ParticipantForm);
		}
	}
}