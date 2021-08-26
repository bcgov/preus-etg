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

        [TestMethod, TestCategory("NOC"), TestCategory("Service")]
        public void GetNationalOccupationalClassificationChildren_WhenPassInParentIDAndLevel_ShouldReturnListOfNocChildrenOrderByNocCode()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();
            var httpContextMock = new Mock<HttpContextBase>();
            var parent = new NationalOccupationalClassification() {
                Code = "Parent",
                Description = "Description",
                ParentId = 3,
                Level = 4,
				Id = 3,
				Left = 1,
				Right = 3
			};
            var child = new NationalOccupationalClassification()
            {
                Code = "Child",
                Description = "Description",
                ParentId = 3,
                Level = 4,
				Id = 1,
				Left = 4,
				Right = 2,
				Parent = parent
			};
			var nocs = new List<NationalOccupationalClassification>() { parent, child };
			var user = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(NationalOccupationalClassificationService), user);
			var dbSetMock = helper.MockDbSet(nocs);
			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(x => x.NationalOccupationalClassifications).Returns(dbSetMock.Object);

			var service = helper.Create<NationalOccupationalClassificationService>();

			// Act
			var results = service.GetNationalOccupationalClassificationChildren(3, 4);

            // Assert
            results.Count().Should().BeGreaterThan(0);
            results.ToList()[0].Code.Should().Be(child.Code);
        }

        [TestMethod, TestCategory("NOC"), TestCategory("Service")]
        public void GetNationalOccupationalClassificationIds_WhenPassInID_ShouldReturnTupleNocIDs()
        {
            // Arrange
            //var loggerMock = new Mock<ILogger>();
            //var httpContextMock = new Mock<HttpContextBase>();
            var level1Noc = new NationalOccupationalClassification()
            {
                Code = "Noc 1",
                Description = "Level1",
                ParentId = 3,
                Level = 1,
                Id = 1

            };
            var level2Noc = new NationalOccupationalClassification()
            {
                Code = "Noc 2",
                Description = "Level2",
                ParentId = 1,
                Level = 2,
                Id = 2
            };
            var level3Noc = new NationalOccupationalClassification()
            {
                Code = "Noc 3",
                Description = "Level3",
                ParentId = 2,
                Level = 3,
                Id = 3
            };
            var level4Noc = new NationalOccupationalClassification()
            {
                Code = "Noc 4",
                Description = "Level4",
                ParentId = 3,
                Level = 4,
                Id = 4
            };
            var nocs = new List<NationalOccupationalClassification>() { level1Noc, level2Noc, level3Noc, level4Noc};
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(NationalOccupationalClassificationService), identity);
			var dbSetMock = helper.MockDbSet(nocs);
			var dbContextMock = helper.GetMock<IDataContext>();
			dbContextMock.Setup(x => x.NationalOccupationalClassifications).Returns(dbSetMock.Object);

			var service = helper.Create<NationalOccupationalClassificationService>();

			// Act
			var results = service.GetNationalOccupationalClassificationIds(1);

			// Assert
			results.Item1.Should().Be(1);
			results.Item2.Should().Be(2);
			results.Item3.Should().Be(3);
			results.Item4.Should().Be(4);
		}
    }
}
