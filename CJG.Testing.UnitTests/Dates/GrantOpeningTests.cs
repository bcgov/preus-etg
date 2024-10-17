using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Infrastructure.Entities;
using CJG.Testing.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;

namespace CJG.Testing.UnitTests.Dates
{
    [TestClass]
    public class GrantOpeningTests
    {
        #region Variables
        #endregion

        #region Initialize
        [TestInitialize]
        public void Setup()
        {

        }
        #endregion

        #region Tests
        [TestMethod]
        [TestCategory("Fiscal Year"), TestCategory("Dates")]
        public void AddFiscalYear_InValidStartDate()
        {
            try
            {
                // Arrange
                // Act
                var fiscal = new FiscalYear("2017-2018", new DateTime(2018, 4, 1).ToUniversalTime(), new DateTime(2018, 3, 31).ToUniversalTime());
            }
            catch (ArgumentException e)
            {
                // Assert
                Assert.IsTrue(e.Message.StartsWith("The start date must be before the end date."));
            }
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Dates")]
        public void AddGrantOpening_ValidStartDate()
        {
            // Arrange
            var applicationAdministrator = EntityHelper.CreateExternalUser();
            var identity = EntityHelper.CreateExternalUser();
			var grantStream = new GrantStream("name", "Criteria", new GrantProgram(), new AccountCode()) { MaxReimbursementAmt = 1000, ReimbursementRate = (2 / 3) };
            var fiscal = new FiscalYear("2017-2018", new DateTime(2017, 4, 1).ToUniversalTime(), new DateTime(2018, 3, 31).ToUniversalTime());
            var trainingPeriod = new TrainingPeriod(fiscal, "Training Period 1", new DateTime(2017, 4, 1).ToUniversalTime(), new DateTime(2017, 8, 31).ToUniversalTime().ToLocalMidnight(), new DateTime(2017, 1, 1).ToUniversalTime(), new DateTime(2017, 2, 1).ToUniversalTime());
            var grantOpening = new GrantOpening(grantStream, trainingPeriod, 1000000);
            var helper = new ServiceHelper(typeof(GrantOpeningService), identity);
			helper.MockDbSet<GrantOpening>();
			var dbContextMock = helper.GetMock<IDataContext>();

            var service = helper.Create<GrantOpeningService>();

            // Act
            service.Add(grantOpening);

			// Assert
			dbContextMock.Object.GrantOpenings.Count().Should().Be(1);
            dbContextMock.Verify(m => m.CommitTransaction(), Times.Exactly(1));
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Dates")]
        public void ManageGrantOpening_Unscheduled()
        {
            // Arrange
            AppDateTime.SetNow(new DateTime(2017, 1, 1).ToUniversalTime());
            var applicationAdministrator = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(applicationAdministrator);
            var grantStream = new GrantStream("name", "Criteria", new GrantProgram(), new AccountCode()) { MaxReimbursementAmt = 1000, ReimbursementRate = (2 / 3) };
            var fiscal = new FiscalYear("2017-2018", new DateTime(2017, 4, 1).ToUniversalTime(), new DateTime(2018, 3, 31).ToUniversalTime());
            var trainingPeriod = new TrainingPeriod(fiscal, "Training Period 1", new DateTime(2017, 4, 1).ToUniversalTime(), new DateTime(2017, 8, 31).ToUniversalTime().ToLocalMidnight(), new DateTime(2017, 1, 1).ToUniversalTime(), new DateTime(2017, 2, 1).ToUniversalTime());
            var grantOpening = new GrantOpening(grantStream, trainingPeriod, 1000000);
            var helper = new ServiceHelper(typeof(GrantOpeningManageScheduledService), identity);

            var service = helper.Create<GrantOpeningManageScheduledService>();

            // Act
            service.ManageStateTransition(grantOpening);
                
            // Assert
            grantOpening.State.Should().Be(GrantOpeningStates.Unscheduled);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Dates")]
        public void ManageGrantOpening_ScheduledToScheduled()
        {
            // Arrange
            AppDateTime.SetNow(new DateTime(2017, 1, 1).ToUniversalTime().AddSeconds(-1));
            var applicationAdministrator = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(applicationAdministrator);
            var grantStream = new GrantStream("name", "Criteria", new GrantProgram(), new AccountCode()) { MaxReimbursementAmt = 1000, ReimbursementRate = (2 / 3) };
            var fiscal = new FiscalYear("2017-2018", new DateTime(2017, 4, 1).ToUniversalTime(), new DateTime(2018, 3, 31).ToUniversalTime());
            var trainingPeriod = new TrainingPeriod(fiscal, "Training Period 1", new DateTime(2017, 4, 1).ToUniversalTime(), new DateTime(2017, 8, 31).ToUniversalTime().ToLocalMidnight(), new DateTime(2017, 1, 1).ToUniversalTime(), new DateTime(2017, 2, 1).ToUniversalTime());
            var grantOpening = new GrantOpening(grantStream, trainingPeriod, 1000000) { State = GrantOpeningStates.Scheduled };
            var helper = new ServiceHelper(typeof(GrantOpeningManageScheduledService), identity);

            var service = helper.Create<GrantOpeningManageScheduledService>();

            // Act
            service.ManageStateTransition(grantOpening);

            // Assert
            grantOpening.State.Should().Be(GrantOpeningStates.Scheduled);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Dates")]
        public void ManageGrantOpening_ScheduledToPublished()
        {
            // Arrange
            AppDateTime.SetNow(new DateTime(2017, 1, 1).ToUniversalTime());
            var applicationAdministrator = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(applicationAdministrator);
            var grantStream = new GrantStream("name", "Criteria", new GrantProgram(), new AccountCode()) { MaxReimbursementAmt = 1000, ReimbursementRate = (2 / 3) };
            var fiscal = new FiscalYear("2017-2018", new DateTime(2017, 4, 1).ToUniversalTime(), new DateTime(2018, 3, 31).ToUniversalTime());
            var trainingPeriod = new TrainingPeriod(fiscal, "Training Period 1", new DateTime(2017, 4, 1).ToUniversalTime(), new DateTime(2017, 8, 31).ToUniversalTime().ToLocalMidnight(), new DateTime(2017, 1, 1).ToUniversalTime(), new DateTime(2017, 2, 1).ToUniversalTime());
            var grantOpening = new GrantOpening(grantStream, trainingPeriod, 1000000) { State = GrantOpeningStates.Scheduled };
            var helper = new ServiceHelper(typeof(GrantOpeningManageScheduledService), identity);

            var service = helper.Create<GrantOpeningManageScheduledService>();

            // Act
            service.ManageStateTransition(grantOpening);

            // Assert
            grantOpening.State.Should().Be(GrantOpeningStates.Published);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Dates")]
        public void ManageGrantOpening_PublishedToPublished()
        {
            // Arrange
            AppDateTime.SetNow(new DateTime(2017, 2, 1).ToUniversalTime().AddSeconds(-1));
            var applicationAdministrator = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(applicationAdministrator);
            var grantStream = new GrantStream("name", "Criteria", new GrantProgram(), new AccountCode()) { MaxReimbursementAmt = 1000, ReimbursementRate = (2 / 3) };
            var fiscal = new FiscalYear("2017-2018", new DateTime(2017, 4, 1).ToUniversalTime(), new DateTime(2018, 3, 31).ToUniversalTime());
            var trainingPeriod = new TrainingPeriod(fiscal, "Training Period 1", new DateTime(2017, 4, 1).ToUniversalTime(), new DateTime(2017, 8, 31).ToUniversalTime().ToLocalMidnight(), new DateTime(2017, 1, 1).ToUniversalTime(), new DateTime(2017, 2, 1).ToUniversalTime());
            var grantOpening = new GrantOpening(grantStream, trainingPeriod, 1000000) { State = GrantOpeningStates.Published };
            var helper = new ServiceHelper(typeof(GrantOpeningManageScheduledService), identity);

            var service = helper.Create<GrantOpeningManageScheduledService>();

            // Act
            service.ManageStateTransition(grantOpening);

            // Assert
            grantOpening.State.Should().Be(GrantOpeningStates.Published);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Dates")]
        public void ManageGrantOpening_PublishedToOpen()
        {
            // Arrange
            AppDateTime.SetNow(new DateTime(2017, 2, 1).ToUniversalTime());
            var applicationAdministrator = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(applicationAdministrator);
            var grantStream = new GrantStream("name", "Criteria", new GrantProgram(), new AccountCode()) { MaxReimbursementAmt = 1000, ReimbursementRate = (2/3) };
            var fiscal = new FiscalYear("2017-2018", new DateTime(2017, 4, 1).ToUniversalTime(), new DateTime(2018, 3, 31).ToUniversalTime());
            var trainingPeriod = new TrainingPeriod(fiscal, "Training Period 1", new DateTime(2017, 4, 1).ToUniversalTime(), new DateTime(2017, 8, 31).ToUniversalTime().ToLocalMidnight(), new DateTime(2017, 1, 1).ToUniversalTime(), new DateTime(2017, 2, 1).ToUniversalTime());
            var grantOpening = new GrantOpening(grantStream, trainingPeriod, 1000000) { State = GrantOpeningStates.Published };
            var helper = new ServiceHelper(typeof(GrantOpeningManageScheduledService), identity);

            var service = helper.Create<GrantOpeningManageScheduledService>();

            // Act
            service.ManageStateTransition(grantOpening);

            // Assert
            grantOpening.State.Should().Be(GrantOpeningStates.Open);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Dates")]
        public void ManageGrantOpening_OpenToOpen()
        {
            // Arrange
            AppDateTime.SetNow(new DateTime(2017, 8, 31).ToLocalMidnight().AddSeconds(-1));
            var applicationAdministrator = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(applicationAdministrator);
            var grantStream = new GrantStream("name", "Criteria", new GrantProgram(), new AccountCode()) { MaxReimbursementAmt = 1000, ReimbursementRate = (2 / 3) };
            var fiscal = new FiscalYear("2017-2018", new DateTime(2017, 4, 1).ToUniversalTime(), new DateTime(2018, 3, 31).ToUniversalTime());
            var trainingPeriod = new TrainingPeriod(fiscal, "Training Period 1", new DateTime(2017, 4, 1).ToUniversalTime(), new DateTime(2017, 8, 31).ToUniversalTime().ToLocalMidnight(), new DateTime(2017, 1, 1).ToUniversalTime(), new DateTime(2017, 2, 1).ToUniversalTime());
            var grantOpening = new GrantOpening(grantStream, trainingPeriod, 1000000) { State = GrantOpeningStates.Open };
            var helper = new ServiceHelper(typeof(GrantOpeningManageScheduledService), identity);

            var service = helper.Create<GrantOpeningManageScheduledService>();

            // Act
            service.ManageStateTransition(grantOpening);

            // Assert
            grantOpening.State.Should().Be(GrantOpeningStates.Open);
        }

        [TestMethod]
        [TestCategory("Grant Opening"), TestCategory("Dates")]
        public void ManageGrantOpening_OpenToClosed()
        {
            // Arrange
            AppDateTime.SetNow(new DateTime(2017, 8, 31).ToUniversalTime().ToLocalMidnight());
            var applicationAdministrator = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(applicationAdministrator);
            var grantStream = new GrantStream("name", "Criteria", new GrantProgram(), new AccountCode()) { MaxReimbursementAmt = 1000, ReimbursementRate = (2 / 3) };
            var fiscal = new FiscalYear("2017-2018", new DateTime(2017, 4, 1).ToUniversalTime(), new DateTime(2018, 3, 31).ToUniversalTime());
            var trainingPeriod = new TrainingPeriod(fiscal, "Training Period 1", new DateTime(2017, 4, 1).ToUniversalTime(), new DateTime(2017, 8, 31).ToUniversalTime().ToLocalMidnight(), new DateTime(2017, 1, 1).ToUniversalTime(), new DateTime(2017, 2, 1).ToUniversalTime());
            var grantOpening = new GrantOpening(grantStream, trainingPeriod, 1000000) { State = GrantOpeningStates.Open };
            var helper = new ServiceHelper(typeof(GrantOpeningManageScheduledService), identity);

            var service = helper.Create<GrantOpeningManageScheduledService>();

            // Act
            service.ManageStateTransition(grantOpening);

            // Assert
            grantOpening.State.Should().Be(GrantOpeningStates.Closed);
        }
        #endregion
    }
}
