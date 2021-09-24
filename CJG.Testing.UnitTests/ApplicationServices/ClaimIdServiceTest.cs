using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Infrastructure.Entities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using System.Web;

namespace CJG.Testing.UnitTests.ApplicationServices
{
	[TestClass]
    public class ClaimIdServiceTest : ServiceUnitTestBase
    {
        [TestInitialize]
        public void Setup()
        {
        }

        [TestMethod, TestCategory("Claim"), TestCategory("Service")]
        public void AddClaimId_PassInAddClaimId_ShouldSaveDataAndReturnID()
        {
            // Arrange
            var claimID = new ClaimId()
            {
                Id = 1
            };
            var loggerMock = new Mock<ILogger>();
            var dbContextMock = new Mock<IDataContext>();
            var httpContextMock = new Mock<HttpContextBase>();
			dbContextMock.Setup(x => x.ClaimIds.Add(It.IsAny<ClaimId>()));
            var service = new ClaimIdService(dbContextMock.Object, httpContextMock.Object, loggerMock.Object);

            // Act
            var result = service.AddClaimId(claimID);

            // Assert
            dbContextMock.Verify(x => x.ClaimIds.Add(It.IsAny<ClaimId>()), Times.Exactly(1));
            dbContextMock.Verify(x => x.Commit(), Times.Exactly(1));
            result.Should().Be(1);
        }
    }
}
