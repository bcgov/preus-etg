using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using CJG.Testing.Core;
using CJG.Core.Entities;
using CJG.Application.Services;
using System.Collections.Generic;
using System;

namespace CJG.Testing.UnitTests.Entities
{
    [TestClass()]
    public class ClaimValidationTests
    {
        ServiceHelper helper { get; set; }

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            helper = new ServiceHelper(typeof(ClaimService), user);
            helper.MockContext();
            helper.MockDbSet<ClaimEligibleCost>();
            helper.MockDbSet<TrainingProgram>();
            helper.MockDbSet<EligibleCost>();
			helper.MockDbSet<ParticipantCost>();
			helper.MockDbSet<ParticipantForm>();
			helper.MockDbSet<TrainingCost>();
            helper.MockDbSet<EligibleExpenseType>();
        }

        [TestMethod, TestCategory("Claim"), TestCategory("Validate")]
        public void Validate_When_Claim_Is_Required_Properties_Error()
        {
			// Arrange
            var claim = new Claim();

            helper.MockDbSet<Claim>(new[] { claim });

            var service = helper.Create<ClaimService>();

            // Act
            var validationResults = service.Validate(claim).ToArray();

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == "The ClaimNumber field is required."));
        }

        [TestMethod, TestCategory("Claim"), TestCategory("Validate")]
        public void Validate_When_Claim_Is_Unaccessed_ParticipantsWithCostsAssigned_Equals_Zero()
        {
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(applicationAdministrator);
            var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.UnderAssessment);
            grantApplication.ApplicationStateExternal = ApplicationStateExternal.AmendClaim;

            var claim = new Claim(1, 1, grantApplication)
            {
                ClaimState = ClaimState.Unassessed
            };

            helper.MockDbSet<GrantApplication>(new[] { grantApplication });
            helper.MockDbSet<Claim>(new[] { claim });

            var service = helper.Create<ClaimService>();

            // Act
            var validationResults = service.Validate(claim).ToArray();

            var validationMsg = "You have participants reported that you have not assigned expenses to in your claim. " +
                                "If a participant has not attended training then they should be removed from your participant list. " +
                                "If participant expenses are not assigned correctly then you should edit your claim and correct them before you submit it.";

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validationMsg));
        }

        [TestMethod, TestCategory("Claim"), TestCategory("Validate")]
        public void Validate_When_Claim_Is_Unaccessed_ParticipantsWithoutCostsAssigned_Greater_Than_Zero()
        {
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(applicationAdministrator);
            var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.UnderAssessment);
            grantApplication.ApplicationStateExternal = ApplicationStateExternal.AmendClaim;

            var invitationKey = Guid.NewGuid();
            var participantForm = new ParticipantForm()
            {
                InvitationKey = invitationKey
            };
            participantForm.GrantApplicationId = grantApplication.Id;
            grantApplication.ParticipantForms.Add(participantForm);

            var claim = EntityHelper.CreateClaim(grantApplication, 1, 1);
            claim.ClaimState = ClaimState.Unassessed;

            helper.MockDbSet<GrantApplication>(new[] { grantApplication });
            helper.MockDbSet<Claim>(new[] { claim });

            var service = helper.Create<ClaimService>();

            // Act
            var validationResults = service.Validate(claim).ToArray();

            var validationMsg = "You have participants reported that you have not assigned expenses to in your claim. " +
                                "If a participant has not attended training then they should be removed from your participant list. " +
                                "If participant expenses are not assigned correctly then you should edit your claim and correct them before you submit it.";

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validationMsg));
        }

        [TestMethod, TestCategory("Claim"), TestCategory("Validate")]
        public void Validate_When_Claim_ParticipantsOnClaim_More_Than_AgreedParticipants()
        {
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(applicationAdministrator);
            var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.UnderAssessment);
            grantApplication.ApplicationStateExternal = ApplicationStateExternal.AmendClaim;

            var trainingCost = new TrainingCost()
            {
                GrantApplication = grantApplication
            };

            var invitationKey = Guid.NewGuid();
            var participantForm = new ParticipantForm()
            {
                InvitationKey = invitationKey
            };
            participantForm.GrantApplicationId = grantApplication.Id;
            grantApplication.ParticipantForms.Add(participantForm);

            var claim = EntityHelper.CreateClaim(grantApplication, 1, 1);
            claim.ClaimState = ClaimState.Unassessed;

            helper.MockDbSet<GrantApplication>(new[] { grantApplication });
            helper.MockDbSet<Claim>(new[] { claim });

            var service = helper.Create<ClaimService>();

            // Act
            var validationResults = service.Validate(claim).ToArray();

            var participantsOnClaim = (from pc in claim.EligibleCosts.SelectMany(ec => ec.ParticipantCosts)
                                       select pc.ParticipantFormId).ToArray().Distinct().Count();

            var validationMsg = "The number of participants (" + participantsOnClaim +
                                ") included in this claim is more than the agreed number of participants (" +
                                claim.GrantApplication.TrainingCost.AgreedParticipants + ").";

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validationMsg));
        }

        [TestMethod, TestCategory("Claim"), TestCategory("Validate")]
        public void Validate_When_Claim_TotalClaimReimbursement_Not_Equal_To_SumReimbursements()
        {
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(applicationAdministrator);
            var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.UnderAssessment);
            grantApplication.ApplicationStateExternal = ApplicationStateExternal.AmendClaim;

            var claim = EntityHelper.CreateClaim(grantApplication, 1, 1);
            claim.ClaimState = ClaimState.Unassessed;
            claim.TotalClaimReimbursement = 100;

            helper.MockDbSet<GrantApplication>(new[] { grantApplication });
            helper.MockDbSet<Claim>(new[] { claim });

            var service = helper.Create<ClaimService>();

            // Act
            var validationResults = service.Validate(claim).ToArray();
            var sum_reimbursements = claim.EligibleCosts.Sum(ec => ec.ParticipantCosts.Sum(pc => pc.ClaimReimbursement));
            var validationMsg = "The total claim must be equal to the sum of the participant reimbursements $" +
                                sum_reimbursements + ".";

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validationMsg));
        }

        [TestMethod, TestCategory("Claim"), TestCategory("Validate")]
        public void Validate_When_Claim_TotalAssessedReimbursement_Not_Equal_To_Sum_Assessed_Reimbursements()
        {
			// Arrange
			var applicationAdministrator = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(applicationAdministrator);
            var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(applicationAdministrator, ApplicationStateInternal.UnderAssessment);
            grantApplication.ApplicationStateExternal = ApplicationStateExternal.AmendClaim;

            var claim = EntityHelper.CreateClaim(grantApplication, 1, 1);
            claim.ClaimState = ClaimState.Unassessed;
            claim.TotalAssessedReimbursement = 50;

            helper.MockDbSet<GrantApplication>(new[] { grantApplication });
            helper.MockDbSet<Claim>(new[] { claim });

            var service = helper.Create<ClaimService>();

            // Act
            var validationResults = service.Validate(claim).ToArray();

            var sum_assessed_reimbursements = claim.EligibleCosts.Sum(ec => ec.ParticipantCosts.Sum(pc => pc.AssessedReimbursement));

            var validationMsg = "The total claim assessed reimbursement must be equal to the sum of the assessed participant reimbursements $" +
                                sum_assessed_reimbursements + ".";

            // Assert
            Assert.AreEqual(true, validationResults.Any(x => x.ErrorMessage == validationMsg));
        }
    }
}