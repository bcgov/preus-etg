using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Infrastructure.Entities;
using CJG.Testing.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
    public class GrantAgreementServiceTests : ServiceUnitTestBase
    {
        [TestInitialize]
        public void Setup()
        {
        }

        [TestMethod, TestCategory("Grant Agreement"), TestCategory("Service")]
        public void AcceptGrantAgreement_WithAgreementId_UpdatesApplicationToAgreementAccepted()
        {
            // Arrange
            var applicationAdministrator = EntityHelper.CreateExternalUser();
            var helper = new ServiceHelper(typeof(GrantAgreementService), applicationAdministrator);
            var service = helper.Create<GrantAgreementService>();

            var grantOpening = EntityHelper.CreateGrantOpening();
            var grantApplication = EntityHelper.CreateGrantApplication(grantOpening, applicationAdministrator, ApplicationStateInternal.OfferIssued);

            var trainingProvider = new TrainingProvider(grantApplication);
            var trainingProgram = new TrainingProgram(grantApplication, trainingProvider) { StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddMonths(3) };
            grantApplication.TrainingProviders.Add(trainingProvider);
            grantApplication.TrainingPrograms.Add(trainingProgram);

            grantApplication.GrantAgreement = new GrantAgreement(grantApplication);

			helper.MockDbSet(grantApplication);

            // Act
            service.AcceptGrantAgreement(grantApplication);

            // Assert
            grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.AgreementAccepted);
        }

        [TestMethod, TestCategory("Grant Agreement"), TestCategory("Service")]
        public void CancelGrantAgreement_WithAgreementAccepted_UpdatesApplicationToCancelledByAgreementHolder()
        {
			// Arrange
			var user = EntityHelper.CreateExternalUser();
            var helper = new ServiceHelper(typeof(GrantAgreementService), user);
            var service = helper.Create<GrantAgreementService>();
			var grantOpening = EntityHelper.CreateGrantOpening();
            var grantApplication = EntityHelper.CreateGrantApplication(grantOpening, user, ApplicationStateInternal.AgreementAccepted);

            helper.MockDbSet(grantApplication);

			// Act
            service.CancelGrantAgreement(grantApplication, "reason");

			// Assert
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.CancelledByAgreementHolder);
        }

        [TestMethod, TestCategory("Grant Agreement"), TestCategory("Service")]
        public void RejectGrantAgreement_WithOfferIssued_UpdatesApplicationToAgreementRejected()
        {
			// Arrange
			var user = EntityHelper.CreateExternalUser();
            var helper = new ServiceHelper(typeof(GrantAgreementService), user);
            var service = helper.Create<GrantAgreementService>();
			helper.MockContext();
            var grantOpening = EntityHelper.CreateGrantOpening();
            var grantApplication = EntityHelper.CreateGrantApplication(grantOpening, user, ApplicationStateInternal.OfferIssued);

            helper.MockDbSet(grantApplication);

			// Act
            service.RejectGrantAgreement(grantApplication, "reason");

			// Assert
			grantApplication.ApplicationStateInternal.Should().Be(ApplicationStateInternal.AgreementRejected);
        }

        [TestMethod, TestCategory("Grant Agreement"), TestCategory("Service")]
        public void AddGrantAgreement_WithAgreementAccepted_UpdatesApplicationToCancelledByAgreementHolder()
        {
			// Arrange
			var user = EntityHelper.CreateExternalUser();
            var helper = new ServiceHelper(typeof(GrantAgreementService), user);
            var dbSetMock = helper.MockDbSet<GrantAgreement>();
            var service = helper.Create<GrantAgreementService>();

            var grantOpening = EntityHelper.CreateGrantOpening();
            var grantApplication = EntityHelper.CreateGrantApplication(grantOpening, user, ApplicationStateInternal.New);

			// Act
            service.Add(new GrantAgreement { GrantApplication = grantApplication });

			// Assert
            dbSetMock.Verify(x=>x.Add(It.IsAny<GrantAgreement>()), Times.Exactly(1));
            helper.GetMock<IDataContext>().Verify(x=>x.Commit(), Times.Exactly(1));
        }
    }
}
