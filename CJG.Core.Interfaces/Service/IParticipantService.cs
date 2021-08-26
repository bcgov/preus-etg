using System;
using CJG.Core.Entities;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
	public interface IParticipantService : IService
	{
		ParticipantForm Get(int id);

		ParticipantForm Add(ParticipantForm newParticipantForm);

		ParticipantCost Add(ParticipantCost newParticipantCost);

		ParticipantCost Update(ParticipantCost participantCost);

		IEnumerable<ParticipantForm> GetParticipantFormsForGrantApplication(int grantApplication);

		IEnumerable<ParticipantCost> GetParticipantCostsForClaimEligibleCost(int claimEligibleCostId);

		IEnumerable<ParticipantCost> GetParticipantCosts(ClaimEligibleCost eligibleCost);

		IEnumerable<ParticipantForm> GetUnemployedParticipantEnrollments(DateTime currentDate, int take);

		int GetParticipantsWithClaimEligibleCostCount(int claimId, int claimVersion);

		void UpdateReportedDate(IEnumerable<ParticipantForm> participantEnrollments, DateTime reportedDate);

		void RemoveParticipant(ParticipantForm participantForm);

		void IncludeParticipant(ParticipantForm participant);

		void ExcludeParticipant(ParticipantForm participant);
	}
}
