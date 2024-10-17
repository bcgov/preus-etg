using CJG.Infrastructure.ReportingService;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CJG.Testing.UnitTests.Jobs.ReportingService
{
	[TestClass]
    public class SdsiReportJobTests
    {
        [TestMethod]
        public void GetTrainingHoursPerWeek_WithOneWeekTrainingDurationAnd40TotalHours_Returns40()
        {
            SdsiReportJob.GetTrainingHoursPerWeek(40, new DateTime(2017, 1, 1), new DateTime(2017, 1, 7))
                .Should().Be(40);
        }

        [TestMethod]
        public void GetTrainingHoursPerWeek_With10HoursTrainingIn10days_Returns7_04()
        {
            SdsiReportJob.GetTrainingHoursPerWeek(10, new DateTime(2017, 1, 1), new DateTime(2017, 1, 10))
                .Should().BeApproximately(7.042, 0.1);
        }

        [TestMethod]
        public void GetTrainingHoursPerWeek_With10HoursTrainingAndFiveWeekDuration_Returns2()
        {
            SdsiReportJob.GetTrainingHoursPerWeek(10, new DateTime(2017, 1, 1), new DateTime(2017, 2, 4))
                .Should().Be(2);
        }

        [TestMethod]
        public void GetTrainingHoursPerWeek_With5HoursTrainingAndOne3DaysDuration_Returns3()
        {
            SdsiReportJob.GetTrainingHoursPerWeek(5, new DateTime(2017, 1, 1), new DateTime(2017, 1, 3))
                .Should().Be(5);
        }

        [TestMethod]
        public void DateDiffInWeeks_WithOneDayDuration_Returns0_14()
        {
            SdsiReportJob.DateDiffInWeeks(new DateTime(2017, 1, 1), new DateTime(2017, 1, 1), true)
                .Should().BeApproximately(0.1428, 0.001); // 1/7;
        }

        [TestMethod]
        public void DateDiffInWeeks_WithTwoWeeksDuration_ReturnsTwo()
        {
            SdsiReportJob.DateDiffInWeeks(new DateTime(2017, 1, 1), new DateTime(2017, 1, 14), true)
                .Should().Be(2);
        }
    }
}
