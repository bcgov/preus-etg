using CJG.Core.Entities;
using System;

namespace CJG.Application.Business
{
	public class ParticipantCostModel
	{
		#region Properties
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
		#endregion

		#region Constructors
		public ParticipantCostModel()
		{

		}
		public ParticipantCostModel(ParticipantCost participantCost, double rate)
		{
			if (participantCost == null)
				throw new ArgumentNullException(nameof(participantCost));

			this.Id = participantCost.Id;
			this.RowVersion = participantCost.RowVersion != null ? Convert.ToBase64String(participantCost.RowVersion) : null;
			this.Name = $"{participantCost.ParticipantForm?.LastName}, {participantCost.ParticipantForm?.FirstName}";
			this.ClaimParticipantCost = participantCost.ClaimParticipantCost;
			this.ClaimEmployerContribution = participantCost.ClaimEmployerContribution;
			this.ClaimReimbursement = participantCost.ClaimReimbursement;
			this.AssessedParticipantCost = participantCost.AssessedParticipantCost;
			this.AssessedEmployerContribution = participantCost.AssessedEmployerContribution;
			this.AssessedReimbursement = participantCost.AssessedReimbursement;
			this.ParticipantFormId = participantCost.ParticipantFormId;

			this.Rate = rate;
		}
		#endregion
	}
}
