using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CJG.Application.Business.Models;
using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;

namespace CJG.Core.Interfaces.Service
{
	public interface IGrantApplicationService : IService
	{
		GrantApplication Get(Guid invitationKey);
		GrantApplication Get(int id);
		GrantApplication Add(GrantApplication newGrantApplication);
		GrantApplication Update(GrantApplication grantApplication, ApplicationWorkflowTrigger trigger = ApplicationWorkflowTrigger.EditApplication, Func<ApplicationWorkflowTrigger, bool> canPerformAction = null);
		void Delete(GrantApplication grantApplication);

		void UpdateDeliveryDates(GrantApplication grantApplication);

		void ChangeGrantOpening(GrantApplication grantApplication);

		int GetApplicationsCountByFiscal(GrantApplication grantApplication);

		decimal GetApplicationsCostByFiscal(GrantApplication grantApplication);

		PageList<GrantApplication> GetGrantApplications(int page, int quantity, ApplicationFilter filter);
		IOrderedQueryable<GrantApplication> GetGrantApplications(int trainingProviderInventoryId, string search);
		IOrderedQueryable<GrantApplication> GetGrantApplicationsForOrg(int orgId, int grantProgramId, string search);
		IOrderedQueryable<GrantApplication> GetGrantApplicationsBySIN(string sin, string search);

		int GetTotalGrantApplications(List<ApplicationStateInternal> applicationStates, int assessorId, int grantOpeningId, int fiscalYearId, int intakePeriodId, int grantProgramId, int grantStreamId, string fileNumber, string applicant);
		int GetTotalGrantApplications(int trainingProviderInventoryId);

		ApplicationType GetDefaultApplicationType();

		GrantApplicationStateChange GetStateChange(int grantApplicationId, ApplicationStateInternal state);

		List<User> GetAvailableApplicationContacts(GrantApplication grantApplication);

		void UnassignAssessor(int assessorId);
		void AssignAssessor(GrantApplication grantApplication, int? assessorId);

		void ChangeApplicationAdministrator(GrantApplication grantApplication, int? applicantId);

		void ChangeAlternateContact(GrantApplication grantApplication, AlternateContactModel model);

		PageList<GrantApplication> GetGrantApplications<TKey>(User user, int page, int quantity, Expression<Func<GrantApplication, TKey>> orderByExpression);

		IEnumerable<GrantApplicationInternalState> GetInternalStates(bool? isActive);

		void EnableScheduledNotifications(GrantApplication grantApplication, bool enabled = true);

		void RevertApplicationStatus(Guid userBCeID, int orgId);

		#region Workflow
		void Submit(GrantApplication grantApplication);

		void Withdraw(GrantApplication grantApplication, string withdrawReason);

		int RestartApplicationFromWithdrawn(int id);

		/// <summary>
		/// Update grant application based on values in grant app == seeId
		/// </summary>
		/// <param name="id"></param>
		/// <param name="seedId"></param>
		/// <returns></returns>
		int DuplicateApplication(GrantApplication id, int seedId);

		/// <summary>
		/// Determine if an Application can be duplicated. If it cannot be duplicated then return the reason
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		string CanDuplicate(int id);

		void SelectForAssessment(GrantApplication grantApplication);

		void BeginAssessment(GrantApplication grantApplication, int internalUserId);

		void RemoveFromAssessment(GrantApplication grantApplication);

		void RecommendForApproval(GrantApplication grantApplication, string reason = null);

		void RecommendForDenial(GrantApplication grantApplication, string reason = null);

		void IssueOffer(GrantApplication grantApplication);

		void ReturnOfferToAssessment(GrantApplication grantApplication);

		void ReturnToDraft(GrantApplication grantApplication);

		void ReturnToAssessment(GrantApplication grantApplication, string reason = null);

		void DenyApplication(GrantApplication grantApplication, string reason);

		void WithdrawOffer(GrantApplication grantApplication, string reason);

		void ReturnWithdrawnOfferToAssessment(GrantApplication grantApplication);

		void ReturnUnfunded(GrantApplication grantApplication);

		void ReturnUnassessed(GrantApplication grantApplication);

		void ReturnUnassessedToNew(GrantApplication grantApplication);

		void SubmitChangeRequest(GrantApplication grantApplication, string reason);

		void RecommendChangeForApproval(GrantApplication grantApplication);

		void RecommendChangeForDenial(GrantApplication grantApplication, string reason = null);

		void ApproveChangeRequest(GrantApplication grantApplication);

		void ReturnChangeToAssessment(GrantApplication grantApplication, string reason = null);

		void DenyChangeRequest(GrantApplication grantApplication, string reason);

		void CancelApplicationAgreement(GrantApplication grantApplication, string reason);

		void ReverseCancelledApplicationAgreement(GrantApplication grantApplication);

		void CloseGrantFile(GrantApplication grantApplication);

		void SubmitCompletionReportToCloseGrantFile(GrantApplication grantApplication);

		void ReturnUnfundedApplications(int grantOpeningId);

		void EnableCompletionReporting(GrantApplication grantApplication);
		#endregion

		IEnumerable<AttachmentModel> GetAttachments(int id);

		void UpdateTrainingCosts(GrantApplication grantApplication);
		TrainingCostModel GetTrainingCostModel(int grantApplicationId);

		TrainingCost Update(TrainingCostModel trainingCost);

		void UpdateDeliveryPartner(GrantApplication grantApplication, int? deliveryPartnerId, IEnumerable<int> selectedServices);

		int CurrentPrioritizationRegionalExceptions();
	}
}