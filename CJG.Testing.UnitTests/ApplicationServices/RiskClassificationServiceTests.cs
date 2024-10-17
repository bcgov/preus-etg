using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Infrastructure.Entities;
using CJG.Testing.Core;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
    public class RiskClassificationServiceTests : ServiceUnitTestBase
    {
		[TestInitialize]
        public void Setup()
        {
		}

        [TestMethod, TestCategory("RiskClassification"), TestCategory("Service")]
        public void AddRiskClassification_WithRiskClassification_AddsNewRiskClassification()
		{
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(RiskClassificationService), user);
			var service = helper.Create<RiskClassificationService>();
			var riskClassification = new RiskClassification();
			helper.MockDbSet<RiskClassification>();

            // Act
            var result = service.Add(riskClassification);

			// Assert
			Assert.AreEqual(riskClassification, result);
            helper.GetMock<IDataContext>().Verify(x => x.Commit(), Times.Once);
		}

		[TestMethod, TestCategory("Risk Classification"), TestCategory("Service")]
        public void SaveRiskClassification_WithRiskClassification_AddsNewRiskClassification()
        {
			// Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(RiskClassificationService), user);
			var service = helper.Create<RiskClassificationService>();
			var riskClassification = new RiskClassification();
			var identity = HttpHelper.CreateIdentity(user);
			helper.MockDbSet<RiskClassification>();

            // Act
            var result = service.Update(riskClassification);

            // Assert
			Assert.AreEqual(riskClassification, result);
			helper.GetMock<IDataContext>().Verify(x => x.Commit(), Times.Once);
        }

		[TestMethod, TestCategory("Risk Classification"), TestCategory("Service")]
		public void DeleteRiskClassification_WithRiskClassification_DeletesRiskClassificationAndDependencies()
        {
            // Arrange
			var user = EntityHelper.CreateExternalUser();
			var helper = new ServiceHelper(typeof(RiskClassificationService), user);
			var service = helper.Create<RiskClassificationService>();
            var identity = HttpHelper.CreateIdentity(user);
            var grantProgram = new[]
            {
                new GrantProgram {Id = 3}
            };
            var payment = new[]
            {
                new AccountCode  {Id = 2}
            };
            var riskClassifications = new RiskClassification() { Id = 4, Caption = "name" };
			helper.MockDbSet(riskClassifications);
			helper.MockDbSet<AccountCode>();

            // Act
            service.Delete(riskClassifications);

			// Assert
			helper.GetMock<IDataContext>().Verify(x => x.Commit(), Times.Once);
			Assert.AreEqual(0, helper.GetMock<IDataContext>().Object.RiskClassifications.Count());
		}

		[TestMethod, TestCategory("Risk Classification"), TestCategory("Service")]
		public void GetAllRiskClassifications()
        {
            // Arrange
            var user = EntityHelper.CreateInternalUser();
            var identity = HttpHelper.CreateIdentity(user, "Assessor");
            var helper = new ServiceHelper(typeof(RiskClassificationService), identity);
            helper.MockDbSet(new[] {
                new RiskClassification
                {
                    Id = 1
                },
                new RiskClassification
                {
                    Id = 2
                },
                new RiskClassification
                {
                    Id = 3
                }
            });
            var service = helper.Create<RiskClassificationService>();

            // Act
            var result = service.GetAll();

            // Assert
            result.Should().HaveCount(3);
        }

		[TestMethod, TestCategory("Risk Classification"), TestCategory("Service")]
		public void GetRiskClassification_WithRiskClassificationId_ReturnsStreamWithMatchedStreamId()
        {
            // Arrange
            var user = EntityHelper.CreateExternalUser();
            var identity = HttpHelper.CreateIdentity(user);
            var helper = new ServiceHelper(typeof(RiskClassificationService), identity);
            helper.MockDbSet( new[] {
                new RiskClassification
                {
                    Id = 1,
                    Caption = "Name 1"
                }
            });
            var service = helper.Create<RiskClassificationService>();

            // Act
            var riskClassification = service.Get(1);

            // Assert
            riskClassification.Caption.Should().Be("Name 1");
        }
    }
}