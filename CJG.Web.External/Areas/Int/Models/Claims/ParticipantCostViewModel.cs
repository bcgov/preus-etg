using CJG.Core.Entities;
using System;

namespace CJG.Web.External.Areas.Int.Models.Claims
{
	public class ParticipantCostViewModel
	{
		#region Properties
		public int Id { get; set; }
		public string RowVersion { get; set; }

		public decimal ClaimParticipantCost { get; set; }
		public decimal ClaimReimbursement { get; set; }
		public decimal ClaimEmployerContribution { get; set; }

		public decimal AssessedParticipantCost { get; set; }
		public decimal AssessedReimbursement { get; set; }
		public decimal AssessedEmployerContribution { get; set; }

		public ParticipantViewModel Participant { get; set; }
		#endregion

		#region Constructors
		public ParticipantCostViewModel() { }

		public ParticipantCostViewModel(ParticipantCost participantCost)
		{
			if (participantCost == null) throw new ArgumentNullException(nameof(participantCost));

			this.Id = participantCost.Id;
			this.RowVersion = Convert.ToBase64String(participantCost.RowVersion);

			this.ClaimParticipantCost = participantCost.ClaimParticipantCost;
			this.ClaimReimbursement = participantCost.ClaimReimbursement;
			this.ClaimEmployerContribution = participantCost.ClaimEmployerContribution;

			this.AssessedParticipantCost = participantCost.AssessedParticipantCost;
			this.AssessedReimbursement = participantCost.AssessedReimbursement;
			this.AssessedEmployerContribution = participantCost.AssessedEmployerContribution;

			this.Participant = new ParticipantViewModel(participantCost.ParticipantForm);
		}
		#endregion
	}
}