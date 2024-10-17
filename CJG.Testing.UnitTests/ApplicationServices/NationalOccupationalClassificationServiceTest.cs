using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Infrastructure.Entities;
using CJG.Testing.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
    public class NationalOccupationalClassificationServiceTest : ServiceUnitTestBase
    {
        [TestInitialize]
        public void Setup()
        {
        }

        [TestMethod, TestCategory("NOC"), TestCategory("Service")]
        public void AddNationalOccupationalClassification_WhenNewNocIsPassed_ShouldAddTheNoc()
        {
            // Arrange
            var newNoc = new NationalOccupationalClassification()
            {
                Code = "Code",
                Description = "Description"
            };
            var loggerMock = new Mock<ILogger>();
            var httpContextMock = new Mock<HttpContextBase>();
            var dbContextMock = new Mock<IDataContext>();
			dbContextMock.Setup(x => x.NationalOccupationalClassifications.Add(It.IsAny<NationalOccupationalClassification>()));
            var service = new NationalOccupationalClassificationService(dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

            // Act
            service.AddNationalOccupationalClassification(newNoc);

            // Assert
            dbContextMock.Verify(x => x.NationalOccupationalClassifications.Add(It.IsAny<NationalOccupationalClassification>()), Times.Exactly(1));
            dbContextMock.Verify(x => x.Commit(), Times.Exactly(1));
        }

        [TestMethod, TestCategory("NOC"), TestCategory("Service")]
        public void UpdateNationalOccupationalClassification_WhenModifiedNocIsPassed_ShouldUpdateTheNoc()
        {
            // Arrange
            var modifiedNoc = new NationalOccupationalClassification()
            {
                Code = "Code",
                Description = "Description"
            };
            var loggerMock = new Mock<ILogger>();
            var httpContextMock = new Mock<HttpContextBase>();
            var dbContextMock = new Mock<IDataContext>();
            dbContextMock.Setup(x => x.Update(It.IsAny<NationalOccupationalClassification>()));
            var service = new NationalOccupationalClassificationService(dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

            // Act
            service.UpdateNationalOccupationalClassification(modifiedNoc);

            // Assert
            dbContextMock.Verify(x => x.Update(It.IsAny<NationalOccupationalClassification>()), Times.Exactly(1));
            dbContextMock.Verify(x => x.Commit(), Times.Exactly(1));
        }

        [TestMethod, TestCategory("NOC"), TestCategory("Service")]
        public void GetNationalOccupationalClassification_WhenNullIDAsParameter_ShouldReturnNull()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();
            var httpContextMock = new Mock<HttpContextBase>();
            var dbContextMock = new Mock<IDataContext>();
            var service = new NationalOccupationalClassificationService(dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

            // Act
            var results = service.GetNationalOccupationalClassification( null);

            // Assert
            results.Should().Be(null);
        }

        [TestMethod, TestCategory("NOC"), TestCategory("Service")]
        public void GetNationalOccupationalClassification_WhenPassInNocID_ShouldReturnNocData()
        {
            // Arrange
            var noc = new NationalOccupationalClassification()
            {
                Code = "Code",
                Description = "Description"
            };
            var dbContextMock = new Mock<IDataContext>();
            var httpContextMock = new Mock<HttpContextBase>();
            var loggerMock = new Mock<ILogger>();
			dbContextMock.Setup(x => x.NationalOccupationalClassifications.Find(It.IsAny<int?>())).Returns(noc);
            var service = new NationalOccupationalClassificationService(dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

            // Act
            var results = service.GetNationalOccupationalClassification(1);

            // Assert
            results.Code.Should().Be("Code");
            results.Description.Should().Be("Description");
        }

        [TestMethod, TestCategory("NOC"), TestCategory("Service")]
        public void GetNationalOccupationalClassifications_WhenPassInNullID_ReturnEmptyList()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();
            var httpContextMock = new Mock<HttpContextBase>();
            var dbContextMock = new Mock<IDataContext>();
			var service = new NationalOccupationalClassificationService(dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

            // Act
            var results = service.GetNationalOccupationalClassifications(null);

            // Assert
            results.Count().Should().Be(0);
        }

        [TestMethod, TestCategory("NOC"), TestCategory("Service")]
        public void GetNationalOccupationalClassifications_WhenPassInNocID_ShouldReturnListOfNocData()
        {
            // Arrange
            var noc = new NationalOccupationalClassification() {
                Code = "Code",
                Description = "Description",
				Left = 1,
				Right = 1,
				Level = 1,
				Id = 1
			};
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(NationalOccupationalClassificationService), identity);
			helper.MockDbSet(noc);

			var service = helper.Create<NationalOccupationalClassificationService>();

			// Act
			var results = service.GetNationalOccupationalClassifications(1);

            // Assert
            results.Count().Should().BeGreaterThan(0);
            results.Contains(noc);
        }
    }
}
