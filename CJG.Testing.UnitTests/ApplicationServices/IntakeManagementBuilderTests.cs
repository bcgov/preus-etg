using System;
using System.Collections.Generic;
using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CJG.Web.External.Helpers;

namespace CJG.Testing.UnitTests.ApplicationServices
{
    [TestClass]
    public class IntakeManagementBuilderTests
    {
        private readonly Mock<IStaticDataService> _staticDataServiceMock = new Mock<IStaticDataService>();
        private readonly Mock<IGrantProgramService> _grantProgramServiceMock = new Mock<IGrantProgramService>();
        private readonly Mock<IGrantStreamService> _grantStreamServiceMock = new Mock<IGrantStreamService>();
        private readonly Mock<IGrantOpeningService> _grantOpeningServiceMock = new Mock<IGrantOpeningService>();

        [TestMethod, TestCategory("Grant Opening")]
        public void SearchTrainingPeriod_WithShiftZero_ReturnsSamePeriod()
        {
            MockTrainingPeriods();
            var builder = CreateIntakeManagementBuilder();
            List<TrainingPeriod> periods;
            int index = builder.SearchTrainingPeriodIndex(x => x.Id == 2, 1, 0, out periods);
            periods[index].Id.Should().Be(2);
        }

        [TestMethod]
        [TestCategory("Grant Opening")]
        public void SearchTrainingPeriod_WithShiftMinusOne_ReturnsPreviousPeriod()
        {
            MockTrainingPeriods();
            var builder = CreateIntakeManagementBuilder();
            List<TrainingPeriod> periods;
            var index = builder.SearchTrainingPeriodIndex(x => x.Id == 4, 1, -1, out periods);
            periods[index].Id.Should().Be(3);
        }

        [TestMethod]
        [TestCategory("Grant Opening")]
        public void SearchTrainingPeriod_WithShiftOne_ReturnsNextPeriod()
        {
            MockTrainingPeriods();
            var builder = CreateIntakeManagementBuilder();
            List<TrainingPeriod> periods;
            var index = builder.SearchTrainingPeriodIndex(x => x.Id == 2, 1, 1, out periods);
            periods[index].Id.Should().Be(3);
        }

        [TestMethod, TestCategory("Grant Opening")]
        public void GetTrainingPeriods_WithCurrentDate_ReturnsThreeTrainingPeriods()
        {
            MockTrainingPeriods();

            var builder = CreateIntakeManagementBuilder();
            var periods = builder.GetTrainingPeriods(1, new DateTime(2017, 02, 10), 1);
            periods.Should().HaveCount(3);
            periods[0].Id.Should().Be(2);
            periods[1].Id.Should().Be(3);
            periods[2].Id.Should().Be(4);
        }

        [TestMethod, TestCategory("Grant Opening")]
        public void FilterTrainingPeriods_WithWithSecondIndex_ReturnsThreeTrainingPeriods()
        {
            var trainingPeriods = CreateTrainingPeriods();
            var periods = IntakeManagementBuilder.FilterTrainingPeriods(trainingPeriods, 1, 1);
            periods.Should().HaveCount(3);
            periods[0].Id.Should().Be(1);
            periods[1].Id.Should().Be(2);
            periods[2].Id.Should().Be(3);
        }

        [TestMethod, TestCategory("Grant Opening")]
        public void FilterTrainingPeriods_WithFirstIndex_ReturnsThreeTrainingPeriods()
        {
            var trainingPeriods = CreateTrainingPeriods();
            var periods = IntakeManagementBuilder.FilterTrainingPeriods(trainingPeriods, 0, 1);
            periods.Should().HaveCount(3);
            periods[0].Id.Should().Be(1);
            periods[1].Id.Should().Be(2);
            periods[2].Id.Should().Be(3);
        }

        private IntakeManagementBuilder CreateIntakeManagementBuilder()
        {
            return new IntakeManagementBuilder(_staticDataServiceMock.Object, _grantProgramServiceMock.Object, _grantStreamServiceMock.Object, _grantOpeningServiceMock.Object);
        }

        private void MockTrainingPeriods()
        {
            _staticDataServiceMock.Setup(x => x.GetTrainingPeriods(1)).Returns(CreateTrainingPeriods);
        }

        private static List<TrainingPeriod> CreateTrainingPeriods()
        {
            return new List<TrainingPeriod>
            {
                new TrainingPeriod {StartDate = new DateTime(2016, 1, 1), EndDate = new DateTime(2016, 1, 31), Id = 1},
                new TrainingPeriod {StartDate = new DateTime(2017, 1, 1), EndDate = new DateTime(2017, 1, 31), Id = 2},
                new TrainingPeriod {StartDate = new DateTime(2017, 2, 1), EndDate = new DateTime(2017, 2, 28), Id = 3},
                new TrainingPeriod {StartDate = new DateTime(2017, 3, 1), EndDate = new DateTime(2017, 3, 31), Id = 4},
                new TrainingPeriod {StartDate = new DateTime(2018, 1, 1), EndDate = new DateTime(2018, 3, 31), Id = 5}
            };
        }
    }
}
