using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Infrastructure.Entities;
using CJG.Testing.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
    public class StaticDataServicesTests : ServiceUnitTestBase
    {
        [TestInitialize]
        public void Setup()
        {
        }

        [TestMethod, TestCategory("Static Data"), TestCategory("Service")]
        public void CanGetDeliveryMethodsThatAreActive()
        {
            // Arrange
            var methodActive = new DeliveryMethod() {
                Caption = "name1",
                RowSequence = 1,
                IsActive = true
            };
            var methodNotActive = new DeliveryMethod() {
                Caption = "name2",
                RowSequence = 2,
                IsActive = false
            };
            var listOfDeliveryMethods = new List<DeliveryMethod>() { methodActive, methodNotActive };

            var deliveryMethodsMock = EntityFrameworkMockHelpers.CreateDbAddMock<DeliveryMethod>(listOfDeliveryMethods);
            deliveryMethodsMock.Setup(x => x.AsNoTracking()).Returns(deliveryMethodsMock.Object);
            var dbContextMock = new Mock<IDataContext>();
			var httpContextMock = new Mock<HttpContextBase>();
            var loggerMock = new Mock<ILogger>();
            dbContextMock.Setup(x => x.Set<DeliveryMethod>()).Returns(deliveryMethodsMock.Object);
            
            var service = new StaticDataService(dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

            // Act
            var results = service.GetDeliveryMethods();

            // Assert
            results.Count().Should().Be(1);
            results.ToList()[0].IsActive.Should().Be(true);
        }

        [TestMethod, TestCategory("Static Data"), TestCategory("Service")]
        public void CanGetCorrectTrainingPeriod()
        {
			// Arrange
			var trainPeriod1 = new TrainingPeriod
            {
                Caption = "Period1",
                StartDate = new DateTime(2017, 03, 01),
                EndDate = new DateTime(2017, 04, 01),
                FiscalYearId = 1,
                GrantOpenings = new List<GrantOpening>()            
            };
            var trainPeriod2 = new TrainingPeriod
            {
                Caption = "Period2",
                StartDate = new DateTime(2017, 04, 01),
                EndDate = new DateTime(2017, 05, 01),
                FiscalYearId = 1,
                GrantOpenings = new List<GrantOpening>()
            };
            var trainPeriod3 = new TrainingPeriod
            {
                Caption = "Period3",
                StartDate = new DateTime(2017, 06, 01),
                EndDate = new DateTime(2017, 06, 01),
                FiscalYearId = 2,
                GrantOpenings = new List<GrantOpening>()
            };
            var listOfTrainingPeriods = new List<TrainingPeriod>() { trainPeriod1, trainPeriod2, trainPeriod3 };

			var user = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(StaticDataService), user);
			var service = helper.Create<StaticDataService>();
            helper.MockDbSet(listOfTrainingPeriods);

			// Act
			var results = service.GetTrainingPeriodsForFiscalYear(1);

            // Assert
            results.Count().Should().Be(2);
            results.FirstOrDefault().Caption.Should().Be("Period1");
        }

		[TestMethod, TestCategory("Static Data"), TestCategory("Service")]
		public void CanGetCorrectFormatRate()
		{
			// Arrange
			var rateFormat1 = new RateFormat {
				Format = "0.00",
				Rate = 0.1f
			};
			var rateFormat2 = new RateFormat {
				Format = "0.000",
				Rate = 0.5f
			};
			var listOfRateFormats = new List<RateFormat>() { rateFormat1, rateFormat2 };
			var rateFormatsMock = EntityFrameworkMockHelpers.CreateDbAddMock<RateFormat>(listOfRateFormats);
			rateFormatsMock.Setup(x => x.AsNoTracking()).Returns(rateFormatsMock.Object);
			var loggerMock = new Mock<ILogger>();
			var dbContextMock = new Mock<IDataContext>();
			var httpContextMock = new Mock<HttpContextBase>();
			dbContextMock.Setup(x => x.Set<RateFormat>()).Returns(rateFormatsMock.Object);
			var user = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(StaticDataService), user);
			var dbSetMock = helper.MockDbSet(listOfRateFormats);
			dbContextMock.Setup(x => x.RateFormats).Returns(dbSetMock.Object);
			dbContextMock.Setup(x => x.RateFormats.AsNoTracking()).Returns(dbSetMock.Object);

			var service = new StaticDataService(dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

			// Act
			var results = service.GetRateFormat(0.5f);

			// Assert
			results.Format.Should().Be("0.000");
		}
	}
}
