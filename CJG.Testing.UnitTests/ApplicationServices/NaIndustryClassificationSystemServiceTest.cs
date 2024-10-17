using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using CJG.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using System.Linq.Expressions;
using CJG.Application.Services;
using NLog;
using CJG.Infrastructure.Entities;
using System.Web;
using CJG.Testing.Core;

namespace CJG.Testing.UnitTests.ApplicationServices
{
    [TestClass]
    public class NaIndustryClassificationSystemServiceTest : ServiceUnitTestBase
    {
        [TestInitialize]
        public void Setup()
        {
        }

        [TestMethod, TestCategory("NAICS"), TestCategory("Service")]
        public void AddNaIndustryClassificationSystem_WhenNewNicsObjectPassedAsParameter_ShouldAddTheNicsObject()
        {
            // Arrange
            NaIndustryClassificationSystem newNcisObj = new NaIndustryClassificationSystem()
            {
                Code = "New NICS CODE",
                Description = "New NICS Description"
            };
            var loggerMock = new Mock<ILogger>();
            var httpContextMock = new Mock<HttpContextBase>();
            var dbContextMock = new Mock<IDataContext>();
			dbContextMock.Setup(x => x.NaIndustryClassificationSystems.Add(It.IsAny<NaIndustryClassificationSystem>()));
            var service = new NaIndustryClassificationSystemService(dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

            // Act
            service.AddNaIndustryClassificationSystem(newNcisObj);

            // Assert
            dbContextMock.Verify(x => x.NaIndustryClassificationSystems.Add(It.IsAny<NaIndustryClassificationSystem>()), Times.Exactly(1));
            dbContextMock.Verify(x => x.Commit(), Times.Exactly(1));
        }

        [TestMethod, TestCategory("NAICS"), TestCategory("Service")]
        public void UpdateNaIndustryClassificationSystem_WhenModifiedNicsObjectPassedAsParameter_ShouldUpdateTheNicsObject()
        {
            // Arrange
            NaIndustryClassificationSystem modifiedNcisObj = new NaIndustryClassificationSystem()
            {
                Code = "New Nics CODE",
                Description = "New Nics Description"
            };
            var loggerMock = new Mock<ILogger>();
            var dbContextMock = new Mock<IDataContext>();
            var httpContextMock = new Mock<HttpContextBase>();
            dbContextMock.Setup(x => x.Update(It.IsAny<NaIndustryClassificationSystem>()));
            var service = new NaIndustryClassificationSystemService(dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

            // Act
            service.UpdateNaIndustryClassificationSystem(modifiedNcisObj);

            // Assert
            dbContextMock.Verify(x => x.Update(It.IsAny<NaIndustryClassificationSystem>()), Times.Exactly(1));
            dbContextMock.Verify(x => x.Commit(), Times.Exactly(1));
        }

        [TestMethod, TestCategory("NAICS"), TestCategory("Service")]
        public void GetNaIndustryClassificationSystemChildren_WhenPassInParentIDAndLevel_ShouldReturnListOfNicsChildrenOrderByNcisCode()
        {
			// Arrange
			var parent = new NaIndustryClassificationSystem()
			{
				Code = "Parent",
                Description = "Description",
				Id = 3,
                ParentId = 2,
                Level = 4,
				Left = 1,
				Right = 3
			};
			var child1 = new NaIndustryClassificationSystem()
            {
                Code = "Child 1",
                Description = "Description",
				Id = 1,
                ParentId = 3,
                Level = 4,
				Left = 4,
				Right = 2,
				Parent = parent
            };

            var nicsList = new List<NaIndustryClassificationSystem>() { parent, child1 };
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(NaIndustryClassificationSystemService), identity);
			helper.MockDbSet(nicsList);

			var service = helper.Create<NaIndustryClassificationSystemService>();

			// Act
			var results = service.GetNaIndustryClassificationSystemChildren(3, 4);

            // Assert
            results.Count().Should().BeGreaterThan(0);
            results.ToList()[0].Code.Should().Be(child1.Code);
        }

        [TestMethod, TestCategory("NAICS"), TestCategory("Service")]
        public void GetNaIndustryClassificationSystems_WhenPassInNICSID_ShouldReturnListOfNicsData()
        {
			// Arrange
			var nic = new NaIndustryClassificationSystem() {
				Code = "Code",
				Description = "Description",
				Left = 1,
				Right = 1,
				Level = 1,
				Id = 1,
				ParentId = 0
			};

			var nicp = new NaIndustryClassificationSystem()
			{
				Code = "Parent",
				Description = "Description",
				Left = 1,
				Right = 1,
				Level = 0,
				Id = 0,
				ParentId = 0
			};

			var nicsList = new List<NaIndustryClassificationSystem>() { nic, nicp };
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(NaIndustryClassificationSystemService), identity);
			helper.MockDbSet(nicsList);

			var service = helper.Create<NaIndustryClassificationSystemService>();

            // Act
            var results = service.GetNaIndustryClassificationSystems(1);

            // Assert
            results.Count().Should().BeGreaterThan(0);
            results.Contains(nic);
        }

        [TestMethod, TestCategory("NAICS"), TestCategory("Service")]
        public void GetNaIndustryClassificationSystems_WhenPassInNullID_ReturnEmptyList()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();
            var httpContextMock = new Mock<HttpContextBase>();
            var dbContextMock = new Mock<IDataContext>();
			var service = new NaIndustryClassificationSystemService(dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

            // Act
            var results = service.GetNaIndustryClassificationSystems(null);

            // Assert
            results.Count().Should().Be(0);
        }

        [TestMethod, TestCategory("NAICS"), TestCategory("Service")]
        public void GetNaIndustryClassificationSystem_WhenNullIDAsParameter_ShouldReturnNull()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();
            var httpContextMock = new Mock<HttpContextBase>();
            var dbContextMock = new Mock<IDataContext>();
			var service = new NaIndustryClassificationSystemService(dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

            // Act
            var results = service.GetNaIndustryClassificationSystem(null);

            // Assert
            results.Should().Be(null);
        }

        [TestMethod, TestCategory("NAICS"), TestCategory("Service")]
        public void GetNaIndustryClassificationSystem_WhenPassInNcisID_ShouldReturnNcisData()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();
            var nics = new NaIndustryClassificationSystem()
            {
                Code = "Code",
                Description = "Description"
            };
            var httpContextMock = new Mock<HttpContextBase>();
            var dbContextMock = new Mock<IDataContext>();
			dbContextMock.Setup(x => x.NaIndustryClassificationSystems.Find(It.IsAny<int?>())).Returns(nics);
            var service = new NaIndustryClassificationSystemService(dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

            // Act
            var results = service.GetNaIndustryClassificationSystem(1);

            // Assert
            results.Code.Should().Be("Code");
            results.Description.Should().Be("Description");
        }

        [TestMethod, TestCategory("NAICS"), TestCategory("Service")]
        public void GetNaIndustryClassificationSystemIds_WhenPassInID_ShouldReturnTupleNciss()
        {
            // Arrange
            var level1Ncis = new NaIndustryClassificationSystem()
            {
                Code = "NICS 1",
                Description = "Level1",
                ParentId = 3,
                Level = 1,
                Id = 1

            };
            var level2Ncis = new NaIndustryClassificationSystem()
            {
                Code = "NICS 2",
                Description = "Level2",
                ParentId = 1,
                Level = 2,
                Id = 2
            };
            var level3Ncis = new NaIndustryClassificationSystem()
            {
                Code = "NICS 3",
                Description = "Level3",
                ParentId = 2,
                Level = 3,
                Id = 3
            };
            var nicsList = new List<NaIndustryClassificationSystem>() { level1Ncis, level2Ncis, level3Ncis };
			var user = EntityHelper.CreateExternalUser();
			var identity = HttpHelper.CreateIdentity(user);
			var helper = new ServiceHelper(typeof(NaIndustryClassificationSystemService), identity);
			helper.MockDbSet(nicsList);

			var service = helper.Create<NaIndustryClassificationSystemService>();

			// Act
			var results = service.GetNaIndustryClassificationSystemIds(3);

            // Assert
            results[0].Should().Be(1);
            results[1].Should().Be(2);
            results[2].Should().Be(3);
        }
    }
}
