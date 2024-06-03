using CJG.Application.Business.Models;
using CJG.Core.Entities;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
	/// <summary>
	/// <typeparamref name="IClaimService"/> interface, provides service methods related to <typeparamref name="Claim"/> objects.
	/// </summary>
	public interface IClaimService : IService
	{
		#region Claims
		Claim Get(int id, int version = 0);
		Claim GetCurrentClaim(int grantApplicationId);
		int GetCurrentClaimVersion(int id);
		Claim AddNewClaim(GrantApplication grantApplication);
		Claim CreateNewClaimVersion(GrantApplication grantApplication);
		Claim Update(Claim claim, bool overrideRates = false);

		IEnumerable<Claim> GetClaims(int id);
		IEnumerable<Claim> GetClaims(ClaimState state);
		int GetTotalClaims(ClaimState state);
		IEnumerable<Claim> GetApprovedClaimsForPaymentRequest(int grantProgramId);
		IEnumerable<Claim> GetApprovedClaimsForAmountOwing(int grantProgramId);
		RequestOnHoldModel GetClaimsByPaymentRequestHold(int grantProgramId, bool requestHold);
		IEnumerable<Claim> GetClaimsForGrantApplication(int grantApplicationId);
		#endregion

		#region ClaimEligibleCosts
		ClaimEligibleCost GetClaimEligibleCost(int claimEligibleCostId);
		string AddEligibleCost(Claim claim, int eligibleCostTypeId, bool commitTransaction = true, ClaimEligibleCost claimEligibleCostToAdd = null);
		Claim Update(ClaimEligibleCost claimEligibleCost, byte[] claimRowVersion, bool commitTransaction = true, ApplicationWorkflowTrigger trigger = ApplicationWorkflowTrigger.EditClaim);
		string DeleteEligibleCost(Claim claim, int claimEligibleCostId);
		Claim RemoveEligibleCostsAddedByAssessor(Claim claim);
		Claim Remove(ClaimEligibleCost claimEligibleCost, byte[] claimRowVersion, ApplicationWorkflowTrigger trigger = ApplicationWorkflowTrigger.EditTrainingCosts);
		#endregion

		#region ParticipantCosts
		ParticipantCost GetParticipantCost(int id);
		IEnumerable<ParticipantCost> GetParticipantCosts(int claimEligibleCostId);
		Claim UpdateParticipantCosts(int claimEligibleCostId, IEnumerable<ParticipantCost> participantCosts, byte[] claimRowVersion);
		Claim UpdateParticipants(Claim claim);
		#endregion

		#region Attachments
		Attachment GetAttachment(int claimId, int claimVersion, int attachmentId);
		void AddReceipt(int id, Attachment attachment);
		Attachment AddReceipt(int claimId, int claimVersion, Attachment attachment);
		void RemoveReceipt(int id, Attachment attachment);
		void RemoveReceipt(int claimId, int claimVersion, Attachment attachment);
		#endregion

		#region Workflow
		void SubmitClaim(Claim claim);
		void WithdrawClaim(Claim claim, string reason);
		void SelectClaimForAssessment(Claim claim);
		void RemoveClaimFromAssessment(Claim claim);
		void AssessClaimReimbursement(Claim claim);
		void AssessClaimEligibility(Claim claim);
		void ApproveClaim(Claim claim);
		void DenyClaim(Claim claim);
		void ReturnClaimToApplicant(Claim claim);
		void ReturnClaimToNew(Claim claim);
		void ReverseClaimDenied(Claim claim);
		void InitiateClaimAmendment(Claim claim);
		void CloseClaimReporting(GrantApplication grantApplication);
		void EnableClaimReporting(GrantApplication grantApplication);
		#endregion
	}
}
