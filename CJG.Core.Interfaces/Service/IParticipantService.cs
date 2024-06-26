﻿using System;
using System.Collections.Generic;
using CJG.Core.Entities;

namespace CJG.Core.Interfaces.Service
{
	public interface IParticipantService : IService
	{
		ParticipantForm Get(int id);
		ParticipantForm Add(ParticipantForm newParticipantForm);
		ParticipantCost Add(ParticipantCost newParticipantCost);
		ParticipantCost Update(ParticipantCost participantCost);

		void ApproveDenyParticipants(int grantApplicationId, Dictionary<int?, bool?> participantApproved);
		void ReportAttendance(Dictionary<int, bool?> participantAttended);

		IEnumerable<ParticipantForm> GetParticipantFormsForGrantApplication(int grantApplication);
		IEnumerable<ParticipantCost> GetParticipantCostsForClaimEligibleCost(int claimEligibleCostId);
		IEnumerable<ParticipantCost> GetParticipantCosts(ClaimEligibleCost eligibleCost);
		IEnumerable<ParticipantForm> GetUnemployedParticipantEnrollments(DateTime currentDate, int take, DateTime cutoffDate);

		int GetParticipantsWithClaimEligibleCostCount(int claimId, int claimVersion);
		IDictionary<string, decimal> GetParticipantYTD(GrantApplication grantApplication);

		void UpdateReportedDate(IEnumerable<ParticipantForm> participantEnrollments, DateTime reportedDate);
		void UpdateExpectedOutcome(ParticipantForm participantForm, ExpectedParticipantOutcome? modelExpectedOutcome);
		void RemoveParticipant(ParticipantForm participantForm);
		void IncludeParticipant(ParticipantForm participant);
		void ExcludeParticipant(ParticipantForm participant);
		IEnumerable<ParticipantForm> GetParticipantFormsBySIN(string sin);

		ParticipantInvitation UpdateParticipantInvitation(ParticipantInvitation participantInvitation);
		ParticipantInvitation RemoveParticipantInvitation(ParticipantInvitation participantInvitation);
		ParticipantInvitation SendParticipantInvitation(ParticipantInvitation participantInvitation);
		ParticipantInvitation GetInvitation(int grantApplicationId, int invitationId);

		ParticipantInvitation CompleteIndividualInvitation(ParticipantForm participantForm, ParticipantInvitation participantInvitation);
	}
}