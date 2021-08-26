using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Infrastructure.Entities;
using CJG.Testing.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
    public class ParticipantServiceTests : ServiceUnitTestBase
    {
        #region Initialize
        [TestInitialize]
        public void Setup()
        {
        }
        #endregion

        #region Tests
        [TestMethod, TestCategory("Participant"), TestCategory("Service")]
        public void AddParticipantConst_WithNewParticipantCost_AddsNewParticipantCostToRepository()
        {
			// Arrange
			var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(ParticipantService), identity);
            helper.MockDbSet<ParticipantCost>();
            var service = helper.Create<ParticipantService>();

			// Act
			service.Add(new ParticipantCost());

			// Assert
            helper.GetMock<IDataContext>().Verify(x => x.Commit(), Times.Once);
        }

        [TestMethod, TestCategory("Participant"), TestCategory("Service")]
        public void AddParticipantForm_WithNewParticipantForm_AddsNewParticipantFormToRepository()
        {
			// Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(ParticipantService), identity);
            var service = helper.Create<ParticipantService>();
            var invitationKey = Guid.NewGuid();
            var participantForm = new ParticipantForm()
            {
                InvitationKey = invitationKey
            };
            var grantApplication = new GrantApplication()
            {
                Id = 1,
                ApplicationStateInternal = ApplicationStateInternal.AgreementAccepted,
                InvitationKey = invitationKey,
                ParticipantForms = new List<ParticipantForm>()
                {
                    participantForm
                }
            };
            participantForm.GrantApplicationId = grantApplication.Id;
            
            var trainingProgram = new TrainingProgram(grantApplication);
            grantApplication.TrainingPrograms.Add(trainingProgram);
            helper.MockDbSet<ParticipantCost>();
            helper.MockDbSet<ParticipantForm>();
            helper.MockDbSet( new[] { grantApplication} );

			// Act
            service.Add(grantApplication.ParticipantForms.First());

			// Assert
            helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Once);
        }

        /// <summary>
        /// Adding a participantForm with a Claim should return a ParticipantForm
        /// </summary>
        [TestMethod, TestCategory("Participant"), TestCategory("Service")]
        public void AddParticipantForm_WithClaim_ShouldReturnParticipantForm()
        {
            // Arrange
            var applicationAdministrator = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(applicationAdministrator);
            var helper = new ServiceHelper(typeof(ParticipantService), identity);
            var service = helper.Create<ParticipantService>();
            var invitationKey = Guid.NewGuid();

            var grantApplication = EntityHelper.CreateGrantApplicationWithAgreement(EntityHelper.CreateGrantOpening(), applicationAdministrator, EntityHelper.CreateInternalUser(), ApplicationStateInternal.AgreementAccepted);
            grantApplication.InvitationKey = invitationKey;
            var claim = EntityHelper.CreateClaim(grantApplication);
            claim.EligibleCosts.Add(new ClaimEligibleCost(claim)
            {
                EligibleExpenseType = EntityHelper.CreateEligibleExpenseType()
            });
            grantApplication.Claims.Add(claim);
            var participantForm = new ParticipantForm(grantApplication, invitationKey);
            grantApplication.ParticipantForms.Add(participantForm);

			helper.MockDbSet<ParticipantForm>();
			helper.MockDbSet( new[] { grantApplication });

            // Act
            var participantFormResult = service.Add(participantForm);

            // Assert
            Assert.IsInstanceOfType(participantFormResult, typeof(ParticipantForm));
            helper.GetMock<IDataContext>().Verify(x => x.CommitTransaction(), Times.Once);
        }

        /// <summary>
        /// Adding a null participantForm should throw an ArgumentNullException.
        /// </summary>
        [TestMethod, TestCategory("Participant"), TestCategory("Service")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddParticipantFormNullShouldThrowArgumentNullException()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(ParticipantService), identity);
            var service = helper.Create<ParticipantService>();
            ParticipantForm participantForm = null;

            // Act
            service.Add(participantForm);

            // Assert (Handled by decorator)
        }

        /// <summary>
        /// Adding a null participantCost should throw an ArgumentNullException.
        /// </summary>
        [TestMethod, TestCategory("Participant"), TestCategory("Service")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddParticipantCostNullShouldThrowArgumentNullException()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(ParticipantService), identity);
            var service = helper.Create<ParticipantService>();
            ParticipantCost participantCost = null;

            // Act
            service.Add(participantCost);

            // Assert (Handled by decorator)
        }

        /// <summary>
        /// Updating a null participantCost should throw an ArgumentNullException.
        /// </summary>
        [TestMethod, TestCategory("Participant"), TestCategory("Service")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateParticipantCostNullShouldThrowArgumentNullException()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(ParticipantService), identity);
            var service = helper.Create<ParticipantService>();
            ParticipantCost participantCost = null;

            // Act
            service.Update(participantCost);

            // Assert (Handled by decorator)
        }

        [TestMethod, TestCategory("Participant"), TestCategory("Service")]
        public void UpdateParticipantCost_WithParticipantCost_UpdatesParticipantCostInRepository()
        {
			// Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(ParticipantService), identity);
            helper.MockDbSet<ParticipantCost>();
            var service = helper.Create<ParticipantService>();

			// Act
            service.Update(new ParticipantCost());

			// Assert
			helper.GetMock<IDataContext>().Verify(x => x.Update(It.IsAny<ParticipantCost>()), Times.Once);
            helper.GetMock<IDataContext>().Verify(x => x.Commit(), Times.Once);
        }
        #endregion
    }
}
