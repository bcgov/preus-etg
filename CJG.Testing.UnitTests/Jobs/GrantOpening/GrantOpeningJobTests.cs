using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using CJG.Infrastructure.GrantOpeningService;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using System;

namespace CJG.Testing.UnitTests.Jobs.GrantOpening
{
	[TestClass]
    public class GrantOpeningJobTests 
    {
        private Mock<IGrantOpeningService> _grantOpeningServiceMock;
        private Mock<IGrantApplicationService> _grantApplicationServiceMock;
        private Mock<ILogger> _loggerMock;

        [TestInitialize]
        public void Setup()
        {
            _grantOpeningServiceMock = new Mock<IGrantOpeningService>();
            _grantApplicationServiceMock = new Mock<IGrantApplicationService>();
            _loggerMock = new Mock<ILogger>();
        }

        [TestMethod, TestCategory("Unit")]
        public void TryVerifyAndUpdateGrantOpening_WithGrantOpeningOpenMatchDate_ReturnsUpdatedResult()
        {
            CreateGrantOpeningJob().TryVerifyAndUpdateGrantOpening(new CJG.Core.Entities.GrantOpening
                {
                    State = GrantOpeningStates.Open,
                    ClosingDate = DateTime.SpecifyKind(new DateTime(2017, 1, 1), DateTimeKind.Utc)
                },
                DateTime.SpecifyKind(new DateTime(2017, 1, 1), DateTimeKind.Utc)).VerifyResult(false, true, GrantOpeningStates.Open, GrantOpeningStates.Closed);
        }

        [TestMethod, TestCategory("Unit")]
        public void TryVerifyAndUpdateGrantOpening_WithGrantOpeningScheduledMatchDate_ReturnsUpdatedPublishedResult()
        {
            CreateGrantOpeningJob().TryVerifyAndUpdateGrantOpening(new CJG.Core.Entities.GrantOpening
                {
                    State = GrantOpeningStates.Scheduled,
                    PublishDate = DateTime.SpecifyKind(new DateTime(2017, 1, 1), DateTimeKind.Utc)
                },
                DateTime.SpecifyKind(new DateTime(2017, 1, 1), DateTimeKind.Utc)).VerifyResult(false, true, GrantOpeningStates.Scheduled, GrantOpeningStates.Published);
        }

        [TestMethod, TestCategory("Unit")]
        public void TryVerifyAndUpdateGrantOpening_WithGrantOpeningPublishedMatchDate_ReturnsUpdatedOpenResult()
        {
            CreateGrantOpeningJob().TryVerifyAndUpdateGrantOpening(new CJG.Core.Entities.GrantOpening
                {
                    State = GrantOpeningStates.Published,
                    OpeningDate = DateTime.SpecifyKind(new DateTime(2017, 1, 1), DateTimeKind.Utc)
                },
                DateTime.SpecifyKind(new DateTime(2017, 1, 1), DateTimeKind.Utc)).VerifyResult(false, true, GrantOpeningStates.Published, GrantOpeningStates.Open);
        }

        [TestMethod, TestCategory("Unit")]
        public void TryVerifyAndUpdateGrantOpening_WithGrantOpeningScheduledPublishOpeningClosingTheSameMatchDate_ReturnsUpdatedClosedResult()
        {
            CreateGrantOpeningJob().TryVerifyAndUpdateGrantOpening(new CJG.Core.Entities.GrantOpening
                {
                    State = GrantOpeningStates.Scheduled,
                    PublishDate = DateTime.SpecifyKind(new DateTime(2017, 1, 1), DateTimeKind.Utc),
                    OpeningDate = DateTime.SpecifyKind(new DateTime(2017, 1, 1), DateTimeKind.Utc),
                    ClosingDate = DateTime.SpecifyKind(new DateTime(2017, 1, 1), DateTimeKind.Utc)
                },
                DateTime.SpecifyKind(new DateTime(2017, 1, 1), DateTimeKind.Utc)).VerifyResult(false, true, GrantOpeningStates.Scheduled, GrantOpeningStates.Closed);
        }

        [TestMethod, TestCategory("Unit")]
        public void TryVerifyAndUpdateGrantOpening_WithGrantOpeningOpenLaterDate_ReturnsIgnoredResult()
        {
            CreateGrantOpeningJob().TryVerifyAndUpdateGrantOpening(new CJG.Core.Entities.GrantOpening
                {
                    State = GrantOpeningStates.Open,
                    ClosingDate = DateTime.SpecifyKind(new DateTime(2017, 1, 2), DateTimeKind.Utc)
                },
                DateTime.SpecifyKind(new DateTime(2017, 1, 1), DateTimeKind.Utc)).VerifyResult(false, false, GrantOpeningStates.Open);
        }

        [TestMethod, TestCategory("Unit")]
        public void TryVerifyAndUpdateGrantOpening_WithGrantOpeningOpenEarlierDate_ReturnsUpdatedResult()
        {
            CreateGrantOpeningJob().TryVerifyAndUpdateGrantOpening(new CJG.Core.Entities.GrantOpening
                {
                    State = GrantOpeningStates.Open,
                    ClosingDate = DateTime.SpecifyKind(new DateTime(2016, 12, 31), DateTimeKind.Utc)
                },
                DateTime.SpecifyKind(new DateTime(2017, 1, 1), DateTimeKind.Utc))
                .VerifyResult(false, true, GrantOpeningStates.Open, GrantOpeningStates.Closed);
        }

        [TestMethod, TestCategory("Unit")]
        public void TryVerifyAndUpdateGrantOpening_WithGrantOpeningClosingOneSecondLater_ReturnsUpdatedResult()
        {
            CreateGrantOpeningJob().TryVerifyAndUpdateGrantOpening(new CJG.Core.Entities.GrantOpening
                {
                    State = GrantOpeningStates.Open,
                    ClosingDate = DateTime.SpecifyKind(new DateTime(2017, 1, 1, 0, 0, 1), DateTimeKind.Utc)
                },
                DateTime.SpecifyKind(new DateTime(2017, 1, 1), DateTimeKind.Utc)).VerifyResult(false, false, GrantOpeningStates.Open, GrantOpeningStates.Open);
        }

        private GrantOpeningJob CreateGrantOpeningJob(IGrantOpeningService grantOpeningService = null)
        {
            return new GrantOpeningJob(grantOpeningService ?? _grantOpeningServiceMock.Object, 
                _loggerMock.Object);
        }
    }

    public static class GrantOpeningStateUpdateResultExtensions
    {
        public static void VerifyResult(this GrantOpeningJob.GrantOpeningStateUpdateResult result, bool hasError, bool wasUpdated, GrantOpeningStates previousState, GrantOpeningStates? newState = null)
        {
            result.HasError.Should().Be(hasError);
            result.WasUpdated.Should().Be(wasUpdated);
            result.PreviousState.Should().Be(previousState);
            result.NewState?.Should().Be(newState);
        }

        
    }
}