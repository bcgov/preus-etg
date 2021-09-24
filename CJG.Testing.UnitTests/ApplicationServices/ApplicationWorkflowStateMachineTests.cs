using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.Entities;
using CJG.Testing.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
	public class ApplicationWorkflowStateMachineTests : ServiceUnitTestBase
	{
		#region Initialize
		[TestInitialize]
		public void Setup()
		{

		}
		#endregion

		#region Tests
		[TestMethod, TestCategory("Workflow"), TestCategory("State Machine")]
		public void SubmitApplication_Success()
		{
			// Arrange
			AppDateTime.SetNow(DateTime.UtcNow);
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var grantOpening = EntityHelper.CreateGrantOpening();
			grantOpening.State = GrantOpeningStates.Open;
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(grantOpening, applicationAdministrator, ApplicationStateInternal.Draft);
			grantApplication.TrainingCost.TrainingCostState = TrainingCostStates.Complete;
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), applicationAdministrator);

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.SubmitApplication();

			// Assert
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Submitted);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.New);
			grantApplication.DateSubmitted.Should().NotBeNull();
		}


		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void WithdrawlApplication_FromNew_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(applicationAdministrator, ApplicationStateInternal.New);
			TrainingProvider tp = grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider;
			tp.TrainingProviderInventoryId = 1;
			tp.TrainingProviderInventory = new TrainingProviderInventory();
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), applicationAdministrator);

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.WithdrawApplication("the reason");

			// Assert
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.ApplicationWithdrawn);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.ApplicationWithdrawn);
			grantApplication.TrainingPrograms.FirstOrDefault().TrainingProgramState.Should().Be(TrainingProgramStates.Incomplete);
			grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.TrainingProviderState.Should().Be(TrainingProviderStates.Incomplete);
			grantApplication.TrainingCost.TrainingCostState.Should().Be(TrainingCostStates.Incomplete);
			grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.TrainingProviderInventory.Should().BeNull();
			grantApplication.TrainingPrograms.FirstOrDefault()?.TrainingProvider.TrainingProviderInventoryId.Should().BeNull();
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void SelectForAssessment_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var assessor = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplication(applicationAdministrator, ApplicationStateInternal.New);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), assessor, "Assessor");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.SelectForAssessment();

			// Assert
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.PendingAssessment);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Submitted);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void BeginAssessment_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var assessor = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(applicationAdministrator, assessor, ApplicationStateInternal.PendingAssessment);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), assessor, "Assessor");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.BeginAssessment(assessor);

			// Assert
			grantApplication.Assessor.Should().Be(assessor);
			grantApplication.AssessorId.Should().Be(assessor.Id);
			grantApplication.IsUnderAssessment().Should().Be(true);
			grantApplication.IsApplicationAssessor(assessor).Should().Be(true);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.UnderAssessment);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Submitted);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void RemoveFromAssessment_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var assessor = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(applicationAdministrator, assessor, ApplicationStateInternal.PendingAssessment);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), assessor, "Assessor");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.RemoveFromAssessment();

			// Assert
			grantApplication.Assessor.Should().Be(assessor);
			grantApplication.AssessorId.Should().Be(assessor.Id);
			grantApplication.IsUnderAssessment().Should().Be(false);
			grantApplication.IsApplicationAssessor(assessor).Should().Be(true);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.New);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Submitted);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void RecommendForApproval_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var assessor = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(applicationAdministrator, assessor, ApplicationStateInternal.UnderAssessment);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), assessor, "Assessor");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.RecommendForApproval();

			// Assert
			grantApplication.Assessor.Should().Be(assessor);
			grantApplication.AssessorId.Should().Be(assessor.Id);
			grantApplication.IsUnderAssessment().Should().Be(true);
			grantApplication.IsApplicationAssessor(assessor).Should().Be(true);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.RecommendedForApproval);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Submitted);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void RecommendForDenial_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var assessor = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(applicationAdministrator, assessor, ApplicationStateInternal.UnderAssessment);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), assessor, "Assessor");
			var DenialReasonsMock = helper.MockDbSet<DenialReason>();

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.RecommendForDenial("{deniedReason:'reason one', selectedReasons:[2,5,8]}");

			// Assert
			grantApplication.Assessor.Should().Be(assessor);
			grantApplication.AssessorId.Should().Be(assessor.Id);
			grantApplication.IsUnderAssessment().Should().Be(true);
			grantApplication.IsApplicationAssessor(assessor).Should().Be(true);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.RecommendedForDenial);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Submitted);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void ReturnToAssessment_FromRecommendForApproval_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var director = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(applicationAdministrator, director, ApplicationStateInternal.RecommendedForApproval);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), director, "Director");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.ReturnToAssessment("Reason");

			// Assert
			grantApplication.Assessor.Should().Be(director);
			grantApplication.AssessorId.Should().Be(director.Id);
			grantApplication.IsUnderAssessment().Should().Be(true);
			grantApplication.IsApplicationAssessor(director).Should().Be(true);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.ReturnedToAssessment);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Submitted);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void ReturnToAssessment_FromRecommendForDenial_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var director = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(applicationAdministrator, director, ApplicationStateInternal.RecommendedForDenial);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), director, "Director");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.ReturnToAssessment("Reason");

			// Assert
			grantApplication.Assessor.Should().Be(director);
			grantApplication.AssessorId.Should().Be(director.Id);
			grantApplication.IsUnderAssessment().Should().Be(true);
			grantApplication.IsApplicationAssessor(director).Should().Be(true);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.ReturnedToAssessment);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Submitted);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void DenyApplication_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var director = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(applicationAdministrator, director, ApplicationStateInternal.RecommendedForDenial);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), director, "Director");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.DenyApplication("Reason");

			// Assert
			grantApplication.Assessor.Should().Be(director);
			grantApplication.AssessorId.Should().Be(director.Id);
			grantApplication.IsUnderAssessment().Should().Be(false);
			grantApplication.IsApplicationAssessor(director).Should().Be(true);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.ApplicationDenied);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.ApplicationDenied);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void IssueOffer_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var director = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(applicationAdministrator, director, ApplicationStateInternal.RecommendedForApproval);
			grantApplication.GrantAgreement = EntityHelper.CreateGrantAgreement(grantApplication);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), director, "Director");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.IssueOffer();

			// Assert
			grantApplication.Assessor.Should().Be(director);
			grantApplication.AssessorId.Should().Be(director.Id);
			grantApplication.IsUnderAssessment().Should().Be(false);
			grantApplication.IsApplicationAssessor(director).Should().Be(true);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.OfferIssued);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.AcceptGrantAgreement);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void WithdrawOffer_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var director = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(applicationAdministrator, director, ApplicationStateInternal.OfferIssued);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), director, "Director");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.WithdrawOffer("Reason");

			// Assert
			grantApplication.Assessor.Should().Be(director);
			grantApplication.AssessorId.Should().Be(director.Id);
			grantApplication.IsUnderAssessment().Should().Be(false);
			grantApplication.IsApplicationAssessor(director).Should().Be(true);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.OfferWithdrawn);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.AgreementWithdrawn);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void RejectGrantAgreement_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var assessor = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(applicationAdministrator, assessor, ApplicationStateInternal.OfferIssued);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), applicationAdministrator);

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.RejectGrantAgreement("Reason");

			// Assert
			grantApplication.Assessor.Should().Be(assessor);
			grantApplication.AssessorId.Should().Be(assessor.Id);
			grantApplication.IsUnderAssessment().Should().Be(false);
			grantApplication.IsApplicationAssessor(assessor).Should().Be(true);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.AgreementRejected);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.AgreementRejected);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void AcceptGrantAgreement_Success()
		{
			// Arrange
			AppDateTime.SetNow(DateTime.UtcNow);
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var assessor = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, assessor, ApplicationStateInternal.OfferIssued);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), applicationAdministrator);

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.AcceptGrantAgreement();

			// Assert
			grantApplication.GrantAgreement.DateAccepted.Should().Be(AppDateTime.UtcNow);
			grantApplication.Assessor.Should().Be(assessor);
			grantApplication.AssessorId.Should().Be(assessor.Id);
			grantApplication.IsUnderAssessment().Should().Be(false);
			grantApplication.IsApplicationAssessor(assessor).Should().Be(true);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.AgreementAccepted);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Approved);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void CancelAgreementHolder_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var assessor = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(applicationAdministrator, assessor, ApplicationStateInternal.AgreementAccepted);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), applicationAdministrator);

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.CancelAgreementHolder("Reason");

			// Assert
			grantApplication.Assessor.Should().Be(assessor);
			grantApplication.AssessorId.Should().Be(assessor.Id);
			grantApplication.IsUnderAssessment().Should().Be(false);
			grantApplication.IsApplicationAssessor(assessor).Should().Be(true);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.CancelledByAgreementHolder);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.CancelledByAgreementHolder);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void CancelAgreementMinistry_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var director = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(applicationAdministrator, director, ApplicationStateInternal.AgreementAccepted);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), director, "Director");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.CancelAgreementMinistry("Reason");

			// Assert
			grantApplication.Assessor.Should().Be(director);
			grantApplication.AssessorId.Should().Be(director.Id);
			grantApplication.IsUnderAssessment().Should().Be(false);
			grantApplication.IsApplicationAssessor(director).Should().Be(true);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.CancelledByMinistry);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.CancelledByMinistry);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void SubmitChangeRequest_FromAgreementAccepted_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var assessor = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, assessor, ApplicationStateInternal.AgreementAccepted);
			EntityHelper.CreateTrainingProviderRequest(grantApplication);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), applicationAdministrator);

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.SubmitChangeRequest("Reason");

			// Assert
			grantApplication.Assessor.Should().Be(assessor);
			grantApplication.AssessorId.Should().Be(assessor.Id);
			grantApplication.IsUnderAssessment().Should().Be(false);
			grantApplication.IsApplicationAssessor(assessor).Should().Be(true);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.ChangeRequest);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.ChangeRequestSubmitted);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void RecommendChangeForApproval_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var assessor = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, assessor, ApplicationStateInternal.ChangeRequest);
			EntityHelper.CreateTrainingProviderRequest(grantApplication);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), assessor, "Assessor");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.RecommendChangeForApproval();

			// Assert
			grantApplication.Assessor.Should().Be(assessor);
			grantApplication.AssessorId.Should().Be(assessor.Id);
			grantApplication.IsUnderAssessment().Should().Be(false);
			grantApplication.IsApplicationAssessor(assessor).Should().Be(true);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.ChangeForApproval);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.ChangeRequestSubmitted);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void RecommendChangeForDenial_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var assessor = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, assessor, ApplicationStateInternal.ChangeRequest);
			EntityHelper.CreateTrainingProviderRequest(grantApplication);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), assessor, "Assessor");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.RecommendChangeForDenial("Reason");

			// Assert
			grantApplication.Assessor.Should().Be(assessor);
			grantApplication.AssessorId.Should().Be(assessor.Id);
			grantApplication.IsUnderAssessment().Should().Be(false);
			grantApplication.IsApplicationAssessor(assessor).Should().Be(true);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.ChangeForDenial);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.ChangeRequestSubmitted);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void ReturnChangeToAssessment_FromChangeForApproval_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var director = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, director, ApplicationStateInternal.ChangeForApproval);
			EntityHelper.CreateTrainingProviderRequest(grantApplication);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), director, "Director");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.ReturnChangeToAssessment("Reason");

			// Assert
			grantApplication.Assessor.Should().Be(director);
			grantApplication.AssessorId.Should().Be(director.Id);
			grantApplication.IsUnderAssessment().Should().Be(false);
			grantApplication.IsApplicationAssessor(director).Should().Be(true);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.ChangeReturned);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.ChangeRequestSubmitted);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void ReturnChangeToAssessment_FromChangeForDenial_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var director = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, director, ApplicationStateInternal.ChangeForDenial);
			EntityHelper.CreateTrainingProviderRequest(grantApplication);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), director, "Director");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.ReturnChangeToAssessment("Reason");

			// Assert
			grantApplication.Assessor.Should().Be(director);
			grantApplication.AssessorId.Should().Be(director.Id);
			grantApplication.IsUnderAssessment().Should().Be(false);
			grantApplication.IsApplicationAssessor(director).Should().Be(true);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.ChangeReturned);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.ChangeRequestSubmitted);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void ApproveChangeRequest_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var director = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, director, ApplicationStateInternal.ChangeForApproval);
			EntityHelper.CreateTrainingProviderRequest(grantApplication);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), director, "Director");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.ApproveChangeRequest();

			// Assert
			grantApplication.Assessor.Should().Be(director);
			grantApplication.AssessorId.Should().Be(director.Id);
			grantApplication.IsUnderAssessment().Should().Be(false);
			grantApplication.IsApplicationAssessor(director).Should().Be(true);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.AgreementAccepted);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.ChangeRequestApproved);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void DenyChangeRequest_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var director = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, director, ApplicationStateInternal.ChangeForDenial);
			EntityHelper.CreateTrainingProviderRequest(grantApplication);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), director, "Director");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.DenyChangeRequest("Reason");

			// Assert
			grantApplication.Assessor.Should().Be(director);
			grantApplication.AssessorId.Should().Be(director.Id);
			grantApplication.IsUnderAssessment().Should().Be(false);
			grantApplication.IsApplicationAssessor(director).Should().Be(true);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.ChangeRequestDenied);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.ChangeRequestDenied);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void SubmitClaim_Success()
		{
			// Arrange
			AppDateTime.SetNow(DateTime.UtcNow);
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var assessor = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, assessor, ApplicationStateInternal.AgreementAccepted);
			grantApplication.InvitationKey = Guid.NewGuid();
			grantApplication.ParticipantForms.Add(new ParticipantForm(grantApplication, Guid.NewGuid()));
			EntityHelper.CreateTrainingProviderRequest(grantApplication);
			var claim = EntityHelper.CreateClaim(grantApplication);
			claim.ClaimState = ClaimState.Complete;
			grantApplication.Claims.Add(claim);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), applicationAdministrator);

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.SubmitClaim(claim);

			// Assert
			claim.DateSubmitted.Should().Be(AppDateTime.UtcNow);
			grantApplication.Assessor.Should().Be(assessor);
			grantApplication.AssessorId.Should().Be(assessor.Id);
			grantApplication.IsUnderAssessment().Should().Be(false);
			grantApplication.IsApplicationAssessor(assessor).Should().Be(true);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.NewClaim);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.ClaimSubmitted);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void SelectClaimForAssessment_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var assessor = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, assessor, ApplicationStateInternal.NewClaim);
			EntityHelper.CreateTrainingProviderRequest(grantApplication);
			var claim = EntityHelper.CreateClaim(grantApplication);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), assessor, "Assessor");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.SelectClaimForAssessment(claim);

			// Assert
			grantApplication.Assessor.Should().Be(assessor);
			grantApplication.AssessorId.Should().Be(assessor.Id);
			grantApplication.IsUnderAssessment().Should().Be(false);
			grantApplication.IsApplicationAssessor(assessor).Should().Be(true);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.ClaimAssessEligibility);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.ClaimSubmitted);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void RemoveClaimFromAssessment_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var assessor = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, assessor, ApplicationStateInternal.ClaimAssessEligibility);
			EntityHelper.CreateTrainingProviderRequest(grantApplication);
			var claim = EntityHelper.CreateClaim(grantApplication);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), assessor, "Assessor");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.RemoveClaimFromAssessment(claim);

			// Assert
			grantApplication.Assessor.Should().Be(assessor);
			grantApplication.AssessorId.Should().Be(assessor.Id);
			grantApplication.IsUnderAssessment().Should().Be(false);
			grantApplication.IsApplicationAssessor(assessor).Should().Be(true);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.NewClaim);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.ClaimSubmitted);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void AssessClaimReimbursement_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var assessor = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, assessor, ApplicationStateInternal.ClaimAssessEligibility);
			EntityHelper.CreateTrainingProviderRequest(grantApplication);
			var claim = EntityHelper.CreateClaim(grantApplication);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), assessor, "Assessor");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.AssessClaimReimbursement(claim);

			// Assert
			grantApplication.Assessor.Should().BeNull();
			grantApplication.AssessorId.Should().BeNull();
			grantApplication.IsUnderAssessment().Should().Be(false);
			grantApplication.IsApplicationAssessor(assessor).Should().Be(false);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.ClaimAssessReimbursement);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.ClaimSubmitted);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void AssessClaimEligibility_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var director = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, director, ApplicationStateInternal.ClaimAssessReimbursement);
			EntityHelper.CreateTrainingProviderRequest(grantApplication);
			var claim = EntityHelper.CreateClaim(grantApplication);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), director, "Director");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.AssessClaimEligibility(claim);

			// Assert
			grantApplication.Assessor.Should().BeNull();
			grantApplication.AssessorId.Should().BeNull();
			grantApplication.IsUnderAssessment().Should().Be(false);
			grantApplication.IsApplicationAssessor(director).Should().Be(false);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.ClaimAssessEligibility);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.ClaimSubmitted);
		}

		/// <summary>
		/// Approving a claim without assessment notes should throw an <typeparamref name="InvalidOperationException"/>.
		/// </summary>
		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ApproveClaim_MissingClaimAssessmentNotesShouldThrowInvalidOperationException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var assessor = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, assessor, ApplicationStateInternal.ClaimAssessReimbursement);
			EntityHelper.CreateTrainingProviderRequest(grantApplication);
			var claim = EntityHelper.CreateClaim(grantApplication);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), assessor, "Director");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act & Assert (Handled by decorator)
			stateMachine.DenyClaim(claim);
		}

		/// <summary>
		/// Approving a claim with an assessment note should succeed.
		/// </summary>
		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void ApproveClaim_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var director = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, director, ApplicationStateInternal.ClaimAssessReimbursement);
			EntityHelper.CreateTrainingProviderRequest(grantApplication);
			var claim = EntityHelper.CreateClaim(grantApplication);
			claim.ClaimAssessmentNotes = "test";
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), director, "Director");

			helper.MockDbSet<Claim>(claim);

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.ApproveClaim(claim);

			// Assert
			grantApplication.Assessor.Should().Be(director);
			grantApplication.AssessorId.Should().Be(director.Id);
			grantApplication.IsUnderAssessment().Should().Be(false);
			grantApplication.IsApplicationAssessor(director).Should().Be(true);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.ClaimApproved);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.ClaimApproved);
		}

		/// <summary>
		/// Denying a claim without assessment notes should throw an <typeparamref name="InvalidOperationException"/>.
		/// </summary>
		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		[ExpectedException(typeof(InvalidOperationException))]
		public void DenyClaim_MissingClaimAssessmentNotesShouldThrowInvalidOperationException()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var assessor = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, assessor, ApplicationStateInternal.ClaimAssessReimbursement);
			EntityHelper.CreateTrainingProviderRequest(grantApplication);
			var claim = EntityHelper.CreateClaim(grantApplication);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), assessor, "Director");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act & Assert (Handled by decorator)
			stateMachine.DenyClaim(claim);
		}

		/// <summary>
		/// Denying a claim with an assessment note should succeed.
		/// </summary>
		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void DenyClaim_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var assessor = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, assessor, ApplicationStateInternal.ClaimAssessReimbursement);
			EntityHelper.CreateTrainingProviderRequest(grantApplication);
			var claim = EntityHelper.CreateClaim(grantApplication);
			claim.ClaimAssessmentNotes = "test";
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), assessor, "Director");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.DenyClaim(claim);

			// Assert
			grantApplication.Assessor.Should().BeNull();
			grantApplication.AssessorId.Should().BeNull();
			grantApplication.IsUnderAssessment().Should().Be(false);
			grantApplication.IsApplicationAssessor(assessor).Should().Be(false);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.ClaimDenied);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.ClaimDenied);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void ReturnClaimToApplicant_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var assessor = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, assessor, ApplicationStateInternal.ClaimAssessEligibility);
			EntityHelper.CreateTrainingProviderRequest(grantApplication);
			var claim = EntityHelper.CreateClaim(grantApplication);
			claim.ClaimAssessmentNotes = "reason";
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), assessor, "Assessor");
			helper.InitializeMockFor<ClaimService>();
			var claimServiceMock = helper.GetMock<IClaimService>();

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.ReturnClaimToApplicant(claim, claim.ClaimAssessmentNotes, claimServiceMock.Object);

			// Assert
			grantApplication.Assessor.Should().Be(assessor);
			grantApplication.AssessorId.Should().Be(assessor.Id);
			grantApplication.IsUnderAssessment().Should().Be(false);
			grantApplication.IsApplicationAssessor(assessor).Should().Be(true);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.ClaimReturnedToApplicant);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.ClaimReturned);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine")]
		public void WithdrawClaim_Success()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var user = EntityHelper.CreateInternalUser();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, user, ApplicationStateInternal.ClaimAssessEligibility);
			grantApplication.Id = 1;
			EntityHelper.CreateTrainingProviderRequest(grantApplication);
			var claim = EntityHelper.CreateClaim(grantApplication);
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), applicationAdministrator);
			helper.InitializeMockFor<ClaimService>();
			var claimServiceMock = helper.GetMock<IClaimService>();
			var stateChangeMock = helper.MockDbSet<GrantApplicationStateChange>();

			var grantOpening = grantApplication.GrantOpening;
			var originalClaimCount = grantOpening.GrantOpeningFinancial.CurrentClaimCount;
			var originalClaimAmount = grantOpening.GrantOpeningFinancial.CurrentClaims;
			var originalOutstandingCommitmentCount = grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount;
			var originalOutstandingCommitmentAmount = grantOpening.GrantOpeningFinancial.OutstandingCommitments;
			var agreedCommitment = grantApplication.GetAgreedCommitment();
			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);
			var grantAppMock = helper.MockDbSet(grantApplication);

			// State Change
			helper.SetMockAs<GrantApplicationService, IGrantApplicationService>().CallBase = true;
			var service = helper.CreateMock<GrantApplicationService>().As<IGrantApplicationService>();
			var stateChangeReason = $"Applicant withdrew claim number {claim.ClaimVersion} for reason: Unit Test";
			stateChangeMock.Object.Add(EntityHelper.CreateStateChangeLogEntry(grantApplication, ApplicationStateInternal.ClaimAssessEligibility, grantApplication.ApplicationStateInternal, applicationAdministrator, user, stateChangeReason));
			stateChangeMock.Object.First().ToState = ApplicationStateInternal.ClaimReturnedToApplicant;
			helper.GetMock<IDataContext>().Setup(x => x.GrantApplicationStateChanges).Returns(stateChangeMock.Object);

			helper.InitializeMockFor<GrantApplicationService>();
			helper.Create<GrantApplicationService>();

			grantAppMock.Object.First().StateChanges.Add(stateChangeMock.Object.First());

			// Act
			stateMachine.WithdrawClaim(claim, "Unit Test", claimServiceMock.Object);

			// Assert
			grantApplication.Assessor.Should().Be(user);
			grantApplication.AssessorId.Should().Be(user.Id);
			grantApplication.IsUnderAssessment().Should().Be(false);
			grantApplication.IsApplicationAssessor(user).Should().Be(true);
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.ClaimReturnedToApplicant);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.ClaimReturned);
			grantOpening.GrantOpeningFinancial.CurrentClaimCount.Equals(originalClaimCount - 1);
			grantOpening.GrantOpeningFinancial.CurrentClaims.Equals(originalClaimAmount - claim.TotalClaimReimbursement);
			grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount.Equals(originalOutstandingCommitmentCount + 1);
			grantOpening.GrantOpeningFinancial.OutstandingCommitments.Equals(originalOutstandingCommitmentAmount + agreedCommitment);

			var stateChange = service.Object.GetStateChange(grantApplication.Id, ApplicationStateInternal.ClaimReturnedToApplicant);
			stateChange.Should().NotBeNull();
			stateChange.Reason.Equals(stateChangeReason);
		}
		#endregion

		#region Grant Opening Intake Tests
		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine"), TestCategory("Intake")]
		public void GrantOpeningIntake_SubmitApplication()
		{
			// Arrange
			AppDateTime.SetNow(DateTime.UtcNow);
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var grantOpening = EntityHelper.CreateGrantOpening();
			grantOpening.State = GrantOpeningStates.Open;
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(grantOpening, applicationAdministrator, ApplicationStateInternal.Draft);
			grantApplication.TrainingCost.TrainingCostState = TrainingCostStates.Complete;
			var estimatedReimbursement = grantApplication.TrainingCost.TotalEstimatedReimbursement;
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), applicationAdministrator);
			helper.MockDbSet(grantOpening);
			helper.SetMockAs<GrantOpeningService, IGrantOpeningService>().CallBase = true;

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.SubmitApplication();

			// Assert
			grantOpening.GrantOpeningIntake.NewCount.Should().Be(1);
			grantOpening.GrantOpeningIntake.NewAmt.Should().Be(estimatedReimbursement);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine"), TestCategory("Intake")]
		public void GrantOpeningIntake_WithdrawApplicationAfterSubmit()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var grantOpening = EntityHelper.CreateGrantOpening();
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(grantOpening, applicationAdministrator, ApplicationStateInternal.New);
			grantApplication.DateSubmitted = DateTime.UtcNow;
			grantOpening.GrantOpeningIntake.NewCount = 1;
			grantOpening.GrantOpeningIntake.NewAmt = grantApplication.TrainingCost.TotalEstimatedReimbursement;

			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), applicationAdministrator);
			helper.MockDbSet(grantOpening);
			helper.SetMockAs<GrantOpeningService, IGrantOpeningService>().CallBase = true;

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.WithdrawApplication("reason");

			// Assert
			grantOpening.GrantOpeningIntake.NewCount.Should().Be(0);
			grantOpening.GrantOpeningIntake.NewAmt.Should().Be(0);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine"), TestCategory("Intake")]
		public void GrantOpeningIntake_ReturnUnfundedApplications()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var director = EntityHelper.CreateInternalUser();
			var grantOpening = EntityHelper.CreateGrantOpening();
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(grantOpening, applicationAdministrator, ApplicationStateInternal.New);
			grantApplication.DateSubmitted = DateTime.UtcNow;
			grantOpening.GrantOpeningIntake.NewCount = 1;
			grantOpening.GrantOpeningIntake.NewAmt = grantApplication.TrainingCost.TotalEstimatedReimbursement;

			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), director, "Director");
			helper.MockDbSet(grantOpening);
			helper.SetMockAs<GrantOpeningService, IGrantOpeningService>().CallBase = true;

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.ReturnUnfundedApplications();

			// Assert
			grantOpening.GrantOpeningIntake.NewCount.Should().Be(0);
			grantOpening.GrantOpeningIntake.NewAmt.Should().Be(0);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine"), TestCategory("Intake")]
		public void GrantOpeningIntake_SelectForAssessment()
		{
			// Arrange
			var assessor = EntityHelper.CreateInternalUser();
			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), assessor, "Assessor");
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var grantOpening = EntityHelper.CreateGrantOpening();
			grantOpening.Id = 1;
			var trainingPeriod = new TrainingPeriod() { Id = 1 };
			grantOpening.TrainingPeriod = trainingPeriod;
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(grantOpening, applicationAdministrator, ApplicationStateInternal.New);
			grantApplication.GrantOpening = grantOpening;
			grantApplication.ApplicationStateInternal = ApplicationStateInternal.New;
			var estimatedReimbursement = grantApplication.TrainingCost.TotalEstimatedReimbursement;
			grantApplication.DateSubmitted = DateTime.UtcNow;
			grantOpening.GrantOpeningIntake.NewCount = 1;
			grantOpening.GrantOpeningIntake.NewAmt = estimatedReimbursement;
			//var newGrantOpening = EntityHelper.CreateGrantOpening();
			//newGrantOpening.GrantOpeningIntake.NewCount = 0;
			//newGrantOpening.GrantOpeningIntake.NewAmt = 0;
			//newGrantOpening.GrantOpeningIntake.PendingAssessmentCount = 1;
			//newGrantOpening.GrantOpeningIntake.PendingAssessmentAmt = estimatedReimbursement;
			//newGrantOpening.GrantOpeningFinancial.CurrentReservations = estimatedReimbursement;

			var dbContextMock = helper.GetMock<IDataContext>();
			var dbSetMock = helper.MockDbSet<GrantOpening>(grantOpening);
			dbContextMock.Setup(x => x.GrantOpenings).Returns(dbSetMock.Object);
			var dbSetMock2 = helper.MockDbSet(grantApplication);
			dbContextMock.Setup(x => x.GrantApplications).Returns(dbSetMock2.Object);
			helper.InitializeMockFor<GrantOpeningService>();
			helper.Create<GrantOpeningService>();

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.SelectForAssessment();

			// Assert
			grantOpening.GrantOpeningIntake.NewCount.Should().Be(1);
			//grantOpening.GrantOpeningIntake.NewAmt.Should().Be(6666.67M);
			grantOpening.GrantOpeningIntake.PendingAssessmentCount.Should().Be(0);
			grantOpening.GrantOpeningIntake.PendingAssessmentAmt.Should().Be(0);
			grantOpening.GrantOpeningFinancial.CurrentReservations.Should().Be(0);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine"), TestCategory("Intake")]
		public void GrantOpeningIntake_WithdrawApplicationAfterSelectForAssessment()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var grantOpening = EntityHelper.CreateGrantOpening();
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(grantOpening, applicationAdministrator, ApplicationStateInternal.PendingAssessment);
			var estimatedReimbursement = grantApplication.TrainingCost.TotalEstimatedReimbursement;
			grantApplication.DateSubmitted = DateTime.UtcNow;
			grantOpening.GrantOpeningIntake.PendingAssessmentCount = 1;
			grantOpening.GrantOpeningIntake.PendingAssessmentAmt = estimatedReimbursement;
			grantOpening.GrantOpeningFinancial.CurrentReservations = estimatedReimbursement;

			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), applicationAdministrator);
			helper.MockDbSet(grantOpening);
			helper.SetMockAs<GrantOpeningService, IGrantOpeningService>().CallBase = true;

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.WithdrawApplication("reason");

			// Assert
			grantOpening.GrantOpeningIntake.PendingAssessmentCount.Should().Be(0);
			grantOpening.GrantOpeningIntake.PendingAssessmentAmt.Should().Be(0);
			grantOpening.GrantOpeningIntake.WithdrawnCount.Should().Be(1);
			grantOpening.GrantOpeningIntake.WithdrawnAmt.Should().Be(estimatedReimbursement);
			grantOpening.GrantOpeningFinancial.CurrentReservations.Should().Be(0);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine"), TestCategory("Intake")]
		public void GrantOpeningIntake_RemoveFromAssessment()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var assessor = EntityHelper.CreateInternalUser();
			var grantOpening = EntityHelper.CreateGrantOpening();
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(grantOpening, applicationAdministrator, ApplicationStateInternal.PendingAssessment);
			var estimatedReimbursement = grantApplication.TrainingCost.TotalEstimatedReimbursement;
			grantApplication.DateSubmitted = DateTime.UtcNow;
			grantOpening.GrantOpeningIntake.PendingAssessmentCount = 1;
			grantOpening.GrantOpeningIntake.PendingAssessmentAmt = estimatedReimbursement;
			grantOpening.GrantOpeningFinancial.CurrentReservations = estimatedReimbursement;

			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), assessor, "Assessor");
			helper.MockDbSet(grantOpening);
			var applications = helper.MockDbSet(grantApplication);
			helper.SetMockAs<GrantOpeningService, IGrantOpeningService>().CallBase = true;

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.RemoveFromAssessment();

			// Assert
			grantOpening.GrantOpeningIntake.PendingAssessmentCount.Should().Be(0);
			grantOpening.GrantOpeningIntake.PendingAssessmentAmt.Should().Be(0);
			grantOpening.GrantOpeningFinancial.CurrentReservations.Should().Be(0);
			grantOpening.GrantOpeningIntake.NewCount.Should().Be(1);
			grantOpening.GrantOpeningIntake.NewAmt.Should().Be(estimatedReimbursement);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine"), TestCategory("Intake")]
		public void GrantOpeningIntake_BeginAssessment()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var assessor = EntityHelper.CreateInternalUser();
			var grantOpening = EntityHelper.CreateGrantOpening();
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(grantOpening, applicationAdministrator, ApplicationStateInternal.PendingAssessment);
			var estimatedReimbursement = grantApplication.TrainingCost.TotalEstimatedReimbursement;
			grantApplication.DateSubmitted = DateTime.UtcNow;
			grantOpening.GrantOpeningIntake.PendingAssessmentCount = 1;
			grantOpening.GrantOpeningIntake.PendingAssessmentAmt = estimatedReimbursement;
			grantOpening.GrantOpeningFinancial.CurrentReservations = estimatedReimbursement;

			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), assessor, "Assessor");
			helper.MockDbSet(grantOpening);
			var applications = helper.MockDbSet(grantApplication);
			helper.SetMockAs<GrantOpeningService, IGrantOpeningService>().CallBase = true;

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.BeginAssessment(assessor);

			// Assert
			grantOpening.GrantOpeningIntake.PendingAssessmentCount.Should().Be(0);
			grantOpening.GrantOpeningIntake.PendingAssessmentAmt.Should().Be(0);
			grantOpening.GrantOpeningIntake.UnderAssessmentCount.Should().Be(1);
			grantOpening.GrantOpeningIntake.UnderAssessmentAmt.Should().Be(estimatedReimbursement);
			grantOpening.GrantOpeningFinancial.CurrentReservations.Should().Be(estimatedReimbursement);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine"), TestCategory("Intake")]
		public void GrantOpeningIntake_WithdrawApplicationAfterBeginAssessment()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var grantOpening = EntityHelper.CreateGrantOpening();
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(grantOpening, applicationAdministrator, ApplicationStateInternal.UnderAssessment);
			var estimatedReimbursement = grantApplication.TrainingCost.TotalEstimatedReimbursement;
			grantApplication.DateSubmitted = DateTime.UtcNow;
			grantOpening.GrantOpeningIntake.UnderAssessmentCount = 1;
			grantOpening.GrantOpeningIntake.UnderAssessmentAmt = estimatedReimbursement;
			grantOpening.GrantOpeningFinancial.CurrentReservations = estimatedReimbursement;

			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), applicationAdministrator);
			helper.MockDbSet(grantOpening);
			helper.SetMockAs<GrantOpeningService, IGrantOpeningService>().CallBase = true;

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.WithdrawApplication("reason");

			// Assert
			grantOpening.GrantOpeningIntake.UnderAssessmentCount.Should().Be(0);
			grantOpening.GrantOpeningIntake.UnderAssessmentAmt.Should().Be(0);
			grantOpening.GrantOpeningIntake.WithdrawnCount.Should().Be(1);
			grantOpening.GrantOpeningIntake.WithdrawnAmt.Should().Be(estimatedReimbursement);
			grantOpening.GrantOpeningFinancial.CurrentReservations.Should().Be(0);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine"), TestCategory("Intake")]
		public void GrantOpeningIntake_WithdrawApplicationAfterRecommendForDenial()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var grantOpening = EntityHelper.CreateGrantOpening();
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(grantOpening, applicationAdministrator, ApplicationStateInternal.RecommendedForDenial);
			var estimatedReimbursement = grantApplication.TrainingCost.TotalEstimatedReimbursement;
			grantApplication.DateSubmitted = DateTime.UtcNow;
			grantOpening.GrantOpeningIntake.UnderAssessmentCount = 1;
			grantOpening.GrantOpeningIntake.UnderAssessmentAmt = estimatedReimbursement;
			grantOpening.GrantOpeningFinancial.CurrentReservations = estimatedReimbursement;

			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), applicationAdministrator);
			helper.MockDbSet(grantOpening);
			helper.SetMockAs<GrantOpeningService, IGrantOpeningService>().CallBase = true;

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.WithdrawApplication("reason");

			// Assert
			grantOpening.GrantOpeningIntake.UnderAssessmentCount.Should().Be(0);
			grantOpening.GrantOpeningIntake.UnderAssessmentAmt.Should().Be(0);
			grantOpening.GrantOpeningIntake.WithdrawnCount.Should().Be(1);
			grantOpening.GrantOpeningIntake.WithdrawnAmt.Should().Be(estimatedReimbursement);
			grantOpening.GrantOpeningFinancial.CurrentReservations.Should().Be(0);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine"), TestCategory("Intake")]
		public void GrantOpeningIntake_WithdrawApplicationAfterRecommendForApproval()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var grantOpening = EntityHelper.CreateGrantOpening();
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(grantOpening, applicationAdministrator, ApplicationStateInternal.RecommendedForApproval);
			var estimatedReimbursement = grantApplication.TrainingCost.TotalEstimatedReimbursement;
			grantApplication.DateSubmitted = DateTime.UtcNow;
			grantOpening.GrantOpeningIntake.UnderAssessmentCount = 1;
			grantOpening.GrantOpeningIntake.UnderAssessmentAmt = estimatedReimbursement;
			grantOpening.GrantOpeningFinancial.CurrentReservations = estimatedReimbursement;

			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), applicationAdministrator);
			helper.MockDbSet(grantOpening);
			helper.SetMockAs<GrantOpeningService, IGrantOpeningService>().CallBase = true;

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.WithdrawApplication("reason");

			// Assert
			grantOpening.GrantOpeningIntake.UnderAssessmentCount.Should().Be(0);
			grantOpening.GrantOpeningIntake.UnderAssessmentAmt.Should().Be(0);
			grantOpening.GrantOpeningIntake.WithdrawnCount.Should().Be(1);
			grantOpening.GrantOpeningIntake.WithdrawnAmt.Should().Be(estimatedReimbursement);
			grantOpening.GrantOpeningFinancial.CurrentReservations.Should().Be(0);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine"), TestCategory("Intake")]
		public void GrantOpeningIntake_WithdrawApplicationAfterReturnToAssessment()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var grantOpening = EntityHelper.CreateGrantOpening();
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(grantOpening, applicationAdministrator, ApplicationStateInternal.ReturnedToAssessment);
			var estimatedReimbursement = grantApplication.TrainingCost.TotalEstimatedReimbursement;
			grantApplication.DateSubmitted = DateTime.UtcNow;
			grantOpening.GrantOpeningIntake.UnderAssessmentCount = 1;
			grantOpening.GrantOpeningIntake.UnderAssessmentAmt = estimatedReimbursement;
			grantOpening.GrantOpeningFinancial.CurrentReservations = estimatedReimbursement;

			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), applicationAdministrator);
			helper.MockDbSet(grantOpening);
			helper.SetMockAs<GrantOpeningService, IGrantOpeningService>().CallBase = true;

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.WithdrawApplication("reason");

			// Assert
			grantOpening.GrantOpeningIntake.UnderAssessmentCount.Should().Be(0);
			grantOpening.GrantOpeningIntake.UnderAssessmentAmt.Should().Be(0);
			grantOpening.GrantOpeningIntake.WithdrawnCount.Should().Be(1);
			grantOpening.GrantOpeningIntake.WithdrawnAmt.Should().Be(grantApplication.TrainingCost.TotalEstimatedReimbursement);
			grantOpening.GrantOpeningFinancial.CurrentReservations.Should().Be(0);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine"), TestCategory("Intake")]
		public void GrantOpeningIntake_DenyApplication()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var director = EntityHelper.CreateInternalUser();
			var grantOpening = EntityHelper.CreateGrantOpening();
			var grantApplication = EntityHelper.CreateGrantApplicationWithCosts(grantOpening, applicationAdministrator, ApplicationStateInternal.RecommendedForDenial);
			var estimatedReimbursement = grantApplication.TrainingCost.TotalEstimatedReimbursement;
			grantApplication.DateSubmitted = DateTime.UtcNow;
			grantApplication.CopyEstimatedIntoAgreed();
			grantOpening.GrantOpeningIntake.UnderAssessmentCount = 1;
			grantOpening.GrantOpeningIntake.UnderAssessmentAmt = estimatedReimbursement;
			grantOpening.GrantOpeningFinancial.CurrentReservations = estimatedReimbursement;

			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), director, "Director");
			helper.MockDbSet(grantOpening);
			var applications = helper.MockDbSet(grantApplication);
			helper.SetMockAs<GrantOpeningService, IGrantOpeningService>().CallBase = true;

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.DenyApplication("Reason");

			// Assert
			grantOpening.GrantOpeningIntake.UnderAssessmentCount.Should().Be(0);
			grantOpening.GrantOpeningIntake.UnderAssessmentAmt.Should().Be(0);
			grantOpening.GrantOpeningIntake.DeniedCount.Should().Be(1);
			grantOpening.GrantOpeningIntake.DeniedAmt.Should().Be(estimatedReimbursement);
			grantOpening.GrantOpeningFinancial.CurrentReservations.Should().Be(0);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine"), TestCategory("Intake")]
		public void GrantOpeningIntake_IssueOffer()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var director = EntityHelper.CreateInternalUser();
			var grantOpening = EntityHelper.CreateGrantOpening();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(grantOpening, applicationAdministrator, director, ApplicationStateInternal.RecommendedForApproval);
			var estimatedReimbursement = grantApplication.TrainingCost.TotalEstimatedReimbursement;
			grantApplication.DateSubmitted = DateTime.UtcNow;
			grantApplication.CopyEstimatedIntoAgreed();
			grantOpening.GrantOpeningIntake.UnderAssessmentCount = 1;
			grantOpening.GrantOpeningIntake.UnderAssessmentAmt = estimatedReimbursement;
			grantOpening.GrantOpeningFinancial.CurrentReservations = estimatedReimbursement;

			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), director, "Director");
			helper.MockDbSet(grantOpening);
			var applications = helper.MockDbSet(grantApplication);
			helper.SetMockAs<GrantOpeningService, IGrantOpeningService>().CallBase = true;

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.IssueOffer();

			// Assert
			grantOpening.GrantOpeningIntake.UnderAssessmentCount.Should().Be(1);
			grantOpening.GrantOpeningIntake.UnderAssessmentAmt.Should().Be(estimatedReimbursement);
			grantOpening.GrantOpeningFinancial.CurrentReservations.Should().Be(estimatedReimbursement);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine"), TestCategory("Intake")]
		public void GrantOpeningIntake_WithdrawOffer()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var director = EntityHelper.CreateInternalUser();
			var grantOpening = EntityHelper.CreateGrantOpening();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(grantOpening, applicationAdministrator, director, ApplicationStateInternal.OfferIssued);
			var estimatedReimbursement = grantApplication.TrainingCost.TotalEstimatedReimbursement;
			grantApplication.DateSubmitted = DateTime.UtcNow;
			grantApplication.CopyEstimatedIntoAgreed();
			grantOpening.GrantOpeningIntake.UnderAssessmentCount = 1;
			grantOpening.GrantOpeningIntake.UnderAssessmentAmt = estimatedReimbursement;
			grantOpening.GrantOpeningFinancial.CurrentReservations = estimatedReimbursement;

			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), director, "Director");
			helper.MockDbSet(grantOpening);
			var applications = helper.MockDbSet(grantApplication);
			helper.SetMockAs<GrantOpeningService, IGrantOpeningService>().CallBase = true;

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.WithdrawOffer("Reason");

			// Assert
			grantOpening.GrantOpeningIntake.UnderAssessmentCount.Should().Be(0);
			grantOpening.GrantOpeningIntake.UnderAssessmentAmt.Should().Be(0);
			grantOpening.GrantOpeningIntake.WithdrawnCount.Should().Be(1);
			grantOpening.GrantOpeningIntake.WithdrawnAmt.Should().Be(estimatedReimbursement);
			grantOpening.GrantOpeningFinancial.CurrentReservations.Should().Be(0);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine"), TestCategory("Intake")]
		public void GrantOpeningIntake_RejectGrantAgreement()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var assessor = EntityHelper.CreateInternalUser();
			var grantOpening = EntityHelper.CreateGrantOpening();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(grantOpening, applicationAdministrator, assessor, ApplicationStateInternal.OfferIssued);
			var estimatedReimbursement = grantApplication.TrainingCost.TotalEstimatedReimbursement;
			grantApplication.DateSubmitted = DateTime.UtcNow;
			grantApplication.CopyEstimatedIntoAgreed();
			grantOpening.GrantOpeningIntake.UnderAssessmentCount = 1;
			grantOpening.GrantOpeningIntake.UnderAssessmentAmt = estimatedReimbursement;
			grantOpening.GrantOpeningFinancial.CurrentReservations = estimatedReimbursement;

			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), applicationAdministrator);
			helper.MockDbSet(grantOpening);
			var applications = helper.MockDbSet(grantApplication);
			helper.SetMockAs<GrantOpeningService, IGrantOpeningService>().CallBase = true;

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.RejectGrantAgreement("Reason");

			// Assert
			grantOpening.GrantOpeningIntake.UnderAssessmentCount.Should().Be(0);
			grantOpening.GrantOpeningIntake.UnderAssessmentAmt.Should().Be(0);
			grantOpening.GrantOpeningIntake.WithdrawnCount.Should().Be(1);
			grantOpening.GrantOpeningIntake.WithdrawnAmt.Should().Be(estimatedReimbursement);
			grantOpening.GrantOpeningFinancial.CurrentReservations.Should().Be(0);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine"), TestCategory("Intake")]
		public void GrantOpeningIntake_AcceptGrantAgreement()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var assessor = EntityHelper.CreateInternalUser();
			var grantOpening = EntityHelper.CreateGrantOpening();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(grantOpening, applicationAdministrator, assessor, ApplicationStateInternal.OfferIssued);
			var estimatedReimbursement = grantApplication.TrainingCost.TotalEstimatedReimbursement;
			var agreedReimbursement = grantApplication.CalculateAgreedMaxReimbursement();
			grantApplication.DateSubmitted = DateTime.UtcNow;
			grantOpening.GrantOpeningIntake.UnderAssessmentCount = 1;
			grantOpening.GrantOpeningIntake.UnderAssessmentAmt = estimatedReimbursement;
			grantOpening.GrantOpeningFinancial.CurrentReservations = estimatedReimbursement;

			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), applicationAdministrator);
			helper.MockDbSet(grantOpening);
			var applications = helper.MockDbSet(grantApplication);
			helper.SetMockAs<GrantOpeningService, IGrantOpeningService>().CallBase = true;

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.AcceptGrantAgreement();

			// Assert
			grantOpening.GrantOpeningIntake.UnderAssessmentCount.Should().Be(0);
			grantOpening.GrantOpeningIntake.UnderAssessmentAmt.Should().Be(0);
			grantOpening.GrantOpeningIntake.ReductionsAmt.Should().Be(estimatedReimbursement - agreedReimbursement);
			grantOpening.GrantOpeningFinancial.CurrentReservations.Should().Be(0);
			grantOpening.GrantOpeningFinancial.AssessedCommitmentsCount.Should().Be(1);
			grantOpening.GrantOpeningFinancial.AssessedCommitments.Should().Be(agreedReimbursement);
			grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount.Should().Be(1);
			grantOpening.GrantOpeningFinancial.OutstandingCommitments.Should().Be(agreedReimbursement);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine"), TestCategory("Intake")]
		public void GrantOpeningIntake_CancelAgreementHolder()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var assessor = EntityHelper.CreateInternalUser();
			var grantOpening = EntityHelper.CreateGrantOpening();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(grantOpening, applicationAdministrator, assessor, ApplicationStateInternal.AgreementAccepted);
			var estimatedReimbursement = grantApplication.TrainingCost.TotalEstimatedReimbursement;
			var agreedReimbursement = grantApplication.CalculateAgreedMaxReimbursement();
			grantApplication.DateSubmitted = DateTime.UtcNow;
			grantOpening.GrantOpeningIntake.ReductionsAmt = estimatedReimbursement - agreedReimbursement;
			grantOpening.GrantOpeningFinancial.AssessedCommitmentsCount = 1;
			grantOpening.GrantOpeningFinancial.AssessedCommitments = agreedReimbursement;
			grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount = 1;
			grantOpening.GrantOpeningFinancial.OutstandingCommitments = agreedReimbursement;

			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), applicationAdministrator);
			helper.MockDbSet(grantOpening);
			var applications = helper.MockDbSet(grantApplication);
			helper.SetMockAs<GrantOpeningService, IGrantOpeningService>().CallBase = true;

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.CancelAgreementHolder("Reason");

			// Assert
			grantOpening.GrantOpeningIntake.ReductionsAmt.Should().Be(estimatedReimbursement - agreedReimbursement);
			grantOpening.GrantOpeningFinancial.AssessedCommitmentsCount.Should().Be(1);
			grantOpening.GrantOpeningFinancial.AssessedCommitments.Should().Be(agreedReimbursement);
			grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount.Should().Be(0);
			grantOpening.GrantOpeningFinancial.OutstandingCommitments.Should().Be(0);
			grantOpening.GrantOpeningFinancial.CancellationsCount.Should().Be(1);
			grantOpening.GrantOpeningFinancial.Cancellations.Should().Be(agreedReimbursement);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine"), TestCategory("Intake")]
		public void GrantOpeningIntake_CancelAgreementMinistry()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var director = EntityHelper.CreateInternalUser();
			var grantOpening = EntityHelper.CreateGrantOpening();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(grantOpening, applicationAdministrator, director, ApplicationStateInternal.AgreementAccepted);
			var estimatedReimbursement = grantApplication.TrainingCost.TotalEstimatedReimbursement;
			var agreedReimbursement = grantApplication.CalculateAgreedMaxReimbursement();
			grantApplication.DateSubmitted = DateTime.UtcNow;
			grantOpening.GrantOpeningIntake.ReductionsAmt = estimatedReimbursement - agreedReimbursement;
			grantOpening.GrantOpeningFinancial.AssessedCommitmentsCount = 1;
			grantOpening.GrantOpeningFinancial.AssessedCommitments = agreedReimbursement;
			grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount = 1;
			grantOpening.GrantOpeningFinancial.OutstandingCommitments = agreedReimbursement;

			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), director, "Director");
			helper.MockDbSet(grantOpening);
			var applications = helper.MockDbSet(grantApplication);
			helper.SetMockAs<GrantOpeningService, IGrantOpeningService>().CallBase = true;

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.CancelAgreementMinistry("Reason");

			// Assert
			grantOpening.GrantOpeningIntake.ReductionsAmt.Should().Be(estimatedReimbursement - agreedReimbursement);
			grantOpening.GrantOpeningFinancial.AssessedCommitmentsCount.Should().Be(1);
			grantOpening.GrantOpeningFinancial.AssessedCommitments.Should().Be(agreedReimbursement);
			grantOpening.GrantOpeningFinancial.OutstandingCommitmentCount.Should().Be(0);
			grantOpening.GrantOpeningFinancial.OutstandingCommitments.Should().Be(0);
			grantOpening.GrantOpeningFinancial.CancellationsCount.Should().Be(1);
			grantOpening.GrantOpeningFinancial.Cancellations.Should().Be(agreedReimbursement);
		}

		#endregion


		#region Grant Application Tests
		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine"), TestCategory("Grant Application")]
		public void GrantApplication_CloseGrantFile()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var director = EntityHelper.CreateInternalUser();
			var grantOpening = EntityHelper.CreateGrantOpening();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(grantOpening, applicationAdministrator, director, ApplicationStateInternal.CompletionReporting);
			grantApplication.Claims.Add(new Claim() {
				GrantApplication = grantApplication,
				ClaimState = ClaimState.ClaimApproved
			});


			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), director, "Director");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.CloseGrantFile("Reason");

			// Assert
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Closed);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine"), TestCategory("Grant Application")]
		public void GrantApplication_SubmitCompletionReportToCloseGrantFile()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var director = EntityHelper.CreateInternalUser();
			var grantOpening = EntityHelper.CreateGrantOpening();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(grantOpening, applicationAdministrator, director, ApplicationStateInternal.CompletionReporting);
			grantApplication.Claims.Add(new Claim() {
				GrantApplication = new GrantApplication(),
				ClaimState = ClaimState.ClaimApproved
			});


			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), applicationAdministrator);


			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.SubmitCompletionReportToCloseGrantFile();

			// Assert
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.Closed);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine"), TestCategory("Grant Application")]
		public void GrantApplication_EnableCompletionReporting()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var director = EntityHelper.CreateInternalUser();
			var grantOpening = EntityHelper.CreateGrantOpening();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(grantOpening, applicationAdministrator, director, ApplicationStateInternal.Closed);
			var claim = new Claim() {
				GrantApplication = grantApplication,
				ClaimState = ClaimState.ClaimApproved,
				GrantApplicationId = grantApplication.Id
			};
			grantApplication.Claims.Add(claim);

			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), director, "Director");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.EnableCompletionReporting();

			// Assert
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.ReportCompletion);
		}
		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine"), TestCategory("Grant Application")]
		public void GrantApplication_AmendClaim()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var director = EntityHelper.CreateInternalUser();
			var grantOpening = EntityHelper.CreateGrantOpening();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(grantOpening, applicationAdministrator, director, ApplicationStateInternal.ClaimDenied);

			var claim = new Claim() {
				GrantApplication = grantApplication,
				GrantApplicationId = grantApplication.Id,
				ClaimState = ClaimState.ClaimApproved,
				ClaimVersion = 2,
				ClaimTypeId = ClaimTypes.SingleAmendableClaim
			};
			grantApplication.Claims.Add(claim);
			var newClaim = new Claim();
			Func<GrantApplication, Claim> func = (t) => {
				newClaim = new Claim() { ClaimVersion = (t.GetCurrentClaim().ClaimVersion + 1) };
				return newClaim;
			};

			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), director, "Director");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.AmendClaim(claim, func);

			// Assert
			//claim.ClaimVersion.Should().Be(0);
			newClaim.ClaimVersion.Should().Be(3);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine"), TestCategory("Grant Application")]
		public void GrantApplication_EnableClaimReporting()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var director = EntityHelper.CreateInternalUser();
			var grantOpening = EntityHelper.CreateGrantOpening();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(grantOpening, applicationAdministrator, director, ApplicationStateInternal.Closed);
			var claim = new Claim() {
				GrantApplication = grantApplication,
				GrantApplicationId = grantApplication.Id,
				ClaimVersion = 2,
				ClaimState = ClaimState.ClaimApproved
			};
			grantApplication.Claims.Add(claim);

			var newClaim = new Claim();
			Func<GrantApplication, Claim> func = (ga) => {
				newClaim = new Claim() { ClaimVersion = ga.GetCurrentClaim().ClaimVersion + 1 };
				ga.Claims.Add(newClaim);
				return newClaim;
			};

			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), director, "Director");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.EnableClaimReporting(func);

			// Assert
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.AgreementAccepted);
			grantApplication.ApplicationStateExternal.Should().Be(ApplicationStateExternal.AmendClaim);
			grantApplication.Claims.Count().Should().Be(2);
			grantApplication.GetCurrentClaim().ClaimVersion.Should().Be(3);
		}

		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine"), TestCategory("Grant Application")]
		public void GrantApplication_CloseClaimReporting()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var director = EntityHelper.CreateInternalUser();
			var grantOpening = EntityHelper.CreateGrantOpening();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(grantOpening, applicationAdministrator, director, ApplicationStateInternal.ClaimDenied);

			var claim = new Claim() {
				GrantApplication = grantApplication,
				GrantApplicationId = grantApplication.Id,
				ClaimVersion = 2,
				ClaimState = ClaimState.ClaimApproved
			};
			grantApplication.Claims.Add(claim);
			Action<GrantApplication> func = (GrantApplication x) => {
				claim.ClaimVersion = 0;
			};

			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), director, "Director");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			stateMachine.CloseClaimReporting(func);

			// Assert
			claim.ClaimVersion.Should().Be(0);
		}
		#endregion

		#region Methods
		[TestMethod]
		[TestCategory("Workflow"), TestCategory("State Machine"), TestCategory("Grant Application")]
		public void GetPermittedTriggers_WithClosedGrantApplication()
		{
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var director = EntityHelper.CreateInternalUser();
			var grantOpening = EntityHelper.CreateGrantOpening();
			var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(grantOpening, applicationAdministrator, director, ApplicationStateInternal.Closed);

			var helper = new ServiceHelper(typeof(ApplicationWorkflowStateMachine), director, "Director");

			var stateMachine = helper.Create<ApplicationWorkflowStateMachine>(grantApplication);

			// Act
			var results = stateMachine.GetPermittedTriggers();

			// Assert
			results.Count().Should().Be(14);
		}
		#endregion
	}
}
